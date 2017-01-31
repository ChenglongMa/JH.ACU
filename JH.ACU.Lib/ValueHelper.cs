using System;

namespace JH.ACU.Lib
{
    /// <summary>
    /// 值转换类
    /// </summary>
    public static class ValueHelper
    {
        #region 大小端判断
        private static readonly bool LittleEndian;

        static ValueHelper()
        {
            unsafe
            {
                var tester = 1;
                LittleEndian = *(byte*)&tester == 1;
            }
        }
        #endregion

        public static byte[] GetBytes(ushort value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(int value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(float value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static byte[] GetBytes(double value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public static short GetShort(byte[] data)
        {
            return BitConverter.ToInt16(Reverse(data), 0);
        }

        public static int GetInt(byte[] data)
        {
            return BitConverter.ToInt32(Reverse(data), 0);
        }

        public static float GetFloat(byte[] data)
        {
            return BitConverter.ToSingle(Reverse(data), 0);
        }

        public static double GetDouble(byte[] data)
        {
            return BitConverter.ToDouble(Reverse(data), 0);
        }

        private static byte[] Reverse(byte[] data)
        {
            if (LittleEndian)
            {
                Array.Reverse(data);
            }
            return data;
        }
    }
}
