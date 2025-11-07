namespace InvoiceApp.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Overdue
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties (not in DB)
        public Customer? Customer { get; set; }
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        public Invoice()
        {
            IssueDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(30);
            CreatedAt = DateTime.Now;
        }

        public bool IsOverdue()
        {
            return Status == "Unpaid" && DueDate < DateTime.Now;
        }

        public void RecalculateTotals()
        {
            Subtotal = Items.Sum(i => i.LineTotal);
            Total = Subtotal + Tax;
        }
    }
}
