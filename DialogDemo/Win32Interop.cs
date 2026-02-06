using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DialogDemo
{
    internal static class Win32Interop
    {
        #region Private Fields

        private const int WS_VISIBLE = 0x10000000;

        // =====================================================================
        // Mask window support
        // =====================================================================
        private const int WS_POPUP = unchecked((int)0x80000000);

        private const string MaskClassName = "SystemWideMaskWindowWpf";

        private static bool _maskClassRegistered;

        #endregion Private Fields

        #region Private Methods

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int x,
            int y,
            int width,
            int height,
            IntPtr parent,
            IntPtr menu,
            IntPtr instance,
            IntPtr param);

        [DllImport("user32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern ushort RegisterClass(ref WNDCLASS wc);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private static IntPtr MaskWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private static void EnsureMaskClassRegistered()
        {
            if (_maskClassRegistered)
                return;

            var wc = new WNDCLASS
            {
                className = MaskClassName,
                lpfnWndProc = MaskWndProc
            };

            RegisterClass(ref wc);
            _maskClassRegistered = true;
        }

        #endregion Private Methods

        #region Private Structs

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WNDCLASS
        {
            public uint style;

            public WndProc lpfnWndProc;

            public int clsExtra;

            public int wndExtra;

            public IntPtr hInstance;

            public IntPtr hIcon;

            public IntPtr hCursor;

            public IntPtr hbrBackground;

            public string? menuName;

            public string className;
        }

        #endregion Private Structs

        #region Public Fields

        public const uint SWP_NOSIZE = 0x0001;

        public const uint SWP_NOMOVE = 0x0002;

        public const uint SWP_SHOWWINDOW = 0x0040;

        public const int SW_RESTORE = 9;

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        #endregion Public Fields

        #region Public Delegates

        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        #endregion Public Delegates

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

        public static IntPtr CreateMaskOverWindow(IntPtr clientHwnd)
        {
            if (clientHwnd == IntPtr.Zero)
                return IntPtr.Zero;

            if (!GetWindowRect(clientHwnd, out RECT r))
                return IntPtr.Zero;

            EnsureMaskClassRegistered();

            int width = r.Right - r.Left;
            int height = r.Bottom - r.Top;

            IntPtr mask = CreateWindowEx(
                0,
                MaskClassName,
                string.Empty,
                WS_VISIBLE | WS_POPUP,
                r.Left,
                r.Top,
                width,
                height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            return mask;
        }

        public static void DestroyMaskWindow(IntPtr maskHwnd)
        {
            if (maskHwnd != IntPtr.Zero)
            {
                DestroyWindow(maskHwnd);
            }
        }

        #endregion Public Methods
    }
}