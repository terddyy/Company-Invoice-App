using System.Data;
using System.Data.SQLite;
using Dapper;

namespace ReminderTask
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly string _dbPath;

        public DatabaseService()
        {
            // Get AppData folder path
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InvoiceApp"
            );

            _dbPath = Path.Combine(appDataPath, "invoice_app.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
        }

        public IDbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
