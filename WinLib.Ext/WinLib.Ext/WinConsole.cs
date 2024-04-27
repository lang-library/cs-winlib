// https://stackoverflow.com/questions/15604014/no-console-output-when-using-allocconsole-and-target-architecture-x86
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Security.Cryptography;
using static WinLib.JsonAPI;
namespace WinLib;
internal static class WinConsole
{
    static public void Initialize(bool alwaysCreateNewConsole = true)
    {
        bool consoleAttached = true;
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            if (alwaysCreateNewConsole
                || (AttachConsole(ATTACH_PARRENT) == 0
                    && Marshal.GetLastWin32Error() != ERROR_ACCESS_DENIED))
            {
                consoleAttached = AllocConsole() != 0;
            }
        }
        else
        {
            consoleAttached = true;
        }
        var stdout = new StreamWriter(Console.OpenStandardOutput(), Encoding.Default);
        stdout.AutoFlush = true;
        Console.SetOut(stdout);
        var stderr = new StreamWriter(Console.OpenStandardError(), Encoding.Default);
        stderr.AutoFlush = true;
        Console.SetError(stderr);
    }
    static public void Deinitialize()
    {
        Console.SetOut(TextWriter.Null);
        Console.SetError(TextWriter.Null);
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            FreeConsole();
        }
    }
    #region Win API Functions and Constants
    [DllImport("kernel32.dll",
               EntryPoint = "AllocConsole",
               SetLastError = true,
               CharSet = CharSet.Auto,
               CallingConvention = CallingConvention.StdCall)]
    private static extern int AllocConsole();
    [DllImport("kernel32.dll",
               EntryPoint = "AttachConsole",
               SetLastError = true,
               CharSet = CharSet.Auto,
               CallingConvention = CallingConvention.StdCall)]
    private static extern UInt32 AttachConsole(UInt32 dwProcessId);
    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();
    private const UInt32 ERROR_ACCESS_DENIED = 5;
    private const UInt32 ATTACH_PARRENT = 0xFFFFFFFF;
    #endregion
}
