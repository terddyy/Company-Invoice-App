using InvoiceApp.Forms;
using InvoiceApp.Utils;

namespace InvoiceApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize logger
            Logger.Initialize();
            Logger.Info("Application starting...");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Logger.Error("Fatal error", ex);
                MessageBox.Show($"A fatal error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Logger.Info("Application shutting down...");
                Logger.Close();
            }
        }
    }
}
