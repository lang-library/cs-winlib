using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using static WinLib.Util;
namespace WinLib.Ext;
internal class Internal
{
    public static string InstallResourceDll(string name)
    {
        int bit = IntPtr.Size * 8;
        return WinLib.Installer.InstallResourceDll(
            typeof(Internal).Assembly,
            WinLib.Dirs.ProfilePath(".javacommons", "WinLib"),
            $"WinLib.Ext:{name}-x{bit}.dll"
            );

    }
    public static string InstallResourceZip(string name)
    {
        int bit = IntPtr.Size * 8;
        string dir = WinLib.Installer.InstallResourceZip(
            typeof(Internal).Assembly,
            WinLib.Dirs.ProfilePath(".javacommons", "WinLib"),
            $"WinLib.Ext:{name}.zip"
            );
        return Path.Combine(dir, $"x{bit}");
    }
}
