using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ivi.Visa;
using NationalInstruments.Visa;

namespace JH.ACU.DAL
{
    /// <summary>
    /// 未完成
    /// </summary>
    [Obsolete("暂时不用该接口")]
    public interface IDalVisa
    {
        MessageBasedSession MbSession { get; set; }
        IMessageBasedRawIO RawIo { get; set; }
    }
}
