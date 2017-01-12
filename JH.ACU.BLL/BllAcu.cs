using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using JH.ACU.DAL;
using Ivi.Visa;
using NationalInstruments.Restricted;
using NationalInstruments.VisaNS;
using LineState = NationalInstruments.VisaNS.LineState;


namespace JH.ACU.BLL
{
    /// <summary>
    /// ACU S模式通讯类
    /// </summary>
    public class BllAcu
    {
        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">端口名称</param>
        public BllAcu(string name)
        {
            _serial = new SerialSession(name) {BaudRate = 9600};
        }

        #endregion

        #region 字段、枚举

        private readonly SerialSession _serial;
        private bool _realTimeFlag;
        public enum MemoryWrite
        {
            RAM = 0x73,
            FRAM = 0x7B
        }

        public enum MemoryRead
        {
            RAM = 0x72,
            FRAM = 0x7A
        }

        public enum MemoryReadRetFC
        {
            RAM = 0x8D,
            FRAM = 0x85
        }

        public enum MemoryWriteRetFC
        {
            RAM = 0x8C,
            FRAM = 0x84
        }

        #endregion


        #region 属性
        public string RomCs { get; set; }
        public string RomVer { get; set; }
        public string AcuSn{ get; set; }
        public string LabVer { get; set; }
        public string Mlfb{ get; set; }
        public string ParaVer { get; set; }
        #endregion

        #region 私有方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">data为CFC+(Address)+data</param>
        /// <returns>CFC+(Address)+data+CheckSum</returns>
        private static byte[] AddChecksum(byte[] data)
        {
            byte[] addCheckSum = new byte[data.Length + 2];
            byte checkSum = (byte)(data.Length + 1);
            addCheckSum[0] = checkSum;
            
            for (int i = 1; i <= data.Length; i++)
            {
                addCheckSum[i] = data[i - 1];
                checkSum ^= data[i - 1];//异或求CheckSum
            }

            addCheckSum[data.Length + 1] = checkSum;
            return addCheckSum;
        }
        public string ByteArrayToAscii(byte[] dataBytes)
        {
            var asciiEncoding = new ASCIIEncoding();
            return asciiEncoding.GetString(dataBytes).Trim();
        }

        public string ByteToAscii(byte data)
        {
            var asciiEncoding = new ASCIIEncoding();
            var dataBytes = new[] { data };
            return asciiEncoding.GetString(dataBytes).Trim();
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 发送命令并读取返回值
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <param name="timeout">超时时间（默认：200ms）</param>
        /// <returns></returns>
        public byte[] WriteAndRead(byte[] dataBytes, int timeout = 200)
        {
            var errorArray = new byte[] {0x0a};
            var dataWithCs = AddChecksum(dataBytes);
            _serial.Write(dataWithCs);
            Thread.Sleep(timeout);
            var length = _serial.AvailableNumber;
            var res = _serial.ReadByteArray(length);
            if (res.Length - dataWithCs.Length == 1)
            {
                //执行ECU reset时返回一个字节
                return res.Skip(dataWithCs.Length).ToArray();
            }
            return res.Length == dataWithCs.Length + res[dataWithCs.Length] + 1 ? res.Skip(1).ToArray() : errorArray;
        }

        public bool Start()
        {
            _serial.Clear();
            _serial.Write(new byte[] { 0x53 });
            Thread.Sleep(50);
            var data = _serial.ReadByteArray(21); //0:53 1:length 2:cfc 3(17):id 
            RomCs = data.Skip(3).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
            RomVer = data.Skip(3 + 2).Take(4).Aggregate("", (current, b) => current + b.ToString("X2"));
            AcuSn = data.Skip(3 + 2 + 4).Take(4).Aggregate("", (current, b) => current + b.ToString("X2"));
            LabVer = data.Skip(3 + 2 + 4 + 4).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
            Mlfb = ByteArrayToAscii(data.Skip(3 + 2 + 4 + 4 + 2).Take(3).ToArray());
            ParaVer = data.Skip(3 + 2 + 4 + 4 + 2 + 3).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
            var cs = new[] {data[data.Length-1]};
            _serial.Write(cs);
            Thread.Sleep(20);
            var res = _serial.ReadByteArray(2)[1];
            return res == 0x4f;
        }

        public bool Stop()
        {
            if (_realTimeFlag)
            {
                StopRtFault();
            }
            var data = new byte[] {0x79, 0xff};
            return WriteAndRead(data)[0] == 0xff;
        }

        public void StopRtFault()
        {
            _serial.AnyCharacterReceived -= _serial_AnyCharacterReceived;
            _serial.DiscardEvent(SerialSessionEventType.AnyCharacterReceived);

            _serial.BreakState=LineState.Asserted;
            Thread.Sleep(100);
            _serial.BreakState=LineState.Unasserted;
            _serial.Clear();
            _realTimeFlag = false;
            Console.WriteLine("已停止");
        }
        public void ReadRtFault()
        {
            try
            {
                byte[] temp = { 0x75 };
                _serial.Write(AddChecksum(temp));
                _serial.AnyCharacterReceived+=_serial_AnyCharacterReceived;
                _serial.EnableEvent(SerialSessionEventType.AnyCharacterReceived, EventMechanism.Handler);
                _realTimeFlag = true;
                //_serial.EnableEvent((EventType)1073684533);//1073684533);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Stop();
            }
        }

        private void _serial_AnyCharacterReceived(object sender, SerialSessionEventArgs e)
        {
            if(_serial.AvailableNumber<=0)return;
            var data = _serial.ReadByteArray(1);
            foreach (var b in data)
            {
                Console.Write(b.ToString("X2")+"\t");
            }
            Console.WriteLine("完毕");
        }

        #endregion

    }
}
