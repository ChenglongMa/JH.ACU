using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.InstrumentConfig;
using NationalInstruments.Restricted;
using NationalInstruments.VisaNS;
using LineState = NationalInstruments.VisaNS.LineState;


namespace JH.ACU.BLL.Instruments
{
    /// <summary>
    /// Acu S模式通讯类
    /// </summary>
    public class BllAcu : IDisposable
    {
        #region 构造函数

        public BllAcu()
        {
            _serial = new SerialSession(BllConfig.GetPortNumber(Config)) {BaudRate = Config.Serial.BaudRate};
        }

        #endregion

        #region 字段、枚举

        private readonly SerialSession _serial;
        private bool _realTimeFlag;
        private bool _isConnected;
        private static Instr Config
        {
            get { return BllConfig.GetInstr(InstrName.Acu); }
        }


        #endregion

        #region 属性

        private List<byte> RealTimeData { get; set; }
        public string RomCs { get; set; }
        public string RomVer { get; set; }
        public string AcuSn { get; set; }
        public string LabVer { get; set; }
        public string Mlfb { get; set; }
        public string ParaVer { get; set; }
        private static readonly byte[] ErrorArray = { 0x0a };

        #endregion

        #region 私有方法

        /// <summary>
        /// 增加校验位
        /// </summary>
        /// <param name="data">data为CFC+(Address)+data</param>
        /// <returns>CFC+(Address)+data+CheckSum</returns>
        private static byte[] AddChecksum(IList<byte> data)
        {
            var addCheckSum = new byte[data.Count + 2];
            var checkSum = (byte) (data.Count + 1);
            addCheckSum[0] = checkSum;

            for (var i = 1; i <= data.Count; i++)
            {
                addCheckSum[i] = data[i - 1];
                checkSum ^= data[i - 1]; //异或求CheckSum
            }

            addCheckSum[data.Count + 1] = checkSum;
            return addCheckSum;
        }

        private void _serial_AnyCharacterReceived(object sender, SerialSessionEventArgs e)
        {
            if (_serial.AvailableNumber <= 0) return;
            var data = _serial.ReadByteArray(_serial.AvailableNumber);
            foreach (var d in data)
            {
                if (!RealTimeData.Contains(d))
                {
                    RealTimeData.Add(d);
                }

            }

        }


        #endregion

        #region 公有方法

        /// <summary>
        /// 发送命令并读取返回值
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="delay">超时时间（默认：200ms）</param>
        /// <returns></returns>
        public byte[] WriteAndRead(byte[] dataBytes, int delay = 200)
        {
            if (dataBytes.IsNullOrEmpty()) return null;
            
            var dataWithCs = AddChecksum(dataBytes);
            _serial.Write(dataWithCs);
            Thread.Sleep(delay);
            var length = _serial.AvailableNumber;
            var res = _serial.ReadByteArray(length);
            if (res.Length - dataWithCs.Length == 1)
            {
                //执行ECU reset时返回一个字节
                return res.Skip(dataWithCs.Length).ToArray();
            }
            return res.Length == dataWithCs.Length + res[dataWithCs.Length] + 1 ? res.Skip(1).ToArray() : ErrorArray;
        }

        public bool Start()
        {
            try
            {
                _serial.Clear();
                _serial.Write(new byte[] {0x53});
                Thread.Sleep(50);
                var data = _serial.ReadByteArray(21); //0:53 1:length 2:cfc 3(17):id 
                RomCs = data.Skip(3).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
                RomVer = data.Skip(3 + 2).Take(4).Aggregate("", (current, b) => current + b.ToString("X2"));
                AcuSn = data.Skip(3 + 2 + 4).Take(4).Aggregate("", (current, b) => current + b.ToString("X2"));
                LabVer = data.Skip(3 + 2 + 4 + 4).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
                Mlfb = data.Skip(3 + 2 + 4 + 4 + 2).Take(3).ToArray().ToAscii();
                ParaVer = data.Skip(3 + 2 + 4 + 4 + 2 + 3)
                    .Take(2)
                    .Aggregate("", (current, b) => current + b.ToString("X2"));
                var cs = new[] {data[data.Length - 1]};
                _serial.Write(cs);
                Thread.Sleep(20);
                var res = _serial.ReadByteArray(2)[1];
                _isConnected = res == 0x4f;
                return _isConnected;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("BllAcu", ex, "ACU 通讯失败");
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (!_isConnected) return true;
                if (_realTimeFlag)
                {
                    StopRtFault();
                }
                var data = new byte[] {0x79, 0xff};
                _isConnected=WriteAndRead(data)[0] != 0xff;
                return !_isConnected;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("BllAcu", ex, "ACU通讯失败");
                return false;
            }
        }

        /// <summary>
        /// 停止读取实时故障
        /// </summary>
        public void StopRtFault()
        {
            _serial.AnyCharacterReceived -= _serial_AnyCharacterReceived;
            _serial.DiscardEvent(SerialSessionEventType.AnyCharacterReceived);
            _serial.BreakState = LineState.Asserted;
            Thread.Sleep(100);
            _serial.BreakState = LineState.Unasserted;
            _serial.Clear();
            _realTimeFlag = false;
        }

