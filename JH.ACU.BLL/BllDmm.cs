using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivi.Visa;
using JH.ACU.DAL;
using JH.ACU.Model;
using NationalInstruments.Visa;

namespace JH.ACU.BLL
{
    /// <summary>
    /// 数字万用表操作类
    /// </summary>
    public class BllDmm:BllVisa
    {
        public BllDmm(InstrName instr) : base(instr)
        {
              /* 基本编程顺序：
               * 1、将万用表设定在一已知的状态（通常在复位状态）
               * 2、将万用表的设定改变为想要的配置
               * 3、设定触发条件
               * 4、启动或导引万用表开始测量
               * 5、触发万用表进行测量
               * 6、从输出缓冲器或内部存储器上取出读数
               * 7、将测量的数据读进总线控制器
               */
        }

        #region 属性 字段

        private enum ControlMode
        {
            Local,
            Remote,
            RemoteWithLock
        }

        #endregion

        public bool Initialize()
        {

        }

        private void SetControlMode(ControlMode mode, int timeout = 30000)
        {
            if (MbSession.HardwareInterfaceType!=HardwareInterfaceType.Serial)return;
            switch (mode)
            {
                case ControlMode.Local:
                    WriteNoRead("SYST:LOC");
                    return;//退出该方法
                case ControlMode.Remote:
                    WriteNoRead("SYST:REM");

                    break;
                case ControlMode.RemoteWithLock:
                    WriteNoRead("SYST:RWL");

                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode", mode, null);
            }
            WaitComplete(timeout);
        }

        private void DmmClear()
        {
            switch (MbSession.HardwareInterfaceType)
            {
                case HardwareInterfaceType.Gpib:
                    MbSession.Clear();
                    break;

                case HardwareInterfaceType.Serial:
                    var serial = MbSession as SerialSession;
                    try
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        serial.FlowControl = SerialFlowControlModes.DtrDsr;
                    }
                    catch (Exception)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        serial.FlowControl = SerialFlowControlModes.None;
                    }
                    serial.ReadTermination = SerialTerminationMethod.TerminationCharacter;
                    serial.WriteTermination = SerialTerminationMethod.TerminationCharacter;
                    serial.TerminationCharacter = 0x0A;
                    serial.BaudRate = Config.BaudRate;
                    serial.Parity = Config.Parity;
                    serial.DataBits = Config.DataBits;
                    serial.StopBits = SerialStopBitsMode.Two;
                    serial.IOProtocol = IOProtocol.Ieee4882;
                    serial.Flush(IOBuffers.ReadWrite, true);
                    serial.SetBufferSize(IOBuffers.ReadWrite, 4096);
                    SetControlMode(ControlMode.Remote);
                    var id = Idn;
                    if (id.Contains("34401")||id.Contains("34410")||id.Contains("4411"))
                    {
                        
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
