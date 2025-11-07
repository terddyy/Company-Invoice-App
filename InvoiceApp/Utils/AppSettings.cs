using Newtonsoft.Json;

namespace InvoiceApp.Utils
{
    public class AppSettings
    {
        public string DatabasePath { get; set; } = "";
        public string InvoicePrefix { get; set; } = "INV";
        public bool InvoiceYearMode { get; set; } = true;
        public SmtpSettings Smtp { get; set; } = new SmtpSettings();
        public ReminderSettings Reminder { get; set; } = new ReminderSettings();
        public CompanyDetails Company { get; set; } = new CompanyDetails();
        
        // Legacy properties for backward compatibility
        [Obsolete("Use Company.Name instead")]
        public string CompanyName 
        { 
            get => Company.Name; 
            set => Company.Name = value; 
        }
        
        [Obsolete("Use Company.Email instead")]
        public string CompanyEmail 
        { 
            get => Company.Email; 
            set => Company.Email = value; 
        }

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

        public void Save()
        {
            string settingsPath = GetSettingsPath();
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            
            string? directory = Path.GetDirectoryName(settingsPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(settingsPath, json);
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

    public class CompanyDetails
    {
        public string Name { get; set; } = "Your Company";
        public string Address { get; set; } = "";
        public string AddressLine2 { get; set; } = "";
        public string City { get; set; } = "";
        public string PostCode { get; set; } = "";
        public string Country { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Website { get; set; } = "";
        
        // Bank details for payment
        public string BankName { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string SortCode { get; set; } = "";
        public string BankTitle { get; set; } = "";
        
        // Tax details
        public string VatNumber { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        
        // Payment terms
        public int DefaultPaymentTermsDays { get; set; } = 7;
        public string PaymentInstructions { get; set; } = "";
    }
}
