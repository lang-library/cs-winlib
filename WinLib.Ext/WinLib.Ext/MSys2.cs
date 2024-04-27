using System;
using System.Collections.Generic;
using System.IO;

namespace WinLib;
public class MSys2
{
    public static string MSys2Bin;
    static MSys2()
    {
        string zipBaseName = "msys2-base-x86_64-20240113";
        string installDir = Installer.InstallZipFromURL(
            $"https://github.com/nuget-tools/JsonDLL.Assets/releases/download/64bit/{zipBaseName}.zip",
            Path.Combine(Dirs.ProfilePath(".javacommons", "WinLib"), @$"msys2"),
            zipBaseName
            );
        MSys2Bin = Path.Combine(installDir, "usr\\bin");
    }
    public static void Initialize()
    {
        ;
    }
    public static int RunBashScript(bool windowed, string script, string cwd = "")
    {
        string bashExe = Path.Combine(MSys2.MSys2Bin, "bash.exe");
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, script);
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = MSys2.MSys2Bin + ";" + PATH;
        int result = ProcessRunner.RunProcess(windowed, bashExe, new string[] { tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        });
        File.Delete(tempFile);
        return result;
    }
    public static bool LaunchBashScript(bool windowed, string script, string cwd = "")
    {
        string bashExe = Path.Combine(MSys2.MSys2Bin, "bash.exe");
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, script);
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = MSys2.MSys2Bin + ";" + PATH;
        bool result = ProcessRunner.LaunchProcess(windowed, bashExe, new string[] { tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        }, tempFile);
        return result;
    }
    public static string BashScriptOutputUtf8(bool merge, string script, string cwd = "")
    {
        string bashExe = Path.Combine(MSys2.MSys2Bin, "bash.exe");
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, script);
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = MSys2.MSys2Bin + ";" + PATH;
        string result = ProcessRunner.ProcessOutputUtf8(merge, bashExe, new string[] { tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        });
        File.Delete(tempFile);
        return result.Trim();
    }
    public static string BashScriptOutputAnsi(bool merge, string script, string cwd = "")
    {
        string bashExe = Path.Combine(MSys2.MSys2Bin, "bash.exe");
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, script);
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = MSys2.MSys2Bin + ";" + PATH;
        string result = ProcessRunner.ProcessOutputLocal8Bit(merge, bashExe, new string[] { tempFile }, cwd, new Dictionary<string, string> {
            { "PATH", PATH }
        });
        File.Delete(tempFile);
        return result.Trim();
    }
    public static string BashScriptOutput(bool merge, string script, string cwd = "")
    {
        string bashExe = Path.Combine(MSys2.MSys2Bin, "bash.exe");
        string tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, script);
        string PATH = Environment.GetEnvironmentVariable("PATH");
        PATH = MSys2.MSys2Bin + ";" + PATH;
        byte[] bytes = ProcessRunner.ProcessOutputBytes(merge, bashExe, new string[] { tempFile }, cwd, new Dictionary<string, string> {
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
