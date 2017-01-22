using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.DAL;
using JH.ACU.Model;

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
    }
}
