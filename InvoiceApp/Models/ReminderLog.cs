namespace InvoiceApp.Models
{
    public class ReminderLog
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTime SentAt { get; set; }
        public string Method { get; set; } = "Email";
        public string Result { get; set; } = string.Empty;

        public ReminderLog()
        {
            SentAt = DateTime.Now;
        }
    }
}
