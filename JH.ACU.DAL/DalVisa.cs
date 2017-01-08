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
    public abstract class DalVisa
    {
        protected DalVisa()
        {
        }
        protected MessageBasedSession MbSession { get; set; }



        public byte[] Read()
        {
            return MbSession.RawIO.Read();
        }

        public byte[] Read(long count)
        {
            return MbSession.RawIO.Read(count);
        }

        public byte[] Read(long count, out ReadStatus readStatus)
        {
            return MbSession.RawIO.Read(count,out readStatus);
        }

        public void Read(byte[] buffer, long index, long count, out long actualCount, out ReadStatus readStatus)
        {
            throw new NotImplementedException();
        }

        public string ReadString()
        {
            throw new NotImplementedException();
        }

        public string ReadString(long count)
        {
            throw new NotImplementedException();
        }

        public string ReadString(long count, out ReadStatus readStatus)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, long index, long count)
        {
            throw new NotImplementedException();
        }



        public virtual void Write(string str)
        {
            MbSession.RawIO.Write(str);
        }

        public void Write(string buffer, long index, long count)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(string buffer)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(string buffer, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(string buffer, VisaAsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(byte[] buffer, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(byte[] buffer, long index, long count)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(byte[] buffer, long index, long count, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(byte[] buffer, VisaAsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginWrite(byte[] buffer, long index, long count, VisaAsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public long EndWrite(IVisaAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(int count)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(int count, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(int count, VisaAsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(byte[] buffer, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(byte[] buffer, long index, long count)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(byte[] buffer, long index, long count, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(byte[] buffer, VisaAsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IVisaAsyncResult BeginRead(byte[] buffer, long index, long count, VisaAsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public long EndRead(IVisaAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string EndReadString(IVisaAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void AbortAsyncOperation(IVisaAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
