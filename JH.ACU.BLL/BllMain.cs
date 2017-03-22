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
            _testCondition = new TestCondition();
            ChamberWorker = new NewBackgroundWorker();
            ChamberWorker.DoWork += ChamberWorker_DoWork;
            ChamberWorker.RunWorkerCompleted += ChamberWorker_RunWorkerCompleted;
            ChamberStay = new NewBackgroundWorker();
            ChamberStay.DoWork+=ChamberStay_DoWork;
            ChamberStay.RunWorkerCompleted+=ChamberStay_RunWorkerCompleted;
            TestWorker = new NewBackgroundWorker();
            TestWorker.DoWork+=TestWorker_DoWork;
            TestWorker.RunWorkerCompleted+=TestWorker_RunWorkerCompleted;
        }

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 启动仪器、设定外部环境
            OpenAllInstrs();


            #endregion

        }

        private void ChamberStay_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
            if (e.Cancelled)
            {
                //操作取消
            }
            else
            {
                //正常完成
            }
        }

        private void ChamberStay_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 使温度保持

            var tick = Environment.TickCount;
            do
            {
                #region 若取消

                if (ChamberStay.CancellationPending)
                {
                    _chamber.Stop();
                    e.Cancel = true;
                    return;
                }

                #endregion
                Thread.Sleep(30000);
                var currTemp = _chamber.GetTemp();
                ChamberStay.ReportProgress(0, currTemp);
            } while (Environment.TickCount - tick <= _testCondition.Temperature.Duration * 60 * 1000);

            #endregion

        }

        /// <summary>
        /// 温度达到要求时执行 //TODO:UI层还可以写一下同样方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChamberWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

            if (_testCondition.Temperature.Enable)
            {
                ChamberStay.RunWorkerAsync();
            }
            TestWorker.RunWorkerAsync();

        }

        /// <summary>
        /// 设定温度直到达到目标温度（误差为1度）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChamberWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!_testCondition.Temperature.Enable) return;
            if (_chamber == null)
            {
                _chamber = new BllChamber();
            }
            var tempValue = (double) e.Argument;
            _chamber.SetTemp(tempValue); //设置温度值
            var tick = Environment.TickCount; //获取当前时刻值
            do
            {
                #region 若取消

                if (ChamberWorker.CancellationPending)
                {
                    _chamber.Stop();
                    e.Cancel = true;
                    return;
                }

                #endregion

                Thread.Sleep(30000);
                var currTemp = _chamber.GetTemp();
                ChamberWorker.ReportProgress(0, currTemp);
            } while (Math.Abs(tempValue - _chamber.GetTemp()) <= 1 || Environment.TickCount - tick < 60*60*1000);
            _chamber.Stay();

        }

        #region 属性字段

        public NewBackgroundWorker ChamberWorker { get; private set; }
        public NewBackgroundWorker ChamberStay { get; private set; }
        public NewBackgroundWorker TestWorker { get; private set; }
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
        private void OpenAllInstrs()
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