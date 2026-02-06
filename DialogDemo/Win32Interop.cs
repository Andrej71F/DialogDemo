// SystemWideModalDialogWin32.cs
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DialogDemo
{
    internal static class Win32Interop
    {
        #region Public Fields

        public const uint SWP_NOSIZE = 0x0001;

        public const uint SWP_NOMOVE = 0x0002;

        public const uint SWP_SHOWWINDOW = 0x0040;

        public const int SW_RESTORE = 9;

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        #endregion Public Fields

        #region Public Methods

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static IntPtr FindWindowByClass(string className)
            => FindWindow(className, null);

        public static IntPtr FindMainWindowByProcessName(string processName)
        {
            foreach (var proc in Process.GetProcessesByName(processName))
            {
                if (proc.MainWindowHandle != IntPtr.Zero)
                    return proc.MainWindowHandle;
            }
            return IntPtr.Zero;
        }

        public static void BringToFront(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero) return;

            ShowWindow(hwnd, SW_RESTORE);
            SetForegroundWindow(hwnd);
            BringWindowToTop(hwnd);

            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
            SetWindowPos(hwnd, HWND_NOTOPMOST, 0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        #endregion Public Methods
    }
}