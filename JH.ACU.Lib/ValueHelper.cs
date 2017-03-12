using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NationalInstruments.Restricted;

namespace JH.ACU.Lib
{
    /// <summary>
    /// 值帮助类
    /// </summary>
    public static class ValueHelper
    {
        #region 值转换

        #region 大小端判断

        private static readonly bool LittleEndian;

        static ValueHelper()
        {
            unsafe
            {
                var tester = 1;
                LittleEndian = *(byte*) &tester == 1;
            }
        }

        private static byte[] Reverse(byte[] data)
        {
            if (LittleEndian)
            {
                Array.Reverse(data);
            }
            return data;
        }

        #endregion

        public static byte[] GetBytes(this ushort value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(this short value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(this int value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(this float value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(this double value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static short GetShort(this byte[] data)
        {
            return BitConverter.ToInt16(Reverse(data), 0);
        }

        public static int GetInt(this byte[] data)
        {
            return BitConverter.ToInt32(Reverse(data), 0);
        }

        public static float GetFloat(this byte[] data)
        {
            return BitConverter.ToSingle(Reverse(data), 0);
        }

        public static double GetDouble(this byte[] data)
        {
            return BitConverter.ToDouble(Reverse(data), 0);
        }

        #endregion

        #region 值判断
        /// <summary>
        /// 判断值是否为null或string.Empty或空白或不含有任何元素
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this IEnumerable data)
        {
            return data == null || data.IsEmpty();
        }
        #endregion

        #region 值获取

        public static int GetBit(this byte value, int index)
        {
            return index < 0 || index > 7 ? -1 : (value >> index) & 1;
        }

        #endregion
    }
}
