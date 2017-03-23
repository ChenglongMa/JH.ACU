using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using JH.ACU.BLL.Instruments;
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
                    var isStart = false;
                    for (int i = 0; i < 3; i++)
                    {
                        _acu = new BllAcu();
                        if(_acu.Start())
                        {
                            isStart = true;
                            break;
                        }
                        //ACU启动失败则重启仪器重试
                        RebootAllInstrs();
                    }
                    if (!isStart) throw new TimeoutException("ACU启动超时,测试中止");
                    // QUES:B: ReadMemory WriteMemory Kline出错的可能性非常低
                    // QUES:C: TestHasFaultCode 需要故障码的测试
                    //以上两项作用未知,个人看法应该测试ACU有没有故障,如是故障件则取消测试
                    foreach (var item in acuItem.Items)
                    {
                        //TODO:根据item值选择进行哪些测试
                    }
                }

            }

        }


        #region 属性字段

        public NewBackgroundWorker TestWorker { get; private set; }
        public NewBackgroundWorker ChamberStay { get; private set; }
        private readonly TestCondition _testCondition;
        private readonly Report _report;
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
            var tvItems = _testCondition.Temperature.Enable
                ? _testCondition.TvItems
                : RemoveTempList(_testCondition.TvItems);
            TestWorker.RunWorkerAsync(tvItems);
        }

        #endregion

    }

    /// <summary>
    /// 继承自BackgroundWorker，设置属性默认值
    /// </summary>
    public class NewBackgroundWorker : BackgroundWorker
    {
        public NewBackgroundWorker()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }
    }
}