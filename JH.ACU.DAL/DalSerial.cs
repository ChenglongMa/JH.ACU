using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa;
using NationalInstruments.Visa;

namespace JH.ACU.DAL
{
    /// <summary>
    /// 串口基础类，ACU、其他仪器通用方法
    /// </summary>
    public class DalSerial : DalVisa
    {
        public SerialSession _serial;//完成后修改为private

        public DalSerial(string name, int baudRate = 9600)
        {
            _serial = new SerialSession(name)
            {
                BaudRate = baudRate,
                SynchronizeCallbacks = true
            };
        }

        public string ByteArrayToAscii(byte[] dataBytes)
        {
            var asciiEncoding = new ASCIIEncoding();
            return asciiEncoding.GetString(dataBytes).Trim();
        }

        public string ByteToAscii(byte data)
        {
            var asciiEncoding = new ASCIIEncoding();
            var dataBytes = new[] {data};
            return asciiEncoding.GetString(dataBytes).Trim();
        }

        public bool WaitForResponse(int timeout, int dataLength)
        {
            var timeStart = Environment.TickCount;
            while (_serial.BytesAvailable < dataLength)
            {
                if (Environment.TickCount - timeStart > timeout) return false;
                Thread.Sleep(5);
            }

            return true;
        }
    }
}