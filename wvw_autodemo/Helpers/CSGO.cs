using System;

namespace wvw_autodemo
{
    public class CSGO
    {
        public static bool ExecuteCmd(string cmd, string args = "")
        {
            var message = cmd + args;

            var cds = Win32.COPYDATASTRUCT.CreateForString(0, message);
            if (cds.lpData == IntPtr.Zero)
                return false;

            var window = Win32.FindWindow("Valve001", null);
            if (window == IntPtr.Zero)
                return false;

            Win32.SendMessage(window, 0x4A, IntPtr.Zero, ref cds);

            cds.Dispose();

            return true;
        }

    }

}