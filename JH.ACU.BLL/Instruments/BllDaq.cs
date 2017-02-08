using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using JH.ACU.DAL;

namespace JH.ACU.BLL.Instruments
{
    public class BllDaq:IDisposable
    {
        #region 构造函数

        public BllDaq()
        {

        }

        #endregion

        #region 属性、字段

        private short _mDev;

        public byte[,] Relays=new byte[8,8];

        #endregion

        #region 私有方法

        #endregion

        #region 公有方法

        /// <summary>
        /// 采集卡初始化
        /// </summary>
        public void Initialize()
        {
            #region 注册板卡

            if (_mDev >= 0) Dispose();
            _mDev = D2KDask.D2K_Register_Card(D2KDask.DAQ_2206, 0);
            if (_mDev < 0) throw new Exception("板卡注册失败");

            #endregion

            #region 配置端口

            #region 数字量输入

            var ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1A, D2KDask.OUTPUT_PORT);
            if (ret < 0) throw new Exception("P1A DIO配置失败");
            ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1B, D2KDask.OUTPUT_PORT);
            if (ret < 0) throw new Exception("P1B DIO配置失败");
            ret = D2KDask.D2K_DIO_PortConfig((ushort) _mDev, D2KDask.Channel_P1C, D2KDask.OUTPUT_PORT);
            if (ret < 0) throw new Exception("P1C DIO配置失败");

            #endregion

            #region 模拟量输出

            #endregion


            #endregion


        }

        public void Dispose()
        {
            if (_mDev>0)
            {
                D2KDask.D2K_Release_Card((ushort) _mDev);
            }
        }
        #endregion

    }
}
