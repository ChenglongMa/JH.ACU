using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.BLL.Instruments;
using Xunit;

namespace JH.ACU.Test
{
    /// <summary>
    /// 单元测试类
    /// </summary>
    public class UnitTest
    {
        private readonly BllDmm _dmm = new BllDmm();

        [Fact]
        public void Test()
        {
            var curr= _dmm.GetCurrent();
            Assert.True(curr.Equals(0));
        }
    }
}
