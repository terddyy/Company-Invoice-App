dotnet run --project InvoiceApp/InvoiceApp.csproj`

    *   To run the reminder task:
        `dotnet run --project Tools/ReminderTask/ReminderTask.csproj`


# InvoiceApp - Implementation Complete

This is a complete implementation of the InvoiceApp MVP as specified in the requirements document.

## Project Structure

```
InvoiceApp/
├── InvoiceApp.sln
├── InvoiceApp/
│   ├── InvoiceApp.csproj
│   ├── Program.cs
│   ├── Forms/
│   │   ├── MainForm.cs (Dashboard with charts)
│   │   ├── CustomerForm.cs (Customer CRUD + CSV import)
│   │   ├── InvoiceListForm.cs (Invoice list view)
│   │   ├── InvoiceForm.cs (Create/Edit invoices)
│   │   ├── SettingsForm.cs (SMTP & app settings)
│   │   └── ReportsForm.cs (Analytics & reports)
│   ├── Models/
│   │   ├── Customer.cs
│   │   ├── Invoice.cs
│   │   ├── InvoiceItem.cs
│   │   └── ReminderLog.cs
│   ├── Services/
│   │   ├── DatabaseService.cs (SQLite management)
│   │   ├── CustomerService.cs (Customer operations)
│   │   ├── InvoiceService.cs (Invoice operations)
│   │   ├── PdfService.cs (PDF generation with iText7)
│   │   └── MailService.cs (Email via MailKit)
│   └── Utils/
│       ├── NumberingHelper.cs (Invoice number generation)
│       ├── CsvImporter.cs (CSV import/export)
│       ├── Logger.cs (Serilog wrapper)
│       └── AppSettings.cs (Configuration management)
└── Tools/
    └── ReminderTask/
        ├── ReminderTask.csproj
        ├── Program.cs (Console app for reminders)
        ├── DatabaseService.cs
        ├── MailService.cs
        └── AppSettings.cs
```

## Building the Application

### Requirements
- Visual Studio 2022 (or later)
- .NET 6.0 SDK
- Windows 10/11

### Build Steps

1. Open `InvoiceApp.sln` in Visual Studio
2. Restore NuGet packages (should happen automatically)
3. Build the solution (Ctrl+Shift+B)
4. Run the application (F5)

### Command Line Build (if .NET SDK is available)

```powershell
cd "d:\TERD\c#_freelance"
dotnet restore
dotnet build
dotnet run --project InvoiceApp\InvoiceApp.csproj
```

## Features Implemented

### ✅ Customer Management
- Create, Read, Update, Delete customers
- CSV import for bulk customer creation
- Customer list with search/filter

### ✅ Invoice Management
- Create invoices with automatic numbering (INV{YEAR}-{NNNN})
- Add multiple line items with quantity and pricing
- Automatic subtotal and total calculation
- Tax support
- Mark invoices as Paid/Unpaid
- Automatic overdue detection
- Invoice status tracking

### ✅ PDF Export
- Generate professional PDF invoices using iText7
- Save to Documents\InvoiceApp\Invoices\
- Automatic PDF opening after generation

### ✅ Email Reminders
- ReminderTask console application
- Configurable reminder settings (days after due, max reminders, interval)
- Email overdue invoices via SMTP
- Reminder logging to track sent emails
- Respects reminder intervals and max limits

### ✅ Dashboard & Reports
- Dashboard with key metrics:
  - Total revenue
  - Outstanding amounts
  - Overdue invoice count
- Top 10 customers by revenue (chart)
- Reports page with detailed analytics

### ✅ Settings
- SMTP configuration
- Company information
- Reminder settings
- Test SMTP connection

### ✅ Database
- SQLite database stored in %APPDATA%\InvoiceApp\invoice_app.db
- Automatic schema creation on first run
- Transaction support for data integrity
- Foreign key relationships

### ✅ Logging
- Serilog file logging
- Logs stored in %APPDATA%\InvoiceApp\Logs\
- 30-day log retention

## First Run Setup

1. Launch InvoiceApp.exe
2. The application will automatically create the database in %APPDATA%\InvoiceApp\
3. Go to Settings menu to configure:
   - Company Name
   - Company Email
   - SMTP settings for email reminders
4. Create your first customer
5. Create your first invoice

## Scheduling Reminders

To schedule automatic reminder emails:

1. Open Windows Task Scheduler
2. Create a new task:
   - **Action:** Start a program
   - **Program:** `d:\TERD\c#_freelance\Tools\ReminderTask\bin\Debug\net6.0\ReminderTask.exe`
   - **Trigger:** Daily at 9:00 AM
   - **Run with highest privileges:** Optional
3. Save the task

The ReminderTask will:
- Query for overdue invoices
- Check reminder history
- Send emails only when:
  - Invoice is overdue by configured days
  - Max reminders not reached
  - Interval days passed since last reminder

## Sample Data

### Sample CSV for Customer Import

Create a file named `customers.csv`:

```csv
Name,Address,Postcode,Email,Phone
Acme Corporation,123 Main St,12345,billing@acme.com,555-1234
TechStart Inc,456 Oak Ave,23456,accounts@techstart.com,555-5678
Global Solutions,789 Pine Rd,34567,payments@globalsol.com,555-9012
Local Business,321 Elm St,45678,info@localbiz.com,555-3456
Enterprise Co,654 Maple Dr,56789,finance@enterprise.com,555-7890
```

Import via: Customers → Import CSV

## Configuration File Location

`%APPDATA%\InvoiceApp\settings.json`

Example:
```json
{
  "CompanyName": "My Company",
  "CompanyEmail": "billing@mycompany.com",
  "InvoicePrefix": "INV",
  "InvoiceYearMode": true,
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "Reminder": {
    "DaysAfterDue": 1,
    "MaxReminders": 3,
    "IntervalDays": 3
  }
}
```

## SMTP Configuration Tips

### Gmail
- Use App Password (not regular password)
- Enable 2-factor authentication
- Generate App Password in Google Account settings
- Host: smtp.gmail.com, Port: 587, SSL: True

### Outlook/Office365
- Host: smtp.office365.com, Port: 587, SSL: True

### Custom SMTP
- Configure according to your email provider's documentation

## Database Location

`%APPDATA%\InvoiceApp\invoice_app.db`

To backup: Copy this file to a safe location

To restore: Replace the file and restart the application

## Troubleshooting

### Database not created
- Ensure write permissions to %APPDATA%
- Run as administrator once

### PDF not generating
- Check Documents\InvoiceApp\Invoices\ folder exists
- Ensure disk space available

### Emails not sending
- Test SMTP in Settings → Test Connection
- Check firewall/antivirus
- Verify SMTP credentials
- Check spam folder

### Charts not displaying
- Ensure all NuGet packages restored
- Rebuild solution

## Technologies Used

- **.NET 6** - Modern .NET framework
- **Windows Forms** - Desktop UI
- **SQLite** - Lightweight embedded database
- **Dapper** - Micro-ORM for database access
- **iText7** - PDF generation
- **MailKit** - Email sending
- **ScottPlot** - Charting library
- **Serilog** - Logging framework
- **Newtonsoft.Json** - JSON configuration

## License

Proprietary - All rights reserved

## Support

For issues or questions, contact the development team.
