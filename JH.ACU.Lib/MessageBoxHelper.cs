using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JH.ACU.Lib
{
    /// <summary>
    /// 显示消息服务
    /// </summary>
    public static class MessageBoxHelper
    {
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="header"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ShowQuestion(string header, string message)
        {
            return MessageBox.Show(message, header, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;
        }
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ShowQuestion(string message)
        {
            return MessageBox.Show(message, "A-Bench Notice",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ShowInformation(string message)
        {
            return MessageBox.Show(message, "A-Bench Notice",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes;
        }
        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void ShowInformationOK(string message)
        {
            MessageBox.Show(message, "A-Bench Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool ShowWarning(string message)
        {
            return MessageBox.Show(message, "A-Bench Notice",
                MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK;
        }

        public static bool ShowStop(string message)
        {
            return MessageBox.Show(message, "A-Bench Notice",
                MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes;
        }

        public static bool ShowError(string message)
        {
            return MessageBox.Show(message, "A-Bench Notice",
                MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK;
        }

        public static bool ShowHand(string message)
        {
            return MessageBox.Show(message, "A-Bench Notice",
                MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes;
        }

        /// <summary>
        /// 鼠标处显示ToolTip信息
        /// </summary>
        /// <param name="form"></param>
        /// <param name="message"></param>
        public static void ShowTipMessage(Form form, string message)
        {
            var tolTip = new ToolTip();
            tolTip.Show(message, form, form.PointToClient(Control.MousePosition), 1000);
        }
    }
}
