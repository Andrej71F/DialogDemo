// SystemWideModalDialogService.cs
using System;

namespace DialogDemo
{
    public static class SystemWideModalDialogService
    {
        #region Private Fields

        private static DialogController _currentController;

        #endregion Private Fields

        #region Private Methods

        private static IntPtr ResolveClientWindow(DialogOptions options)
        {
            DialogClientTarget target = options.ClientTarget;

            // If no target provided → use default VB6 config
            if (target == null)
            {
                target = DialogClientTarget.CreateDefaultVb6Target();
            }

            // 1. Direct HWND
            if (target.WindowHandle != IntPtr.Zero)
                return target.WindowHandle;

            // 2. Class names array
            if (target.WindowClassNames != null && target.WindowClassNames.Length > 0)
            {
                foreach (var className in target.WindowClassNames)
                {
                    if (string.IsNullOrWhiteSpace(className))
                        continue;

                    var hwnd = Win32Interop.FindWindowByClass(className);
                    if (hwnd != IntPtr.Zero)
                        return hwnd;
                }
            }

            // 3. Process name
            if (!string.IsNullOrEmpty(target.ProcessName))
                return Win32Interop.FindMainWindowByProcessName(target.ProcessName);

            // Nothing found
            return IntPtr.Zero;
        }

        #endregion Private Methods

        #region Public Methods

        public static DialogResultInfo ShowDialog(DialogOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            IntPtr clientHwnd = ResolveClientWindow(options);

            if (options.ModalMode == DialogModalMode.BlockClientWindow &&
                clientHwnd != IntPtr.Zero)
            {
                Win32Interop.EnableWindow(clientHwnd, false);
            }

            try
            {
                _currentController = new DialogController(options, clientHwnd);
                var result = _currentController.ShowModal();
                return result;
            }
            finally
            {
                if (options.ModalMode == DialogModalMode.BlockClientWindow &&
                    clientHwnd != IntPtr.Zero)
                {
                    Win32Interop.EnableWindow(clientHwnd, true);
                    Win32Interop.SetForegroundWindow(clientHwnd);
                }

                _currentController = null;
            }
        }

        public static void UpdateMainMessage(string text)
            => _currentController?.UpdateMainMessage(text);

        public static void UpdateSubMessage(string text)
            => _currentController?.UpdateSubMessage(text);

        public static void Close()
            => _currentController?.CloseFromExternal();

        #endregion Public Methods
    }
}