        /// <summary>
        /// 读取实时故障
        /// </summary>
        /// <returns></returns>
        public bool RealTimeService(params byte[] command)
        {
            try
            {
                if (command.IsNullOrEmpty())
                {
                    return false;
                }
                RealTimeData = new List<byte>();
                _serial.Write(AddChecksum(command));
                Thread.Sleep(50);
                var data=_serial.ReadByteArray(_serial.AvailableNumber);
                _serial.AnyCharacterReceived += _serial_AnyCharacterReceived;
                _serial.EnableEvent(SerialSessionEventType.AnyCharacterReceived, EventMechanism.Handler);
                _realTimeFlag = true;
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("BllAcu", ex);
                Stop();
                return false;
            }
        }

        public bool FindRealTimeValue(byte code,out double value)//QUES:返回值类型待定
        {
            var connect = RealTimeService(0x77, code);
            if (!connect) throw new Exception("ACU连接失败");
            throw new NotImplementedException("待确认");
        }
        /// <summary>
        /// 指示是否找到指定故障码
        /// </summary>
        /// <param name="code">指定故障码</param>
        /// <returns></returns>
        public bool HasFoundDtc(byte code)
        {
            var connect = RealTimeService(0x75);
            if (!connect) throw new Exception("ACU连接失败");
            //最多重复10次
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(200);
                if (RealTimeData.Contains(code)) //若事件读取到的RealTimeData中含有所需code则返回True;
                {
                    StopRtFault();
                    return true;
                }
            }
            StopRtFault();
            return false;
        }

        /// <summary>
        /// 读取RAM或FRAM数据
        /// </summary>
        /// <param name="memoryRead">RAM or FRAM</param>
        /// <param name="addressHigh">地址高字节</param>
        /// <param name="addressLow">地址低字节</param>
        /// <param name="readCount">读取数量</param>
        /// <param name="memoryStr"></param>
        /// <returns></returns>
        public bool ReadMemory(MemoryRead memoryRead, byte addressHigh, byte addressLow, byte readCount,out string memoryStr)
        {
            var temp = new byte[4];
            temp[0] = (byte) memoryRead;
            temp[1] = addressHigh;
            temp[2] = addressLow;
            temp[3] = readCount;
            var outPut = WriteAndRead(temp);
            //memoryStr = outPut.Aggregate<byte, string>(null, (current, b) => current + "0x" + b.ToString("X2") + " ");
            memoryStr = outPut.Aggregate<byte, string>(null, (current, b) => current + b.ToString("X2"));
            return outPut.Length == 1 && outPut[0] == ErrorArray[0];
        }

        /// <summary>
        /// 设置指定WL开关状态
        /// </summary>
        /// <param name="index">WL1=1;WL2=2</param>
        /// <param name="isOn">是否打开</param>
        /// <returns>命令是否成功执行</returns>
        public bool EnableWarnLamp(int index, bool isOn)
        {
            Command command;
            if (index == 1 && isOn)
            {
                command = Command.WL1On;
            }
            else if (index == 1 && !isOn)
            {
                command = Command.WL1Off;
            }
            else if (index == 2 && isOn)
            {
                command = Command.WL2On;

            }
            else if (index == 2 && !isOn)
            {
                command = Command.WL2Off;
            }
            else
            {
                return false;
            }
            return Execute(command);
        }

        /// <summary>
        /// 赋值相应code,执行内部功能
        /// </summary>
        /// <param name="command"></param>
        /// <returns>执行命令是否成功</returns>
        public bool Execute(Command command)
        {
            byte[] temp = {0x79, (byte) command};
            var res = WriteAndRead(temp);
            if (res.IsNullOrEmpty())
            {
                return false;
            }
            return res[0] != 0x0a;
        }

        public void Dispose()
        {
            Stop();
        }

        #endregion

        /// <summary>
        /// 设置碰撞输出状态
        /// </summary>
        /// <param name="crashIndex"></param>
        /// <returns></returns>
        public bool EnableCrashOut(int crashIndex)
        {
            Command command;
            switch (crashIndex)
            {
                default:
                case 1:
                    command=Command.CrashOutput;
                    break;
                case 2:
                    command=Command.CrashOutput2;
                    break;
            }
            return Execute(command);
        }
    }

    public enum MemoryWrite : byte
    {
        RAM = 0x73,
        FRAM = 0x7B
    }

    public enum MemoryRead : byte
    {
        RAM = 0x72,
        FRAM = 0x7A
    }

    public enum MemoryReadRetFC : byte
    {
        RAM = 0x8D,
        FRAM = 0x85
    }

    public enum MemoryWriteRetFC : byte
    {
        RAM = 0x8C,
        FRAM = 0x84
    }

    /// <summary>
    /// User Function Codes
    /// </summary>
    public enum Command : byte
    {
        ClearDtc = 0,
        ClearEepromCrash = 1,
        ClearFramCrash = 2,
        StartDiag = 3,
        StopDiag = 4,
        FrontInjectionStart = 5,
        FrontInjectionData = 6,
        FrontInjectionStop = 7,
        CrashOutput = 8,
        CrashOutput2 = 8,//TODO:协议中相同
        ClearOperationTimerCounter = 10,
        ClearFramIgnCounter = 11,
        FramMassErase = 13,
        DFlashMassErase = 14,
        SideInjectionStart = 15,
        SideInjectionData = 16,
        SideInjectionStop = 17,
        WL1On = 20,
        WL1Off = 21,
        WL2On = 22,
        WL2Off = 23,
        AlgorithmReset = 30,
        AcuReset = 0xff,
    }
}