using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public class MailService
    {
        private string _smtpHost;
        private int _smtpPort;
        private bool _useSsl;
        private string _username;
        private string _password;
        private string _fromEmail;
        private string _companyName;

        public MailService()
        {
            // Default settings - will be loaded from configuration
            _smtpHost = "smtp.gmail.com";
            _smtpPort = 587;
            _useSsl = true;
            _username = "";
            _password = "";
            _fromEmail = "";
            _companyName = "Your Company";
        }

        public void Configure(string host, int port, bool useSsl, string username, string password, string fromEmail, string companyName)
        {
            _smtpHost = host;
            _smtpPort = port;
            _useSsl = useSsl;
            _username = username;
            _password = password;
            _fromEmail = fromEmail;
            _companyName = companyName;
        }

        public bool TestConnection()
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpHost, _smtpPort, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                    
                    if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                    {
                        client.Authenticate(_username, _password);
                    }
                    
                    client.Disconnect(true);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool SendReminderEmail(Invoice invoice, string? pdfPath = null)
        {
            if (invoice.Customer == null || string.IsNullOrEmpty(invoice.Customer.Email))
            {
                return false;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_companyName, _fromEmail));
                message.To.Add(new MailboxAddress(invoice.Customer.Name, invoice.Customer.Email));
                message.Subject = $"[Reminder] Invoice {invoice.InvoiceNumber} is overdue";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = GetReminderEmailBody(invoice);

                // Attach PDF if provided
                if (!string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                {
                    bodyBuilder.Attachments.Add(pdfPath);
                }

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpHost, _smtpPort, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                    
                    if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                    {
                        client.Authenticate(_username, _password);
                    }
                    
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private string GetReminderEmailBody(Invoice invoice)
        {
            return $@"Dear {invoice.Customer?.Name},

Our records show invoice {invoice.InvoiceNumber} (issued {invoice.IssueDate:yyyy-MM-dd}, due {invoice.DueDate:yyyy-MM-dd}) is outstanding. 

Amount due: {invoice.Total:C}

Please arrange payment at your earliest convenience. A PDF copy is attached.

Thank you,
{_companyName}";
        }

        public bool SendInvoiceEmail(Invoice invoice, string pdfPath)
        {
            if (invoice.Customer == null || string.IsNullOrEmpty(invoice.Customer.Email))
            {
                return false;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_companyName, _fromEmail));
                message.To.Add(new MailboxAddress(invoice.Customer.Name, invoice.Customer.Email));
                message.Subject = $"Invoice {invoice.InvoiceNumber}";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = $@"Dear {invoice.Customer.Name},

Please find attached invoice {invoice.InvoiceNumber} for {invoice.Total:C}.

Due date: {invoice.DueDate:yyyy-MM-dd}

Thank you for your business!

{_companyName}";

                if (File.Exists(pdfPath))
                {
                    bodyBuilder.Attachments.Add(pdfPath);
                }

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpHost, _smtpPort, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                    
                    if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
                    {
                        client.Authenticate(_username, _password);
                    }
                    
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
