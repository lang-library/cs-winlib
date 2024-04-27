using System;

namespace WinLib;
public class DLL0
{
    public static JsonAPI API = null;
    static DLL0()
    {
        string dllPath = Internal.InstallResourceDll("dll0");
        API = new JsonAPI(dllPath);
    }
}
