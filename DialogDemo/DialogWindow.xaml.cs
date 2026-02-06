using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DialogDemo
{
    public partial class DialogWindow : Window
    {
        #region Private Fields

        private readonly System.Collections.Generic.Dictionary<Key, Button> _hotKeyMap =
            new System.Collections.Generic.Dictionary<Key, Button>();

        private Button _btn1, _btn2, _btn3, _btn4;

        private Button _defaultButton;

        private Button _cancelButton;

        #endregion Private Fields

        #region Private Methods

        // ---------------------------------------------------------
        // Dynamic font scaling based on window size
        // ---------------------------------------------------------
        private void UpdateFontScaling()
        {
            // 600 is the baseline width for Medium size
            double scale = Width / 600.0;

            Resources["Font.Title"] = 16 * scale;
            Resources["Font.Main"] = 14 * scale;
            Resources["Font.Sub"] = 12 * scale;
            Resources["Font.Button"] = 13 * scale;
        }

        // ---------------------------------------------------------
        // Window size logic (DialogSize + CustomDef)
        // ---------------------------------------------------------
        private void ApplySize(DialogOptions options)
        {
            const double SMALL_W = 420;
            const double SMALL_H = 260;

            const double MEDIUM_W = 600;
            const double MEDIUM_H = 360;

            const double LARGE_W = 800;
            const double LARGE_H = 520;

            double finalWidth;
            double finalHeight;

            if (options.DialogSize != DialogSize.CustomDef)
            {
                switch (options.DialogSize)
                {
                    case DialogSize.Small:
                        finalWidth = SMALL_W;
                        finalHeight = SMALL_H;
                        break;

                    case DialogSize.Large:
                        finalWidth = LARGE_W;
                        finalHeight = LARGE_H;
                        break;

                    case DialogSize.Medium:
                    default:
                        finalWidth = MEDIUM_W;
                        finalHeight = MEDIUM_H;
                        break;
                }

                Width = finalWidth;
                Height = finalHeight;
                return;
            }

            bool hasW = options.Width.HasValue;
            bool hasH = options.Height.HasValue;

            if (hasW && hasH)
            {
                Width = options.Width.Value;
                Height = options.Height.Value;
                return;
            }

            if (hasW && !hasH)
            {
                double w = options.Width.Value;

                if (w <= (SMALL_W + MEDIUM_W) / 2)
                    Height = SMALL_H;
                else if (w <= (MEDIUM_W + LARGE_W) / 2)
                    Height = MEDIUM_H;
                else
                    Height = LARGE_H;

                Width = w;
                return;
            }

            if (!hasW && hasH)
            {
                double h = options.Height.Value;

                if (h <= (SMALL_H + MEDIUM_H) / 2)
                    Width = SMALL_W;
                else if (h <= (MEDIUM_H + LARGE_H) / 2)
                    Width = MEDIUM_W;
                else
                    Width = LARGE_W;

                Height = h;
                return;
            }

            Width = MEDIUM_W;
            Height = MEDIUM_H;
        }

        // ---------------------------------------------------------
        // UI construction
        // ---------------------------------------------------------
        private void BuildUi(DialogOptions opt)
        {
            Title = opt.Title ?? string.Empty;
            TitleText.Text = opt.Title ?? string.Empty;

            MainMessage.Text = opt.MainMessage ?? string.Empty;
            SubMessage.Text = opt.SubMessage ?? string.Empty;

            _btn1 = CreateButton(opt.Button1, DialogButtonResult.Option1);
            _btn2 = CreateButton(opt.Button2, DialogButtonResult.Option2);
            _btn3 = CreateButton(opt.Button3, DialogButtonResult.Option3);
            _btn4 = CreateButton(opt.Button4, DialogButtonResult.Option4);

            AddButtonIfVisible(_btn1);
            AddButtonIfVisible(_btn2);
            AddButtonIfVisible(_btn3);
            AddButtonIfVisible(_btn4);
        }

        private Button CreateButton(DialogButtonConfig cfg, DialogButtonResult result)
        {
            if (cfg == null || string.IsNullOrWhiteSpace(cfg.Text))
                return null;

            var btn = new Button
            {
                Content = cfg.Text,
                Margin = new Thickness(5),
                MinWidth = 100,
                Tag = result
            };

            btn.Click += (s, e) => ButtonClicked?.Invoke(this, result);

            if (cfg.HotKey.HasValue)
                _hotKeyMap[cfg.HotKey.Value] = btn;

            if (cfg.IsDefault)
                _defaultButton = btn;

            if (cfg.IsCancel)
                _cancelButton = btn;

            btn.Style = (Style)FindResource("DialogButtonStyle");

            return btn;
        }

        private void AddButtonIfVisible(Button btn)
        {
            if (btn != null)
                ButtonsPanel.Children.Add(btn);
        }

        // ---------------------------------------------------------
        // Theme application (colors only, no font sizes)
        // ---------------------------------------------------------
        private void ApplyTheme(DialogTheme theme)
        {
            if (theme == null) return;

            TitleBar.Background = theme.TitleBackground;
            TitleText.Foreground = theme.TitleForeground;

            MainMessage.Foreground = theme.MainForeground;
            MainMessage.Background = theme.MainBackground;

            SubMessage.Foreground = theme.SubForeground;
            SubMessage.Background = theme.SubBackground;

            foreach (var child in ButtonsPanel.Children)
            {
                if (child is Button b)
                {
                    b.Background = theme.ButtonBackground;
                    b.Foreground = theme.ButtonForeground;
                }
            }
        }

        private void ApplyIcon(DialogIcon icon)
        {
            System.Drawing.Icon sysIcon = icon switch
            {
                DialogIcon.Info => System.Drawing.SystemIcons.Information,
                DialogIcon.Warning => System.Drawing.SystemIcons.Warning,
                DialogIcon.Error => System.Drawing.SystemIcons.Error,
                DialogIcon.Question => System.Drawing.SystemIcons.Question,
                _ => null
            };

            if (sysIcon != null)
            {
                IconImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    sysIcon.Handle,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }

        private void ForceFocus()
        {
            var target = _defaultButton ?? _btn1;

            if (target != null)
            {
                target.Focus();
                Keyboard.Focus(target);
            }

            Activate();
            Focus();
        }

        #endregion Private Methods

        #region Protected Methods

        // ---------------------------------------------------------
        // Keyboard navigation
        // ---------------------------------------------------------
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            var buttons = new[] { _btn1, _btn2, _btn3, _btn4 }
                .Where(b => b != null)
                .ToList();

            if (buttons.Count == 0)
                return;

            var focused = Keyboard.FocusedElement as Button;
            int index = focused != null ? buttons.IndexOf(focused) : -1;

            if (_hotKeyMap.TryGetValue(e.Key, out var hotBtn))
            {
                hotBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;
                return;
            }

            switch (e.Key)
            {
                case Key.Enter:
                    if (_defaultButton != null)
                    {
                        _defaultButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        e.Handled = true;
                        return;
                    }
                    if (focused != null)
                    {
                        focused.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        e.Handled = true;
                    }
                    break;

                case Key.Escape:
                    if (_cancelButton != null)
                    {
                        _cancelButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        e.Handled = true;
                        return;
                    }
                    break;

                case Key.Left:
                    if (index > 0)
                    {
                        buttons[index - 1].Focus();
                        e.Handled = true;
                    }
                    break;

                case Key.Right:
                    if (index >= 0 && index < buttons.Count - 1)
                    {
                        buttons[index + 1].Focus();
                        e.Handled = true;
                    }
                    break;

                case Key.Tab:
                    if (index == -1)
                    {
                        buttons[0].Focus();
                        e.Handled = true;
                    }
                    else
                    {
                        int next = (index + 1) % buttons.Count;
                        buttons[next].Focus();
                        e.Handled = true;
                    }
                    break;

                case Key.Space:
                    if (focused != null)
                    {
                        focused.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DialogClosed?.Invoke(this, EventArgs.Empty);
        }

        #endregion Protected Methods

        #region Public Constructors

        public DialogWindow(DialogOptions options)
        {
            InitializeComponent();

            Topmost = true;

            ApplySize(options);
            UpdateFontScaling();
            BuildUi(options);
            ApplyTheme(options.Theme);
            ApplyIcon(options.Icon);

            Loaded += (_, __) => ForceFocus();
            Activated += (_, __) => ForceFocus();
        }

        #endregion Public Constructors

        #region Public Events

        // ---------------------------------------------------------
        // Public API
        // ---------------------------------------------------------
        public event EventHandler<DialogButtonResult> ButtonClicked;

        public event EventHandler DialogClosed;

        #endregion Public Events

        #region Public Methods

        public void SetProgress(double percent)
        {
            if (AutoCloseProgress.Visibility != Visibility.Visible)
                AutoCloseProgress.Visibility = Visibility.Visible;

            AutoCloseProgress.Value = percent;
        }

        public void UpdateMainMessage(string text) => MainMessage.Text = text ?? string.Empty;

        public void UpdateSubMessage(string text) => SubMessage.Text = text ?? string.Empty;

        public void SimulateButtonClick(DialogButtonResult result)
        {
            ButtonClicked?.Invoke(this, result);
        }

        #endregion Public Methods
    }
}