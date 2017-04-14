using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.InstrumentConfig;

namespace JH.ACU.BLL.Abstract
{
    /// <summary>
    /// Modbus TCP协议
    /// </summary>
    public abstract class BllModbusTcp:IDisposable
    {
        #region 构造函数

        protected BllModbusTcp(InstrName name)
        {
            Config = BllConfig.GetInstr(name);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (Config.TcpIp != null)
            {
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Config.TcpIp.Timeout);
                var ip = new IPEndPoint(IPAddress.Parse(Config.TcpIp.IpAddress), Config.TcpIp.Port);
                _socket.Connect(ip);
            }
            else
            {
                throw new NullReferenceException(string.Format("配置文件缺失,仪器名：{0}", name));
            }
        }

        ~BllModbusTcp()
        {
            Dispose(false);
        }
        #endregion

        #region 属性、字段

        private readonly Socket _socket;
        protected Instr Config { get; set; }
        private bool _disposed;

        /// <summary>
        /// 默认路由地址
        /// </summary>
        protected abstract byte IdentifierAddress { get; set; }

        #region Transaction Identifier

        /// <summary>
        /// 数据序号标识
        /// </summary>
        private ushort CurrentDataIndex { get; set; }

        private ushort NextDataIndex()
        {
            return ++CurrentDataIndex;
        }

        #endregion

        #endregion

        #region 私有方法

        private byte[] Read(int length)
        {
            var data = new byte[length];
            _socket.Receive(data);
            return data;
        }

        private void Write(byte[] data)
        {
            _socket.Send(data);
        }

        /// <summary>
        /// 从设备读取数据
        /// </summary>
        /// <param name="function">功能码</param>
        /// <param name="address">双字节地址,例:0x002B</param>
        /// <param name="count">读取数量</param>
        /// <returns></returns>
        private byte[] Receive(FunctionCode function, ushort address, ushort count)
        {
            var sendData = new List<byte>(255);

            //[1].Send
            sendData.AddRange(NextDataIndex().ToBytes()); //0~1.(Transaction Identifier)
            sendData.AddRange(new byte[] {0, 0}); //2~3:Protocol Identifier,0 = MODBUS protocol
            sendData.AddRange(((ushort) 6).ToBytes()); //4~5:后续的Byte数量（针对读请求，后续为6个byte）
            sendData.Add(IdentifierAddress); //6:Unit Identifier:This field is used for intra-system routing purpose.
            sendData.Add((byte) function); //7.Function Code eg. 3 (Read Multiple Register)
            sendData.AddRange(address.ToBytes()); //8~9.起始地址
            sendData.AddRange(count.ToBytes()); //10~11.需要读取的寄存器数量
            Write(sendData.ToArray()); //发送读请求

            //[2].防止连续读写引起前台UI线程阻塞
            //Application.DoEvents();

            //[3].读取Response Header : 完后会返回8个byte的Response Header
            var receiveData = Read(256); //缓冲区中的数据总量不超过256byte，一次读256byte，防止残余数据影响下次读取
            var identifier = (ushort) ((receiveData[0] << 8) + receiveData[1]);

            //[4].读取返回数据：根据ResponseHeader，读取后续的数据
            if (identifier != CurrentDataIndex) //请求的数据标识与返回的标识不一致，则丢掉数据包
            {
                return new byte[0];
            }
            var length = receiveData[8]; //最后一个字节，记录寄存器中数据的Byte数
            var result = new byte[length];
            Array.Copy(receiveData, 9, result, 0, length);
            return result;
        }

        /// <summary>
        /// 向设备发送数据
        /// </summary>
        /// <param name="function">功能码</param>
        /// <param name="address">双字节初始地址,例0x002B</param>
        /// <param name="data">从#13开始写入的数据值</param>
        private void Send(FunctionCode function, ushort address, params byte[] data)
        {
            if (data.IsNullOrEmpty())
            {
                return;
            }
            //[0]:填充0，清掉剩余的寄存器
            byte[] input;
            if (data.Length < 60)
            {
                input = new byte[60];
                Array.Copy(data, input, data.Length);
            }
            else
            {
                input = data;
            }
            var values = new List<byte>();

            //[1].Write Header：MODBUS Application Protocol header
            values.AddRange(NextDataIndex().ToBytes()); //0~1.(Transaction Identifier)
            values.AddRange(new byte[] {0, 0}); //2~3:Protocol Identifier,0 = MODBUS protocol
            values.AddRange(ValueHelper.ToBytes((byte) (data.Length + 7))); //4~5:后续的Byte数量

            values.Add(IdentifierAddress); //6:Unit Identifier:This field is used for intra-system routing purpose.
            values.Add((byte) function); //7.Function Code eg. 16(0x10) (Write Multiple Register)

            values.AddRange(address.ToBytes()); //8~9.起始地址
            values.AddRange(((ushort) (data.Length/2)).ToBytes()); //10~11.寄存器数量

            values.Add((byte) data.Length); //12.数据的Byte数量

            //[2].增加数据
            values.AddRange(input); //13~End:需要发送的数据

            //[3].写数据
            Write(values.ToArray());

            //[4].防止连续读写引起前台UI线程阻塞
            //Application.DoEvents();

            //[5].读取Response: 写完后会返回12个byte的结果
            var res = Read(12);
            if (res[7] == (byte) function + 0x80)
            {
                var dataStr = data.Aggregate("", (current, b) => current + "0x" + b.ToString("X2") + ",");
                throw new Exception(string.Format("命令发送失败,异常码为:0x{0},起始地址:0x{1},数据为:{2}", res[8].ToString("X2"),
                    address.ToString("X4"), dataStr));
            }
            if (res[7] != (byte) function)
            {
                var dataStr = data.Aggregate("", (current, b) => current + "0x" + b.ToString("X2") + ",");
                throw new Exception(string.Format("命令发送失败,起始地址:0x{0},数据为:{1}",
                    address.ToString("X4"), dataStr));
            }
        }

        #endregion

        #region 公有/继承方法

        /// <summary>
        /// 功能码0x10
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        protected void SendMultiRegisters(ushort address, params byte[] data)
        {
            Send(FunctionCode.WriteReg, address, data);
        }

        /// <summary>
        /// 功能码0x0F
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        protected void SendMultiCoils(ushort address, params byte[] data)
        {
            Send(FunctionCode.WriteCoils, address, data);
        }

        /// <summary>
        /// 功能码0x01
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected byte[] ReceiveCoil(ushort address, ushort count)
        {
            return Receive(FunctionCode.ReadCoils, address, count);
        }

        /// <summary>
        /// 功能码0x03
        /// </summary>
        /// <param name="address"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected byte[] ReceiveRegister(ushort address, ushort count)
        {
            return Receive(FunctionCode.ReadReg, address, count);
        }


        public void Dispose()
        {
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                //清理托管资源
            }
            // 清理非托管资源
            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            _socket.Close();
            _socket.Dispose();

            //让类型知道自己已经被释放
            _disposed = true;
        }

        #endregion

    }

    /// <summary>
    /// 功能码
    /// </summary>
    public enum FunctionCode : byte
    {
        /// <summary>
        /// Read Coil Status
        /// </summary>
        ReadCoils = 0x01,

        /// <summary>
        /// Read Holding Registers
        /// </summary>
        ReadReg = 0x03,

        /// <summary>
        /// Write Multiple Coils
        /// </summary>
        WriteCoils = 0x0F,

        /// <summary>
        /// Write Multiple Registers
        /// </summary>
        WriteReg = 0x10
    }

}