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

            if (target == null)
            {
                target = DialogClientTarget.CreateDefaultVb6Target();
            }

            if (target.WindowHandle != IntPtr.Zero)
                return target.WindowHandle;

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

            if (!string.IsNullOrEmpty(target.ProcessName))
                return Win32Interop.FindMainWindowByProcessName(target.ProcessName);

            return IntPtr.Zero;
        }

        #endregion Private Methods

        #region Public Methods

        public static DialogResultInfo ShowDialog(DialogOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            IntPtr clientHwnd = ResolveClientWindow(options);
            IntPtr maskHwnd = IntPtr.Zero;

            if (options.ModalMode == DialogModalMode.BlockClientWindow &&
                clientHwnd != IntPtr.Zero)
            {
                // 1) создаём маску поверх VB6
                maskHwnd = Win32Interop.CreateMaskOverWindow(clientHwnd);

                // 2) опционально — дополнительно отключаем окно VB6
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

                // Убираем маску в любом случае
                if (maskHwnd != IntPtr.Zero)
                {
                    Win32Interop.DestroyMaskWindow(maskHwnd);
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