using System;
using System.Collections;
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

        public static byte[] ToBytes(this ushort value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] ToBytes(this short value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] ToBytes(this int value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] ToBytes(this float value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] ToBytes(this double value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static short ToShort(this byte[] data)
        {
            return BitConverter.ToInt16(Reverse(data), 0);
        }

        public static int ToInt(this byte[] data)
        {
            return BitConverter.ToInt32(Reverse(data), 0);
        }

        public static float ToFloat(this byte[] data)
        {
            return BitConverter.ToSingle(Reverse(data), 0);
        }

        public static double ToDouble(this byte[] data)
        {
            return BitConverter.ToDouble(Reverse(data), 0);
        }
        /// <summary>
        /// 将数组转换为ASCII字符串
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public static string ToAscii(this byte[] dataBytes)
        {
            var asciiEncoding = new ASCIIEncoding();
            return asciiEncoding.GetString(dataBytes).Trim();
        }
        /// <summary>
        /// 将byte转换为ASCII字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToAscii(this byte data)
        {
            var asciiEncoding = new ASCIIEncoding();
            var dataBytes = new[] { data };
            return asciiEncoding.GetString(dataBytes).Trim();
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
        /// <summary>
        /// 获取byte值的bit位值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index">索引，0-7</param>
        /// <returns></returns>
        public static int GetBit(this byte value, int index)
        {
            return index < 0 || index > 7 ? -1 : (value >> index) & 1;
        }

        #endregion
    }
}
