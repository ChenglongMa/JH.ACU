using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JH.ACU.DAL;

namespace JH.ACU.BLL.Instruments
{
    public class BllDaq : IDisposable
    {
        #region 构造函数

        public BllDaq()
        {

        }

        #endregion

        #region 属性、字段

        private short _mDev;

        /// <summary>
        /// PB5不复位常量
        /// </summary>
        private const byte NoReset = 0x20; // QUES:看电路图是高电平复位，但原代码中是低电平复位，待测试

        public byte[,] Relays = new byte[8, 8];

        #endregion

        #region 私有方法

        #region Basic Method

        private void DoWritePort(ushort channel, byte mask, int delay = 10)
        {
            var res = D2KDask.D2K_DO_WritePort((ushort) _mDev, channel, mask);
            Thread.Sleep(delay);
            D2KDask.ThrowException((D2KDask.Error) res);
        }

        /// <summary>
        /// Data 0~7 用于选择一组中某几个继电器
        /// </summary>
        /// <param name="mask"></param>
        private void WriteToPA(byte mask)
        {
            DoWritePort(D2KDask.Channel_P1A, mask);
        }

        /// <summary>
        /// PB0~3为板卡使能,PB5为Reset位,其余位保留
        /// </summary>
        /// <param name="mask"></param>
        private void WriteToPB(byte mask)
        {
            DoWritePort(D2KDask.Channel_P1B, mask);
        }

        /// <summary>
        /// PC0~3=A0~3,用于选择某组继电器
        /// PC4~6=AREL300~302,PC7保留
        /// </summary>
        /// <param name="mask"></param>
        private void WriteToPC(byte mask)
        {
            DoWritePort(D2KDask.Channel_P1C, mask);
        }

        /// <summary>
        /// PC4~6=AREL300~302,PC7保留
        /// </summary>
        /// <param name="mask">单字节 0x00~0x0F</param>
        private void WriteToPCh(byte mask)
        {
            var hMask = (byte) (mask << 4);
            DoWritePort(D2KDask.Channel_P1CH, hMask);
        }

        /// <summary>
        /// PC0~3=A0~3,用于选择某组继电器
        /// </summary>
        /// <param name="mask">单字节 0x00~0x0F</param>
        private void WriteToPCl(byte mask)
        {
            DoWritePort(D2KDask.Channel_P1CL, mask);
        }

        #endregion

        /// <summary>
        /// 板卡使能
        /// </summary>
        /// <param name="cardNum">取值范围0~7</param>
        private void EnableCard(byte cardNum)
        {
            WriteToPB((byte)(NoReset | cardNum));
            WriteToPB((byte)(NoReset | 0x08 | cardNum));
            WriteToPB((byte)(NoReset | cardNum));
        }
        /// <summary>
        /// 继电器组使能
        /// </summary>
        /// <param name="mask">取值范围0x00~0x0F,当赋值0x0F时所有继电器均不使能</param>
        private void EnableGroup(byte mask)
        {
            WriteToPCl(mask);
        }
        /// <summary>
        /// 继电器使能
        /// </summary>
        /// <param name="relay"></param>
        private void EnableRelay(byte relay)
        {
            WriteToPA(relay);
        }
        #endregion

        #region 公有方法

        //public bool SetSbRelayGroupStatus(byte cardNum, byte group, byte status)
        //{

        //}



        /// <summary>
        /// 采集卡初始化
        /// </summary>
        public void Initialize()
        {
            #region 注册板卡

            if (_mDev >= 0) Dispose();
            _mDev = D2KDask.D2K_Register_Card(D2KDask.DAQ_2206, 0);
            if (_mDev < 0) throw new Exception("板卡注册失败");

            #endregion

            #region 配置端口

            #region 数字量输出

            var ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1A, D2KDask.OUTPUT_PORT);
            if (ret < 0) throw new Exception("P1A DIO配置失败");
            ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1B, D2KDask.OUTPUT_PORT);
            if (ret < 0) throw new Exception("P1B DIO配置失败");
            ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1C, D2KDask.OUTPUT_PORT);
            if (ret < 0) throw new Exception("P1C DIO配置失败");

            #endregion

            #region 模拟量输入

            #endregion


            #endregion


        }

        public void Dispose()
        {
            if (_mDev > 0)
            {
                D2KDask.D2K_Release_Card((ushort) _mDev);
            }
        }

        #endregion

    }
}