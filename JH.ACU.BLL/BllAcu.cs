using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using JH.ACU.DAL;
using Ivi.Visa;
using NationalInstruments.Visa;


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
            _dalSerial = new DalSerial(name);
            _dalSerial._serial = new SerialSession(name) { BaudRate = 9600 };
        }

        #endregion

        #region 字段、枚举
        private readonly DalSerial _dalSerial;
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
        private byte[] AddChecksum(byte[] data)//
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
            _dalSerial._serial.RawIO.Write(dataWithCs);
            //if (!_dalSerial.WaitForResponse(timeout, dataWithCs.Length+1))
            //{
            //    return errorArray;//返回自定义错误报文
            //}
            Thread.Sleep(timeout);
            var length = _dalSerial._serial.BytesAvailable;
            var res = _dalSerial._serial.RawIO.Read(length);
            if (res.Length - dataWithCs.Length == 1)
            {
                //执行ECU reset时返回一个字节
                return res.Skip(dataWithCs.Length).ToArray();
            }
            return res.Length == dataWithCs.Length + res[dataWithCs.Length] + 1 ? res.Skip(1).ToArray() : errorArray;
        }

        public bool Start()
        {
            _dalSerial._serial.Clear();
            _dalSerial._serial.RawIO.Write(new byte[] { 0x53 });
            if (!_dalSerial.WaitForResponse(100, 21)) return false;
            var data = _dalSerial._serial.RawIO.Read(21); //0:53 1:length 2:cfc 3(17):id 
            RomCs = data.Skip(3).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
            RomVer = data.Skip(3 + 2).Take(4).Aggregate("", (current, b) => current + b.ToString("X2"));
            AcuSn = data.Skip(3 + 2 + 4).Take(4).Aggregate("", (current, b) => current + b.ToString("X2"));
            LabVer = data.Skip(3 + 2 + 4 + 4).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
            Mlfb = _dalSerial.ByteArrayToAscii(data.Skip(3 + 2 + 4 + 4 + 2).Take(3).ToArray());
            ParaVer = data.Skip(3 + 2 + 4 + 4 + 2 + 3).Take(2).Aggregate("", (current, b) => current + b.ToString("X2"));
            var cs = new[] {data[data.Length-1]};
            _dalSerial._serial.RawIO.Write(cs);
            if (!_dalSerial.WaitForResponse(100, 1)) return false;
            var res = _dalSerial._serial.RawIO.Read(2)[1];
            return res == 0x4f;
        }

        public bool Stop()
        {
            var data = new byte[] {0x79, 0xff};
            return WriteAndRead(data)[0] == 0xff;
        }

        #endregion

    }
}
