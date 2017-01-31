using System;
using JH.ACU.BLL.Abstract;
using JH.ACU.Model;

namespace JH.ACU.BLL.Instruments
{
    /// <summary>
    /// 温箱业务类
    /// </summary>
    public class BllChamber : BllModbusTcp
    {
        #region 构造函数

        public BllChamber(InstrName name) : base(name)
        {
        }

        #endregion

        #region 属性、字段


        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        public void SetTemp(float temp)
        {
            throw new NotImplementedException();
        }

        public float GetTemp()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
