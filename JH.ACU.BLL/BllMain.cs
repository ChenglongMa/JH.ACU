using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using JH.ACU.BLL.Config;
using JH.ACU.BLL.Instruments;
using JH.ACU.Lib;
using JH.ACU.Lib.Component;
using JH.ACU.Model;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.BLL
{
    public class BllMain
    {
        public BllMain()
        {
            _report = new Report();
            _testCondition = new TestCondition();
            SpecUnits = BllConfig.GetSpecConfig();

            TestWorker = new NewBackgroundWorker();
            TestWorker.DoWork += TestWorker_DoWork;
            TestWorker.RunWorkerCompleted += TestWorker_RunWorkerCompleted;
            ChamberStay = new NewBackgroundWorker();
            ChamberStay.DoWork+=ChamberStay_DoWork;
            ChamberStay.RunWorkerCompleted+=ChamberStay_RunWorkerCompleted;
        }

        private void ChamberStay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
            if (e.Cancelled)
            {
                //操作取消
            }
            else
            {
                //正常完成
            }
        }

        private void ChamberStay_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 使温度保持
            _chamber.Stay();
            var tick = Environment.TickCount;
            do
            {
                #region 若取消

                if (ChamberStay.CancellationPending)
                {
                    _chamber.Stop();
                    e.Cancel = true;
                    return;
                }

                #endregion
                Thread.Sleep(30000);
                var currTemp = _chamber.GetTemp();
                ChamberStay.ReportProgress(0, currTemp);
            } while (Environment.TickCount - tick <= _testCondition.Temperature.Duration * 60 * 1000);

            #endregion

        }

        /// <summary>
        /// 测试完成时执行 //TODO:UI层还可以写一下同样方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
            if (e.Cancelled)
            {
                //操作取消
                return;
            }


        }

        /// <summary>
        /// 1 设定温度直到达到目标温度（误差为1度）
        /// 2.1 温度保持
        /// 2.2 开始测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var tvItems = (List<double[]>) e.Argument;//传入温度电压测试条件集合
            foreach (var tvItem in tvItems) //tvItem[0]:温度值;tvItem[1]:电压值
            {
                #region 温度操作

                if (_testCondition.Temperature.Enable) //若值为Ture则执行温度操作
                {
                    if (_chamber == null)
                    {
                        _chamber = new BllChamber();
                    }
                    var tempTarget = tvItem[0];
                    _chamber.SetTemp(tempTarget); //设置温度值
                    var tick = Environment.TickCount; //获取当前时刻值
                    do
                    {
                        #region 若取消

                        if (TestWorker.CancellationPending)
                        {
                            _chamber.Stop();
                            e.Cancel = true;
                            return;
                        }

                        #endregion

                        Thread.Sleep(30000);
                        _report.Temp = _chamber.GetTemp(); 
                        TestWorker.ReportProgress(0, _report);//通知UI
                    } while (Math.Abs(tempTarget - _chamber.GetTemp()) <= 1 || Environment.TickCount - tick < 60*60*1000);
                    ChamberStay.RunWorkerAsync();
                }

                #endregion

                #region 启动仪器、设定外部环境

                OpenAllInstrs();
                var voltTarget = tvItem[1];
                /*			// 电源基本设定
 
                            double dVoltInit = 0.0;
                            g_daqAnalog.GetVoltFromChannel(2, MES_VOLT_IGNCH, dVoltInit);
                            g_fVoltInit = dVoltInit;
                            Synchronize(FormMain->ViewVoltCurr);
                            g_iVoltCount = 1;
                            g_bVoltReal = true;
                            ViewProgressInfo(false, 2);*/
                _pwr.Ocp = false;
                _pwr.OutPutState = false;
                _pwr.OutputVoltage = voltTarget;
                _pwr.OutputCurrent = 5.0;
                _pwr.OutPutState = true; //开始输出电压
                #endregion

                //TODO:DAQ读取PIN脚电压,检查是否设置成功
                foreach (var acuItem in _testCondition.AcuItems)
                {
                    var boardIndex = acuItem.Index;
                    if (boardIndex < 0 || boardIndex > 7) continue;
                    _daq.OpenBoard((byte) boardIndex);
                    //TODO:DAQ读取PIN脚电压,测试外部环境,如失败则抛出异常中止测试
                    if (!StartAcu(boardIndex))
                    {
                        continue;
                    }
                    // QUES:B: ReadMemory WriteMemory Kline出错的可能性非常低
                    //此项作用未知,个人看法应该测试ACU有没有故障,如是故障件则取消测试
                    if (!TestHasDtc(acuItem))
                    {
                        continue;
                    }

                }

            }

        }
        /// <summary>
        /// 启动ACU，自动重连三次
        /// </summary>
        /// <param name="index">ACU索引</param>
        /// <returns></returns>
        private bool StartAcu(int index)
        {
            var isStart = false;
            for (int i = 0; i < 3; i++)
            {
                _acu = new BllAcu();
                if (_acu.Start())
                {
                    isStart = true;
                    break;
                }
                //ACU启动失败则重启仪器重试
                RebootAllInstrs();
            }
            if (!isStart)
            {
                LogHelper.WriteWarningLog(LogFileName, string.Format("ACU#{0}启动失败", index));
            }
            return isStart;
        }
        /// <summary>
        /// 含有故障码测试，对应SPEC_unit.txt中01-87项
        /// </summary>
        /// <param name="acuItem"></param>
        /// <returns>测试过程有无异常</returns>
        private bool TestHasDtc(AcuItems acuItem)
        {
            try
            {
                //A:Squib测试
                //01-16 Too High
                //17-32 Too Low
                //31-48 To Ground
                //49-64 To Battery
                for (int iMode = 1; iMode <= 4; iMode++)
                {
                    for (int iSquib = 1; iSquib <= 16; iSquib++)
                    {
                        if(acuItem.Items.Contains(iSquib*iMode))
                        {
                            SpecUnits.Find(s => s.Index == iSquib*iMode).Result = TestSquib(acuItem.Index, iSquib, iMode);
                        }
                    }
                }
                //B:Belt测试
                return true;//测试过程无异常
            }
            catch (Exception ex)
            {
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName,e);
                return false;
            }
        }

        /// <summary>
        /// 回路测试
        /// </summary>
        /// <param name="boardIndex">ACU索引</param>
        /// <param name="squib">回路数 1-16</param>
        /// <param name="mode">故障情况，共4种</param>
        private double TestSquib(int boardIndex,int squib, int mode)
        {
            var spec = SpecUnits.FirstOrDefault(s => s.Index == squib*mode);
            if (spec == null) return 0;
            var range = spec.Specification.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
            var minValue = double.Parse(range[0]);
            var maxValue = double.Parse(range[1]);
            var dtc = spec.Dtc;
            //A:TODO 打开继电器
            _daq
            //B:开始测试
            return FindSquib(minValue, maxValue, dtc, mode);

        }

        private double FindSquib(double minValue, double maxValue, byte dtc, int mode)
        {
            if (maxValue - minValue <= GlobalConst.Precision)
            {
                return (maxValue - minValue)/2;
            }
            FindResult findresult;
            // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool forwardMax;//TODO:查找方向
            switch (mode)
            {
                case 1:
                    forwardMax = true;
                    break;
                case 2:
                    forwardMax = false;
                    break;
                case 3:
                    forwardMax = false;
                    break;
                case 4:
                    forwardMax = false;//QUES:为什么为false
                    break;
                default:
                    forwardMax = true;
                    break;
            }
            //QUES:原程序中mode<2时启动电阻箱#2，所以需要确认两个电阻箱分别控制哪几项
            //本例暂时只用_prs0;
            bool acuConnect;
            _prs0.SetResistance(minValue);
            var findMin = _acu.HasFoundDtc(dtc,out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }
            _prs0.SetResistance(maxValue);
            var findMax = _acu.HasFoundDtc(dtc,out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }
            if (!forwardMax)
            {
                findMax = !findMax;
                findMin = !findMin;
            }
            if (!findMax && !findMin)
            {
                findresult=FindResult.AboveMax;
            }
            else if (findMax && !findMin)
            {
                findresult=FindResult.InBetween;
            }
            else if(findMax)
            {
                findresult=FindResult.UnderMin;
            }
            else
            {
                findresult=FindResult.Error;
            }

            switch (findresult)
            {
                case FindResult.Error:
                    throw new Exception(string.Format("寻找DTC:0x{0}失败", dtc.ToString("X2")));
                case FindResult.InBetween:
                    var mid = (maxValue + minValue)/2;
                    if (forwardMax)
                    {
                        maxValue = mid;
                    }
                    else
                    {
                        minValue = mid;
                    }
                    break;
                case FindResult.UnderMin:
                    maxValue = minValue;
                    minValue = minValue/2.0;
                    break;
                case FindResult.AboveMax:
                    minValue = maxValue;
                    maxValue = maxValue*2.0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return FindSquib(minValue, maxValue, dtc, mode);
        }

        #region 属性字段

        public NewBackgroundWorker TestWorker { get; private set; }
        public NewBackgroundWorker ChamberStay { get; private set; }
        private readonly TestCondition _testCondition;
        private readonly Report _report;
        /// <summary>
        /// 传值至UI层，填表用
        /// </summary>
        public List<SpecUnit> SpecUnits { get; private set; }
        private static readonly string LogFileName = "TestLog" + DateTime.Now.ToString("yy_MM_dd");
        #region 各类仪器

        private BllAcu _acu;
        private BllChamber _chamber;
        private BllDaq _daq;
        private BllDmm _dmm;
        private BllPrs _prs0;
        private BllPrs _prs1;
        private BllPwr _pwr;

        #endregion

        #endregion

        #region 私有方法

        /// <summary>
        /// 开启所有仪器
        /// </summary>
        private void OpenAllInstrs()
        {
            //_chamber = new BllChamber();
            _daq = new BllDaq();
            _daq.Initialize();
            _dmm = new BllDmm();
            _dmm.Initialize();
            _prs0 = new BllPrs(InstrName.Prs0);
            _prs0.Initialize();
            _prs1 = new BllPrs(InstrName.Prs0);
            _prs1.Initialize();
            _pwr = new BllPwr();
            _pwr.Initialize();
        }

        /// <summary>
        /// 关闭所有仪器
        /// </summary>
        private void CloseAllInstrs()
        {
            _daq.ResetAll();
            _daq.Dispose();
            _dmm.Dispose();
            _prs0.Dispose();
            _prs1.Dispose();
            _pwr.Dispose();
        }

        /// <summary>
        /// 重启仪器
        /// </summary>
        private void RebootAllInstrs()
        {
            CloseAllInstrs();
            Thread.Sleep(300);
            OpenAllInstrs();
        }

        /// <summary>
        /// 当温度不需要控制时,去除温度条件
        /// </summary>
        /// <param name="tvItems"></param>
        /// <returns></returns>
        private static List<double[]> RemoveTempList(List<double[]> tvItems)
        {
            var tem = new List<double>();
            foreach (var tvItem in tvItems.Where(tvItem => !tem.Contains(tvItem[1])))
            {
                tem.Add(tvItem[1]);
            }
            return tem.Select(value => new[] {0, value}).ToList();
        }

        #endregion

        #region 公有方法

        public void AutoRun()
        {
            var tvItems = _testCondition.Temperature.Enable ? _testCondition.TvItems : RemoveTempList(_testCondition.TvItems);
            TestWorker.RunWorkerAsync(tvItems);
        }

        #endregion
    }
}