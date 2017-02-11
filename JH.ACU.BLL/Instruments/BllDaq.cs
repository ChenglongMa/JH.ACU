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

        private short _mDev = -1;
        private int _currBoard = -1;

        /// <summary>
        /// PB5不复位常量
        /// </summary>
        private const byte NoReset = 0x20;

        /// <summary>
        /// 继电器组状态值
        /// 共8个子板，每个子板有14组继电器
        /// </summary>
        private byte[,] _relaysGroupMask = new byte[8, 14];

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
        private void SelectRelays(byte mask)
        {
            DoWritePort(D2KDask.Channel_P1A, mask);
        }

        /// <summary>
        /// PB0~3=A4~6 + En_138 为板卡使能,PB5为Reset位,其余位保留
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
        /// PC0~3=A0~3,继电器组使能 74HC273
        /// </summary>
        /// <param name="groupNum">取值范围0x00~0x0F,当赋值0x0F时所有继电器均不使能，即相当于关闭该组</param>
        private void SelectRelayGroup(byte groupNum)
        {
            DoWritePort(D2KDask.Channel_P1CL, groupNum);
        }

        #endregion

        /// <summary>
        /// 板卡使能
        /// </summary>
        /// <param name="boardNum">取值范围0~7</param>
        private void Enable(byte boardNum)
        {
            WriteToPB((byte) (NoReset | boardNum));
            WriteToPB((byte) (NoReset | 0x08 | boardNum));
            WriteToPB((byte) (NoReset | boardNum));
            _currBoard = boardNum;
        }

        private void SetRelaysGroup(byte boardNum, byte groupNum, byte mask)
        {

            SelectRelays(mask);
            SelectRelayGroup(groupNum);
            Enable(boardNum);
            _relaysGroupMask[boardNum, groupNum] = mask;
        }
        #endregion

        #region 公有方法

        //public bool SetSbRelayGroupStatus(byte boardNum, byte groupNum, byte status)
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
            D2KDask.ThrowException((D2KDask.Error) _mDev);

            #endregion

            #region 配置端口

            #region 数字量输出

            var ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1A, D2KDask.OUTPUT_PORT);
            D2KDask.ThrowException((D2KDask.Error) ret, new Exception("P1A DIO配置失败"));
            ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1B, D2KDask.OUTPUT_PORT);
            D2KDask.ThrowException((D2KDask.Error) ret, new Exception("P1B DIO配置失败"));
            ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1C, D2KDask.OUTPUT_PORT);
            D2KDask.ThrowException((D2KDask.Error) ret, new Exception("P1C DIO配置失败"));

            #endregion

            #region 模拟量输入

            //TODO：待完善

            #endregion


            #endregion


        }
        /// <summary>
        /// 复位
        /// 即关闭所有板卡
        /// </summary>
        public void Reset()
        {
            SelectRelays(0);
            SelectRelayGroup(0x0f);
            WriteToPB(NoReset & 0);
            _relaysGroupMask = new byte[8, 14];
            _currBoard = -1;
        }

        public void Dispose()
        {
            if (_mDev > 0)
            {
                Reset();
                D2KDask.D2K_Release_Card((ushort) _mDev);
                _mDev = -1;
            }
        }

        #endregion

    }
}