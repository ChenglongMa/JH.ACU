using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using NationalInstruments.Restricted;

namespace JH.ACU.BLL
{
    public class BllMain
    {
        public BllMain()
        {
            _report = new Report();
            _specUnits = BllConfig.GetSpecItems();
            TestWorker = new NewBackgroundWorker();
            TestWorker.DoWork += TestWorker_DoWork;
            TestWorker.RunWorkerCompleted += TestWorker_RunWorkerCompleted;
            ChamberStay = new NewBackgroundWorker();
            ChamberStay.DoWork += ChamberStay_DoWork;
            ChamberStay.RunWorkerCompleted += ChamberStay_RunWorkerCompleted;
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

                if (ChamberStay.CancellationPending||TestWorker.CancellationPending)
                {
                    _chamber.Stop();
                    e.Cancel = true;
                    return;
                }

                #endregion

                Thread.Sleep(30000);
                var currTemp = _chamber.GetTemp();
                ChamberStay.ReportProgress(0, currTemp);
            } while (Environment.TickCount - tick <= TestCondition.Temperature.Duration*60*1000);

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
            }
            else
            {
                //正常完成
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
            TestEventArgs = e;
            _report.Message = "Test is running...";
            var tvItems = (Dictionary<TvType, double[]>)e.Argument; //传入温度电压测试条件集合
            foreach (var tvItem in tvItems) //tvItem[0]:温度值;tvItem[1]:电压值
            {
                _tvItem = tvItem;
                _tvIndex = tvItems.IndexOf(tvItem);
                SelectedTvType = tvItem.Key;
                #region 通知UI

                var tempTarget = tvItem.Value[0];
                _report.SettingTemp = tempTarget;
                VoltTarget = tvItem.Value[1];
                var specUnits = new List<SpecItem>();
                specUnits.AddRange(_specUnits);
                if (_report.SpecUnitsDict.ContainsKey(tvItem.Key))
                {
                    _report.SpecUnitsDict[tvItem.Key] = specUnits;
                }
                else
                {
                    _report.SpecUnitsDict.Add(tvItem.Key, specUnits);
                }
                _report.ChamberEnable = TestCondition.Temperature.Enable;
                TestWorker.ReportProgress(0, _report);
                #endregion

                #region 温度操作

                if (TestCondition.Temperature.Enable) //若值为Ture则执行温度操作
                {
                    _report.Message = "Chamber is working...";
                    if (_chamber == null)
                    {
                        _chamber = new BllChamber();
                    }
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

                        #region 通知UI

                        _report.ActualTemp = _chamber.GetTemp();
                        var progress = Math.Abs(_report.ActualTemp/tempTarget)*60;
                        TestWorker.ReportProgress((int) progress, _report);

                        #endregion
                    } while (Math.Abs(tempTarget - _chamber.GetTemp()) <= 1 || Environment.TickCount - tick < 60*60*1000);
                    ChamberStay.RunWorkerAsync();
                }

                #endregion

                #region 启动仪器、设定外部环境

                OpenAllInstrs();
                _pwr.Ocp = false;
                _pwr.OutPutState = false;
                _pwr.OutputVoltage = VoltTarget;
                _pwr.OutputCurrent = 5.0;
                _pwr.OutPutState = true; //开始输出电压

                #endregion

                foreach (var acuItem in TestCondition.AcuItems)
                {
                    #region 若取消测试

                    if (TestWorker.CancellationPending)
                    {
                        CloseAllInstrs();
                        e.Cancel = true;
                        return;
                    }

                    #endregion

                    var boardIndex = acuItem.Index;
                    if (boardIndex < 0 || boardIndex > 7) continue;
                    _report.AcuIndex = boardIndex + 1;//ACU Index显示值从1开始
                    _daq.OpenBoard((byte) boardIndex);
                    //QUES:DAQ读取PIN脚电压,测试外部环境,如失败则抛出异常中止测试
                    if (!AcuExecute(boardIndex))
                    {
                        continue;
                    }
                    _report.AcuName = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", _acu.RomCs, _acu.RomVer, _acu.Mlfb,
                        _acu.AcuSn, _acu.LabVer, _acu.ParaVer);

                    // QUES:B: ReadMemory WriteMemory Kline出错的可能性非常低
                    //以上步骤测试ACU有没有故障,返回故障码
                    if (!TestHasDtc(acuItem))
                    {
                        //continue;
                    }
                    if (!TestWarnLamp(acuItem))
                    {
                        //continue;
                    }
                    if (!TestHoldTime(acuItem))
                    {
                        //continue;
                    }
                    if (!TestACUCurr(acuItem))
                    {
                        //continue;
                    }
                    if (!TestCrashOut(acuItem))
                    {
                        //continue;
                    }

                }

            }

        }

        private bool TestCrashOut(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                //QUES:测试过程有异常是否直接跳过后面测试
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        private bool TestACUCurr(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;
                throw new NotImplementedException();

            }
            catch (Exception ex)
            {
                //QUES:测试过程有异常是否直接跳过后面测试
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        private bool TestHoldTime(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                //QUES:测试过程有异常是否直接跳过后面测试
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        #region 告警灯测试方法

        /// <summary>
        /// 告警灯测试
        /// </summary>
        /// <param name="acuItem"></param>
        /// <returns></returns>
        private bool TestWarnLamp(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;

                #region 告警灯电压测试

                if (items.Contains((int) WLTest.WL1VoltOn))
                {
                    if (!SubTestWL(WLTest.WL1VoltOn, () => GetWLVolt(acuItem.Index, 1, true)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL1VoltOff))
                {
                    if (!SubTestWL(WLTest.WL1VoltOff,()=>GetWLVolt(acuItem.Index, 1, false)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2VoltOn))
                {
                    if (!SubTestWL(WLTest.WL2VoltOn, () => GetWLVolt(acuItem.Index, 2, true)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2VoltOff))
                {
                    if (!SubTestWL(WLTest.WL2VoltOff, () => GetWLVolt(acuItem.Index, 2, false)))
                    {
                        return true;
                    }
                }

                #endregion

                #region 告警灯电流测试

                if (items.Contains((int) WLTest.WL1CurrentNormal))
                {
                    if (!SubTestWL(WLTest.WL1CurrentNormal, () => GetWLCurr(acuItem.Index, WLTest.WL1CurrentNormal)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL1CurrentShort))
                {
                    if (!SubTestWL(WLTest.WL1CurrentShort, () => GetWLCurr(acuItem.Index, WLTest.WL1CurrentShort)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2CurrentNormal))
                {
                    if (!SubTestWL(WLTest.WL2CurrentNormal, () => GetWLCurr(acuItem.Index, WLTest.WL2CurrentNormal)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2CurrentShort))
                {
                    if (!SubTestWL(WLTest.WL2CurrentShort, () => GetWLCurr(acuItem.Index, WLTest.WL2CurrentShort)))
                    {
                        return true;
                    }
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                //QUES:测试过程有异常是否直接跳过后面测试
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        /// <summary>
        /// 告警灯测试子方法
        /// </summary>
        /// <param name="wlTest"></param>
        /// <param name="func">测试电压或电流的方法委托</param>
        /// <returns>测试是否被取消</returns>
        private bool SubTestWL(WLTest wlTest, Func<double> func)
        {
            FindSpec((int) wlTest).ResultInfo = "Progressing...";
            var progress = GetProgress((int) wlTest, _tvIndex);
            TestWorker.ReportProgress((int) (progress*0.5), _report);
            var res = func(); //GetWLVolt(acuIndex, 1, true);
            if ((int) res == (int) TestResult.Cancelled)
            {
                FindSpec((int) wlTest).ResultInfo = "Cancelled";
                return false;
            }
            if ((int) res == (int) TestResult.Failed)
            {
                FindSpec((int) wlTest).ResultInfo = "Failed";
            }
            else
            {
                FindSpec((int) wlTest).ResultInfo = "Passed";
                FindSpec((int) wlTest).ResultValue = res;
            }
            TestWorker.ReportProgress(progress, _report);
            return true;
        }

        /// <summary>
        /// 获取ACU告警灯正常或短路时电流
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="wlTest"></param>
        /// <returns></returns>
        private double GetWLCurr(int acuIndex, WLTest wlTest)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs();
                return -1;
            }

            #endregion

            int wlIndex, relayIndex, relayIndex2 = -1;
            switch (wlTest)
            {
                default:
                    LogHelper.WriteWarningLog(LogFileName, "WLTest赋值错误：" + wlTest);
                    return -1;
                case WLTest.WL1CurrentNormal:
                    wlIndex = 1;
                    relayIndex = 271;
                    break;
                case WLTest.WL1CurrentShort:
                    wlIndex = 1;
                    relayIndex = 271;
                    relayIndex2 = 270;
                    break;
                case WLTest.WL2CurrentNormal:
                    wlIndex = 2;
                    relayIndex = 275;

                    break;
                case WLTest.WL2CurrentShort:
                    relayIndex = 275;
                    relayIndex2 = 274;
                    wlIndex = 2;

                    break;
            }
            //A:打开设置
            AcuExecute(acuIndex, () => _acu.EnableWarnLamp(wlIndex, true));
            _daq.SetSubRelayStatus((byte) acuIndex, 264, true);
            _daq.SetSubRelayStatus((byte) acuIndex, relayIndex, true);
            if (relayIndex2 > 0) //大于0表示测试项为Short类
            {
                _daq.SetSubRelayStatus((byte) acuIndex, relayIndex2, true);
            }
            //B:开始测试
            var res = _dmm.GetCurrent();
            //C:复位设置
            _daq.SetSubRelayStatus((byte) acuIndex, 264, false);
            _daq.SetSubRelayStatus((byte) acuIndex, relayIndex, false);
            if (relayIndex2 > 0) //大于0表示测试项为Short类
            {
                _daq.SetSubRelayStatus((byte) acuIndex, relayIndex2, false);
            }
            return res;
        }

        /// <summary>
        /// 获取ACU告警灯打开或关闭时电压
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="wLIndex"></param>
        /// <param name="isOn"></param>
        /// <returns></returns>
        private double GetWLVolt(int acuIndex, int wLIndex, bool isOn)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs();
                return -1;
            }

            #endregion

            //A:打开设置
            AcuExecute(acuIndex, () => _acu.EnableWarnLamp(wLIndex, isOn));
            _daq.SetSubRelayStatus((byte) acuIndex, 264, true);
            switch (wLIndex)
            {
                case 1:
                    _daq.SetSubRelayStatus((byte) acuIndex, 272, true);
                    break;
                case 2:
                    _daq.SetSubRelayStatus((byte) acuIndex, 276, true);
                    break;
            }
            //B:开始测试
            var res = _dmm.GetVoltage();
            //C:复位设置
            switch (wLIndex)
            {
                case 1:
                    _daq.SetSubRelayStatus((byte) acuIndex, 272, false);
                    break;
                case 2:
                    _daq.SetSubRelayStatus((byte) acuIndex, 276, false);
                    break;
            }
            return res;
        }

        #endregion

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
                //33-48 To Ground
                //49-64 To Battery
                for (int iMode = 1; iMode <= SquibModeNum; iMode++)
                {
                    for (int iSquib = 1; iSquib <= SquibNum; iSquib++)
                    {
                        var itemIndex = SquibNum*(iMode - 1) + iSquib;
                        if (acuItem.Items.Contains(itemIndex))
                        {
                            FindSpec(itemIndex).ResultInfo = "Progressing...";
                            var progress = GetProgress(itemIndex, _tvIndex);
                            TestWorker.ReportProgress((int) (progress*0.5), _report);
                            var res = TestSquib(acuItem.Index, iSquib, iMode);
                            if ((int) res == (int)TestResult.Cancelled)
                            {
                                FindSpec(itemIndex).ResultInfo = "Cancelled";
                                return true;
                            }
                            FindSpec(itemIndex).ResultInfo = (int) res == (int)TestResult.Failed ? "Failed" : "Passed";
                            TestWorker.ReportProgress(progress, _report);

                        }
                    }
                }
                //B:Belt测试
                //TODO:规范需要修改
                //同一测试情况归于一类
                for (int iMode = 0; iMode < BeltModeNum; iMode++)
                {
                    for (int iBelt = 0; iBelt < BeltNum; iBelt++)
                    {
                        var itemIndex = BeltNum*(iMode - 1) + iBelt + SquibNum*SquibModeNum;
                        if (acuItem.Items.Contains(itemIndex))
                        {
                            FindSpec(itemIndex).ResultInfo = "Progressing...";
                            var progress = GetProgress(itemIndex, _tvIndex);
                            TestWorker.ReportProgress((int)(progress * 0.5), _report);
                            var res = TestBelt(acuItem.Index, iBelt, iMode);
                            if ((int)res == (int)TestResult.Cancelled)
                            {
                                FindSpec(itemIndex).ResultInfo = "Cancelled";
                                return true;
                            }
                            FindSpec(itemIndex).ResultInfo = (int)res == (int)TestResult.Failed ? "Failed" : "Passed";
                            TestWorker.ReportProgress(progress, _report);
                        }
                    }
                }
                //C:Volt测试
                for (int iMode = 0; iMode < VoltModeNum; iMode++)
                {
                    var itemIndex = VoltNum*(iMode - 1) + 1 + SquibNum*SquibModeNum + BeltModeNum*BeltNum;
                    if (acuItem.Items.Contains(itemIndex))
                    {
                        FindSpec(itemIndex).ResultInfo = "Progressing...";
                        var progress = GetProgress(itemIndex, _tvIndex);
                        TestWorker.ReportProgress((int)(progress * 0.5), _report);
                        var res = TestVolt(acuItem.Index, iMode);
                        if ((int)res == (int)TestResult.Cancelled)
                        {
                            FindSpec(itemIndex).ResultInfo = "Cancelled";
                            return true;
                        }
                        FindSpec(itemIndex).ResultInfo = (int)res == (int)TestResult.Failed ? "Failed" : "Passed";
                        TestWorker.ReportProgress(progress, _report);

                    }
                }
                _pwr.OutputVoltage = VoltTarget; //电压测试完成后将外围电压恢复到原先状态
                _pwr.OutPutState = true; //可省略，保险起见保留此句
                //D:SIS测试
                for (int iMode = 0; iMode < SisModeNum; iMode++)
                {
                    for (int iSis = 0; iSis < SisNum; iSis++)
                    {
                        var itemIndex = SisNum*(iMode - 1) + iSis + SquibNum*SquibModeNum + BeltModeNum*BeltNum + VoltModeNum*VoltNum;
                        if (acuItem.Items.Contains(itemIndex))
                        {
                            FindSpec(itemIndex).ResultInfo = "Progressing...";
                            var progress = GetProgress(itemIndex, _tvIndex);
                            TestWorker.ReportProgress((int)(progress * 0.5), _report);
                            var res = TestSis(acuItem.Index, iSis, iMode);
                            if ((int)res == (int)TestResult.Cancelled)
                            {
                                FindSpec(itemIndex).ResultInfo = "Cancelled";
                                return true;
                            }
                            FindSpec(itemIndex).ResultInfo = (int)res == (int)TestResult.Failed ? "Failed" : "Passed";
                            TestWorker.ReportProgress(progress, _report);

                        }
                    }
                }
                return true; //测试过程无异常
            }
            catch (Exception ex)
            {
                //QUES:测试过程有异常是否直接跳过后面测试
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        #region SIS测试方法

        /// <summary>
        /// SIS测试
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="sisIndex"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private double TestSis(int acuIndex, int sisIndex, int mode)
        {
            double minValue, maxValue;
            byte dtc;
            var itemIndex = SisNum*(mode - 1) + sisIndex + SquibNum*SquibModeNum + BeltModeNum*BeltNum + VoltModeNum*VoltNum;
            var spec = FindSpec(itemIndex, out minValue, out maxValue, out dtc);
            //0：先设置好电阻箱//QUES:测试时验证是否合理
            _prs0.SetResistance(maxValue);
            _prs1.SetResistance(maxValue);
            //A:打开继电器
            _daq.SetSisInTestMode(acuIndex, sisIndex, (SisMode) mode);
            //B:开始测试
            var res = FindSis(minValue, maxValue, dtc, mode);
            if ((int)res == (int)TestResult.Failed) return (double)TestResult.Failed;
            if ((int)res == (int)TestResult.Cancelled) return (double)TestResult.Cancelled;
            _daq.SetSisInReadMode(acuIndex, sisIndex);
            res = _dmm.GetFourWireRes(); //该返回值为实际测量值
            res += GlobalConst.AmendResistance; //根据线阻 修正测试结果
            spec.ResultValue = res;
            //C:复位继电器
            _daq.SetSisReset(acuIndex, sisIndex);
            return res;
        }

        /// <summary>
        /// 根据取值范围及故障码查找临界阻值
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="dtc">故障码</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private double FindSis(double minValue, double maxValue, byte dtc, int mode)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs();
                return (double)TestResult.Cancelled;
            }

            #endregion

            //若最大值及最小值之差足够小则返回两者平均值
            if (maxValue - minValue <= GlobalConst.Precision)
            {
                return (maxValue - minValue)/2;
            }
            if (minValue < 0.1 || maxValue > 2000) return (double)TestResult.Failed;
            FindResult findresult;
            // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool forwardMax, acuConnect;
            BllPrs prs;

            #region 根据测试模式设置寻找方向及电阻箱选择

            switch ((SisMode) mode)
            {
                default:
                case SisMode.ToGround:
                    forwardMax = false;
                    prs = _prs1;
                    break;
                case SisMode.ToBattery:
                    forwardMax = false;
                    prs = _prs1;
                    break;
            }

            #endregion

            #region 设置电阻箱输出阻值并检查ACU是否报错

            prs.SetResistance(minValue);
            var findMin = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }
            prs.SetResistance(maxValue);
            var findMax = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }

            #endregion

            #region 根据ACU反馈给FindResult赋值

            if (!forwardMax)
            {
                findMax = !findMax;
                findMin = !findMin;
            }
            if (!findMax && !findMin)
            {
                findresult = FindResult.AboveMax;
            }
            else if (findMax && !findMin)
            {
                findresult = FindResult.InBetween;
            }
            else if (findMax)
            {
                findresult = FindResult.UnderMin;
            }
            else
            {
                findresult = FindResult.Error;
            }

            #endregion

            #region 根据FindResult值重新设定大小范围

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

            #endregion

            //递归调用精确查找结果
            return FindSis(minValue, maxValue, dtc, mode);
        }

        #endregion

        #region Belt测试方法

        /// <summary>
        /// Belt开关测试
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="belt">开关类型 DSB PSB PADS</param>
        /// <param name="mode">故障情况，共4种</param>
        /// <returns></returns>
        private double TestBelt(int acuIndex, int belt, int mode)
        {
            var itemIndex = BeltNum*(mode - 1) + belt + SquibNum*SquibModeNum;
            double minValue;
            double maxValue;
            byte dtc;
            var spec = FindSpec(itemIndex, out minValue, out maxValue, out dtc);
            //0：先设置好电阻箱//QUES:测试时验证是否合理
            _prs0.SetResistance(maxValue);
            _prs1.SetResistance(maxValue);
            //A:打开继电器
            _daq.SetBeltInTestMode(acuIndex, belt, (BeltMode) mode);
            //B:开始测试
            var res = FindBelt(minValue, maxValue, dtc, mode);
            if ((int)res == (int)TestResult.Failed) return (double)TestResult.Failed;
            if ((int)res == (int)TestResult.Cancelled) return (double)TestResult.Cancelled;
            _daq.SetBeltInReadMode(acuIndex, belt);
            res = _dmm.GetFourWireRes(); //该返回值为实际测量值
            res += GlobalConst.AmendResistance; //根据线阻 修正测试结果
            spec.ResultValue = res;
            //C:复位继电器
            _daq.SetBeltReset(acuIndex, belt);
            return res;
        }

        /// <summary>
        /// 根据取值范围及故障码查找临界阻值
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="dtc">故障码</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private double FindBelt(double minValue, double maxValue, byte dtc, int mode)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs();
                return (double)TestResult.Cancelled;
            }

            #endregion

            //若最大值及最小值之差足够小则返回两者平均值
            if (maxValue - minValue <= GlobalConst.Precision)
            {
                return (maxValue - minValue)/2;
            }
            if (minValue < 0.1 || maxValue > 2000) return (double)TestResult.Failed;
            FindResult findresult;
            // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool forwardMax, acuConnect;
            BllPrs prs;

            #region 根据测试模式设置寻找方向及电阻箱选择

            switch ((BeltMode) mode)
            {
                default:
                case BeltMode.UnbuckledOrDisabled:
                    forwardMax = false;
                    prs = _prs1;
                    break;
                case BeltMode.BuckledOrEnabled:
                    forwardMax = true;
                    prs = _prs1;
                    break;
                case BeltMode.ToGround:
                    throw new NotImplementedException("配置待定");
                    forwardMax = false;
                    prs = _prs1;
                    break;
                case BeltMode.ToBattery:
                    throw new NotImplementedException("配置待定");
                    forwardMax = false;
                    prs = _prs1;
                    break;
            }

            #endregion

            #region 设置电阻箱输出阻值并检查ACU是否报错

            prs.SetResistance(minValue);
            var findMin = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }
            prs.SetResistance(maxValue);
            var findMax = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }

            #endregion

            #region 根据ACU反馈给FindResult赋值

            if (!forwardMax)
            {
                findMax = !findMax;
                findMin = !findMin;
            }
            if (!findMax && !findMin)
            {
                findresult = FindResult.AboveMax;
            }
            else if (findMax && !findMin)
            {
                findresult = FindResult.InBetween;
            }
            else if (findMax)
            {
                findresult = FindResult.UnderMin;
            }
            else
            {
                findresult = FindResult.Error;
            }

            #endregion

            #region 根据FindResult值重新设定大小范围

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

            #endregion

            //递归调用精确查找结果
            return FindBelt(minValue, maxValue, dtc, mode);
        }

        #endregion

        #region 电压测试方法

        private double TestVolt(int acuIndex, int mode)
        {
            var itemIndex = VoltNum*(mode - 1) + 1 + SquibNum*SquibModeNum + BeltModeNum*BeltNum;
            double minValue;
            double maxValue;
            byte dtc;
            var spec = FindSpec(itemIndex, out minValue, out maxValue, out dtc);
            //A:打开继电器-正常情况下已打开
            //B:开始测试
            var res = FindVolt(minValue, maxValue, dtc, mode);
            if ((int)res == (int)TestResult.Failed) return (double)TestResult.Failed;
            if ((int)res == (int)TestResult.Cancelled) return (double)TestResult.Cancelled;
            _daq.SetSubRelayStatus((byte)acuIndex, 266, true);
            res = _dmm.GetVoltage(); //该返回值为实际测量值
            spec.ResultValue = res;
            //C:复位继电器
            _daq.SetSubRelayStatus((byte) acuIndex, 266, false);

            return res;
        }

        private double FindVolt(double minValue, double maxValue, byte dtc, int mode)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs();
                return (double) TestResult.Cancelled;
            }

            #endregion

            //若最大值及最小值之差足够小则返回两者平均值
            if (maxValue - minValue <= GlobalConst.Precision)
            {
                return (maxValue - minValue)/2;
            }
            if (minValue < 0.1 || maxValue > 20) return (double)TestResult.Failed;
            FindResult findresult;
            // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool acuConnect;

            #region 根据测试模式设置寻找方向

            var forwardMax = ((BatteryMode) mode) == BatteryMode.TooHigh;

            #endregion

            #region 设置电源输出电压并检查ACU是否报错

            _pwr.OutputVoltage = minValue;
            _pwr.OutPutState = true;
            var findMin = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }
            _pwr.OutputVoltage = maxValue;
            _pwr.OutPutState = true;
            var findMax = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }

            #endregion

            #region 根据ACU反馈给FindResult赋值

            if (!forwardMax)
            {
                findMax = !findMax;
                findMin = !findMin;
            }
            if (!findMax && !findMin)
            {
                findresult = FindResult.AboveMax;
            }
            else if (findMax && !findMin)
            {
                findresult = FindResult.InBetween;
            }
            else if (findMax)
            {
                findresult = FindResult.UnderMin;
            }
            else
            {
                findresult = FindResult.Error;
            }

            #endregion

            #region 根据FindResult值重新设定大小范围

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

            #endregion

            //递归调用精确查找结果
            return FindVolt(minValue, maxValue, dtc, mode);
        }

        #endregion

        #region 回路测试方法

        /// <summary>
        /// 回路测试
        /// </summary>
        /// <param name="acuIndex">ACU索引</param>
        /// <param name="squib">回路数 1-16</param>
        /// <param name="mode">故障情况，共4种</param>
        private double TestSquib(int acuIndex, int squib, int mode)
        {
            var itemIndex = SquibNum*(mode - 1) + squib;
            double minValue;
            double maxValue;
            byte dtc;
            var spec = FindSpec(itemIndex, out minValue, out maxValue, out dtc);
            //0：先设置好电阻箱//QUES:测试时验证是否合理
            _prs0.SetResistance(maxValue);
            _prs1.SetResistance(maxValue);
            //A:打开继电器
            _daq.SetFcInTestMode(acuIndex, squib, (SquibMode) mode);
            //B:开始测试
            var res = FindSquib(minValue, maxValue, dtc, mode); //QUES：该返回值为代码计算出的值，调试时查看与测量结果偏差
            if ((int) res == (int) TestResult.Failed) return (double) TestResult.Failed;
            if ((int)res == (int)TestResult.Cancelled) return (double)TestResult.Cancelled;
            _daq.SetFcInReadMode(acuIndex, squib, (SquibMode) mode);
            res = _dmm.GetFourWireRes(); //该返回值为实际测量值
            res += GlobalConst.AmendResistance; //根据线阻 修正测试结果
            spec.ResultValue = res;
            //C:复位继电器
            _daq.SetFcReset(acuIndex, squib);
            return res;
        }

        /// <summary>
        /// 根据取值范围及故障码查找临界阻值
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="dtc">故障码</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private double FindSquib(double minValue, double maxValue, byte dtc, int mode)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs();
                return (double) TestResult.Cancelled;
            }

            #endregion
            FindResult findresult;
            // 正向 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 反向 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool forwardMax, acuConnect;
            BllPrs prs;

            #region 根据测试模式设置寻找方向及电阻箱选择

            switch ((SquibMode) mode)
            {
                default:
                case SquibMode.TooHigh:
                    forwardMax = true;
                    prs = _prs0;
                    break;
                case SquibMode.TooLow:
                    forwardMax = false;
                    prs = _prs0;
                    break;
                case SquibMode.ToGround:
                    forwardMax = false;
                    prs = _prs1;
                    break;
                case SquibMode.ToBattery:
                    forwardMax = false;
                    prs = _prs1;
                    break;
            }

            #endregion

            #region 设置电阻箱输出阻值并检查ACU是否报错

            prs.SetResistance(minValue);
            var findMin = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }
            prs.SetResistance(maxValue);
            var findMax = _acu.HasFoundDtc(dtc, out acuConnect);
            if (!acuConnect)
            {
                throw new Exception("ACU连接失败");
            }

            #endregion

            #region 根据ACU反馈给FindResult赋值

            if (!forwardMax)
            {
                findMax = !findMax;
                findMin = !findMin;
            }
            if (!findMax && !findMin)
            {
                findresult = FindResult.AboveMax;
            }
            else if (findMax && !findMin)
            {
                findresult = FindResult.InBetween;
            }
            else if (findMax)
            {
                findresult = FindResult.UnderMin;
            }
            else
            {
                findresult = FindResult.Error;
            }

            #endregion

            #region 根据FindResult值重新设定大小范围

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

            #endregion
            //若最大值及最小值之差足够小则返回两者平均值
            if (maxValue - minValue <= GlobalConst.Precision)
            {
                return (maxValue - minValue) / 2;
            }
            if (minValue < 0.1 || maxValue > 2000)
            {
                return (double) TestResult.Failed;
            }
            //递归调用精确查找结果
            return FindSquib(minValue, maxValue, dtc, mode);
        }

        #endregion

        #region 属性字段
        public TvType SelectedTvType { get; private set; }
        public NewBackgroundWorker TestWorker { get; private set; }
        private DoWorkEventArgs TestEventArgs { get; set; }
        private NewBackgroundWorker ChamberStay { get; set; }
        private TestCondition TestCondition { get; set; }
        private readonly Report _report;
        private KeyValuePair<TvType, double[]> _tvItem;
        private int _tvIndex;//正在测试温度电压组合的项数
        private double _voltTarget;

        private double VoltTarget
        {
            get
            {
                return _voltTarget;
            }
            set { _voltTarget = value;
                _report.SettingVolt = value;
            }
        }

        #region 从规范中归纳的常量值 SPEC_unit.txt

        private const int SquibNum = 16; //回路数常量
        private const int SquibModeNum = 4; //回路测试情况常量
        private const int BeltNum = 3; //开关数常量
        private const int BeltModeNum = 4; //开关测试情况常量
        public const int VoltNum = 1; //测电压处常量
        public const int VoltModeNum = 2; //电压测试情况常量
        private const int SisNum = 6; //侧传感器数常量
        private const int SisModeNum = 2; //侧传感器测试情况常量

        private enum WLTest
        {
            WL1VoltOff = 88,
            WL1VoltOn = 89,
            WL1CurrentNormal = 90,
            WL1CurrentShort = 91,
            WL2VoltOff = 92,
            WL2VoltOn = 93,
            WL2CurrentNormal = 94,
            WL2CurrentShort = 95,
        }

        #endregion

        /// <summary>
        /// 配置中的规范集合
        /// </summary>
        private readonly List<SpecItem> _specUnits;

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
            _prs1 = new BllPrs(InstrName.Prs1);
            _prs1.Initialize();
            _pwr = new BllPwr();
            _pwr.Initialize();
        }

        /// <summary>
        /// 关闭所有仪器
        /// </summary>
        private void CloseAllInstrs()
        {
            if (_daq != null)
            {
                _daq.ResetAll();
                _daq.Dispose();
            }
            if (_dmm != null) _dmm.Dispose();
            if (_prs0 != null) _prs0.Dispose();
            if (_prs1 != null) _prs1.Dispose();
            if (_pwr != null) _pwr.Dispose();
        }

        /// <summary>
        /// 启动ACU，自动重连三次
        /// </summary>
        /// <param name="index">ACU索引</param>
        /// <param name="func">尝试执行的ACU命令委托</param>
        /// <returns></returns>
        private bool AcuExecute(int index, Func<bool> func = null)
        {
            var isStart = false;
            for (int i = 0; i < 3; i++)
            {
                #region 若取消测试

                if (TestWorker.CancellationPending)
                {
                    TestEventArgs.Cancel = true;
                    _acu.Stop();
                    return true;
                }

                #endregion

                var canTest = _daq.CheckPowerStatus(VoltTarget);
                if (!canTest)
                {
                    LogHelper.WriteWarningLog(LogFileName, string.Format("外围设备供电不正常[{0}]", i));
                }
                _acu = _acu ?? new BllAcu();
                if (func != null)
                {
                    if (func())
                    {
                        isStart = true;
                        break;
                    }
                }
                if (_acu.Start())
                {
                    if (func == null)
                    {
                        isStart = true;
                        break;
                    }
                }
                //ACU启动失败则重启仪器重试
                RebootAllInstrs();
                _daq.OpenBoard((byte) index);
            }
            if (!isStart)
            {
                LogHelper.WriteWarningLog(LogFileName, string.Format("ACU#{0}启动失败", index));
            }
            return isStart;
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
        private static Dictionary<TvType, double[]> RemoveTempList(Dictionary<TvType, double[]> tvItems)
        {
            return
                tvItems.Where(
                    tvItem =>
                        tvItem.Key == TvType.NorTempLowVolt || tvItem.Key == TvType.NorTempNorVolt ||
                        tvItem.Key == TvType.NorTempHighVolt).ToDictionary(tvItem => tvItem.Key, tvItem => tvItem.Value);
        }

        /// <summary>
        /// 解析规范文件中的item
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="dtc"></param>
        private SpecItem FindSpec(int itemIndex, out double minValue, out double maxValue, out byte dtc)
        {
            try
            {
                var spec = _report.SpecUnitsDict[_tvItem.Key].Find(s => s.Index == itemIndex);
                if (spec == null) throw new FileNotFoundException(string.Format("未找到项目:{0}", itemIndex));
                var range = spec.Specification.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                minValue = double.Parse(range[0]);
                maxValue = double.Parse(range[1]);
                dtc = spec.Dtc;
                return spec;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("规范解析失败[{0}]，错误信息：{1}", itemIndex, ex.Message), ex);
            }
        }

        /// <summary>
        /// 解析规范文件中的item
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        private SpecItem FindSpec(int itemIndex)
        {
            try
            {
                var spec = _report.SpecUnitsDict[_tvItem.Key].Find(s => s.Index == itemIndex);
                if (spec == null) throw new FileNotFoundException(string.Format("未找到项目:{0}", itemIndex));
                return spec;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("规范解析失败[{0}]，错误信息：{1}", itemIndex, ex.Message), ex);
            }
        }

        private int GetProgress(int specIndex, int tvIndex)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 公有方法

        public void Start(TestCondition testCondition)
        {
            TestCondition = testCondition;
            var tvItems = TestCondition.Temperature.Enable ? TestCondition.TvItems : RemoveTempList(TestCondition.TvItems);
            TestWorker.RunWorkerAsync(tvItems);
        }

        #endregion
    }
}