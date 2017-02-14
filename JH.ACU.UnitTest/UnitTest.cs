using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JH.ACU.BLL.Instruments;
using Xunit;

namespace JH.ACU.UnitTest
{
    public class UnitTest
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private byte[,] _relaysGroupMask = new byte[8, 14];

        private void GetCurrMask(byte iCardNum, int iRelay, bool bEnable, ref byte iGroup, ref byte iMask)
        {
            if (iCardNum > 7) return;
            iGroup = (byte) ((iRelay - 200)/10);
            if (iGroup > 13) return;
            int iBit = (iRelay - 200)%10;
            uint ulPaStatus = 0x01;
            ulPaStatus = ulPaStatus << iBit;
            if (false == bEnable)
            {
                ulPaStatus = ~ulPaStatus;
                iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & ulPaStatus);
                return;
            }

            if (iBit >= 0 && iBit <= 3)
            {
                if (iGroup == 6)
                    iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & 0x3F | ulPaStatus);
                if (iGroup == 7)
                    iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & 0x8F | ulPaStatus);
                if ((iGroup != 6) && (iGroup != 7))
                    iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & 0x0F | ulPaStatus);
            }
            else
            {
                if (iGroup == 6)
                    iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & 0xF0 | ulPaStatus);
                if (iGroup == 7)
                    iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & 0xF8 | ulPaStatus);
                if ((iGroup != 6) && (iGroup != 7))
                    iMask = (byte) (_relaysGroupMask[iCardNum,iGroup] & 0xF0 | ulPaStatus);

            }

        }

        public void Test()
        {
            byte groupNum = 0, mask = 0;
            GetCurrMask(4, 205, true, ref groupNum, ref mask);

        }

        public void GetRelaysMask(byte boardIndex, int relayIndex, bool enable, out byte groupIndex, out byte mask)
        {
            if (boardIndex > 8 - 1) throw new ArgumentException("输入板卡索引无效", "boardIndex");
            if (relayIndex < 200 || relayIndex > 340 || (relayIndex > 287 && relayIndex < 310))
            {
                throw new ArgumentException("输入继电器索引无效", "relayIndex");
            }
            if (relayIndex > 199 && relayIndex < 288)
            {
                groupIndex = (byte)((relayIndex - 200) / 10);
            }
            else
            {
                groupIndex = (byte)((relayIndex - 200) / 10 - 2);

            }
            if (groupIndex > 13 - 1) throw new ArgumentException("输入继电器索引无效", "relayIndex");

            var iBit = relayIndex % 10;
            if (iBit > 7) throw new ArgumentException("输入继电器索引无效", "relayIndex");
            byte ulPaStatus = 0x01;
            ulPaStatus = (byte)(ulPaStatus << iBit);
            if (!enable)
            {
                ulPaStatus = (byte)~ulPaStatus;
                mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & ulPaStatus);
                return;
            }
            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] | ulPaStatus);

            #region 原方法内容，作用待定

            //if (iBit >= 0 && iBit <= 3)
            //{
            //    switch (groupIndex)
            //    {
            //        case 6:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0x3F | ulPaStatus);
            //            break;
            //        case 7:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0x8F | ulPaStatus);
            //            break;
            //        default:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0x0F | ulPaStatus);
            //            break;
            //    }
            //}
            //else
            //{
            //    switch (groupIndex)
            //    {
            //        case 6:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0xF0 | ulPaStatus);
            //            break;
            //        case 7:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0xF8 | ulPaStatus);
            //            break;
            //        default:
            //            mask = (byte)(_relaysGroupMask[boardIndex, groupIndex] & 0xF0 | ulPaStatus);
            //            break;
            //    }

            //}

            #endregion

        }

        [Fact]
        public void Test1()
        {
            for (int i = 0; i < 350; i++)
            {
                byte mask;
                byte group;
                if (i<200||i>340)
                {
                    Assert.Throws<ArgumentException>(() => GetRelaysMask(3, i, true, out group, out mask));
                }
            }
        }
    }
}
