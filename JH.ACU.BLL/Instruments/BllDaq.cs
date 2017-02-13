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
        private int _currGroup = -1;

        /// <summary>
        /// PB5不复位常量
        /// </summary>
        private const byte NoReset = 0x20;

        private const int BoardNum = 8, GroupNum = 13;

        /// <summary>
        /// 继电器组状态值
        /// 共8个子板，每个子板有14组继电器
        /// </summary>
        private byte[,] _relaysGroupMask = new byte[BoardNum, GroupNum];

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
        /// <param name="groupIndex">取值范围0x00~0x0F,当赋值0x0F时所有继电器均不使能，即相当于关闭该组</param>
        private void SelectRelayGroup(byte groupIndex)
        {
            DoWritePort(D2KDask.Channel_P1CL, groupIndex);
        }

        #endregion

        /// <summary>
        /// 板卡使能
        /// </summary>
        /// <param name="boardIndex">取值范围0~7</param>
        private void Enable(byte boardIndex)
        {
            WriteToPB((byte) (NoReset | boardIndex));
            WriteToPB((byte) (NoReset | 0x08 | boardIndex));
            WriteToPB((byte) (NoReset | boardIndex));
        }

        /// <summary>
        /// 设置指定板卡指定继电器组的状态
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="groupIndex"></param>
        /// <param name="mask"></param>
        private void SetRelaysGroup(byte boardIndex, byte groupIndex, byte mask)
        {
            if (boardIndex > BoardNum - 1) throw new ArgumentException("输入板卡索引无效", "boardIndex");
            if (groupIndex > GroupNum - 1) throw new ArgumentException("输入继电器组索引无效", "groupIndex");
            SelectRelays(mask);
            SelectRelayGroup(groupIndex);
            Enable(boardIndex);
            _currBoard = boardIndex;
            _currGroup = groupIndex;
            _relaysGroupMask[boardIndex, groupIndex] = mask;
        }
        /// <summary>
        /// 获取继电器所在组及更改后的状态
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="relayIndex"></param>
        /// <param name="enable"></param>
        /// <param name="groupIndex"></param>
        /// <param name="mask"></param>
        private void GetRelaysMask(byte boardIndex, int relayIndex, bool enable, out byte groupIndex, out byte mask)
        {
            //TODO:relay取值有范围
            if (boardIndex > BoardNum - 1) throw new ArgumentException("输入板卡索引无效", "boardIndex");
            groupIndex = (byte)((relayIndex - 200) / 10);
            if (groupIndex > GroupNum - 1) throw new ArgumentException("输入继电器组索引无效", "groupIndex");

            var iBit = (relayIndex - 200) % 10;
            byte ulPaStatus = 0x01;
            ulPaStatus = (byte)(ulPaStatus << iBit);
            if (!enable)
            {
                ulPaStatus = (byte)~ulPaStatus;
                mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & ulPaStatus);
                return;
            }
            mask = (byte) (_relaysGroupMask[boardIndex, groupIndex] | ulPaStatus);

            #region 原方法内容，作用待定

            //if (iBit >= 0 && iBit <= 3)
            //{
            //    switch (groupIndex)
            //    {
            //        case 6:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0x3F | ulPaStatus);
            //            break;
            //        case 7:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0x8F | ulPaStatus);
            //            break;
            //        default:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0x0F | ulPaStatus);
            //            break;
            //    }
            //}
            //else
            //{
            //    switch (groupIndex)
            //    {
            //        case 6:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0xF0 | ulPaStatus);
            //            break;
            //        case 7:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0xF8 | ulPaStatus);
            //            break;
            //        default:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0xF0 | ulPaStatus);
            //            break;
            //    }

            //}

            #endregion

        }
        #endregion

        #region 公有方法

        /// <summary>
        /// 设置某个板卡某个继电器是否使能
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="relay"></param>
        /// <param name="enable"></param>
        public void SetRelayStatus(byte boardIndex, int relay, bool enable)
        {
            byte groupIndex, mask;
            GetRelaysMask(boardIndex, relay, enable, out groupIndex, out mask);
            if (boardIndex != _currBoard)
            {
                if (_currBoard != -1)
                {
                    ResetBoard((byte) _currBoard); //将原板复位至初始状态
                }
            }
            SetRelaysGroup(boardIndex, groupIndex, mask); //将现板设置为新状态
        }

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
        /// 将指定板卡设置为初始状态,即仅保持ACU上电,其余继电器断开
        /// </summary>
        /// <param name="boardIndex"></param>
        public void ResetBoard(byte boardIndex)
        {
            for (byte i = 0; i < GroupNum; i++)
            {
                var mask = (byte) (i == 6 ? 0x30 & _relaysGroupMask[boardIndex, i] : 0x00);
                //QUES:原程序中将Kline及DAQ1,2保持原有状态,作用未知,如此串口控制哪个ACU?
                //mask = (byte) (i == 7 ? 0x88 & _relaysGroupMask[boardIndex, i] : mask);
                if (_relaysGroupMask[boardIndex, i] != mask)
                {
                    SetRelaysGroup(boardIndex, i, mask);
                }
            }
        }

        /// <summary>
        /// 复位
        /// 即关闭所有板卡,ACU断电
        /// </summary>
        public void ResetAll()
        {
            SelectRelays(0);
            SelectRelayGroup(0x0f);
            WriteToPB(NoReset & 0);
            _relaysGroupMask = new byte[8, 14];
            _currBoard = -1;

            #region 原程序内代码段

            //QUES:作用未知
            //// 保持子板选择使能有效
            //iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x08);
            //if (iRetCode != NoError) return false;

            #endregion

        }

        public void Dispose()
        {
            if (_mDev >= 0)
            {
                ResetAll();
                D2KDask.D2K_Release_Card((ushort) _mDev);
                _mDev = -1;
            }
        }

        #endregion

    }
}