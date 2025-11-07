using Dapper;
using InvoiceApp.Models;
using InvoiceApp.Utils;

namespace InvoiceApp.Services
{
    public class InvoiceService
    {
        private readonly DatabaseService _db;
        private readonly CustomerService _customerService;

        public InvoiceService(DatabaseService db, CustomerService customerService)
        {
            _db = db;
            _customerService = customerService;
        }

        public List<Invoice> GetAllInvoices()
        {
            using (var connection = _db.GetConnection())
            {
                var invoices = connection.Query<Invoice>("SELECT * FROM Invoices ORDER BY IssueDate DESC").ToList();
                
                foreach (var invoice in invoices)
                {
                    invoice.Customer = _customerService.GetCustomerById(invoice.CustomerId);
                    invoice.Items = GetInvoiceItems(invoice.Id);
                }
                
                return invoices;
            }
        }

        public Invoice? GetInvoiceById(int id)
        {
            using (var connection = _db.GetConnection())
            {
                var invoice = connection.QueryFirstOrDefault<Invoice>("SELECT * FROM Invoices WHERE Id = @Id", new { Id = id });
                
                if (invoice != null)
                {
                    invoice.Customer = _customerService.GetCustomerById(invoice.CustomerId);
                    invoice.Items = GetInvoiceItems(invoice.Id);
                }
                
                return invoice;
            }
        }

        public List<InvoiceItem> GetInvoiceItems(int invoiceId)
        {
            using (var connection = _db.GetConnection())
            {
                return connection.Query<InvoiceItem>("SELECT * FROM InvoiceItems WHERE InvoiceId = @InvoiceId", 
                    new { InvoiceId = invoiceId }).ToList();
            }
        }

        public int CreateInvoice(Invoice invoice)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Generate invoice number
                        invoice.InvoiceNumber = NumberingHelper.GetNextInvoiceNumber(connection, transaction);

                        // Insert invoice
                        string invoiceSql = @"INSERT INTO Invoices 
                            (InvoiceNumber, CustomerId, IssueDate, DueDate, Subtotal, Tax, Total, Status, Notes, CreatedAt) 
                            VALUES (@InvoiceNumber, @CustomerId, @IssueDate, @DueDate, @Subtotal, @Tax, @Total, @Status, @Notes, @CreatedAt);
                            SELECT last_insert_rowid();";
                        
                        int invoiceId = connection.ExecuteScalar<int>(invoiceSql, invoice, transaction);
                        invoice.Id = invoiceId;

                        // Insert invoice items
                        foreach (var item in invoice.Items)
                        {
                            item.InvoiceId = invoiceId;
                            string itemSql = @"INSERT INTO InvoiceItems 
                                (InvoiceId, Description, Quantity, UnitPrice, LineTotal) 
                                VALUES (@InvoiceId, @Description, @Quantity, @UnitPrice, @LineTotal)";
                            connection.Execute(itemSql, item, transaction);
                        }

                        transaction.Commit();
                        return invoiceId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void UpdateInvoice(Invoice invoice)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update invoice
                        string invoiceSql = @"UPDATE Invoices 
                            SET CustomerId = @CustomerId, IssueDate = @IssueDate, DueDate = @DueDate, 
                                Subtotal = @Subtotal, Tax = @Tax, Total = @Total, Status = @Status, Notes = @Notes 
                            WHERE Id = @Id";
                        connection.Execute(invoiceSql, invoice, transaction);

                        // Delete existing items
                        connection.Execute("DELETE FROM InvoiceItems WHERE InvoiceId = @Id", new { invoice.Id }, transaction);

                        // Insert updated items
                        foreach (var item in invoice.Items)
                        {
                            item.InvoiceId = invoice.Id;
                            string itemSql = @"INSERT INTO InvoiceItems 
                                (InvoiceId, Description, Quantity, UnitPrice, LineTotal) 
                                VALUES (@InvoiceId, @Description, @Quantity, @UnitPrice, @LineTotal)";
                            connection.Execute(itemSql, item, transaction);
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteInvoice(int id)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute("DELETE FROM InvoiceItems WHERE InvoiceId = @Id", new { Id = id }, transaction);
                        connection.Execute("DELETE FROM Invoices WHERE Id = @Id", new { Id = id }, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void MarkInvoiceAsPaid(int id)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Execute("UPDATE Invoices SET Status = 'Paid' WHERE Id = @Id", new { Id = id });
            }
        }

        public void MarkInvoiceAsUnpaid(int id)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Execute("UPDATE Invoices SET Status = 'Unpaid' WHERE Id = @Id", new { Id = id });
            }
        }

        public List<Invoice> GetOverdueInvoices()
        {
            using (var connection = _db.GetConnection())
            {
                string sql = @"SELECT * FROM Invoices 
                              WHERE Status = 'Unpaid' AND date(DueDate) < date('now') 
                              ORDER BY DueDate";
                
                var invoices = connection.Query<Invoice>(sql).ToList();
                
                foreach (var invoice in invoices)
                {
                    invoice.Customer = _customerService.GetCustomerById(invoice.CustomerId);
                    invoice.Items = GetInvoiceItems(invoice.Id);
                }
                
                return invoices;
            }
        }

        public void UpdateOverdueStatus()
        {
            using (var connection = _db.GetConnection())
            {
                connection.Execute(@"UPDATE Invoices 
                                    SET Status = 'Overdue' 
                                    WHERE Status = 'Unpaid' AND date(DueDate) < date('now')");
            }
        }

        public decimal GetTotalRevenue()
        {
            using (var connection = _db.GetConnection())
            {
                return connection.ExecuteScalar<decimal>("SELECT COALESCE(SUM(Total), 0) FROM Invoices WHERE Status = 'Paid'");
            }
        }

        public decimal GetTotalOutstanding()
        {
            using (var connection = _db.GetConnection())
            {
                return connection.ExecuteScalar<decimal>("SELECT COALESCE(SUM(Total), 0) FROM Invoices WHERE Status IN ('Unpaid', 'Overdue')");
            }
        }
    }
}
