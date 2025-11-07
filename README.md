<div align="center">

# 🧾 Company Invoice App  
**A full-featured desktop invoicing system built with .NET 6, complete with PDF generation, email reminders, and a rich analytics dashboard.**

![.NET](https://img.shields.io/badge/.NET-6.0-blue?style=flat-square&logo=dotnet)
![SQLite](https://img.shields.io/badge/Database-SQLite-lightgrey?style=flat-square&logo=sqlite)
![C#](https://img.shields.io/badge/Language-C%23-239120?style=flat-square&logo=csharp)
![License](https://img.shields.io/badge/License-Proprietary-red?style=flat-square)

</div>

---

## 📋 Overview

**Company Invoice App** is a robust and modern desktop application that allows small businesses to efficiently manage customers, create invoices, send automatic email reminders, and generate insightful analytics — all in one place.  

It includes a separate background task for sending scheduled payment reminders via SMTP.

---

## 🏗️ Project Structure

InvoiceApp/
├── InvoiceApp.sln
├── InvoiceApp/
│ ├── InvoiceApp.csproj
│ ├── Program.cs
│ ├── Forms/
│ │ ├── MainForm.cs (Dashboard with charts)
│ │ ├── CustomerForm.cs (Customer CRUD + CSV import)
│ │ ├── InvoiceListForm.cs (Invoice list view)
│ │ ├── InvoiceForm.cs (Create/Edit invoices)
│ │ ├── SettingsForm.cs (SMTP & app settings)
│ │ └── ReportsForm.cs (Analytics & reports)
│ ├── Models/
│ ├── Services/
│ │ ├── DatabaseService.cs
│ │ ├── CustomerService.cs
│ │ ├── InvoiceService.cs
│ │ ├── PdfService.cs
│ │ └── MailService.cs
│ └── Utils/
│ ├── CsvImporter.cs
│ ├── Logger.cs
│ └── AppSettings.cs
└── Tools/
└── ReminderTask/
├── ReminderTask.csproj
├── Program.cs
├── DatabaseService.cs
├── MailService.cs
└── AppSettings.cs


---

## ⚙️ Installation & Setup

### 🧱 Requirements
- Visual Studio 2022 (or later)  
- .NET 6.0 SDK  
- Windows 10/11  

### 💻 Build Steps
1. Open `InvoiceApp.sln` in Visual Studio  
2. Restore NuGet packages (automatically)  
3. Build the solution (`Ctrl + Shift + B`)  
4. Run the application (`F5`)

### 🧾 Command Line (Alternative)
```bash
cd "d:\TERD\c#_freelance"
dotnet restore
dotnet build
dotnet run --project InvoiceApp\InvoiceApp.csproj

✨ Features
👥 Customer Management

Add, edit, delete customers

Import bulk customers via CSV

Search and filter customer records

💸 Invoice Management

Auto invoice numbering: INV{YEAR}-{NNNN}

Multiple line items with quantity, pricing, and tax

Auto subtotal and total calculation

Mark invoices as Paid / Unpaid

Overdue tracking and invoice status

📄 PDF Export

Generate professional PDF invoices using iText7

Save to Documents\InvoiceApp\Invoices\

Automatically open PDF after generation

📧 Email Reminders

Separate ReminderTask console app for automation

Configurable:

Days after due date

Max reminders

Interval between reminders

Sends overdue invoice emails via SMTP

Logs reminder history for accuracy

📊 Dashboard & Reports

Real-time statistics:

Total revenue

Outstanding balance

Overdue invoice count

Top 10 customers by revenue

Reports page with detailed analytics

⚙️ Settings

SMTP configuration (Gmail, Outlook, etc.)

Company information setup

Reminder interval customization

Test SMTP connection directly from app

🗄️ Database

SQLite stored in %APPDATA%\InvoiceApp\invoice_app.db

Auto schema creation on first run

Supports transactions and foreign keys

🧾 Logging

Uses Serilog for structured file logging

Logs stored in %APPDATA%\InvoiceApp\Logs\

30-day log retention

🕒 Running Reminder Task
Manual Run
dotnet run --project Tools/ReminderTask/ReminderTask.csproj

Scheduled Run (Windows Task Scheduler)

Open Task Scheduler

Create a new task

Action: Start a program

Program:

d:\TERD\c#_freelance\Tools\ReminderTask\bin\Debug\net6.0\ReminderTask.exe


Trigger: Daily at 9:00 AM

Run with highest privileges: Optional

🧰 Configuration
📁 File Location

%APPDATA%\InvoiceApp\settings.json

🧩 Example Configuration
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

💌 SMTP Configuration Tips
Provider	Host	Port	SSL	Notes
Gmail	smtp.gmail.com	587	✅	Use App Password (2FA required)
Outlook	smtp.office365.com	587	✅	Use Office 365 credentials
Custom SMTP	(varies)	—	—	Refer to provider docs
🧾 Sample Data (CSV Import)

customers.csv

Name,Address,Postcode,Email,Phone
Acme Corporation,123 Main St,12345,billing@acme.com,555-1234
TechStart Inc,456 Oak Ave,23456,accounts@techstart.com,555-5678
Global Solutions,789 Pine Rd,34567,payments@globalsol.com,555-9012
Local Business,321 Elm St,45678,info@localbiz.com,555-3456
Enterprise Co,654 Maple Dr,56789,finance@enterprise.com,555-7890


Import via: Customers → Import CSV

🧩 Technologies Used

.NET 6 — Modern cross-platform framework

Windows Forms — Desktop UI framework

SQLite — Embedded local database

Dapper — Lightweight ORM

iText7 — PDF generation library

MailKit — SMTP email sending

ScottPlot — Chart and graph visualization

Serilog — File-based structured logging

Newtonsoft.Json — Config file management

🧾 License

Proprietary — All rights reserved.
