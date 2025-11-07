using Dapper;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public class CustomerService
    {
        private readonly DatabaseService _db;

        public CustomerService(DatabaseService db)
        {
            _db = db;
        }

        public List<Customer> GetAllCustomers()
        {
            using (var connection = _db.GetConnection())
            {
                return connection.Query<Customer>("SELECT * FROM Customers ORDER BY Name").ToList();
            }
        }

        public Customer? GetCustomerById(int id)
        {
            using (var connection = _db.GetConnection())
            {
                return connection.QueryFirstOrDefault<Customer>("SELECT * FROM Customers WHERE Id = @Id", new { Id = id });
            }
        }

        public int CreateCustomer(Customer customer)
        {
            using (var connection = _db.GetConnection())
            {
                string sql = @"INSERT INTO Customers (Name, Address, Postcode, Email, Phone, Notes, CreatedAt) 
                              VALUES (@Name, @Address, @Postcode, @Email, @Phone, @Notes, @CreatedAt);
                              SELECT last_insert_rowid();";
                
                return connection.ExecuteScalar<int>(sql, customer);
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var connection = _db.GetConnection())
            {
                string sql = @"UPDATE Customers 
                              SET Name = @Name, Address = @Address, Postcode = @Postcode, 
                                  Email = @Email, Phone = @Phone, Notes = @Notes 
                              WHERE Id = @Id";
                
                connection.Execute(sql, customer);
            }
        }

        public void DeleteCustomer(int id)
        {
            using (var connection = _db.GetConnection())
            {
                connection.Execute("DELETE FROM Customers WHERE Id = @Id", new { Id = id });
            }
        }

        public int ImportCustomersFromCsv(List<Customer> customers)
        {
            int imported = 0;
            foreach (var customer in customers)
            {
                try
                {
                    CreateCustomer(customer);
                    imported++;
                }
                catch
                {
                    // Skip duplicates or invalid records
                }
            }
            return imported;
        }

        public List<(Customer Customer, decimal TotalRevenue)> GetTopCustomersByRevenue(int limit = 10)
        {
            using (var connection = _db.GetConnection())
            {
                string sql = @"
                    SELECT c.Id, c.Name, c.Address, c.Postcode, c.Email, c.Phone, c.Notes, c.CreatedAt,
                           CAST(COALESCE(SUM(i.Total), 0.0) AS REAL) as TotalRevenue
                    FROM Customers c
                    LEFT JOIN Invoices i ON c.Id = i.CustomerId
                    GROUP BY c.Id
                    ORDER BY TotalRevenue DESC
                    LIMIT @Limit";

                var results = connection.Query<CustomerRevenueDto>(sql, new { Limit = limit }).ToList();
                
                return results.Select(r => (
                    new Customer
                    {
                        Id = r.Id,
                        Name = r.Name ?? "",
                        Address = r.Address ?? "",
                        Postcode = r.Postcode ?? "",
                        Email = r.Email ?? "",
                        Phone = r.Phone ?? "",
                        Notes = r.Notes ?? "",
                        CreatedAt = string.IsNullOrEmpty(r.CreatedAt) ? DateTime.Now : DateTime.Parse(r.CreatedAt)
                    },
                    (decimal)r.TotalRevenue
                )).ToList();
            }
        }

        private class CustomerRevenueDto
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Address { get; set; }
            public string? Postcode { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Notes { get; set; }
            public string? CreatedAt { get; set; }
            public double TotalRevenue { get; set; }
        }
    }
}
