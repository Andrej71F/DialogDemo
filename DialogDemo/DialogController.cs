using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;

namespace DialogDemo
{
    internal sealed class DialogController
    {
        #region Private Fields

        private readonly DialogOptions _options;

        private readonly IntPtr _clientHwnd;

        private readonly AutoResetEvent _windowCreated = new(false);

        private readonly AutoResetEvent _windowClosed = new(false);

        private Thread _uiThread;

        private Dispatcher _dispatcher;

        private DialogWindow _window;

        private DialogResultInfo _result = new();

        private System.Timers.Timer _countdownTimer;

        private int _secondsLeft;

        #endregion Private Fields

        #region Private Methods

        private void UiThreadStart()
        {
            try
            {
                _window = new DialogWindow(_options);
                _window.ButtonClicked += OnButtonClicked;
                _window.DialogClosed += OnDialogClosed;

                _dispatcher = Dispatcher.CurrentDispatcher;

                _windowCreated.Set();

                _window.Show();

                Dispatcher.Run();
            }
            catch
            {
                _result.Result = DialogButtonResult.DialogFailed;
                _result.ClosedByExternalRequest = true;
                _windowClosed.Set();
            }
        }

        private void OnButtonClicked(object sender, DialogButtonResult e)
        {
            _result.Result = e;
            CloseWindowInternal();
        }

        private void OnDialogClosed(object sender, EventArgs e)
        {
            CloseWindowInternal();
        }

        private void CloseWindowInternal()
        {
            if (_dispatcher == null)
            {
                _windowClosed.Set();
                return;
            }

            _dispatcher.BeginInvoke(new Action(() =>
            {
                if (_window != null)
                {
                    _window.Close();
                    _window = null;
                }

                _windowClosed.Set();
                Dispatcher.ExitAllFrames();
            }));
        }

        private void StartCountdownIfNeeded()
        {
            if (_options.AutoCloseSeconds <= 3)
                return;

            _secondsLeft = _options.AutoCloseSeconds;

            _countdownTimer = new System.Timers.Timer(1000);
            _countdownTimer.AutoReset = true;
            _countdownTimer.Elapsed += CountdownTick;
            _countdownTimer.Start();
        }

        private void CountdownTick(object sender, ElapsedEventArgs e)
        {
            if (_dispatcher == null)
                return;

            _secondsLeft--;

            double percent = (_secondsLeft / (double)_options.AutoCloseSeconds) * 100.0;
            if (percent < 0) percent = 0;

            _dispatcher.BeginInvoke(new Action(() =>
            {
                _window?.SetProgress(percent);
            }));

            if (_secondsLeft <= 0)
            {
                _countdownTimer.Stop();
                _countdownTimer.Dispose();
                _countdownTimer = null;

                _dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_window == null)
                        return;

                    _result.Result = DialogButtonResult.Timeout;
                    _result.ClosedByExternalRequest = true;

                    _window.SimulateButtonClick(DialogButtonResult.Timeout);
                }));
            }
        }

        private void StopCountdown()
        {
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Dispose();
                _countdownTimer = null;
            }
        }

        #endregion Private Methods

        #region Public Constructors

        public DialogController(DialogOptions options, IntPtr clientHwnd)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _clientHwnd = clientHwnd;
        }

        #endregion Public Constructors

        #region Public Methods

        public DialogResultInfo ShowModal()
        {
            try
            {
                _uiThread = new Thread(UiThreadStart)
                {
                    IsBackground = true,
                    Name = "SystemWideModalDialog.UI"
                };
                _uiThread.SetApartmentState(ApartmentState.STA);
                _uiThread.Start();

                _windowCreated.WaitOne();

                StartCountdownIfNeeded();

                _windowClosed.WaitOne();

                StopCountdown();

                return _result;
            }
            catch
            {
                _result.Result = DialogButtonResult.DialogFailed;
                _result.ClosedByExternalRequest = true;
                return _result;
            }
        }

        public void UpdateMainMessage(string text)
        {
            _dispatcher?.BeginInvoke(new Action(() =>
            {
                _window?.UpdateMainMessage(text);
            }));
        }

        public void UpdateSubMessage(string text)
        {
            _dispatcher?.BeginInvoke(new Action(() =>
            {
                _window?.UpdateSubMessage(text);
            }));
        }

        public void CloseFromExternal()
        {
            _result.ClosedByExternalRequest = true;
            CloseWindowInternal();
        }

        #endregion Public Methods
    }
}