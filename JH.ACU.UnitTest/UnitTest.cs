using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
