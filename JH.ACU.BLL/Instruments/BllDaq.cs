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
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.BLL.Instruments
{
    public class BllDaq : IDisposable
    {
        #region 构造函数

        public BllDaq()
        {
        }

        ~BllDaq()
        {
            Dispose(false);
        }

        #endregion

        #region 属性、字段

        private short _mDev = -1;
        private int _currBoard = -1;
        private int _currGroup = -1;
        private readonly double _amplify12V = (4.7 + 4.7)/4.7;
        private readonly double _amplifyPow = (10.0 + 4.7)/4.7;
        private bool _disposed;

        /// <summary>
        /// PB5不复位常量
        /// </summary>
        private const byte NoReset = 0x20;

        private const int BoardNum = 8, GroupNum = 13;

        private byte _mainRelayMask;

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
                var pwr = (_relaysGroupMask[_currBoard, 6] & 0x30) == 0x30; //k264 k265同时使能
                var kLineAcout1 = (_relaysGroupMask[_currBoard, 7] & 0x88) == 0x88; //k273 k277同时使能
                var cout2 = (_relaysGroupMask[_currBoard, 12] & 0x01) == 0x01; //k340使能
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

        /// <summary>
        /// 回路继电器分组
        /// </summary>
        public static readonly int[,] FcGroup =
        {
            {214, 215, 216, 217}, {220, 221, 222, 223}, {224, 225, 226, 227}, {230, 231, 232, 233},
            {234, 235, 236, 237}, {240, 241, 242, 243}, {244, 245, 246, 247}, {250, 251, 252, 253},
            {254, 255, 256, 257}, {260, 261, 262, 263}, {310, 311, 312, 313}, {314, 315, 316, 317},
            {320, 321, 322, 323}, {324, 325, 326, 327}, {330, 331, 332, 333}, {334, 335, 336, 337}
        };

        /// <summary>
        /// 开关继电器分组
        /// 使用示例:BeltGroup[(int) BeltSwitch.Dsb,index1]
        /// </summary>
        public static readonly int[,] BeltGroup =
        {
            {200, 201, 202, 203}, {204, 205, 206, 207}, {210, 211, 212, 213}
        };

        /// <summary>
        /// AI通道命名
        /// </summary>

        #endregion

        #region 私有方法

        #region Basic Method

        private void DoWritePort(ushort channel, byte mask, int delay = 100)
        {
            var res = D2KDask.D2K_DO_WritePort((ushort) _mDev, channel, mask);
            Thread.Sleep(delay);
            D2KDask.ThrowException((D2KDask.Error) res, channel, mask);
        }

        private double AiReadChannel(ushort channel)
        {
            double value;
            var res = D2KDask.D2K_AI_VReadChannel((ushort) _mDev, channel, out value);
            D2KDask.ThrowException((D2KDask.Error) res, string.Format("AI Point读取失败，Channel:{0}", channel));
            return value;
        }

        /// <summary>
        /// in D2KDASK DEMO
        /// </summary>
        /// <returns></returns>
        public double[] AiReadSingleBuffer(ushort channel,int bufferSize=1000)
        {
            ushort bufId;
            byte stopped;
            uint accessCnt;
            uint startPos;
            var dataBuffer = Marshal.AllocHGlobal(sizeof(short) * bufferSize);
            var voltageArray = new double[bufferSize];
            var ret = D2KDask.D2K_AI_Config((ushort) _mDev, D2KDask.DAQ2K_AI_ADCONVSRC_Int, D2KDask.DAQ2K_AI_TRGSRC_SOFT,
                0, 0, 1, true);
            D2KDask.ThrowException((D2KDask.Error) ret, channel);
            ret = D2KDask.D2K_AI_ContBufferSetup((ushort) _mDev, dataBuffer, (uint) bufferSize, out bufId);
            D2KDask.ThrowException((D2KDask.Error) ret, channel);
            ret = D2KDask.D2K_AI_ContReadChannel((ushort) _mDev, channel, bufId, 1000, 400, 400, D2KDask.ASYNCH_OP);
            D2KDask.ThrowException((D2KDask.Error) ret, channel); //             100,40000,40000
            do
            {
                ret = D2KDask.D2K_AI_AsyncCheck((ushort) _mDev, out stopped, out accessCnt);
                D2KDask.ThrowException((D2KDask.Error) ret, channel);
            } while (stopped == 0);

            ret = D2KDask.D2K_AI_AsyncClear((ushort) _mDev, out startPos, out accessCnt);
            D2KDask.ThrowException((D2KDask.Error) ret, channel);
            ret = D2KDask.D2K_AI_ContVScale((ushort)_mDev, D2KDask.AD_B_10_V, dataBuffer, voltageArray, bufferSize);
            D2KDask.ThrowException((D2KDask.Error) ret, channel);
            Marshal.FreeHGlobal(dataBuffer);
            return voltageArray;
        }

        /// <summary>
        /// in old program
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        [Obsolete("测试失败",true)]
        private double AiReadSingleBuffer2(ushort channel)
        {
            short[] pwBuffer = {};
            ushort bufId;
            const uint countScan = 100;
            D2KDask.D2K_AI_CH_Config((ushort) _mDev, (short) channel, D2KDask.AD_B_10_V);
            D2KDask.D2K_AI_ContBufferSetup((ushort) _mDev, pwBuffer, countScan, out bufId);
            var ret = D2KDask.D2K_AI_ContReadChannel((ushort) _mDev, channel, bufId, countScan, 40000, 40000,
                D2KDask.SYNCH_OP);
            D2KDask.ThrowException((D2KDask.Error) ret, string.Format("AI Multi读取失败，Channel:{0}", channel));
            var voltsum = pwBuffer.Aggregate<short, long>(0, (current, s) => current + s);
            return (double) voltsum/pwBuffer.Length*10D/32768D; //+32768.0是C语言中常见类型short(一些情况下也是int）的取值范围的上限
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

        private double GetVoltFromChannelByPoint(AiChannel channel)
        {
            var res = AiReadChannel((ushort) channel);
            switch (channel)
            {
                case AiChannel._5VCh:
                    break;
                case AiChannel._12VCh:
                    res *= _amplify12V;
                    break;
                case AiChannel.PowCh:
                    res *= _amplifyPow;
                    break;
                case AiChannel.Tsensor:
                    break;
            }
            return res;
        }

        private double GetVoltFromChannelByMulti(AiChannel channel)
        {
            //var res = AiReadSingleBuffer2((ushort) channel);
            var res = AiReadSingleBuffer((ushort)channel).Average();
            //var res = Fun((ushort)channel)[0];
            switch (channel)
            {
                case AiChannel._5VCh:
                    break;
                case AiChannel._12VCh:
                    res *= _amplify12V;
                    break;
                case AiChannel.PowCh:
                    res *= _amplifyPow;
                    break;
                case AiChannel.Tsensor:
                    break;
            }
            return res;
        }

        /// <summary>
        /// 板卡使能
        /// </summary>
        /// <param name="boardIndex">取值范围0~7</param>
        private void EnableBoard(byte boardIndex)
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
            EnableBoard(boardIndex);
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
                groupIndex = (byte) ((relayIndex - 200)/10 - 1);
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
        }

        #endregion

        #region 公有方法

        #region AI

        /// <summary>
        /// 读取通道电压
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public double GetVoltFromChannel(AiChannel channel, AiReadMode mode)
        {
            double res;
            switch (mode)
            {
                case AiReadMode.Point:
                    res = GetVoltFromChannelByPoint(channel);
                    break;
                case AiReadMode.Multi:
                    res = GetVoltFromChannelByMulti(channel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
            return res;
        }

        /// <summary>
        /// 检查DAQ供电状态
        /// </summary>
        /// <param name="voltTarget">目标电压，即Pwr输出电压</param>
        /// <returns></returns>
        public bool CheckPowerStatus(double voltTarget)
        {
            foreach (AiChannel aChannel in Enum.GetValues(typeof(AiChannel)))
            {
                var channel = aChannel;
                foreach (var res in from AiReadMode aMode in Enum.GetValues(typeof(AiReadMode))
                                    where channel != AiChannel.Tsensor
                                    select GetVoltFromChannel(channel, aMode))
                {
                    switch (aChannel)
                    {
                        case AiChannel._5VCh:
                            if (Math.Abs(res - 5D) > 1D)
                            {
                                return false;
                            }
                            break;
                        case AiChannel._12VCh:
                            if (Math.Abs(res - 12D) > 1D)
                            {
                                return false;
                            }
                            break;
                        case AiChannel.PowCh:
                            if (Math.Abs(res - voltTarget) > 1D)
                            {
                                return false;
                            }
                            break;
                        case AiChannel.Tsensor:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            return true;
        }

        public void SetCrashConfig(out short[] buffer)
        {
            var dataBuffer = Marshal.AllocHGlobal(sizeof(short) * 5000);
            buffer = new short[5000];
            ushort bufId;
            var ret = D2KDask.D2K_AIO_Config((ushort) _mDev, D2KDask.DAQ2K_IntTimeBase,
                D2KDask.Above_High_Level | D2KDask.CH0ATRIG, 0x80, 0x80); //最后两个参数MES_ANATRIG_H、MES_ANATRIG_L
            D2KDask.ThrowException((D2KDask.Error) ret);
            ret = D2KDask.D2K_AI_CH_Config((ushort) _mDev, 0, D2KDask.AD_B_10_V);
            D2KDask.ThrowException((D2KDask.Error) ret);
            ret = D2KDask.D2K_AI_Config((ushort) _mDev, D2KDask.DAQ2K_AI_ADCONVSRC_Int,
                D2KDask.DAQ2K_AI_TRGSRC_SOFT | D2KDask.DAQ2K_AI_TRGMOD_POST, 0, 0, 0, true);
            D2KDask.ThrowException((D2KDask.Error) ret);
            ret = D2KDask.D2K_AI_ContBufferSetup((ushort)_mDev, dataBuffer, 5000, out bufId);
            D2KDask.ThrowException((D2KDask.Error) ret);
            ret = D2KDask.D2K_AI_ContReadChannel((ushort) _mDev, 0, bufId, 5000, 40000, 40000, D2KDask.SYNCH_OP);
            D2KDask.ThrowException((D2KDask.Error) ret);
            //后加的
            //ret = D2KDask.D2K_AI_ContVScale((ushort)_mDev, D2KDask.AD_B_10_V, dataBuffer, buffer, 5000);
            //D2KDask.ThrowException((D2KDask.Error)ret);
            Marshal.Copy(dataBuffer, buffer, 0, 5000);
            Marshal.FreeHGlobal(dataBuffer);

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

            ret = D2KDask.D2K_AI_CH_Config((ushort) _mDev, -1, D2KDask.AD_B_10_V);
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
            Thread.Sleep(100);
            SetRelaysGroup(boardIndex, 6, mask6);
            Thread.Sleep(100);
            SetRelaysGroup(boardIndex, 7, mask7);
            Thread.Sleep(100);
            SetRelaysGroup(boardIndex, 12, mask12);
            Thread.Sleep(100);
        }

        #region ACU Belt测试用

        /// <summary>
        /// 将指定ACU指定Belt设置为测试状态
        /// </summary>
        /// <param name="acuIndex">ACU索引 0-7</param>
        /// <param name="beltIndex">Belt索引 DSB PSB PADS</param>
        /// <param name="mode">Belt测试模式</param>
        public void SetBeltInTestMode(int acuIndex, int beltIndex, BeltMode mode)
        {
            if ((RelaysGroupMask[acuIndex, 7] & 0x08) != 0x08)
            {
                SetSubRelayStatus((byte) acuIndex, 273, true); //接通kLine
            }
            beltIndex--;
            //Tips:以下注释以DSB为例
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 0], false); //k200
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 1], true); //k201
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 2], false); //k202
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 3], true); //k203
            SetMainRelayStatus(302, true);
            if (mode == BeltMode.ToGround || mode == BeltMode.ToBattery) return; //QUES:还未确定要开启哪些继电器
            //SetMainRelayStatus(302,mode != BeltMode.ToBattery);//QUES:存疑
        }

        /// <summary>
        /// 将指定ACU指定Belt设置为读取状态
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="beltIndex"></param>
        public void SetBeltInReadMode(int acuIndex, int beltIndex)
        {
            beltIndex--;
            //Tips:以下注释以DSB为例
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 0], true); //k200
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 1], false); //k201
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 2], true); //k202
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 3], true); //k203

        }

        /// <summary>
        /// 将指定ACU指定Belt复位
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="beltIndex"></param>
        public void SetBeltReset(int acuIndex, int beltIndex)
        {
            beltIndex--;
            //Tips:以下注释以DSB为例
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 0], true); //k200
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 1], false); //k201
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 2], true); //k202
            SetSubRelayStatus((byte) acuIndex, BeltGroup[beltIndex, 3], true); //k203
            SetMainRelayStatus(302, false);
            SetMainRelayStatus(301, false); //QUES:是否赋值未知

        }

        #endregion

        #region ACU SIS测试用

        /// <summary>
        /// 将指定ACU指定SIS设置为测试状态
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="sisIndex">侧传感器索引 1-6</param>
        /// <param name="mode">SIS测试模式</param>
        public void SetSisInTestMode(int acuIndex, int sisIndex, SisMode mode)
        {
            if ((RelaysGroupMask[acuIndex, 7] & 0x08) != 0x08)
            {
                SetSubRelayStatus((byte) acuIndex, 273, true); //接通kLine
            }
            var relayIndex = 279 + sisIndex;
            SetSubRelayStatus((byte) acuIndex, relayIndex, true);
            SetSubRelayStatus((byte) acuIndex, 284, false);
            SetSubRelayStatus((byte) acuIndex, 285, false);
            SetMainRelayStatus(301, mode == SisMode.ToBattery);
            SetMainRelayStatus(302, mode == SisMode.ToGround);
        }

        /// <summary>
        /// 将指定ACU指定SIS设置为DMM读取状态
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="sisIndex"></param>
        public void SetSisInReadMode(int acuIndex, int sisIndex)
        {
            var relayIndex = 279 + sisIndex;
            SetSubRelayStatus((byte) acuIndex, relayIndex, false);
            SetMainRelayStatus(301, false);
            SetMainRelayStatus(302, false);
            SetSubRelayStatus((byte) acuIndex, 284, true);
            SetSubRelayStatus((byte) acuIndex, 285, true);
        }

        /// <summary>
        /// 将指定ACU指定SIS恢复到初始状态
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="sisIndex"></param>
        public void SetSisReset(int acuIndex, int sisIndex)
        {
            var relayIndex = 279 + sisIndex;
            SetSubRelayStatus((byte) acuIndex, relayIndex, false);
            SetMainRelayStatus(301, false);
            SetMainRelayStatus(302, false);
            SetSubRelayStatus((byte) acuIndex, 284, false);
            SetSubRelayStatus((byte) acuIndex, 285, false);
        }

        #endregion

        #region ACU回路测试用

        /// <summary>
        /// 将指定ACU指定回路设置为测试状态
        /// </summary>
        /// <param name="acuIndex">ACU索引 0-7</param>
        /// <param name="squibIndex">回路索引 1-16</param>
        /// <param name="mode">回路测试模式</param>
        public void SetFcInTestMode(int acuIndex, int squibIndex, SquibMode mode)
        {
            if ((RelaysGroupMask[acuIndex, 7] & 0x08) != 0x08)
            {
                SetSubRelayStatus((byte) acuIndex, 273, true); //接通kLine
            }
            squibIndex--;
            //Tips:以下注释以FC#1为例
            switch (mode)
            {
                case SquibMode.TooHigh:
                case SquibMode.TooLow:

                    #region 断开固定电阻，接入电阻箱#1

                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 0], false); //k214断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 1], true); //k215闭合
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 2], false); //k216断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 3], true); //k217闭合

                    #endregion

                    break;
                case SquibMode.ToGround:
                case SquibMode.ToBattery:

                    #region 断开固定电阻，接入电阻箱#2

                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 0], false); //k214断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 1], false); //k215断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 2], false); //k216断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 3], true); //k217闭合

                    #endregion

                    #region 将电阻箱#2接电源或接地

                    SetMainRelayStatus(300, true);
                    SetMainRelayStatus(301, mode == SquibMode.ToBattery);
                    SetMainRelayStatus(302, mode == SquibMode.ToGround);

                    #endregion

                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
        }

        /// <summary>
        /// 将指定ACU指定回路设置为DMM读取状态
        /// </summary>
        /// <param name="acuIndex">从0开始</param>
        /// <param name="squibIndex">从1开始</param>
        /// <param name="mode"></param>
        public void SetFcInReadMode(int acuIndex, int squibIndex, SquibMode mode)
        {
            squibIndex--;
            //Tips:以下注释以FC#1为例
            switch (mode)
            {
                case SquibMode.TooHigh:
                case SquibMode.TooLow:

                    #region 将DMM接到电阻箱#1两端

                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 0], true); //k214闭合
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 1], false); //k215断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 2], true); //k216闭合
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 3], true); //k217闭合

                    #endregion

                    break;
                case SquibMode.ToGround:
                case SquibMode.ToBattery:

                    #region 将DMM接到电阻箱#2两端

                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 0], false); //k214断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 1], false); //k215断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 2], false); //k216断开
                    SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 3], false); //k217断开
                    SetMainRelayStatus(300, false);
                    SetMainRelayStatus(301, false);
                    SetMainRelayStatus(302, false);
                    SetSubRelayStatus((byte) acuIndex, 284, true); //连接PRS2+、DMMS+、DMM+
                    SetSubRelayStatus((byte) acuIndex, 285, true); //连接PRS2-、DMMS-、DMM-

                    #endregion

                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
        }

        /// <summary>
        /// 将指定ACU指定回路恢复到连接固定电阻状态
        /// </summary>
        /// <param name="acuIndex"></param>
        /// <param name="squibIndex"></param>
        public void SetFcReset(int acuIndex, int squibIndex)
        {
            if ((RelaysGroupMask[acuIndex, 7] & 0x08) != 0x08)
            {
                SetSubRelayStatus((byte) acuIndex, 273, true); //接通kLine
            }
            squibIndex--;
            //Tips:以下注释以FC#1为例
            SetMainRelayStatus(300, false);
            SetMainRelayStatus(301, false);
            SetMainRelayStatus(302, false);
            SetSubRelayStatus((byte) acuIndex, 284, false);
            SetSubRelayStatus((byte) acuIndex, 285, false);
            SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 0], false); //k214断开
            SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 1], false); //k215断开
            SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 2], false); //k216断开
            SetSubRelayStatus((byte) acuIndex, FcGroup[squibIndex, 3], false); //k217断开
        }

        #endregion

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
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);

        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                // 清理托管资源
            }
            //清理非托管资源
            if (_mDev >= 0)
            {
                ResetAll();
                D2KDask.D2K_Release_Card((ushort) _mDev);
                _mDev = -1;
            }
            //让类型知道自己已经被释放
            _disposed = true;
        }

        #endregion

        /// <summary>
        /// in old program//TODO:待验证
        /// </summary>
        /// <param name="crashOutType"></param>
        /// <param name="voltBuf"></param>
        public void GetCrashCheck(CrashOutType crashOutType, double[] voltBuf)
        {
            short i, j = 0, k = 0;
            var iCount = new short[100];
            var fRate = new double[100];
            bool bRet = false;

            for (i = 1; i < 1000; i++)
            {
                if (Math.Abs(voltBuf[i] - voltBuf[j]) >= 1)
                {
                    iCount[k] = (short) (i - j + 1);
                    k++;
                    j = (short) (i + 1);
                }
            }
            switch (crashOutType)
            {
                case CrashOutType.Advanced:
                    for (i = 1; i < 100; i++)
                    {
                        if (iCount[i] == 0)
                        {
                            for (j = 1; j < (i - 2)/2*2; j++)
                            {
                                fRate[j] = (float) iCount[2*j]/iCount[2*j - 1];
                                if (Math.Abs(2*j - i) <= 1)
                                {
                                    k = j;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    for (i = 1; i < k; i++)
                    {
                        if (Math.Abs(fRate[i] - 5.0) <= 0.2 || Math.Abs(fRate[i] - 0.2) <= 0.02) bRet = true;
                        else bRet = false;//QUES:不知道有什么意义
                    }
                    break;
                case CrashOutType.Conventional:
                    bRet = Math.Abs(iCount[1]*5 - 200.0) <= 2.0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("crashOutType", crashOutType, null);
            }
        }
    }

    public enum AiChannel : ushort
    {
        _5VCh = 1,
        _12VCh = 2,
        PowCh = 3,
        Tsensor = 4,
    }

    /// <summary>
    /// AI读取方式
    /// </summary>
    public enum AiReadMode
    {
        Point,
        Multi,
    }
}