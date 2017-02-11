using System;
using Ivi.Visa;
using JH.ACU.BLL.Abstract;
using JH.ACU.Model;
using NationalInstruments.Visa;
using System.Collections.Generic;

namespace JH.ACU.BLL.Instruments
{
    /// <summary>
    /// 数字万用表操作类 仅适用于34401型号
    /// </summary>
    public class BllDmm : BllVisa
    {
        #region 构造函数

        public BllDmm(InstrName instr = InstrName.DMM) : base(instr)
        {
            /* 基本编程顺序：
               * 1、将万用表设定在一已知的状态（通常在复位状态）
               * 2、将万用表的设定改变为想要的配置
               * 3、设定触发条件
               * 4、启动或导引万用表开始测量
               * 5、触发万用表进行测量
               * 6、从输出缓冲器或内部存储器上取出读数
               * 7、将测量的数据读进总线控制器
               */
            _dmm = new Dmm();
        }

        #endregion

        #region 属性 字段

        private const string DcVolt = ":CONF:VOLT:DC ";
        private const string AcVolt = ":CONF:VOLT:AC ";
        private const string TwoWireRes = ":CONF:RES ";
        private const string FourWireRes = ":CONF:FRES ";
        private const string DcCurr = ":CONF:CURR:DC ";
        private const string AcCurr = ":CONF:CURR:AC ";
        private const string Frequency = ":CONF:FREQ ";
        private const string Period = ":CONF:PER ";
        private const string Continuity = ":CONF:CONT ";
        private const string DiodeChecking = ":CONF:DIOD ";
        private const string DcVoltRatio = ":CONF:VOLT:DC:RAT ";
        private const string Temperature = ":CONF:TEMP ";
        private const string Capacitance = ":CONF:CAP ";

        /// <summary>
        /// 获取或设置前面板显示器状态
        /// </summary>
        private bool Display
        {
            get { return WriteAndRead("DISPlay?") == "1"; }
            set { WriteNoRead(value ? "DISPlay ON" : "DISPlay OFF"); }
        }

        private readonly Dmm _dmm;

        private class Dmm
        {
            public TriggerSource TriggerSource { get; set; }
            public MathStorage MathStorage { get; set; }
        }

        private enum ControlMode
        {
            Local,
            Remote,
            RemoteWithLock
        }

        private enum TriggerSource
        {
            Immediate,
            Bus,
            External,
            Internal,
        }

        private enum MathStorage
        {
            Off,
            InternalBuffer,
            OutputBuffer,
        }

        public enum AutoZero
        {
            On,
            Off,
            Once,
        }

        /// <summary>
        /// 获取或设置采样点数
        /// </summary>
        [Obsolete("测试失败", true)]
        public int SampleCount
        {
            get { return Convert.ToInt32(WriteAndRead("SAMP:COUN?")); }
            set { WriteAndRead(string.Format("SAMP:COUN {0}", value)); }
        }

        #endregion

        #region 私有函数


        private void DefaultSetup()
        {
            WriteNoRead("*ESE 60;*SRE 56;*CLS;:STAT:QUES:ENAB 32767");
        }

