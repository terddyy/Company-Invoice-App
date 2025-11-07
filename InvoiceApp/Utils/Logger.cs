using Serilog;

namespace InvoiceApp.Utils
{
    public static class Logger
    {
        private static ILogger? _logger;

        public static void Initialize()
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InvoiceApp",
                "Logs",
                "app-.log"
            );

            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(logPath, 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .CreateLogger();
        }

        public static void Info(string message)
        {
            _logger?.Information(message);
        }

        public static void Error(string message, Exception? ex = null)
        {
            if (ex != null)
                _logger?.Error(ex, message);
            else
                _logger?.Error(message);
        }

        public static void Warning(string message)
        {
            _logger?.Warning(message);
        }

        public static void Debug(string message)
        {
            _logger?.Debug(message);
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
