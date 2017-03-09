using System;
using JH.ACU.BLL.Abstract;
using JH.ACU.Lib;
using JH.ACU.Model;

namespace JH.ACU.BLL.Instruments
{
    /// <summary>
    /// 温箱业务类
    /// </summary>
    public class BllChamber : BllModbusTcp
    {
        #region 构造函数

        public BllChamber(InstrName name = InstrName.Chamber) : base(name)
        {
            IdentifierAddress = 0;//温箱默认地址为0
        }

        #endregion

        #region 属性、字段
        private enum RunEnum:ushort
        {
            Stop=0,
            Run=1,
            Stay=2,
            /// <summary>
            /// 跳步,只用于程序控制中
            /// </summary>
            Skip=4,
        }

        #endregion

        #region 私有方法

        private void RunInConst(RunEnum isRun)
        {
            var value = ValueHelper.GetBytes((ushort) isRun);
            SendMultiRegisters(47,value);
        }
        #endregion

        #region 公有方法

        /// <summary>
        /// 设置定值温度
        /// </summary>
        /// <param name="temp"></param>
        public void SetTemp(double temp)
        {
            var value = (short) (temp*10);
            var highByte = (byte) ((value >> 8) & 0xFF);
            var lowByte = (byte) (value & 0xFF);
            SendMultiRegisters(43, highByte, lowByte);
        }

        /// <summary>
        /// 获取温度
        /// </summary>
        /// <returns></returns>
        public double GetTemp()
        {
            var data = ReceiveRegister(0, 1);
            var res = (short) ((data[0] & 0xFF) << 8) | (data[1] & 0xFF);
            return Convert.ToDouble(res)/10;
        }

        /// <summary>
        /// 定值运行
        /// </summary>
        public void Run()
        {
            RunInConst(RunEnum.Run);
        }
        /// <summary>
        /// 定值运行停止
        /// </summary>
        public void Stop()
        {
            RunInConst(RunEnum.Stop);
        }
        /// <summary>
        /// 定值运行保持
        /// </summary>
        public void Stay()
        {
            RunInConst(RunEnum.Stay);
        }
        #endregion

    }

}
