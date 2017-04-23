using System;
using System.Runtime.InteropServices;

namespace AerialForWindows.Services {
    public static class MonitorHelper {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint msg,
            IntPtr wParam,
            IntPtr lParam
        );

        public static void PowerOff() {
            SendMessage(
                (IntPtr) 0xffff, // HWND_BROADCAST
                0x0112, // WM_SYSCOMMAND
                (IntPtr) 0xf170, // SC_MONITORPOWER
                (IntPtr) 0x0002 // POWER_OFF
            );
        }
    }
}