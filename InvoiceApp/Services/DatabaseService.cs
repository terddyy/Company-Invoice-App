using System.Data;
using System.Data.SQLite;
using Dapper;
using InvoiceApp.Models;

namespace InvoiceApp.Services
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

            // Ensure directory exists
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _dbPath = Path.Combine(appDataPath, "invoice_app.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";

            InitializeDatabase();
        }

        public DatabaseService(string dbPath)
        {
            _dbPath = dbPath;
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeDatabase();
        }

        public IDbConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        private void InitializeDatabase()
        {
            bool isNewDatabase = !File.Exists(_dbPath);

            using (var connection = GetConnection())
            {
                connection.Open();

                if (isNewDatabase)
                {
                    CreateSchema(connection);
                }
            }
        }

        private void CreateSchema(IDbConnection connection)
        {
            string schema = @"
                CREATE TABLE IF NOT EXISTS Users (
                  Id INTEGER PRIMARY KEY AUTOINCREMENT,
                  Email TEXT UNIQUE NOT NULL,
                  PasswordHash TEXT NOT NULL,
                  Name TEXT,
                  Role TEXT DEFAULT 'staff',
                  CreatedAt TEXT DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Customers (
                  Id INTEGER PRIMARY KEY AUTOINCREMENT,
                  Name TEXT NOT NULL,
                  Address TEXT,
                  Postcode TEXT,
                  Email TEXT,
                  Phone TEXT,
                  Notes TEXT,
                  CreatedAt TEXT DEFAULT (datetime('now'))
                );

                CREATE TABLE IF NOT EXISTS Invoices (
                  Id INTEGER PRIMARY KEY AUTOINCREMENT,
                  InvoiceNumber TEXT UNIQUE NOT NULL,
                  CustomerId INTEGER NOT NULL,
                  IssueDate TEXT NOT NULL,
                  DueDate TEXT NOT NULL,
                  Subtotal REAL DEFAULT 0,
                  Tax REAL DEFAULT 0,
                  Total REAL DEFAULT 0,
                  Status TEXT DEFAULT 'Unpaid',
                  Notes TEXT,
                  CreatedAt TEXT DEFAULT (datetime('now')),
                  FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
                );

                CREATE TABLE IF NOT EXISTS InvoiceItems (
                  Id INTEGER PRIMARY KEY AUTOINCREMENT,
                  InvoiceId INTEGER NOT NULL,
                  Description TEXT,
                  Quantity REAL DEFAULT 1,
                  UnitPrice REAL DEFAULT 0,
                  LineTotal REAL DEFAULT 0,
                  FOREIGN KEY(InvoiceId) REFERENCES Invoices(Id)
                );

                CREATE TABLE IF NOT EXISTS ReminderLog (
                  Id INTEGER PRIMARY KEY AUTOINCREMENT,
                  InvoiceId INTEGER NOT NULL,
                  SentAt TEXT DEFAULT (datetime('now')),
                  Method TEXT,
                  Result TEXT,
                  FOREIGN KEY(InvoiceId) REFERENCES Invoices(Id)
                );

                CREATE TABLE IF NOT EXISTS Settings (
                  Key TEXT PRIMARY KEY,
                  Value TEXT
                );

                CREATE INDEX IF NOT EXISTS idx_invoices_customerid ON Invoices(CustomerId);
                CREATE INDEX IF NOT EXISTS idx_invoiceitems_invoiceid ON InvoiceItems(InvoiceId);
                CREATE INDEX IF NOT EXISTS idx_reminderlog_invoiceid ON ReminderLog(InvoiceId);
            ";

            connection.Execute(schema);
        }

        public string GetDatabasePath()
        {
            return _dbPath;
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
