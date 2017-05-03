using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using JH.ACU.BLL.Abstract;
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
            ChamberReset = new NewBackgroundWorker();
            ChamberReset.DoWork+=ChamberReset_DoWork;
        }

        private void ChamberReset_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!TestCondition.Temperature.Enable) return;
            _chamber = _chamber ?? new BllChamber();
            _chamber.SetTemp(25);
            _chamber.Run();
            while (Math.Abs(ActualTemp-25)>0.5)
            {
                SettingTemp = 25;
                ChamberReset.ReportProgress(0,_report);
                Thread.Sleep(3000);
            }
            _chamber.Stop();
        }

        private void ChamberStay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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
                #region 若取消或测试结束

                if (ChamberStay.CancellationPending || TestWorker.CancellationPending || !TestWorker.IsBusy)
                {
                    _chamber.Stop();
                    e.Cancel = true;
                    return;
                }

                #endregion

                Thread.Sleep(3000);

                #region 通知UI

                _report.Message = "Chamber is staying...";
                _report.ActualTemp = ActualTemp;
                var progress = 60 +
                               Math.Abs((Environment.TickCount - tick)/(TestCondition.Temperature.Duration*60*1000))*40;
                TestWorker.ReportProgress((int) progress, _report);

                #endregion

            } while (Environment.TickCount - tick <= TestCondition.Temperature.Duration*60*1000);

            #endregion

        }

        /// <summary>
        /// 测试完成时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //操作取消
            }
            else
            {
                //正常完成
            }
            _acu.IfNotNull(a => a.Stop());
            CloseAllInstrs(true);
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
            var tvItems = (Dictionary<TvType, double[]>) e.Argument; //传入温度电压测试条件集合
            _tvCount = tvItems.Count;
            foreach (var tvItem in tvItems) //tvItem[0]:温度值;tvItem[1]:电压值
            {
                _tvItem = tvItem;
                _tvIndex = tvItems.IndexOf(tvItem);
                SelectedTvType = tvItem.Key;

                #region 通知UI

                SettingTemp = tvItem.Value[0];
                SettingVolt = tvItem.Value[1];
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
                    var initTemp = _chamber.GetTemp();//初始温度值
                    _chamber.SetTemp(SettingTemp); //设置温度值
                    _chamber.Run();
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

                        Thread.Sleep(3000);

                        #region 通知UI

                        _report.ActualTemp = ActualTemp;
                        var progress = (1 -
                                        Math.Abs(_report.ActualTemp - SettingTemp)/Math.Abs(initTemp - SettingTemp))*
                                       60;
                        TestWorker.ReportProgress((int) progress, _report);

                        #endregion
                    } while (Math.Abs(SettingTemp - ActualTemp) > 0.5 && Environment.TickCount - tick < 60*60*1000);
                    if (TestCondition.Temperature.Duration > 0)
                    {
                        ChamberStay.RunWorkerAsync();
                    }
                    var delay = Convert.ToInt32(tvItem.Value[2]);
                    Thread.Sleep(delay);
                }

                #endregion

                #region 启动仪器、设定外部环境

                OpenAllInstrs();
                _pwr.Ocp = false;
                _pwr.OutPutState = false;
                _pwr.OutputVoltage = SettingVolt;
                _pwr.OutputCurrent = 5.0;
                _pwr.OutPutState = true; //开始输出电压

                #endregion

                foreach (var acuItem in TestCondition.AcuItems)
                {
                    #region 若取消测试

                    if (TestWorker.CancellationPending)
                    {
                        CloseAllInstrs(true);
                        e.Cancel = true;
                        return;
                    }

                    #endregion

                    _specCount = acuItem.Items.Count;
                    var boardIndex = acuItem.Index;
                    if (boardIndex < 0 || boardIndex > 7) continue;
                    _report.AcuIndex = boardIndex + 1; //ACU Index显示值从1开始
                    _daq.OpenBoard((byte) boardIndex);
                    if (!AcuExecute(boardIndex))
                    {
                        continue;
                    }
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
                    if (!TestAcuCurr(acuItem))
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

        #region 碰撞输出测试

        /// <summary>
        /// 碰撞输出测试
        /// </summary>
        /// <param name="acuItem"></param>
        /// <returns></returns>
        private bool TestCrashOut(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;
                if (items.Contains((int) CrashOutTest.Crash1))
                {
                    var res = TestFrame((int) CrashOutTest.Crash1, () => GetCrashOut(acuItem.Index, CrashOutTest.Crash1));
                    if (!res) return true;
                }
                if (items.Contains((int) CrashOutTest.Crash2))
                {
                    var res = TestFrame((int) CrashOutTest.Crash2, () => GetCrashOut(acuItem.Index, CrashOutTest.Crash2));
                    if (!res) return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        /// <summary>
        /// 获取碰撞输出状态
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="crashOut"></param>
        /// <returns></returns>
        private double GetCrashOut(int acuIndex, CrashOutTest crashOut)
        {
            #region 若取消测试

            if (TestWorker.CancellationPending)
            {
                TestEventArgs.Cancel = true;
                _acu.Stop();
                CloseAllInstrs(true);
                return (double) TestResult.Cancelled;
            }

            #endregion

            double res;
            int crashIndex, relayIndex;
            switch (crashOut)
            {
                case CrashOutTest.Crash1:
                    crashIndex = 1;
                    relayIndex = 277;
                    break;
                case CrashOutTest.Crash2:
                    crashIndex = 2;
                    relayIndex = 340;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("crashOut", crashOut, null);
            }
            try
            {
                //A:ACU、继电器准备
                AcuExecute(acuIndex, () => _acu.EnableCrashOut(crashIndex));
                _daq.SetSubRelayStatus((byte) acuIndex, relayIndex, true);
                //B:开始测试
                short[] buffer;
                _daq.SetCrashConfig(out buffer);
                var voltBuf = new double[1000];
                var j = 0;
                for (int i = 0; i < 5000; i++)
                {
                    if (i%5 == 0)
                    {
                        voltBuf[j] = buffer[i]*10D/32768D;
                        j++;
                    }
                }
                _daq.GetCrashCheck(TestCondition.CrashOutType, voltBuf);
                res = (double) TestResult.Passed;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(LogFileName, ex);
                res = (double) TestResult.Failed;
            }
            finally
            {
                //C:继电器复位 
                _daq.SetSubRelayStatus((byte) acuIndex, relayIndex, false);
            }
            return res;
        }

        #endregion

        #region ACU电流测试

        /// <summary>
        /// ACU电流测试
        /// </summary>
        /// <param name="acuItem"></param>
        /// <returns></returns>
        private bool TestAcuCurr(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;
                if (!items.Contains(AcuCurrentItemIndex)) return true;
                TestFrame(AcuCurrentItemIndex, () => FindCurr(acuItem.Index));
                return true;
            }
            catch (Exception ex)
            {
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        private double FindCurr(int acuIndex)
        {
            try
            {
                _daq.SetSubRelayStatus((byte) acuIndex, 267, true);
                double res = 0;
                for (int i = 0; i < 5; i++)
                {
                    #region 若取消测试

                    if (TestWorker.CancellationPending)
                    {
                        TestEventArgs.Cancel = true;
                        _acu.Stop();
                        CloseAllInstrs(true);
                        return (double) TestResult.Cancelled;
                    }

                    #endregion

                    Thread.Sleep(200);
                    res += _dmm.GetCurrent();
                }
                _daq.SetSubRelayStatus((byte) acuIndex, 267, false);
                return res/5D;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(LogFileName, ex);
                return (double) TestResult.Failed;
            }
        }

        #endregion

        #region 能量保持测试方法

        /// <summary>
        /// 能量保持时间测试
        /// </summary>
        /// <param name="acuItem"></param>
        /// <returns></returns>
        private bool TestHoldTime(AcuItems acuItem)
        {
            try
            {
                var items = acuItem.Items;
                if (items.Contains(AcuHoldTimeItemIndex))
                {
                    TestFrame(AcuHoldTimeItemIndex, () => FindHoldTime(acuItem.Index));
                }
                return true;
            }
            catch (Exception ex)
            {
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
        }

        /// <summary>
        /// 查找能量保持时间
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <returns></returns>
        private double FindHoldTime(int acuIndex)
        {
            //0:DAQ操作
            _daq.SetSubRelayStatus((byte) acuIndex, 264, true);
            _daq.SetSubRelayStatus((byte) acuIndex, 265, true);
            _daq.SetSubRelayStatus((byte) acuIndex, 272, true);
            //1:DMM操作
            AcuExecute(acuIndex, () => _acu.EnableWarnLamp(1, true));
            //var volt = ActualVolt;
            _dmm.SetAutoZero(BllDmm.AutoZero.Off);
            _dmm.Display = false;
            AcuExecute(acuIndex, () => _acu.Stop()); //将ACU退出S模式
            Thread.Sleep(1000);
            //2:开始测试
            _daq.SetSubRelayStatus((byte) acuIndex, 265, false); //ACU断电
            var tick = Environment.TickCount; //开始时间
            var data = _dmm.GetVoltage(2500, 20, 0.1); //TODO:同步到UI
            var duration = Environment.TickCount - tick;
            //3:计算HoldTime//QUES:数据待确认
            var len = data.Length;
            var index = 0;
            for (var i = 0; i < len; i++)
            {
                if (Math.Abs(data[i] - 3) < 0.5)
                {
                    index = i;
                    break;
                }
            }
            var holdTime = Convert.ToDouble(index)/Convert.ToDouble(len)*Convert.ToDouble(duration);
            //4:恢复原状
            _dmm.Reset();
            _daq.CloseBoard((byte) acuIndex);
            _daq.OpenBoard((byte) acuIndex);
            AcuExecute(acuIndex);
            var spec = FindSpec(AcuHoldTimeItemIndex);
            SetSpecResult(acuIndex, ref spec, holdTime);
            if (index == 0) return (double)TestResult.Failed;
            return holdTime;
        }

        #endregion

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
                    if (!TestFrame((int) WLTest.WL1VoltOn, () => GetWLVolt(acuItem.Index, 1, true)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL1VoltOff))
                {
                    if (!TestFrame((int) WLTest.WL1VoltOff, () => GetWLVolt(acuItem.Index, 1, false)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2VoltOn))
                {
                    if (!TestFrame((int) WLTest.WL2VoltOn, () => GetWLVolt(acuItem.Index, 2, true)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2VoltOff))
                {
                    if (!TestFrame((int) WLTest.WL2VoltOff, () => GetWLVolt(acuItem.Index, 2, false)))
                    {
                        return true;
                    }
                }

                #endregion

                #region 告警灯电流测试

                if (items.Contains((int) WLTest.WL1CurrentNormal))
                {
                    if (
                        !TestFrame((int) WLTest.WL1CurrentNormal,
                            () => GetWLCurr(acuItem.Index, WLTest.WL1CurrentNormal)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL1CurrentShort))
                {
                    if (!TestFrame((int) WLTest.WL1CurrentShort, () => GetWLCurr(acuItem.Index, WLTest.WL1CurrentShort)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2CurrentNormal))
                {
                    if (
                        !TestFrame((int) WLTest.WL2CurrentNormal,
                            () => GetWLCurr(acuItem.Index, WLTest.WL2CurrentNormal)))
                    {
                        return true;
                    }
                }
                if (items.Contains((int) WLTest.WL2CurrentShort))
                {
                    if (!TestFrame((int) WLTest.WL2CurrentShort, () => GetWLCurr(acuItem.Index, WLTest.WL2CurrentShort)))
                    {
                        return true;
                    }
                }

                #endregion

                return true;
            }
            catch (Exception ex)
            {
                var e = new Exception(string.Format("ACU{0}异常：{1}", acuItem.Index, ex.Message));
                LogHelper.WriteErrorLog(LogFileName, e);
                return false;
            }
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
                CloseAllInstrs(true);
                return (double) TestResult.Cancelled;
                ;
            }

            #endregion

            int wlIndex, relayIndex, relayIndex2 = -1;
            switch (wlTest)
            {
                default:
                    LogHelper.WriteWarningLog(LogFileName, "WLTest赋值错误：" + wlTest);
                    return (double) TestResult.Cancelled;
                    ;
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
                CloseAllInstrs(true);
                return (double) TestResult.Cancelled;
                ;
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
            var res = ActualVolt;
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
                            var squib = iSquib;
                            var mode = iMode;
                            var res = TestFrame(itemIndex, () => TestSquib(acuItem.Index, squib, mode));
                            if (!res) return true;
                        }
                    }
                }
                //B:Belt测试
                //TODO:规范需要修改
                //同一测试情况归于一类
                for (int iMode = 1; iMode <= BeltModeNum; iMode++)
                {
                    for (int iBelt = 1; iBelt <= BeltNum; iBelt++)
                    {
                        var itemIndex = BeltNum*(iMode - 1) + iBelt + SquibNum*SquibModeNum;
                        Debug.WriteLine(itemIndex);
                        if (acuItem.Items.Contains(itemIndex))
                        {
                            var belt = iBelt;
                            var mode = iMode;
                            var res = TestFrame(itemIndex, () => TestBelt(acuItem.Index, belt, mode));
                            if (!res) return true;
                        }
                    }
                }
                //C:Volt测试
                for (int iMode = 1; iMode <= VoltModeNum; iMode++)
                {
                    var itemIndex = VoltNum*(iMode - 1) + 1 + SquibNum*SquibModeNum + BeltModeNum*BeltNum;
                    Debug.WriteLine(itemIndex);
                    if (acuItem.Items.Contains(itemIndex))
                    {
                        var mode = iMode;
                        var res = TestFrame(itemIndex, () => TestVolt(acuItem.Index, mode));
                        if (!res) return true;
                    }
                }
                _pwr.OutputVoltage = SettingVolt; //电压测试完成后将外围电压恢复到原先状态
                _pwr.OutPutState = true; //可省略，保险起见保留此句
                //D:SIS测试
                for (int iMode = 1; iMode <= SisModeNum; iMode++)
                {
                    for (int iSis = 1; iSis <= SisNum; iSis++)
                    {
                        var itemIndex = SisNum*(iMode - 1) + iSis + SquibNum*SquibModeNum + BeltModeNum*BeltNum +
                                        VoltModeNum*VoltNum;
                        Debug.WriteLine("sis"+itemIndex);
                        if (acuItem.Items.Contains(itemIndex))
                        {
                            var sis = iSis;
                            var mode = iMode;
                            var res = TestFrame(itemIndex, () => TestSis(acuItem.Index, sis, mode));
                            if (!res) return true;
                        }
                    }
                }
                return true; //测试过程无异常
            }
            catch (Exception ex)
            {
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
            var itemIndex = SisNum*(mode - 1) + sisIndex + SquibNum*SquibModeNum + BeltModeNum*BeltNum +
                            VoltModeNum*VoltNum;
            var spec = FindSpec(itemIndex, out minValue, out maxValue, out dtc);
            //0：先设置好电阻箱
            _prs1.SetResistance(maxValue);
            //A:打开继电器
            _daq.SetSisInTestMode(acuIndex, sisIndex, (SisMode) mode);
            //B:开始测试
            //var res = FindSis(minValue, maxValue, dtc, mode);
            var res = FindRes(_prs1, false, minValue, maxValue, dtc);
            if ((int) res == (int) TestResult.Failed) return (double) TestResult.Failed;
            if ((int) res == (int) TestResult.Cancelled) return (double) TestResult.Cancelled;
            _daq.SetSisInReadMode(acuIndex, sisIndex);
            res = _dmm.GetFourWireRes(); //该返回值为实际测量值
            res -= Properties.Settings.Default.AmendResistance; //根据线阻 修正测试结果
            SetSpecResult(acuIndex, ref spec, res);
            //C:复位继电器
            _daq.SetSisReset(acuIndex, sisIndex);
            return res;
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
            //0：先设置好电阻箱
            // 升序 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 降序 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool ascending;
            BllPrs prs;
            SelectResAndDirection((BeltMode) mode, out ascending, out prs);
            prs.SetResistance(maxValue);
            //A:打开继电器
            _daq.SetBeltInTestMode(acuIndex, belt, (BeltMode) mode);
            //B:开始测试
            var res = FindRes(prs, ascending, minValue, maxValue, dtc);
            if ((int) res == (int) TestResult.Failed) return (double) TestResult.Failed;
            if ((int) res == (int) TestResult.Cancelled) return (double) TestResult.Cancelled;
            _daq.SetBeltInReadMode(acuIndex, belt);
            res = _dmm.GetFourWireRes(); //该返回值为实际测量值
            res -= Properties.Settings.Default.AmendResistance; //根据线阻 修正测试结果
            SetSpecResult(acuIndex, ref spec, res);
            //C:复位继电器
            _daq.SetBeltReset(acuIndex, belt);
            return res;
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
            var ascending = (BatteryMode) mode == BatteryMode.TooHigh;
            //A:打开继电器-正常情况下已打开
            //B:开始测试
            var res = FindRes(_pwr, ascending, minValue, maxValue, dtc);
            if ((int) res == (int) TestResult.Failed) return (double) TestResult.Failed;
            if ((int) res == (int) TestResult.Cancelled) return (double) TestResult.Cancelled;
            _daq.SetSubRelayStatus((byte) acuIndex, 266, true);
            res = ActualVolt; //该返回值为实际测量值
            SetSpecResult(acuIndex,ref spec,res);
            //C:复位继电器
            _daq.SetSubRelayStatus((byte) acuIndex, 266, false);

            return res;
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
            //0：先设置好电阻箱
            // 升序 小电阻肯定没有：表示错误码在测试规范的大值处附近
            // 降序 大电阻肯定没有：表示错误码在测试范围的小值处附近
            bool ascending;
            BllPrs prs;
            SelectResAndDirection((SquibMode) mode, out ascending, out prs);
            prs.SetResistance(maxValue);
            //A:打开继电器
            _daq.SetFcInTestMode(acuIndex, squib, (SquibMode) mode);
            //B:开始测试
            var res = FindRes(prs, ascending, minValue, maxValue, dtc);
            if ((int) res == (int) TestResult.Failed) return (double) TestResult.Failed;
            if ((int) res == (int) TestResult.Cancelled) return (double) TestResult.Cancelled;
            _daq.SetFcInReadMode(acuIndex, squib, (SquibMode) mode);
            res = _dmm.GetFourWireRes(); //该返回值为实际测量值
            res -= Properties.Settings.Default.AmendResistance; //根据线阻 修正测试结果
            SetSpecResult(acuIndex, ref spec, res);
            //C:复位继电器
            _daq.SetFcReset(acuIndex, squib);
            return res;
        }

        /// <summary>
        /// 根据取值范围及故障码查找临界阻值
        /// </summary>
        /// <param name="instr">选用的仪器</param>
        /// <param name="ascending">是否升序查找</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="dtc">故障码</param>
        /// <returns></returns>
        private double FindRes<T>(T instr, bool ascending, double minValue, double maxValue, byte dtc) where T : BllVisa
        {
            var findRange = false;
            for (int i = 0; i < 3; i++)
            {
                #region 若取消测试

                if (TestWorker.CancellationPending)
                {
                    TestEventArgs.Cancel = true;
                    _acu.Stop();
                    CloseAllInstrs(true);
                    return (double) TestResult.Cancelled;
                }

                #endregion

                var findMin = FindDtc(instr, minValue, dtc);
                var findMax = FindDtc(instr, maxValue, dtc);

                var findresult = GetFindResult(ascending, findMin, findMax);

                #region 根据FindResult值重新设定大小范围

                switch (findresult)
                {
                    case FindResult.InBetween:
                        findRange = true;
                        break;
                    case FindResult.UnderMin:
                        maxValue = minValue;
                        minValue = minValue/2.0;
                        break;
                    case FindResult.AboveMax:
                        minValue = maxValue;
                        maxValue = maxValue*2.0;
                        break;
                }
                if (findRange)
                {
                    break;
                }

                #endregion
            }
            if (!findRange) return (double) TestResult.Failed;
            while (true)
            {
                #region 若取消测试

                if (TestWorker.CancellationPending)
                {
                    TestEventArgs.Cancel = true;
                    _acu.Stop();
                    CloseAllInstrs(true);
                    return (double) TestResult.Cancelled;
                }

                #endregion

                var midValue = (minValue + maxValue)*0.5;
                //若最大值及最小值之差足够小则返回两者平均值
                if (maxValue - minValue <= GlobalConst.Precision)
                {
                    return midValue;
                }
                var findMid = FindDtc(instr, midValue, dtc);
                if ((ascending && findMid) || (!ascending && !findMid))
                {
                    maxValue = midValue;
                }
                else
                {
                    minValue = midValue;
                }
            }
        }
        #endregion

        /// <summary>
        /// 测试框架
        /// </summary>
        /// <param name="specIndex"></param>
        /// <param name="func">测试方法委托</param>
        /// <returns>测试是否完成,取消则返回False</returns>
        private bool TestFrame(int specIndex, Func<double> func)
        {
            var spec = FindSpec(specIndex);
            spec.ResultInfo = "Progressing...";
            var progress = GetProgress(specIndex, _tvIndex);
            TestWorker.ReportProgress(progress, _report);
            var res = func();
            switch ((int) res)
            {
                case (int) TestResult.Cancelled:
                    spec.ResultInfo = "Cancelled";
                    TestWorker.ReportProgress(progress, _report);
                    return false;
                case (int) TestResult.Failed:
                    spec.ResultInfo = "Failed";
                    break;
                default:
                    spec.ResultInfo = "Passed";
                    //spec.ResultValueList[acuIndex] = res;
                    break;
            }
            TestWorker.ReportProgress(progress, _report);
            return true;
        }

        #region 属性字段

        public TvType SelectedTvType { get; private set; }
        public NewBackgroundWorker TestWorker { get; private set; }
        private DoWorkEventArgs TestEventArgs { get; set; }
        private NewBackgroundWorker ChamberStay { get; set; }
        public NewBackgroundWorker ChamberReset { get; private set; }
        private TestCondition TestCondition { get; set; }
        private readonly Report _report;
        private KeyValuePair<TvType, double[]> _tvItem; //正在测试条件组合
        private int _tvIndex; //正在测试温度电压组合的项数
        private int _tvCount; //需要测试温度电压组合总数
        private int _specCount; //需要测试项的总数

        #region 实际值

        /// <summary>
        /// 电压实际测量值
        /// </summary>
        private double ActualVolt
        {
            get
            {
                if (_dmm == null) return 0;
                var volt = _dmm.GetVoltage();
                _report.ActualVolt = volt;
                return volt;
            }
        }

        /// <summary>
        /// 温度实际测量值
        /// </summary>
        private double ActualTemp
        {
            get
            {
                if (_chamber == null) return 0;
                var temp = _chamber.GetTemp();
                _report.ActualTemp = temp;
                return temp;
            }
        }

        #endregion

        #region 目标值

        private double _settingVolt;

        private double SettingVolt
        {
            get { return _settingVolt; }
            set
            {
                _settingVolt = value;
                _report.SettingVolt = value;
            }
        }

        private double _settingTemp;

        private double SettingTemp
        {
            get { return _settingTemp; }
            set
            {
                _settingTemp = value;
                _report.SettingTemp = value;
            }
        }

        #endregion

        #region 从规范中归纳的常量值 SPEC_unit.txt

        private const int SquibNum = 16; //回路数常量
        private const int SquibModeNum = 4; //回路测试情况常量
        private const int BeltNum = 3; //开关数常量
        private const int BeltModeNum = 3; //开关测试情况常量//bug：理论值为4
        private const int VoltNum = 1; //测电压处常量
        private const int VoltModeNum = 2; //电压测试情况常量
        private const int SisNum = 6; //侧传感器数常量
        private const int SisModeNum = 2; //侧传感器测试情况常量

        private enum WLTest : int
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

        private const int AcuHoldTimeItemIndex = 96;
        private const int AcuCurrentItemIndex = 97;

        private enum CrashOutTest : int
        {
            Crash1 = 98,
            Crash2 = 99,
        }

        #endregion

        /// <summary>
        /// 配置中的规范集合
        /// </summary>
        private readonly List<SpecItem> _specUnits;

        private const string LogFileName = "BllMain";

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

        private void SelectResAndDirection(BeltMode mode, out bool ascending, out BllPrs prs)
        {
            #region 根据测试模式设置寻找方向及电阻箱选择

            switch (mode)
            {
                default:
                case BeltMode.UnbuckledOrDisabled:
                    ascending = false;
                    prs = _prs1;
                    break;
                case BeltMode.BuckledOrEnabled:
                    ascending = true;
                    prs = _prs1;
                    break;
                case BeltMode.ToGround:
                    throw new NotImplementedException("配置待定,暂时不测试");
                    ascending = false;
                    prs = _prs1;
                    break;
                case BeltMode.ToBattery://QUES：需要确认
                    ascending = false;
                    prs = _prs1;
                    break;
            }

            #endregion
        }
        /// <summary>
        /// 根据测试模式设置寻找方向及电阻箱选择
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="ascending"></param>
        /// <param name="prs"></param>
        private void SelectResAndDirection(SquibMode mode, out bool ascending, out BllPrs prs)
        {
            #region 根据测试模式设置寻找方向及电阻箱选择

            switch (mode)
            {
                default:
                case SquibMode.TooHigh:
                    ascending = true;
                    prs = _prs0;
                    break;
                case SquibMode.TooLow:
                    ascending = false;
                    prs = _prs0;
                    break;
                case SquibMode.ToGround:
                    ascending = false;
                    prs = _prs1;
                    break;
                case SquibMode.ToBattery:
                    ascending = false;
                    prs = _prs1;
                    break;
            }

            #endregion
        }

        /// <summary>
        /// 根据ACU反馈给FindResult赋值
        /// </summary>
        /// <param name="ascending">查找方向</param>
        /// <param name="findMin">最小值是否找到DTC</param>
        /// <param name="findMax">最大值是否找到DTC</param>
        /// <returns></returns>
        private static FindResult GetFindResult(bool ascending, bool findMin, bool findMax)
        {
            if (!ascending)
            {
                findMax = !findMax;
                findMin = !findMin;
            }
            if (!findMax && !findMin)
            {
                return FindResult.AboveMax;
            }
            if (findMax && !findMin)
            {
                return FindResult.InBetween;
            }
            if (findMax)
            {
                return FindResult.UnderMin;
            }
            return FindResult.Error;
        }

        /// <summary>
        /// 设置电阻箱输出阻值并检查ACU是否报错
        /// </summary>
        /// <param name="instr"></param>
        /// <param name="value"></param>
        /// <param name="dtc"></param>
        /// <returns></returns>
        private bool FindDtc<T>(T instr,double value,byte dtc) where T:BllVisa
        {
            var prs = instr as BllPrs;
            prs.IfNotNull(p => p.SetResistance(value - Properties.Settings.Default.AmendResistance));//QUES:此处是否应该减去线阻
            var pwr = instr as BllPwr;
            pwr.IfNotNull(p =>
            {
                p.OutputVoltage = value;
                p.OutPutState = true;
            });
            var hasFoundDtc = _acu.HasFoundDtc(dtc);
            return hasFoundDtc;
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

                var canTest = _daq.CheckPowerStatus(SettingVolt);
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
                    _report.AcuName = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", _acu.RomCs, _acu.RomVer, _acu.Mlfb, _acu.AcuSn, _acu.LabVer, _acu.ParaVer);
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
            CloseAllInstrs(false);
            Thread.Sleep(500);
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

        /// <summary>
        /// 给相应ACU结果赋值
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="spec"></param>
        /// <param name="value"></param>
        private void SetSpecResult(int acuIndex, ref SpecItem spec, object value)
        {
            switch (acuIndex)
            {
                case 0:
                    spec.AcuResult1 = value;
                    return;
                case 1:
                    spec.AcuResult2 = value;
                    return;
                case 2:
                    spec.AcuResult3 = value;
                    return;
                case 3:
                    spec.AcuResult4 = value;
                    return;
                case 4:
                    spec.AcuResult5 = value;
                    return;
                case 5:
                    spec.AcuResult6 = value;
                    return;
                case 6:
                    spec.AcuResult7 = value;
                    return;
                case 7:
                    spec.AcuResult8 = value;
                    return;
            }
        }

        /// <summary>
        /// 根据测试项和温度电压组合项计算进度值
        /// </summary>
        /// <param name="specIndex"></param>
        /// <param name="tvIndex"></param>
        /// <returns></returns>
        private int GetProgress(int specIndex, int tvIndex)
        {
            return (int) ((Convert.ToDouble(specIndex)/Convert.ToDouble(_specCount) + tvIndex)/_tvCount*100);
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 开始测试
        /// </summary>
        /// <param name="testCondition"></param>
        public void Start(TestCondition testCondition)
        {
            TestCondition = testCondition;
            var tvItems = TestCondition.Temperature.Enable
                ? TestCondition.TvItems
                : RemoveTempList(TestCondition.TvItems);
            TestWorker.RunWorkerAsync(tvItems);
        }

        /// <summary>
        /// 关闭所有仪器
        /// </summary>
        /// <param name="closeChamber">是否关闭温箱</param>
        private void CloseAllInstrs(bool closeChamber)
        {
            _pwr.IfNotNull(p => p.Dispose());
            _prs0.IfNotNull(prs => prs.Dispose());
            _prs1.IfNotNull(prs => prs.Dispose());
            if (closeChamber && _chamber != null)
            {
                ChamberReset.RunWorkerAsync();
            }
            _dmm.IfNotNull(d => d.Dispose());
            _daq.IfNotNull(d => d.Dispose());
        }

        #endregion
    }
}