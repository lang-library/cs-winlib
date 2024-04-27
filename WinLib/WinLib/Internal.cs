using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using static WinLib.Util;
namespace WinLib;
internal class Internal
{
    public static string InstallResourceDll(string name)
    {
        int bit = IntPtr.Size * 8;
        return Installer.InstallResourceDll(
            typeof(Internal).Assembly,
            Dirs.ProfilePath(".javacommons", "WinLib"),
            $"WinLib:{name}-x{bit}.dll"
            );

    }
    public static string InstallResourceZip(string name)
    {
        int bit = IntPtr.Size * 8;
        string dir = Installer.InstallResourceZip(
            typeof(Internal).Assembly,
            Dirs.ProfilePath(".javacommons", "WinLib"),
            $"WinLib:{name}.zip"
            );
        return Path.Combine(dir, $"x{bit}");
    }
}
