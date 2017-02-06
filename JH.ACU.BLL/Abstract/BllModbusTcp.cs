using System;
using System.Collections.Generic;
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
    public abstract class BllModbusTcp
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
                throw new NullReferenceException("配置文件缺失");
            }
        }

        #endregion

        #region 属性、字段

        private readonly Socket _socket;
        protected Instr Config { get; set; }

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
        protected byte[] Receive(ushort address, short count)
        {
            var sendData = new List<byte>(255);

            //[1].Send
            sendData.AddRange(ValueHelper.GetBytes(NextDataIndex()));//1~2.(Transaction Identifier)
            sendData.AddRange(new byte[] { 0, 0 });//3~4:Protocol Identifier,0 = MODBUS protocol
            sendData.AddRange(ValueHelper.GetBytes((ushort)6));//5~6:后续的Byte数量（针对读请求，后续为6个byte）
            sendData.Add(0);//7:Unit Identifier:This field is used for intra-system routing purpose.
            sendData.Add((byte)FunctionCode.Read);//8.Function Code : 3 (Read Multiple Register)
            sendData.AddRange(ValueHelper.GetBytes(address));//9~10.起始地址
            sendData.AddRange(ValueHelper.GetBytes(count));//11~12.需要读取的寄存器数量
            Write(sendData.ToArray()); //发送读请求

            //[2].防止连续读写引起前台UI线程阻塞
            //Application.DoEvents();

            //[3].读取Response Header : 完后会返回8个byte的Response Header
            var receiveData = Read(256);//缓冲区中的数据总量不超过256byte，一次读256byte，防止残余数据影响下次读取
            var identifier = (ushort)((receiveData[0] << 8) + receiveData[1]);

            //[4].读取返回数据：根据ResponseHeader，读取后续的数据
            if (identifier != CurrentDataIndex) //请求的数据标识与返回的标识不一致，则丢掉数据包
            {
                return new byte[0];
            }
            var length = receiveData[8];//最后一个字节，记录寄存器中数据的Byte数
            var result = new byte[length];
            Array.Copy(receiveData, 9, result, 0, length);
            return result;
        }


        protected byte[] Send(ushort address, byte[] data)
        {
            //[0]:填充0，清掉剩余的寄存器
            if (data.Length < 60)
            {
                var input = data;
                data = new byte[60];
                Array.Copy(input, data, input.Length);
            }
            var values = new List<byte>(255);

            //[1].Write Header：MODBUS Application Protocol header
            values.AddRange(ValueHelper.GetBytes(NextDataIndex()));//1~2.(Transaction Identifier)
            values.AddRange(new byte[] { 0, 0 });//3~4:Protocol Identifier,0 = MODBUS protocol
            values.AddRange(ValueHelper.GetBytes((byte)(data.Length + 7)));//5~6:后续的Byte数量

            values.Add(0);//7:Unit Identifier:This field is used for intra-system routing purpose.
            values.Add((byte)FunctionCode.Write);//8.Function Code : 16 (Write Multiple Register)

            values.AddRange(ValueHelper.GetBytes(address));//9~10.起始地址
            values.AddRange(ValueHelper.GetBytes((ushort)(data.Length / 2)));//11~12.寄存器数量

            values.Add((byte)data.Length);//13.数据的Byte数量

            //[2].增加数据
            values.AddRange(data);//14~End:需要发送的数据

            //[3].写数据
            Write(values.ToArray());

            //[4].防止连续读写引起前台UI线程阻塞
            //Application.DoEvents();

            //[5].读取Response: 写完后会返回12个byte的结果
            return Read(12);
        }

        #endregion

        #region 公有方法


        #endregion

    }

    public enum FunctionCode : byte
    {
        //TODO:未完成
        /// <summary>
        /// Read Multiple Registers
        /// </summary>
        Read = 0x03,

        /// <summary>
        /// Write Multiple Registers
        /// </summary>
        Write = 0x10
    }

}