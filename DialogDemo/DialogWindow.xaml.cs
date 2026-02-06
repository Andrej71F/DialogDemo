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
                MinWidth = 80,
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

        private void ApplyTheme(DialogTheme theme)
        {
            if (theme == null) return;

            TitleBar.Background = theme.TitleBackground;
            TitleText.Foreground = theme.TitleForeground;
            TitleText.FontSize = theme.TitleFontSize;
            TitleText.FontFamily = new FontFamily(theme.TitleFontFamily);

            MainMessage.Foreground = theme.MainForeground;
            MainMessage.Background = theme.MainBackground;
            MainMessage.FontSize = theme.MainFontSize;
            MainMessage.FontFamily = new FontFamily(theme.MainFontFamily);

            SubMessage.Foreground = theme.SubForeground;
            SubMessage.Background = theme.SubBackground;
            SubMessage.FontSize = theme.SubFontSize;
            SubMessage.FontFamily = new FontFamily(theme.SubFontFamily);

            foreach (var child in ButtonsPanel.Children)
            {
                if (child is Button b)
                {
                    b.Background = theme.ButtonBackground;
                    b.Foreground = theme.ButtonForeground;
                    b.FontSize = theme.ButtonFontSize;
                    b.FontFamily = new FontFamily(theme.ButtonFontFamily);
                }
            }
        }

        private void ApplyIcon(DialogIcon icon)
        {
            IconBlock.Text = icon switch
            {
                DialogIcon.Info => "ℹ",
                DialogIcon.Warning => "⚠",
                DialogIcon.Error => "✖",
                DialogIcon.Question => "?",
                _ => string.Empty
            };
        }

        #endregion Private Methods

        #region Protected Methods

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

            BuildUi(options);
            ApplyTheme(options.Theme);
            ApplyIcon(options.Icon);

            Loaded += (_, __) => ForceFocus();
            Activated += (_, __) => ForceFocus();
        }

        #endregion Public Constructors

        #region Public Events

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