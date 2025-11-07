namespace InvoiceApp.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public void CalculateLineTotal()
        {
            LineTotal = Quantity * UnitPrice;
        }
    }
}
