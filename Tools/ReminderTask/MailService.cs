using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ReminderTask
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

        public MailService(string host, int port, bool useSsl, string username, string password, string fromEmail, string companyName)
        {
            _smtpHost = host;
            _smtpPort = port;
            _useSsl = useSsl;
            _username = username;
            _password = password;
            _fromEmail = fromEmail;
            _companyName = companyName;
        }

        public bool SendReminderEmail(string customerName, string customerEmail, string invoiceNumber, 
            DateTime issueDate, DateTime dueDate, decimal total)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_companyName, _fromEmail));
                message.To.Add(new MailboxAddress(customerName, customerEmail));
                message.Subject = $"[Reminder] Invoice {invoiceNumber} is overdue";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = $@"Dear {customerName},

Our records show invoice {invoiceNumber} (issued {issueDate:yyyy-MM-dd}, due {dueDate:yyyy-MM-dd}) is outstanding. 

Amount due: {total:C}

Please arrange payment at your earliest convenience.

Thank you,
{_companyName}";

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
