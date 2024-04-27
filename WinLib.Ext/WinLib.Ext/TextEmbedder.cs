using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace WinLib;
public static class TextEmbedder
{
    const long MinimumCheckLength = 8192;
    //const long MinimumCheckLength = 256;
    internal class SearchResult
    {
        public long Length {
            get; set;
        }
        public long StartPos {
            get; set;
        }
        public long EndPos {
            get; set;
        }
    }
    private static SearchResult CheckTailBytes(long offset, byte[] bytes)
    {
        SearchResult result = new SearchResult() {
            Length = offset + bytes.Length, StartPos = -1, EndPos = -1
        };
        const string neutral = "IBM437";
        string part = Encoding.GetEncoding(neutral).GetString(bytes);
        string pattern = @"\[/embed(:[0-9a-zA-Z]+)?\]\s*$";
        Match m = Regex.Match(part, pattern);
        if (m.Success)
        {
            string startTag = $"[embed{m.Groups[1].Value}]";
            string endTag = $"[/embed{m.Groups[1].Value}]";
            result.EndPos = part.LastIndexOf(endTag);
            if (result.EndPos >= 0)
            {
                int idx = part.LastIndexOf(startTag, (int)result.EndPos);
                if (idx >= 0)
                {
                    result.Length = offset + idx;
                    result.StartPos = idx + startTag.Length;
                    long len = result.EndPos - result.StartPos;
                    string s = part.Substring((int)result.StartPos, (int)len);
                }
            }
        }
        return result;
    }
    private static long GetLength(string path)
    {
        if (path.StartsWith("http:") || path.StartsWith("https:"))
        {
            using (var fs = new PartialHTTPStream(path))
            {
                return fs.Length;
            }
        }
        using (var fs = File.OpenRead(path))
        {
            return fs.Length;
        }
    }
    private static byte[] GetHeadBytes(string path, long size)
    {
        if (path.StartsWith("http:") || path.StartsWith("https:"))
        {
            using (var fs = new PartialHTTPStream(path))
            {
                long fileLen = fs.Length;
                if (size > fileLen) size = fileLen;
                byte[] result = new byte[size];
                fs.Read(result, 0, result.Length);
                return result;
            }
        }
        using (var fs = File.OpenRead(path))
        {
            long fileLen = fs.Length;
            if (size > fileLen) size = fileLen;
            byte[] result = new byte[size];
            fs.Read(result, 0, result.Length);
            return result;
        }
    }
    private static byte[] GetTailBytes(string path, long size)
    {
        if (path.StartsWith("http:") || path.StartsWith("https:"))
        {
            using (var fs = new PartialHTTPStream(path))
            {
                long fileLen = fs.Length;
                if (size > fileLen) size = fileLen;
                long pos = fileLen - size;
                byte[] result = new byte[size];
                fs.Seek(pos, SeekOrigin.Begin);
                fs.Read(result, 0, result.Length);
                return result;
            }
        }
        using (var fs = File.OpenRead(path))
        {
            long fileLen = fs.Length;
            if (size > fileLen) size = fileLen;
            long pos = fileLen - size;
            byte[] result = new byte[size];
            fs.Seek(pos, SeekOrigin.Begin);
            fs.Read(result, 0, result.Length);
            return result;
        }
    }
    public static string TextEmbed(string path)
    {
        try
        {
            long fileLen = GetLength(path);
            long checkLen = MinimumCheckLength;
            while (true)
            {
                if (checkLen > fileLen) checkLen = fileLen;
                {
                }
                byte[] check = GetTailBytes(path, checkLen);
                SearchResult checkResult = CheckTailBytes(fileLen - checkLen, check);
                if (checkResult.EndPos < 0) return null;
                if (checkResult.StartPos >= 0)
                {
                    long len = checkResult.EndPos - checkResult.StartPos;
                    byte[] result = new byte[len];
                    Array.Copy(check, checkResult.StartPos, result, 0, len);
                    return Encoding.UTF8.GetString(result).Trim();
                }
                if (checkLen >= fileLen)
                {
                    return null;
                }
                checkLen *= 2;
            }
        }
        catch (Exception e)
        {
            Util.Log(e.ToString());
            return null;
        }
    }
    public static long SizeActual(string path)
    {
        try
        {
            long fileLen = GetLength(path);
            long checkLen = MinimumCheckLength;
            while (true)
            {
                if (checkLen > fileLen) checkLen = fileLen;
                byte[] check = GetTailBytes(path, checkLen);
                SearchResult checkResult = CheckTailBytes(fileLen - checkLen, check);
                if (checkResult.EndPos < 0) return checkResult.Length;
                if (checkResult.StartPos >= 0)
                {
                    return checkResult.Length;
                }
                if (checkLen >= fileLen)
                {
                    return checkResult.Length;
                }
                checkLen *= 2;
            }
        }
        catch (Exception e)
        {
            Util.Log(e.ToString());
            return 0;
        }
    }
    public static string TextActual(string path)
    {
        try
        {
            long size = SizeActual(path);
            return Encoding.UTF8.GetString(GetHeadBytes(path, size));
        }
        catch (Exception e)
        {
            Util.Log(e.ToString());
            return null;
        }
    }
}
