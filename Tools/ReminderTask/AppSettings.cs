using Newtonsoft.Json;

namespace ReminderTask
{
    public class AppSettings
    {
        public string DatabasePath { get; set; } = "";
        public string InvoicePrefix { get; set; } = "INV";
        public bool InvoiceYearMode { get; set; } = true;
        public SmtpSettings Smtp { get; set; } = new SmtpSettings();
        public ReminderSettings Reminder { get; set; } = new ReminderSettings();
        public string CompanyName { get; set; } = "Your Company";
        public string CompanyEmail { get; set; } = "";

        public static AppSettings Load()
        {
            string settingsPath = GetSettingsPath();
            
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }

            return new AppSettings();
        }

        private static string GetSettingsPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InvoiceApp",
                "settings.json"
            );
        }
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class ReminderSettings
    {
        public int DaysAfterDue { get; set; } = 1;
        public int MaxReminders { get; set; } = 3;
        public int IntervalDays { get; set; } = 3;
    }
}
