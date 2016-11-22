using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JH.ACU.DAL;

namespace JH.ACU.BLL
{
    public class BllGpib
    {
        #region GPIB操作包括DMM,PWR,PRS0,PRS1
        
        public short DMMaddr = 22, PWRaddr = 8, PRS0addr = 16, PRS1addr = 17;
        private int ud = 0;
        private int ibsta, iberr, ibcnt, ibcntl;
        public bool Write(int addr, string strWrite)
        {
            //Open and intialize an GPIB instrument  

            int dev = DalGpib.ibdev(0, addr, 0, (int)DalGpib.gpib_timeout.T1s, 1, 0);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in initializing the GPIB instrument.");  
                return false;
            }

            //clear the specific GPIB instrument  
            DalGpib.ibclr(dev);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in clearing the GPIB device.");  
                return false;
            }

            //Write a string command to a GPIB instrument using the ibwrt() command  
            DalGpib.ibwrt(dev, strWrite, strWrite.Length);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in writing the string command to the GPIB instrument.");  
                return false;
            }

            //Offline the GPIB interface card  
            DalGpib.ibonl(dev, 0);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in offline the GPIB interface card.");  
                return false;
            }
            return true;
        }

        public bool Read(int addr, string strWrite, string strRead)
        {
            StringBuilder str = new StringBuilder(100);
            //Open and intialize an GPIB instrument  
            int dev = DalGpib.ibdev(0, addr, 0, (int)DalGpib.gpib_timeout.T1s, 1, 0);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in initializing the GPIB instrument.");  
                return false;
            }

            //clear the specific GPIB instrument  
            DalGpib.ibclr(dev);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in clearing the GPIB device.");  
                return false;
            }

            //Write a string command to a GPIB instrument using the ibwrt() command  
            DalGpib.ibwrt(dev, strWrite, strWrite.Length);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in writing the string command to the GPIB instrument.");  
                return false;
            }

            //Read the response string from the GPIB instrument using the ibrd() command  
            DalGpib.ibrd(dev, str, 100);
            strRead = str.ToString();
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in reading the response string from the GPIB instrument.");  
                return false;
            }

            //Offline the GPIB interface card  
            DalGpib.ibonl(dev, 0);
            DalGpib.gpib_get_globals(out ibsta, out iberr, out ibcnt, out ibcntl);
            if ((ibsta & (int)DalGpib.ibsta_bits.ERR) != 0)
            {
                //MessageBox.Show("Error in offline the GPIB interface card.");  
                return false;
            }
            return true;
        }

        #region 数字万用表操作集合

        /// <summary>  
        /// 数字万用表：读取电压  
        /// </summary>  
        public bool DMM_ReadVoltage(double value)
        {
            try
            {
                string strRead = "";
                if (Write(DMMaddr, "CONF:VOLT:DC DEF") == false) return false;
                if (Read(DMMaddr, "READ?", strRead) == false) return false;
                value = Convert.ToDouble(strRead);
                return true;
            }
            catch
            {
                //MessageBox.Show("Voltage Read Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：读取电流  
        /// </summary>  
        public bool DMM_ReadCurrent(double value)
        {
            try
            {
                string strRead = "";
                if (Write(DMMaddr, "CONF:CURR:DC DEF") == false) return false;
                if (Read(DMMaddr, "READ?", strRead) == false) return false;
                value = Convert.ToDouble(strRead);
                return true;
            }
            catch
            {
                //MessageBox.Show("Voltage Read Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：读取阻值  
        /// </summary>  
        public bool DMM_ReadRes(double value)
        {
            try
            {
                string strRead = "";
                if (Write(DMMaddr, "CONF:RES DEF") == false) return false;
                if (Read(DMMaddr, "READ?", strRead) == false) return false;
                value = Convert.ToDouble(strRead);
                return true;
            }
            catch
            {
                //MessageBox.Show("Voltage Read Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：读取频率  
        /// </summary>  
        public bool DMM_ReadFreq(double value)
        {
            try
            {
                string strRead = "";
                if (Write(DMMaddr, "CONF:FRES:DC DEF") == false) return false;
                if (Read(DMMaddr, "READ?", strRead) == false) return false;
                value = Convert.ToDouble(strRead);
                return true;
            }
            catch
            {
                //MessageBox.Show("Voltage Read Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：设定直流电压范围  
        /// </summary>  
        public bool DMM_SetDCRange(double VRange, double VDelta)
        {
            try
            {
                return Write(DMMaddr, "CONF:VOLT:DC " + VRange.ToString() + "," + VDelta.ToString());
            }
            catch
            {
                ////MessageBox.Show("Voltage DC Range Setting Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：恢复设置  
        /// </summary>  
        public bool DMM_Reset()
        {
            try
            {
                if (Write(DMMaddr, "*RST") == false) return false;
                return Write(DMMaddr, "*CLS");
            }
            catch
            {
                ////MessageBox.Show("DMM Reset Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：设定自动调零  
        /// </summary>  
        public bool DMM_AutoZero(bool autoZero)
        {
            try
            {
                if (autoZero) return Write(DMMaddr, "ZERO:AUTO ON");
                else return Write(DMMaddr, "ZERO:AUTO OFF");
            }
            catch
            {
                ////MessageBox.Show("DMM Auto Zero Setting Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：设定触发延时  
        /// </summary>  
        public bool DMM_TrigDelay(double trigDelay)
        {
            try
            {
                return Write(DMMaddr, "TRIG:DEL " + trigDelay.ToString());
            }
            catch
            {
                ////MessageBox.Show("DMM Auto Zero Setting Fail!");  
                return false;
            }
        }

        /// <summary>  
        /// 数字万用表：设定采样点数  
        /// </summary>  
        public bool DMM_SampleCounts(uint sampleCount)
        {
            try
            {
                return Write(DMMaddr, "SAMP:COUN " + sampleCount.ToString());
            }
            catch
            {
                ////MessageBox.Show("DMM Sample Counts Setting Fail!");  
                return false;
            }
        }

        #endregion

        #region 程控电源操作集合

        /// <summary>  
        /// 设置电压值  
        /// </summary>  
        public bool PWR_SetVoltage(double voltageSet)
        {
            try
            {
                return Write(PWRaddr, ":CHAN1:VOLT " + voltageSet.ToString() + "/n");
            }
            catch
            {
                ////MessageBox.Show("PWR Voltage Setting Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 设置电流值  
        /// </summary>  
        public bool PWR_SetCurrent(double currentSet)
        {
            try
            {
                return Write(PWRaddr, ":CHAN1:CURR " + currentSet.ToString() + "/n");
            }
            catch
            {
                ////MessageBox.Show("PWR Current Setting Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 限流保护  
        /// </summary>  
        public bool PWR_LimitCurrent(bool currentLimit)
        {
            try
            {
                return Write(PWRaddr, ":CHAN1:PROT:CURR " + currentLimit.ToString() + "/n");
            }
            catch
            {
                ////MessageBox.Show("PWR Current (Un)Limit Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 限压保护  
        /// </summary>  
        public bool PWR_LimitVoltage(bool voltageLimit)
        {
            try
            {
                return Write(PWRaddr, ":CHAN1:PROT:VOLT " + voltageLimit.ToString() + "/n");
            }
            catch
            {
                ////MessageBox.Show("PWR Voltage (Un)Limit Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 启动输出  
        /// </summary>  
        public bool PWR_Output(bool output)
        {
            try
            {
                if (output)
                {
                    if (Write(PWRaddr, "OUTP:PROT:CLE/n") == false) return false;
                    return Write(PWRaddr, "OUTP:STAT 1/n");
                }
                else return Write(PWRaddr, "OUTP:STAT 0/n");
            }
            catch
            {
                ////MessageBox.Show("PWR (Un)Output Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 获取输出状态  
        /// </summary>  
        public bool PWR_GetOutput(bool output)
        {
            string strRead = "";
            output = false;
            try
            {
                if (Read(PWRaddr, "OUTP:STAT?/n", strRead) == false) return false;
                if (strRead == "1/n") output = true;
                return true;
            }
            catch
            {
                //MessageBox.Show("PWR (Un)Output Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 获取电压  
        /// </summary>  
        public bool PWR_GetVoltage(double value)
        {
            string strRead = "";
            try
            {
                if (Read(PWRaddr, ":CHAN1:MEAS:VOLT?/n", strRead) == false) return false;
                value = Convert.ToDouble(strRead);
                return true;
            }
            catch
            {
                //MessageBox.Show("PWR Get Voltage Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 获取电流  
        /// </summary>  
        public bool PWR_GetCurrent(double value)
        {
            string strRead = "";
            try
            {
                if (Read(PWRaddr, ":CHAN1:MEAS:CURR?/n", strRead) == false) return false;
                value = Convert.ToDouble(strRead);
                return true;
            }
            catch
            {
                //MessageBox.Show("PWR Get Current Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 恢复设置  
        /// </summary>  
        public bool PWR_Reset()
        {
            try
            {
                return Write(PWRaddr, "*RST/n");
            }
            catch
            {
                ////MessageBox.Show("PWR Reset Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 清除状态  
        /// </summary>  
        public bool PWR_Clear()
        {
            try
            {
                return Write(PWRaddr, "*CLS/n");
            }
            catch
            {
                ////MessageBox.Show("PWR Reset Fail");  
                return false;
            }
        }

        #endregion

        #region 程控变阻箱操作集合

        /// <summary>  
        /// 设置电阻值0  
        /// </summary>  
        public bool PRS0_SetResistance(ulong resSet)
        {
            try
            {
                string resString = "";
                string resStringAdd = resSet.ToString();
                if (resStringAdd.Length < 2) resString = "SOURce:DATA 200000000000";
                else if (resStringAdd.Length > 8) resString = "SOURce:DATA 100000000000";
                else
                {
                    for (short i = 12; i > resStringAdd.Length; i--) resString += "0";
                    //resStringAdd.PadRight(12, '0');  
                    resString += resStringAdd;
                    resString = "SOURce:DATA " + resString;
                }
                return (Write(PRS0addr, resString));
            }
            catch
            {
                ////MessageBox.Show("PRS0 Setting Fail");  
                return false;
            }
        }

        /// <summary>  
        /// 设置电阻值1  
        /// </summary>  
        public bool PRS1_SetResistance(ulong resSet)
        {
            try
            {
                string resString = "";
                string resStringAdd = resSet.ToString();
                if (resStringAdd.Length < 2) resString = "SOURce:DATA 200000000000";
                else if (resStringAdd.Length > 8) resString = "SOURce:DATA 100000000000";
                else
                {
                    for (short i = 12; i > resStringAdd.Length; i--) resString += "0";
                    //resStringAdd.PadRight(12, '0');  
                    resString += resStringAdd;
                    resString = "SOURce:DATA " + resString;
                }
                return (Write(PRS1addr, resString));
            }
            catch
            {
                ////MessageBox.Show("PRS1 Setting Fail");  
                return false;
            }
        }

        #endregion

        #endregion  

    }
}
