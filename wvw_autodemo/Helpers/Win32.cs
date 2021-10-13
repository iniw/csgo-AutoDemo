using System;
using System.Runtime.InteropServices;

namespace wvw_autodemo
{
    public class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT : IDisposable
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;

            public void Dispose()
            {
                if (lpData != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(lpData);
                    lpData = IntPtr.Zero;
                    cbData = 0;
                }
            }
            public static COPYDATASTRUCT CreateForString(int dwData, string value, bool Unicode = false)
            {
                var result = new COPYDATASTRUCT();
                result.dwData = (IntPtr)dwData;
                result.lpData = Unicode ? Marshal.StringToCoTaskMemUni(value) : Marshal.StringToCoTaskMemAnsi(value);
                result.cbData = value.Length + 1;
                return result;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);
    }
}