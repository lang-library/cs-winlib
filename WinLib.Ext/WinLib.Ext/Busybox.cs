using System;
using System.Collections.Generic;
using System.IO;
namespace WinLib;
public class Busybox
{
    static string resDir = Internal.InstallResourceZip("res");
    static Busybox()
    {
    }
    public static void Initialize()
    {
        ;
    }
    public static int Run(string script, string cwd = "")
    {
        return RunBashScript(false, script, cwd);
    }
    public static int RunBashScript(bool windowed, string script, string cwd = "")
    {
        string busyboxExe = Path.Combine(resDir, "busybox.exe");
        string tempFile = Path.GetTempFileName();
        if (IntPtr.Size == 8)
        {
            File.WriteAllText(tempFile, script);
        }
        else
        {
            DLL1.API.Call("write_all_text_local8bit", new string[] { tempFile, script });
        }
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = resDir + ";" + PATH;
        int result = ProcessRunner.RunProcess(windowed, busyboxExe, new string[] { "bash", tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        });
        File.Delete(tempFile);
        return result;
    }
    public static bool Launch(string script, string cwd = "")
    {
        return LaunchBashScript(false, script, cwd);

    }
    public static bool LaunchBashScript(bool windowed, string script, string cwd = "")
    {
        string busyboxExe = Path.Combine(resDir, "busybox.exe");
        string tempFile = Path.GetTempFileName();
        if (IntPtr.Size == 8)
        {
            File.WriteAllText(tempFile, script);
        }
        else
        {
            DLL1.API.Call("write_all_text_local8bit", new string[] { tempFile, script });
        }
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = resDir + ";" + PATH;
        bool result = ProcessRunner.LaunchProcess(windowed, busyboxExe, new string[] { "bash", tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        }, tempFile);
        return result;
    }
    public static string Output(string script, string cwd = "")
    {
        return BashScriptOutput(false, script, cwd);
    }
    public static string BashScriptOutput(bool merge, string script, string cwd = "")
    {
        string busyboxExe = Path.Combine(resDir, "busybox.exe");
        string tempFile = Path.GetTempFileName();
        if (IntPtr.Size == 8)
        {
            File.WriteAllText(tempFile, script);
        }
        else
        {
            DLL1.API.Call("write_all_text_local8bit", new string[] { tempFile, script });
        }
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = resDir + ";" + PATH;
        byte[] bytes = ProcessRunner.ProcessOutputBytes(merge, busyboxExe, new string[] { "bash", tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        });
        string result;
        using (var ms = new MemoryStream(bytes))
        {
            var encoding = EncodeChecker.GetJpEncoding(bytes, true);
            result = encoding.GetString(bytes);
        }
        File.Delete(tempFile);
        return result.Trim();
    }
}
