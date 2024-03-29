﻿using System;
using System.Threading;
using System.Windows.Forms;
using JH.ACU.Lib;

namespace JH.ACU.UI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //设置应用程序处理异常方式：ThreadException处理  
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常  
                Application.ThreadException += Application_ThreadException;
                //处理非UI线程异常  
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;  
 
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                _mainForm=new MainForm();
                Application.Run(_mainForm);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(ex.Message);
                LogHelper.WriteErrorLog("Main",ex);
            }
        }

        private static MainForm _mainForm;
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            MessageBoxHelper.ShowError(ex.Message);
            LogHelper.WriteErrorLog("non-UI Exception", ex, string.Format("Runtime terminating: {0}", e.IsTerminating));
            if (_mainForm != null)
            {
                _mainForm.SetControlEnabled(true);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            MessageBoxHelper.ShowError(ex.Message);
            LogHelper.WriteErrorLog("UI Exception", ex);
            if (_mainForm != null)
            {
                _mainForm.SetControlEnabled(true);
            }
        }
    }
}
