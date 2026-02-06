using System.Windows;

namespace DialogDemo
{
    internal class Program
    {
        #region Private Methods

        private static void Main()
        {
            //RunLegacyDialogTest();
            RunTestWindow();
        }

        private static void RunLegacyDialogTest()
        {
            var options = new DialogOptions
            {
                Title = "AVS Voucher",
                MainMessage = "Card: 6364530000029556\nRemaining: 25,00 EUR",
                SubMessage = "Do you want to continue?",
                Icon = DialogIcon.Question,

                ModalMode = DialogModalMode.BlockClientWindow,
                ClientTarget = DialogClientTarget.CreateDefaultVb6Target(),

                Button1 = new DialogButtonConfig
                {
                    Text = "Bestätigung",
                    HotKey = System.Windows.Input.Key.Enter,
                    IsDefault = true
                },
                Button2 = new DialogButtonConfig
                {
                    Text = "Aufladung",
                    HotKey = System.Windows.Input.Key.P
                },
                Button3 = new DialogButtonConfig
                {
                    Text = "Abbruch",
                    HotKey = System.Windows.Input.Key.Escape,
                    IsCancel = true
                },

                AutoCloseSeconds = 300,
                Theme = DialogTheme.Sand(),
                Height = 800,
                Width = 800,
                DialogSize = DialogSize.Large
            };

            var thread = new Thread(() =>
            {
                var result = SystemWideModalDialogService.ShowDialog(options);
                Console.WriteLine($"Result: {result.Result}, ClosedByExternal: {result.ClosedByExternalRequest}");
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            Thread.Sleep(3000);
            SystemWideModalDialogService.UpdateSubMessage("Please wait… processing.");

            thread.Join();
        }

        private static void RunTestWindow()
        {
            var thread = new Thread(() =>
            {
                var app = new Application();
                var window = new DialogTestWindow();
                app.Run(window);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        #endregion Private Methods
    }
}