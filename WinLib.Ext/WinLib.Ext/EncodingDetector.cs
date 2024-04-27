using System;
using System.Text;

namespace FatWinLib;

public class EncodingDetector
{
    public static Encoding DetectEncoding(byte[] contents)
    {
        if (contents == null || contents.Length == 0)
        {
            return Encoding.Default;
        }

        return TestCodePage(Encoding.UTF8, contents)
               ?? TestCodePage(Encoding.GetEncoding(932), contents)
               ?? TestCodePage(Encoding.Unicode, contents)
               ?? TestCodePage(Encoding.BigEndianUnicode, contents)
               ?? TestCodePage(Encoding.GetEncoding(1252), contents) // Western European
               ?? TestCodePage(Encoding.GetEncoding(28591), contents) // ISO Western European
               ?? TestCodePage(Encoding.ASCII, contents)
               ?? TestCodePage(Encoding.Default, contents); // likely Unicode
    }
    private static Encoding TestCodePage(Encoding testCode, byte[] byteArray)
    {
        try
        {
            var encoding = Encoding.GetEncoding(testCode.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            var a = encoding.GetCharCount(byteArray);
            return testCode;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
