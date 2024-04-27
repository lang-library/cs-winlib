// https://codereview.stackexchange.com/questions/70679/seekable-http-range-stream
using System.IO;
using System.Net;
using System;
using static WinLib.Util;
//using CurlThin;
//using CurlThin.Enums;
//using CurlThin.Helpers;
//using CurlThin.SafeHandles;
using System.Text;
using System.Web.Routing;
namespace WinLib;
class PartialHTTPStream : Stream, IDisposable
{
    Stream stream = null;
    WebResponse resp = null;
    public string Url {
        get; private set;
    }
    public override bool CanRead {
        get { return true; }
    }
    public override bool CanWrite {
        get { return false; }
    }
    public override bool CanSeek {
        get { return true; }
    }
    long position = 0;
    public override long Position
    {
        get { return position; }
        set
        {
            long len = this.Length;
            if (value < 0 || value > len)
            {
                throw new ArgumentException($"Position out of range: {value}");
            }
            position = value;
            //Log($"Seek {value}");
        }
    }
    long? length;
    public override long Length
    {
        get
        {
            if (length == null)
            {
                HttpWebRequest req = null;
                try
                {
                    req = HttpWebRequest.CreateHttp(Url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = true;
                    length = req.GetResponse().ContentLength;
                }
                finally
                {
                    if (req != null)
                    {
                        // 連続呼び出しでエラーになる場合があるのでその対策
                        req.Abort();
                    }
                }
            }
            return length.Value;
        }
    }
    public PartialHTTPStream(string Url) {
        this.Url = Url;
    }
    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }
    public override int Read(byte[] buffer, int offset, int count)
    {
        //Log(new { offset = offset, count = count, Position = Position, Length = Length });
        if (count <= 0) return 0;
        HttpWebRequest req = null;
        try
        {
            req = HttpWebRequest.CreateHttp(Url);
            //Log("(1)");
            //req.AddRange(Position, Position + count - 1);
            req.AddRange(Position);
            req.AllowAutoRedirect = true;
            //Log("(2)");
            try
            {
                //Log("(3)");
                resp = req.GetResponse();
                //Log("(4)");
            }
            catch (WebException ex)
            {
                throw ex;
            }
            //Log("(5)");
            int rest = count;
            int nread = 0;
            using (Stream stream = resp.GetResponseStream())
            {
                while (true)
                {
                    int len = stream.Read(buffer, offset, rest);
                    if (len == 0) break;
                    nread += len;
                    offset += len;
                    rest -= len;
                }
            }
            //Log(nread, "nread");
            //Log("(6)");
            Position += nread;
            return nread;
        }
        finally
        {
            if (req != null)
            {
                // 連続呼び出しでエラーになる場合があるのでその対策
                req.Abort();
            }
        }
    }
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
    public override long Seek(long pos, SeekOrigin origin)
    {
        switch (origin)
        {
        case SeekOrigin.End:
            Position = Length + pos;
            break;
        case SeekOrigin.Begin:
            Position = pos;
            break;
        case SeekOrigin.Current:
            Position += pos;
            break;
        }
        return Position;
    }
    public override void Flush() {
    }
    new void Dispose()
    {
        base.Dispose();
        if (stream != null)
        {
            stream.Dispose();
            stream = null;
        }
        if (resp != null)
        {
            resp.Dispose();
            resp = null;
        }
    }
}
