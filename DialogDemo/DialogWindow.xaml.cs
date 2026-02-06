// DialogWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DialogDemo
{
    public partial class DialogWindow : Window
    {
        #region Private Fields

        private Button _btn1, _btn2, _btn3, _btn4;

        #endregion Private Fields

        #region Private Methods

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

            AddButtonIfVisible(_btn4);
            AddButtonIfVisible(_btn3);
            AddButtonIfVisible(_btn2);
            AddButtonIfVisible(_btn1);
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