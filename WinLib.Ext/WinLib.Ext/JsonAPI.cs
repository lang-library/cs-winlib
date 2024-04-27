using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace WinLib;
public class JsonAPI
{
    static Dictionary<int, JsonAPI> apiMap = new Dictionary<int, JsonAPI>();
    IntPtr Handle = IntPtr.Zero;
    IntPtr CallPtr = IntPtr.Zero;
    IntPtr LastErrorPtr = IntPtr.Zero;
    delegate IntPtr proto_Call(IntPtr name, IntPtr args);
    delegate IntPtr proto_LastError();
    public JsonAPI()
    {
        // for server
    }
    public JsonAPI(string dllSpec)
    {
        string dllPath = Util.FindExePath(dllSpec);
        if (dllPath is null)
        {
            Util.Log(dllSpec, "dllSpec");
            Util.Log(dllPath, "dllPath");
            Environment.Exit(1);
        }
        this.LoadDll(dllPath);
    }
    public JsonAPI(string dllSpec, string cwd)
    {
        string dllPath = Util.FindExePath(dllSpec, cwd);
        if (dllPath is null)
        {
            Util.Log(dllSpec, "dllSpec");
            Util.Log(dllPath, "dllPath");
            Environment.Exit(1);
        }
        this.LoadDll(dllPath);
    }
    public JsonAPI(string dllSpec, Assembly assembly)
    {
        string dllPath = Util.FindExePath(dllSpec, assembly);
        if (dllPath is null)
        {
            Util.Log(dllSpec, "dllSpec");
            Util.Log(dllPath, "dllPath");
            Environment.Exit(1);
        }
        this.LoadDll(dllPath);
    }
    private void LoadDll(string dllPath)
    {
        this.Handle = LoadLibraryExW(
            dllPath,
            IntPtr.Zero,
            LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH
            );
        if (this.Handle == IntPtr.Zero)
        {
            Util.Log($"DLL not loaded: {dllPath}");
            Environment.Exit(1);
        }
        this.CallPtr = GetProcAddress(Handle, "Call");
        if (this.CallPtr == IntPtr.Zero)
        {
            Util.Log("Call() not found");
            Environment.Exit(1);
        }
        this.LastErrorPtr = GetProcAddress(Handle, "LastError");
#if false
        if (this.LastErrorPtr == IntPtr.Zero)
        {
            Util.Log("LastError() not found");
            Environment.Exit(1);
        }
#endif
    }
    public string LastError()
    {
        if (this.LastErrorPtr == IntPtr.Zero) return "";
        proto_LastError pLastError = (proto_LastError)Marshal.GetDelegateForFunctionPointer(this.LastErrorPtr, typeof(proto_LastError));
        IntPtr pResult = pLastError();
        //if (pResult == IntPtr.Zero) return null;
        string result = Util.UTF8AddrToString(pResult);
        return result;
    }
    public string CallToJson(dynamic name, dynamic args)
    {
        IntPtr pName = Util.StringToUTF8Addr(name);
        proto_Call pCall = (proto_Call)Marshal.GetDelegateForFunctionPointer(this.CallPtr, typeof(proto_Call));
        var argsJson = Util.ToJson(args);
        IntPtr pArgsJson = Util.StringToUTF8Addr(argsJson);
        IntPtr pResult = pCall(pName, pArgsJson);
        string result = Util.UTF8AddrToString(pResult);
        Marshal.FreeHGlobal(pName);
        Marshal.FreeHGlobal(pArgsJson);
        string error = LastError();
        if (error != "")
        {
            throw new Exception(error);
        }
        return result;
    }
    public dynamic Call(dynamic name, dynamic args)
    {
        return Util.FromJson(CallToJson(name, args));
    }
#if false
    public dynamic CallToObject(dynamic name, dynamic args)
    {
        var result = Call(name, args);
        return Util.ToObject(result);
    }
#endif
    static ThreadLocal<IntPtr> HandleCallPtr = new ThreadLocal<IntPtr>();
    static ThreadLocal<IntPtr> HandleLastErrorPtr = new ThreadLocal<IntPtr>();
    public IntPtr HandleCall(Type apiType, IntPtr nameAddr, IntPtr inputAddr)
    {
        if (HandleCallPtr.Value != IntPtr.Zero)
        {
            Util.FreeHGlobal(HandleCallPtr.Value);
            HandleCallPtr.Value = IntPtr.Zero;
        }
        if (HandleLastErrorPtr.Value != IntPtr.Zero)
        {
            Util.FreeHGlobal(HandleLastErrorPtr.Value);
            HandleLastErrorPtr.Value = IntPtr.Zero;
        }
        var name = Util.UTF8AddrToString(nameAddr);
        var input = Util.UTF8AddrToString(inputAddr);
        var args = Util.ToObject(Util.ParseJson(input));
        MethodInfo mi = apiType.GetMethod(name);
        dynamic result = null;
        if (mi == null)
        {
            result = $"API not found: {name}";
            HandleLastErrorPtr.Value = Util.StringToUTF8Addr($"API not found: {name}");
        }
        else
        {
            try
            {
                result = mi.Invoke(null, new object[] { args });
                HandleLastErrorPtr.Value = Util.StringToUTF8Addr("");
            }
            catch (TargetInvocationException ex)
            {
                result = ex.InnerException.ToString();
                HandleLastErrorPtr.Value = Util.StringToUTF8Addr(ex.InnerException.ToString());
            }
        }
        string output = Util.ToJson(result);
        HandleCallPtr.Value = Util.StringToUTF8Addr(output);
        return HandleCallPtr.Value;
    }
    public IntPtr HandleLastError()
    {
        return HandleLastErrorPtr.Value;
    }
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr LoadLibraryW(string lpFileName);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibraryExW(string dllToLoad, IntPtr hFile, LoadLibraryFlags flags);
    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = false)]
    internal static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
    [System.Flags]
    public enum LoadLibraryFlags : uint
    {
        DONT_RESOLVE_DLL_REFERENCES = 0x00000001,
        LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010,
        LOAD_LIBRARY_AS_DATAFILE = 0x00000002,
        LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 0x00000040,
        LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020,
        LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008,
        LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100,
        LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800,
        LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000
    }
}
