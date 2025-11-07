using System.Data;
using Dapper;

namespace InvoiceApp.Utils
{
    public static class NumberingHelper
    {
        public static string GetNextInvoiceNumber(IDbConnection connection, IDbTransaction? transaction = null)
        {
            // Get current year
            int year = DateTime.Now.Year;
            string prefix = "INV";

            // Get max invoice number for current year
            string sql = @"SELECT MAX(CAST(SUBSTR(InvoiceNumber, -4) AS INTEGER)) 
                          FROM Invoices 
                          WHERE InvoiceNumber LIKE @Pattern";
            
            string pattern = $"{prefix}{year}-%";
            
            int? maxNumber = connection.ExecuteScalar<int?>(sql, new { Pattern = pattern }, transaction);
            
            int nextNumber = (maxNumber ?? 0) + 1;
            
            return $"{prefix}{year}-{nextNumber:D4}";
        }

        public static string FormatInvoiceNumber(int year, int number, string prefix = "INV")
        {
            return $"{prefix}{year}-{number:D4}";
        }
    }
}
