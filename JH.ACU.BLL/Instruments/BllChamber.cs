using System;
using System.Linq;
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
            IdentifierAddress = 0; //温箱默认地址为0
        }

        protected sealed override byte IdentifierAddress { get; set; }

        #endregion

        #region 属性、字段

        private enum RunEnum : ushort
        {
            Stop = 0,
            Run = 1,
            Stay = 2,

            /// <summary>
            /// 跳步,只用于程序控制中
            /// </summary>
            Skip = 4,
        }

        #endregion

        #region 私有方法

        private void RunInConst(RunEnum isRun)
        {
            var value = ((ushort) isRun).GetBytes();
            SendMultiRegisters(47, value);
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
            var data = value.GetBytes();
            SendMultiRegisters(43, data);
        }

        /// <summary>
        /// 获取温度
        /// </summary>
        /// <returns></returns>
        public double GetTemp()
        {
            var data = ReceiveRegister(0, 1).Take(2).ToArray();
            var res = data.GetShort();
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