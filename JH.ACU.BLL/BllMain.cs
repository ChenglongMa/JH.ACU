using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using JH.ACU.BLL.Instruments;
using JH.ACU.Model;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.BLL
{
    public class BllMain
    {
        public BllMain()
        {
            _testCondition=new TestCondition();
            TempWorker = new NewBackgroundWorker();
            TempWorker.DoWork+=TempWorker_DoWork;
            TempWorker.RunWorkerCompleted+=TempWorker_RunWorkerCompleted;
        }
        /// <summary>
        /// 温度达到要求时执行 //TODO:UI层还可以写一下同样方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TempWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
            if (e.Cancelled)
            {
                //操作取消
                return;
            }
            {
                //TODO；补充后续操作
            }
        }

        /// <summary>
        /// 设定温度直到达到目标温度（误差为1度）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TempWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!_testCondition.Temperature.Enable) return;
            if (_chamber == null)
            {
                _chamber = new BllChamber();
            }
            var tempValue = (double) e.Argument;
            _chamber.SetTemp(tempValue);//设置温度值
            var tick = Environment.TickCount;//获取当前时刻值
            do
            {
                #region 若取消

                if (TempWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                #endregion
                Thread.Sleep(30000);
                var currTemp = _chamber.GetTemp();
                TempWorker.ReportProgress(0, currTemp);
            } while (Math.Abs(tempValue - _chamber.GetTemp()) <= 1 || Environment.TickCount - tick < 60*60*1000);
            tick = Environment.TickCount;
            _chamber.Stay();

            #region 使温度保持

            //do
            //{
            //    #region 若取消

            //    if (TempWorker.CancellationPending)
            //    {
            //        e.Cancel = true;
            //        break;
            //    }

            //    #endregion
            //    Thread.Sleep(30000);

            //    var currTemp = _chamber.GetTemp();
            //    TempWorker.ReportProgress(0,currTemp);
            //} while (Environment.TickCount - tick <= _testCondition.Temperature.Duration * 60 * 1000);

            #endregion


        }

        #region 属性字段

        public readonly NewBackgroundWorker TempWorker;
        private readonly TestCondition _testCondition;

        #region 各类仪器

        private BllAcu _acu;
        private BllChamber _chamber;
        private BllDaq _daq;
        private BllDmm _dmm;
        private BllPrs _prs0;
        private BllPrs _prs1;
        private BllPwr _pwr;

        #endregion



        #endregion

        #region 私有方法
        /// <summary>
        /// 开启所有仪器
        /// </summary>
        private void Open()
        {
            _chamber = new BllChamber();
            _daq = new BllDaq();
            _daq.Initialize();
            _dmm = new BllDmm();
            _dmm.Initialize();
            _prs0 = new BllPrs(InstrName.Prs0);
            _prs0.Initialize();
            _prs1 = new BllPrs(InstrName.Prs0);
            _prs1.Initialize();
            _pwr = new BllPwr();
            _pwr.Initialize();
        }

        #endregion

        #region 公有方法

        #endregion

    }
    /// <summary>
    /// 继承自BackgroundWorker，设置属性默认值
    /// </summary>
    public class NewBackgroundWorker : BackgroundWorker
    {
        public NewBackgroundWorker()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;
        }
    }
}
