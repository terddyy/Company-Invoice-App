using Dapper;
using Serilog;

namespace ReminderTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize logger
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InvoiceApp",
                "Logs",
                "reminder-.log"
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(logPath, 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .CreateLogger();

            Log.Information("ReminderTask starting...");

            try
            {
                // Load settings
                var settings = AppSettings.Load();
                
                if (string.IsNullOrEmpty(settings.CompanyEmail) || string.IsNullOrEmpty(settings.Smtp.Username))
                {
                    Log.Warning("SMTP settings not configured. Please configure settings in the main application.");
                    Console.WriteLine("SMTP settings not configured. Please configure settings in the main application.");
                    return;
                }

                // Initialize services
                var db = new DatabaseService();
                if (!db.TestConnection())
                {
                    Log.Error("Cannot connect to database");
                    Console.WriteLine("Cannot connect to database");
                    return;
                }

                var mailService = new MailService(
                    settings.Smtp.Host,
                    settings.Smtp.Port,
                    settings.Smtp.UseSsl,
                    settings.Smtp.Username,
                    settings.Smtp.Password,
                    settings.CompanyEmail,
                    settings.CompanyName
                );

                // Find overdue invoices
                var overdueInvoices = GetOverdueInvoicesForReminder(db, settings);
                
                Log.Information($"Found {overdueInvoices.Count} overdue invoices to send reminders");
                Console.WriteLine($"Found {overdueInvoices.Count} overdue invoices to send reminders");

                int successCount = 0;
                int failCount = 0;

                foreach (var invoice in overdueInvoices)
                {
                    try
                    {
                        Log.Information($"Sending reminder for invoice {invoice.InvoiceNumber} to {invoice.CustomerEmail}");
                        
                        bool sent = mailService.SendReminderEmail(
                            invoice.CustomerName,
                            invoice.CustomerEmail,
                            invoice.InvoiceNumber,
                            invoice.IssueDate,
                            invoice.DueDate,
                            invoice.Total
                        );

                        if (sent)
                        {
                            // Log reminder
                            LogReminder(db, invoice.Id, "Email", "Sent successfully");
                            successCount++;
                            Log.Information($"Reminder sent successfully for invoice {invoice.InvoiceNumber}");
                            Console.WriteLine($"✓ Sent reminder for invoice {invoice.InvoiceNumber}");
                        }
                        else
                        {
                            LogReminder(db, invoice.Id, "Email", "Failed to send");
                            failCount++;
                            Log.Warning($"Failed to send reminder for invoice {invoice.InvoiceNumber}");
                            Console.WriteLine($"✗ Failed to send reminder for invoice {invoice.InvoiceNumber}");
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        Log.Error(ex, $"Error sending reminder for invoice {invoice.InvoiceNumber}");
                        Console.WriteLine($"✗ Error sending reminder for invoice {invoice.InvoiceNumber}: {ex.Message}");
                    }
                }

                Log.Information($"ReminderTask completed. Sent: {successCount}, Failed: {failCount}");
                Console.WriteLine($"\nCompleted. Sent: {successCount}, Failed: {failCount}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fatal error in ReminderTask");
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static List<OverdueInvoice> GetOverdueInvoicesForReminder(DatabaseService db, AppSettings settings)
        {
            using (var connection = db.GetConnection())
            {
                // Query for overdue invoices that need reminders
                string sql = @"
                    SELECT 
                        i.Id,
                        i.InvoiceNumber,
                        i.IssueDate,
                        i.DueDate,
                        i.Total,
                        c.Name as CustomerName,
                        c.Email as CustomerEmail,
                        (SELECT COUNT(*) FROM ReminderLog WHERE InvoiceId = i.Id) as RemindersSent,
                        (SELECT MAX(SentAt) FROM ReminderLog WHERE InvoiceId = i.Id) as LastReminderDate
                    FROM Invoices i
                    INNER JOIN Customers c ON i.CustomerId = c.Id
                    WHERE i.Status = 'Unpaid'
                        AND date(i.DueDate) < date('now')
                        AND c.Email IS NOT NULL
                        AND c.Email != ''
                ";

                var invoices = connection.Query<OverdueInvoice>(sql).ToList();

                // Filter based on reminder settings
                var filtered = new List<OverdueInvoice>();
                foreach (var invoice in invoices)
                {
                    // Check if max reminders reached
                    if (invoice.RemindersSent >= settings.Reminder.MaxReminders)
                    {
                        continue;
                    }

                    // Check if enough days passed since due date
                    int daysSinceDue = (DateTime.Now - invoice.DueDate).Days;
                    if (daysSinceDue < settings.Reminder.DaysAfterDue)
                    {
                        continue;
                    }

                    // Check if enough time passed since last reminder
                    if (invoice.LastReminderDate.HasValue)
                    {
                        int daysSinceLastReminder = (DateTime.Now - invoice.LastReminderDate.Value).Days;
                        if (daysSinceLastReminder < settings.Reminder.IntervalDays)
                        {
                            continue;
                        }
                    }

                    filtered.Add(invoice);
                }

                return filtered;
            }
        }

        static void LogReminder(DatabaseService db, int invoiceId, string method, string result)
        {
            using (var connection = db.GetConnection())
            {
                string sql = @"INSERT INTO ReminderLog (InvoiceId, Method, Result) 
                              VALUES (@InvoiceId, @Method, @Result)";
                connection.Execute(sql, new { InvoiceId = invoiceId, Method = method, Result = result });
            }
        }
    }

    class OverdueInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Total { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public int RemindersSent { get; set; }
        public DateTime? LastReminderDate { get; set; }
    }
}
