using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

        private byte _mainRelayMask = 0;

        /// <summary>
        /// 继电器组状态值
        /// 共8个子板，每个子板有14组继电器
        /// </summary>
        private byte[,] _relaysGroupMask = new byte[BoardNum, GroupNum];

        private bool IsOpened
        {
            get
            {
                if (_currBoard == -1) return false;
                var pwr = (_relaysGroupMask[_currBoard, 6] & 0x30) == 0x30;//k264 k265同时使能
                var kLineAcout1 = (_relaysGroupMask[_currBoard, 7] & 0x88) == 0x88;//k273 k277同时使能
                var cout2 = (_relaysGroupMask[_currBoard, 12] & 0x01) == 0x01;//k340使能
                return pwr && kLineAcout1 && cout2;
            }
        }

        /// <summary>
        /// 获取继电器组状态值
        /// </summary>
        public byte[,] RelaysGroupMask
        {
            get { return _relaysGroupMask; }
        }

        #endregion

        #region 私有方法

        #region Basic Method

        private void DoWritePort(ushort channel, byte mask, int delay = 100)//TODO:注意延时问题
        {
            var res = D2KDask.D2K_DO_WritePort((ushort) _mDev, channel, mask);
            Thread.Sleep(delay);
            D2KDask.ThrowException((D2KDask.Error) res);
        }

        private double AiReadChannel(ushort channel)
        {
            double value;
            var res = D2KDask.D2K_AI_VReadChannel((ushort) _mDev, channel, out value);
            D2KDask.ThrowException((D2KDask.Error) res);
            return value;
        }
        /// <summary>
        /// in D2KDASK DEMO
        /// </summary>
        /// <returns></returns>
        private double[] AiReadSingleBuffer(ushort channel)
        {
            ushort bufId;
            byte stopped;
            uint accessCnt;
            uint startPos;
            var dataBuffer = Marshal.AllocHGlobal(sizeof(short) * 1000);
            double[] voltageArray;

            var ret = D2KDask.D2K_AI_Config((ushort)_mDev, D2KDask.DAQ2K_AI_ADCONVSRC_Int, D2KDask.DAQ2K_AI_TRGSRC_SOFT, 0, 0, 1, true);
            D2KDask.ThrowException((D2KDask.Error) ret);
            ret = D2KDask.D2K_AI_ContBufferSetup((ushort)_mDev, dataBuffer, 1000, out bufId);
            D2KDask.ThrowException((D2KDask.Error)ret);
            ret = D2KDask.D2K_AI_ContReadChannel((ushort)_mDev, channel, bufId, 1000, 400, 400, D2KDask.ASYNCH_OP);
            D2KDask.ThrowException((D2KDask.Error)ret);//                       100,40000,40000
            do
            {
                ret = D2KDask.D2K_AI_AsyncCheck((ushort)_mDev, out stopped, out accessCnt);
                D2KDask.ThrowException((D2KDask.Error)ret);
            } while (stopped == 0);

            ret = D2KDask.D2K_AI_AsyncClear((ushort)_mDev, out startPos, out accessCnt);
            D2KDask.ThrowException((D2KDask.Error)ret);

            ret = D2KDask.D2K_AI_ContVScale((ushort)_mDev, D2KDask.AD_B_10_V, dataBuffer,out voltageArray, 1000);
            D2KDask.ThrowException((D2KDask.Error)ret);
            return voltageArray;
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
            var hMask = (byte) (mask/0x10);
            var lMask = (byte) (mask%0x10);
            _mainRelayMask = hMask;
        }

        /// <summary>
        /// PC4~6=AREL300~302 , PC7保留
        /// </summary>
        /// <param name="mask">单字节 0x00~0x0F</param>
        private void SelectMainRelay(byte mask)
        {
            var hMask = (byte) (mask << 4);
            DoWritePort(D2KDask.Channel_P1CH, hMask);
            _mainRelayMask = mask;
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
        /// <param name="relayIndex">只能输入从板上的继电器索引</param>
        /// <param name="enable"></param>
        /// <param name="groupIndex"></param>
        /// <param name="mask"></param>
        private void GetRelaysMask(byte boardIndex, int relayIndex, bool enable, out byte groupIndex, out byte mask)
        {
            if (boardIndex > BoardNum - 1) throw new ArgumentException("输入板卡索引无效", "boardIndex");
            if (relayIndex < 200 || relayIndex > 340 || (relayIndex > 287 && relayIndex < 310))
            {
                throw new ArgumentException("输入继电器索引无效", "relayIndex");
            }
            if (relayIndex > 199 && relayIndex < 288)
            {
                groupIndex = (byte) ((relayIndex - 200)/10);
            }
            else
            {
                groupIndex = (byte) ((relayIndex - 200)/10 - 2);

            }
            if (groupIndex > GroupNum - 1) throw new ArgumentException("输入继电器索引无效", "relayIndex");

            var iBit = relayIndex%10;
            if (iBit > 7) throw new ArgumentException("输入继电器索引无效", "relayIndex");
            byte ulPaStatus = 0x01;
            ulPaStatus = (byte) (ulPaStatus << iBit);
            if (!enable)
            {
                ulPaStatus = (byte) ~ulPaStatus;
                mask = (byte) (_relaysGroupMask[boardIndex, groupIndex] & ulPaStatus);
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

        #region AI

        public double GetVoltFromChannelBySingle(ushort channel)
        {
            var res = AiReadChannel(channel);
            switch (channel)
            {
                case 1:		// 5V 没有分压
                    break;
                case 2:		// 12V 分压
                    res = res * 2.0;
                    break;
                case 3:		// POW_ECU 分压
                    res = res * 3.12;
                    break;
                case 4:		// AD_T Sensor 分压
                    break;
            }
            return res;
        }
        #endregion

        #region DO

        /// <summary>
        /// 设置某个板卡某个继电器是否使能
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="relayIndex">只能输入从板上的继电器索引</param>
        /// <param name="enable"></param>
        public void SetSubRelayStatus(byte boardIndex, int relayIndex, bool enable)
        {
            byte groupIndex, mask;
            GetRelaysMask(boardIndex, relayIndex, enable, out groupIndex, out mask);
            if (boardIndex != _currBoard)
            {
                if (_currBoard != -1)
                {
                    CloseBoard((byte) _currBoard); //将原板复位至初始状态
                }
                if (!IsOpened)
                {
                    OpenBoard(boardIndex);
                }
            }
            SetRelaysGroup(boardIndex, groupIndex, mask); //将现板设置为新状态
        }

        //public void Set
        /// <summary>
        /// 设置主板某个继电器是否使能
        /// </summary>
        /// <param name="relayIndex">取值范围300~302</param>
        /// <param name="enable"></param>
        public void SetMainRelayStatus(int relayIndex, bool enable)
        {
            if (relayIndex < 300 || relayIndex > 302) throw new ArgumentException("输入继电器索引无效", "relayIndex");
            var iBit = relayIndex%10;
            byte mask;
            if (enable)
            {
                mask = (byte) (_mainRelayMask | iBit);
            }
            else
            {
                iBit = ~iBit;
                mask = (byte) (_mainRelayMask & iBit);
            }
            SelectMainRelay(mask);
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

            ResetAll();

            #endregion

            #region 模拟量输入

            //TODO：待完善
            ret =D2KDask.D2K_AI_CH_Config((ushort)_mDev, -1, D2KDask.AD_B_10_V);
            D2KDask.ThrowException((D2KDask.Error) ret, new Exception("AI配置失败"));


            #endregion


            #endregion


        }

        /// <summary>
        /// 将指定板卡设置为初始状态,即仅保持ACU上电,其余继电器断开
        /// </summary>
        /// <param name="boardIndex"></param>
        public void CloseBoard(byte boardIndex)
        {
            for (byte i = 0; i < GroupNum; i++)
            {
                var mask = (byte) (i == 6 ? 0x30 & _relaysGroupMask[boardIndex, i] : 0x00);
                if (_relaysGroupMask[boardIndex, i] != mask)
                {
                    SetRelaysGroup(boardIndex, i, mask);
                }
            }
        }

        /// <summary>
        /// 准备开始测试环境,注意时序
        /// </summary>
        /// <param name="boardIndex"></param>
        public void OpenBoard(byte boardIndex)
        {
            byte mask6 = (byte) (0x30 | _relaysGroupMask[boardIndex, 6]);
            byte mask7 = (byte) (0x88 | _relaysGroupMask[boardIndex, 7]);
            byte mask12 = (byte) (0x01 | _relaysGroupMask[boardIndex, 12]);
            SetRelaysGroup(boardIndex, 6, mask6);
            //Thread.Sleep(100);
            SetRelaysGroup(boardIndex, 7, mask7);
            //Thread.Sleep(100);
            SetRelaysGroup(boardIndex, 12, mask12);
            //Thread.Sleep(100);
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
            _currGroup = -1;

            #region 原程序内代码段

            //QUES:作用未知
            //// 保持子板选择使能有效
            //iRetCode = D2K_DO_WritePort(m_iHandleCard, Channel_P1B, 0x08);
            //if (iRetCode != NoError) return false;

            #endregion

        }

        #endregion


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