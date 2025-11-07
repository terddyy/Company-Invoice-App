using InvoiceApp.Models;

namespace InvoiceApp.Utils
{
    public class CsvImporter
    {
        public static List<Customer> ImportCustomersFromCsv(string filePath)
        {
            var customers = new List<Customer>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("CSV file not found", filePath);
            }

            var lines = File.ReadAllLines(filePath);
            
            if (lines.Length == 0)
            {
                return customers;
            }

            // Check if first line is header
            bool hasHeader = lines[0].Contains("Name") || lines[0].Contains("name");
            int startIndex = hasHeader ? 1 : 0;

            for (int i = startIndex; i < lines.Length; i++)
            {
                try
                {
                    var fields = ParseCsvLine(lines[i]);
                    
                    if (fields.Count == 0 || string.IsNullOrWhiteSpace(fields[0]))
                    {
                        continue; // Skip empty lines
                    }

                    var customer = new Customer
                    {
                        Name = fields.Count > 0 ? fields[0].Trim() : "",
                        Address = fields.Count > 1 ? fields[1].Trim() : null,
                        Postcode = fields.Count > 2 ? fields[2].Trim() : null,
                        Email = fields.Count > 3 ? fields[3].Trim() : null,
                        Phone = fields.Count > 4 ? fields[4].Trim() : null
                    };

                    if (!string.IsNullOrEmpty(customer.Name))
                    {
                        customers.Add(customer);
                    }
                }
                catch
                {
                    // Skip invalid rows
                    continue;
                }
            }

            return customers;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            fields.Add(currentField);
            return fields;
        }

        public static void ExportCustomersToCsv(List<Customer> customers, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Name,Address,Postcode,Email,Phone");

                // Write customer data
                foreach (var customer in customers)
                {
                    writer.WriteLine($"\"{EscapeCsv(customer.Name)}\",\"{EscapeCsv(customer.Address)}\",\"{EscapeCsv(customer.Postcode)}\",\"{EscapeCsv(customer.Email)}\",\"{EscapeCsv(customer.Phone)}\"");
                }
            }
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            
            return value.Replace("\"", "\"\"");
        }
    }
}
