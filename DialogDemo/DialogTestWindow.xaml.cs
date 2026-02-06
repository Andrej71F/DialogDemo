using DialogDemo.DialogDemo;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DialogDemo
{
    public partial class DialogTestWindow : Window
    {
        #region Private Fields

        private DialogWindow _dialog;

        private DialogOptions _options;

        #endregion Private Fields

        #region Private Methods

        private static bool TryParseKey(string text, out Key key)
        {
            key = Key.None;
            if (string.IsNullOrWhiteSpace(text))
                return false;

            try
            {
                key = (Key)Enum.Parse(typeof(Key), text, ignoreCase: true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static double ParseDoubleOrDefault(string text, double defaultValue)
        {
            return double.TryParse(text, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var v)
                ? v
                : defaultValue;
        }

        private void OpenDialogButton_Click(object sender, RoutedEventArgs e)
        {
            _options = BuildOptionsFromUi();

            AppendResult("Opening dialog...");

            var thread = new Thread(() =>
            {
                var result = SystemWideModalDialogService.ShowDialog(_options);

                Dispatcher.Invoke(() =>
                {
                    AppendResult($"Dialog closed. Result={result.Result}, External={result.ClosedByExternalRequest}");
                });
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void CloseDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog != null)
            {
                _dialog.Close();
                _dialog = null;
                AppendResult("Dialog closed manually.");
            }
        }

        private void UpdateMainButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog == null) return;
            _dialog.UpdateMainMessage(MainMessageTextBox.Text);
            AppendResult("Main message updated.");
        }

        private void UpdateSubButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dialog == null) return;
            _dialog.UpdateSubMessage(SubMessageTextBox.Text);
            AppendResult("Sub message updated.");
        }

        private void Dialog_ButtonClicked(object sender, DialogButtonResult e)
        {
            AppendResult($"Button clicked: {e}");
        }

        private void Dialog_DialogClosed(object sender, EventArgs e)
        {
            AppendResult("Dialog closed (DialogClosed event).");
            _dialog = null;
        }

        private DialogOptions BuildOptionsFromUi()
        {
            var options = new DialogOptions
            {
                Title = "AVS Voucher",
                MainMessage = MainMessageTextBox.Text,
                SubMessage = SubMessageTextBox.Text,
                Icon = DialogIcon.Question,
                ModalMode = DialogModalMode.BlockClientWindow,
                ClientTarget = DialogClientTarget.CreateDefaultVb6Target()
            };

            // Timeout
            if (int.TryParse(TimeoutTextBox.Text, out var seconds) && seconds > 0)
                options.AutoCloseSeconds = seconds;

            // Size
            var sizeEnum = (DialogSize)SizeComboBox.SelectedItem;
            options.DialogSize = sizeEnum;

            if (sizeEnum == DialogSize.CustomDef)
            {
                if (double.TryParse(WidthTextBox.Text, out var w))
                    options.Width = w;

                if (double.TryParse(HeightTextBox.Text, out var h))
                    options.Height = h;
            }

            // Theme
            var themeEnum = (DialogThemeType)ThemeComboBox.SelectedItem;
            options.Theme = themeEnum.Resolve();

            // Buttons
            options.Button1 = BuildButtonConfig(Button1TextBox.Text, Button1HotKeyTextBox.Text,
                isDefault: Button1DefaultCheckBox.IsChecked == true, isCancel: false);

            options.Button2 = BuildButtonConfig(Button2TextBox.Text, Button2HotKeyTextBox.Text,
                isDefault: false, isCancel: false);

            options.Button3 = BuildButtonConfig(Button3TextBox.Text, Button3HotKeyTextBox.Text,
                isDefault: false, isCancel: Button3CancelCheckBox.IsChecked == true);

            options.Button4 = BuildButtonConfig(Button4TextBox.Text, Button4HotKeyTextBox.Text,
                isDefault: false, isCancel: false);

            // Layout
            if (UseCustomLayoutCheckBox.IsChecked == true)
            {
                options.Layout = new DialogLayoutDefinition
                {
                    TitleStar = ParseDoubleOrDefault(TitleStarTextBox.Text, 0.10),
                    MainStar = ParseDoubleOrDefault(MainStarTextBox.Text, 0.40),
                    SubStar = ParseDoubleOrDefault(SubStarTextBox.Text, 0.25),
                    ProgressStar = ParseDoubleOrDefault(ProgressStarTextBox.Text, 0.05),
                    ButtonsStar = ParseDoubleOrDefault(ButtonsStarTextBox.Text, 0.20),
                    TitleIconStar = ParseDoubleOrDefault(TitleIconStarTextBox.Text, 0.20),
                    TitleTextStar = ParseDoubleOrDefault(TitleTextStarTextBox.Text, 0.80),
                    ButtonsLeftStar = ParseDoubleOrDefault(ButtonsLeftStarTextBox.Text, 0.60),
                    ButtonsRightStar = ParseDoubleOrDefault(ButtonsRightStarTextBox.Text, 0.40)
                };
            }
            else
            {
                options.Layout = null;
            }

            return options;
        }

        private DialogButtonConfig BuildButtonConfig(string text, string hotKeyText, bool isDefault, bool isCancel)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var cfg = new DialogButtonConfig
            {
                Text = text,
                IsDefault = isDefault,
                IsCancel = isCancel
            };

            if (TryParseKey(hotKeyText, out var key))
                cfg.HotKey = key;

            return cfg;
        }

        private void AppendResult(string message)
        {
            ResultTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            ResultTextBox.ScrollToEnd();
        }

        #endregion Private Methods

        #region Public Constructors

        public DialogTestWindow()
        {
            InitializeComponent();
        }

        #endregion Public Constructors
    }
}