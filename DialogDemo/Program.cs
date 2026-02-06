namespace DialogDemo
{
    internal class Program
    {
        #region Private Methods

        private static void Main()
        {
            var options = new DialogOptions
            {
                Title = "AVS Voucher",
                MainMessage = "Card: 6364530000029556\nRemaining: 25,00 EUR",
                SubMessage = "Do you want to continue?",
                Icon = DialogIcon.Question,

                ModalMode = DialogModalMode.BlockClientWindow,
                ClientTarget = DialogClientTarget.CreateDefaultVb6Target(),

                // Buttons (left → right)
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

                AutoCloseSeconds = 30,
                Theme = DialogTheme.Sand()
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

        #endregion Private Methods
    }
}