using System.Runtime.InteropServices;

#nullable disable

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct Utsname
{
    public fixed byte sysname[65];
    public fixed byte nodename[65];
    public fixed byte release[65];
    public fixed byte version[65];
    public fixed byte machine[65];
    public fixed byte domainname[65];
}

internal static class Native
{
    [DllImport("libc.so.6", CallingConvention = CallingConvention.Cdecl)]
    private static extern int uname(ref Utsname buf);

    public static unsafe string GetOsName()
    {
        Utsname u;
        uname(ref u);
        string os = Marshal.PtrToStringAnsi((IntPtr)u.sysname);
        string ver = Marshal.PtrToStringAnsi((IntPtr)u.release);
        return $"{os} {ver}";
    }
}
