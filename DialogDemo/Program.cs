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

                // Default: Block VB6 window
                ModalMode = DialogModalMode.BlockClientWindow,

                // Use default VB6 window class array
                ClientTarget = DialogClientTarget.CreateDefaultVb6Target(),

                // Buttons
                Button1 = new DialogButtonConfig { Text = "Yes" },
                Button2 = new DialogButtonConfig { Text = "No" },
                Button3 = new DialogButtonConfig { Text = "Cancel" },

                // Auto-close after 30 seconds (progress bar will appear)
                AutoCloseSeconds = 30,

                // Theme Sand
                Theme = DialogTheme.Sand()
            };

            var thread = new Thread(() =>
            {
                var result = SystemWideModalDialogService.ShowDialog(options);
                Console.WriteLine($"Result: {result.Result}, ClosedByExternal: {result.ClosedByExternalRequest}");
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            // Example: update submessage after 3 seconds
            Thread.Sleep(3000);
            SystemWideModalDialogService.UpdateSubMessage("Please wait… processing.");

            // Example: force external close
            // Thread.Sleep(6000);
            // SystemWideModalDialogService.Close();

            thread.Join();
        }

        #endregion Private Methods
    }
}