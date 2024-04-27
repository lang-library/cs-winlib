using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FatWinLib;
public class DLL1
{
    public static WinLib.JsonAPI API = null;
    static DLL1()
    {
        string dllPath = Internal.InstallResourceDll("dll1");
        API = new WinLib.JsonAPI(dllPath);
    }
}
