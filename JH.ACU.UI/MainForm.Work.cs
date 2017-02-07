using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.BLL.Instruments;
using JH.ACU.Lib;
using JH.ACU.Model;

namespace JH.ACU.UI
{
    partial class MainForm
    {
        private BllAcu _acu;
        private BllChamber _chamber;
        private BllDaq _daq;
        private BllDmm _dmm;
        private BllPrs _prs0;
        private BllPrs _prs1;
        private BllPwr _pwr;
        //打开设备，
        private bool Open()
        {
            try
            {
                _acu = new BllAcu();
                _chamber = new BllChamber();
                _daq = new BllDaq();
                _dmm = new BllDmm();
                _prs0 = new BllPrs(InstrName.PRS0);
                _prs1 = new BllPrs(InstrName.PRS1);
                _pwr = new BllPwr();
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("设备端口打开失败\n错误信息:" + ex.Message);
                return false;
            }
        }
        //设备初始化，包括自检
        private bool Initialize()
        {
            try
            {
                _pwr.Initialize();
                _dmm.Initialize();
                _prs0.Initialize();
                _prs1.Initialize();
                //ACU 不需要初始化
                //温箱不需要初始化
                //
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("设备初始化失败\n错误信息:" + ex.Message);
                return false;
            }
        }
    }
}
