using Microsoft.Win32;

namespace wvw_autodemo
{
    public class RegistryInfo
    {
        public static readonly RegistryKey STARTUPKEY = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public static readonly string STEAMPATH = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", "").ToString();
    }
}