        private void SetControlMode(ControlMode mode, int timeout = 30000)
        {
            if (MbSession.HardwareInterfaceType != HardwareInterfaceType.Serial) return;
            switch (mode)
            {
                case ControlMode.Local:
                    WriteNoRead("SYST:LOC");
                    return; //退出该方法
                case ControlMode.Remote:
                    WriteNoRead("SYST:REM");

                    break;
                case ControlMode.RemoteWithLock:
                    WriteNoRead("SYST:RWL");

                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
            WaitComplete(timeout);
        }

        private void DmmClear()
        {
            switch (MbSession.HardwareInterfaceType)
            {
                case HardwareInterfaceType.Gpib:
                    MbSession.Clear();
                    break;

                case HardwareInterfaceType.Serial:
                    var serial = MbSession as SerialSession;
                    try
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        serial.FlowControl = SerialFlowControlModes.DtrDsr;
                    }
                    catch (Exception)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        serial.FlowControl = SerialFlowControlModes.None;
                    }
                    serial.ReadTermination = SerialTerminationMethod.TerminationCharacter;
                    serial.WriteTermination = SerialTerminationMethod.TerminationCharacter;
                    serial.TerminationCharacter = 0x0A;
                    serial.BaudRate = Config.Serial.BaudRate;
                    serial.Parity = Config.Serial.Parity;
                    serial.DataBits = Config.Serial.DataBits;
                    serial.StopBits = SerialStopBitsMode.Two;
                    serial.IOProtocol = IOProtocol.Ieee4882;
                    serial.Flush(IOBuffers.ReadWrite, true);
                    serial.SetBufferSize(IOBuffers.ReadWrite, 4096);
                    SetControlMode(ControlMode.Remote);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //QUES:need public?
        private void SetTrigger(TriggerSource trigger, bool isAuto = true, double delay = 0)
        {
            var command = isAuto ? ":TRIG:DEL:AUTO ON;" : ":TRIG:DEL " + delay;
            switch (trigger)
            {
                case TriggerSource.Immediate:
                    command = ":TRIG:SOUR IMM;" + command;
                    break;
                case TriggerSource.Bus:
                    command = ":TRIG:SOUR BUS;" + command;

                    break;
                case TriggerSource.External:
                    command = ":TRIG:SOUR EXT;" + command;

                    break;
                case TriggerSource.Internal:
                    //The 34401 and 34410 do not support internal trigger source. 
                    command = ":TRIG:SOUR INT;" + command;
                    return;
                default:
                    throw new ArgumentOutOfRangeException("trigger", trigger, null);
            }
            _dmm.TriggerSource = trigger;
            WriteNoRead(command);
        }
        [Obsolete("测试失败",true)]
        private void SetMultiPoint(int triggerCount = 1, int sampleCount = 1)
        {
            if (triggerCount <=1 || sampleCount <= 1)
            {
                return;
            }
            var command = string.Format(":TRIG:COUN {0};", triggerCount);
            command += string.Format(":SAMP:COUN {0};", sampleCount);
            WriteNoRead(command);
        }

        private double DmmRead(Dmm dmm)
        {
            //throw new NotImplementedException("返回值需要转化");
            string command;
            if (dmm.TriggerSource == TriggerSource.Bus || dmm.MathStorage == MathStorage.InternalBuffer)
            {
                command = "INIT;";
                WriteNoRead(command);
                return Convert.ToDouble(WriteAndRead(":FETC?"));
            }
            else
            {
                command = "READ?";
                return Convert.ToDouble(WriteAndRead(command));
            }

        }

        //private string Fetch()
        //{
        //    throw new NotImplementedException("返回值需要转化");
        //    return WriteAndRead(":FETC?");

        //}

        public void SetAutoZero(AutoZero autoZero)
        {
            string command;
            switch (autoZero)
            {
                case AutoZero.On:
                    command = ":ZERO:AUTO ON;";
                    break;
                case AutoZero.Off:
                    command = ":ZERO:AUTO OFF;";
                    break;
                case AutoZero.Once:
                    command = ":ZERO:AUTO ONCE;";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("autoZero", autoZero, null);
            }
            WriteNoRead(command);
        }

        #endregion

        #region 公有函数

        /// <summary>
        /// 初始化数字万用表
        /// </summary>
        /// <returns></returns>
        public override void Initialize()
        {
            try
            {
                MbSession.TimeoutMilliseconds = 10000;
                DmmClear();
                Display = false; 
#if DEBUG
                Display = true;
#endif

                //SampleCount = 2500;//设置失败
                var id = Idn;
                if (!id.Contains("34401") && !id.Contains("34410") && !id.Contains("4411"))
                {
                    throw new NullReferenceException("数字万用表型号读取异常");
                }
                _dmm.TriggerSource = TriggerSource.Immediate;
                _dmm.MathStorage = MathStorage.Off;
                Reset();
                DefaultSetup();
            }
            catch (Exception ex)
            {
                MbSession.Dispose();
                throw new Exception(Error, ex);
            }
        }

        /// <summary>
        /// Read single point
        /// </summary>
        /// <returns></returns>
        public double Read()
        {
            //throw new NotImplementedException("返回值需要转化");
            SetTrigger(TriggerSource.Immediate);
            //SetMultiPoint(1, 1);
            return DmmRead(_dmm);
        }

        /// <summary>
        /// Read multiple point
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <returns></returns>
        public double Read(int sampleCount)
        {
            //throw new NotImplementedException("返回值需要转化");
            SetTrigger(TriggerSource.Immediate);
            //SetMultiPoint(sampleCount: sampleCount);
            return DmmRead(_dmm);
        }

        /// <summary>
        /// Read (math)
        /// </summary>
        /// <param name="resetAfterRead"></param>
        /// <returns></returns>
        public double[] Read(bool resetAfterRead)
        {
            //throw new NotImplementedException("返回值需要转化");
            var command = ":CALC:AVER:MIN?;:CALC:AVER:MAX?;:CALC:AVER:AVER?;:CALC:AVER:COUN?;";
            command += resetAfterRead ? ":CALC:STAT ON;" : "";
            var data = WriteAndRead(command).Split(';');
            var res=new List<double>();
            foreach (var d in data)
            {
                res.Add(Convert.ToDouble(d));
            }
            return res.ToArray();
        }

        /// <summary>
        /// 设置功能、量程、分辨率
        /// </summary>
        /// <param name="function">功能代码</param>
        /// <param name="range">量程</param>
        /// <param name="resolution">分辨率</param>
        public void SetFunction(string function, double range = 0, double resolution = 0)
        {
            if (range == 0 && resolution == 0)
            {
                WriteNoRead(function + "DEF");
                return;
            }
            switch (function)
            {
                case DcVolt:
                case AcVolt:
                case TwoWireRes:
                case FourWireRes:
                case DcCurr:
                case AcCurr:
                case Capacitance:
                    WriteNoRead(function + range + "," + resolution);
                    break;
                case Frequency:
                case Period:
                case Continuity:
                case DiodeChecking:
                case DcVoltRatio:
                case Temperature:
                    WriteNoRead(function + "DEF");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(function, "功能代码不正确");
            }
        }

        /// <summary>
        /// 获取实时电流(single point)
        /// </summary>
        /// <param name="range"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public double GetCurrent(double range = 0, double resolution = 0)
        {
            SetFunction(DcCurr, range, resolution);
            return Read();
        }

        /// <summary>
        /// 获取实时电压(single point)
        /// </summary>
        /// <param name="range"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public double GetVoltage(double range = 0, double resolution = 0)
        {
            SetFunction(DcVolt, range, resolution);
            return Read();
        }

        /// <summary>
        /// 获取四线电阻值(single point)
        /// </summary>
        /// <param name="range"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public double GetFourWireRes(double range = 0, double resolution = 0)
        {
            SetFunction(FourWireRes, range, resolution);
            return Read();
        }

        /// <summary>
        /// 获取两线电阻值(single point)
        /// </summary>
        /// <param name="range"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public double GetRes(double range = 0, double resolution = 0)
        {
            SetFunction(TwoWireRes, range, resolution);
            return Read();
        }

        /// <summary>
        /// 获取频率(single point)
        /// </summary>
        /// <returns></returns>
        public double GetFrequency()
        {
            SetFunction(Frequency);
            return Read();
        }

        #endregion
    }
}