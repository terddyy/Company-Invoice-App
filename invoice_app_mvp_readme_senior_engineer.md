# InvoiceApp — MVP README (Senior Engineer)

**Project:** InvoiceApp — Windows Desktop Invoice Maker & Tracker (MVP)

**Purpose:**
A lightweight, installable Windows desktop application (WinForms) that lets small businesses generate and track invoices, manage customers, send overdue reminders, export invoices to PDF, and view analytics on top customers. Designed as a production-ready MVP: reliable, secure, and easy to deploy on Windows machines.

---

## Table of Contents
1. Overview
2. Scope & MVP Feature List
3. Tech Stack & Libraries
4. System Architecture
5. Project Structure
6. Database Schema
7. Installation & Setup (Developer)
8. Configuration (App Settings)
9. Running the App (User)
10. Sending Reminders (Automation)
11. Export / Backup / Import
12. Security & Hardening
13. Testing Checklist
14. Future Roadmap (After MVP)
15. Troubleshooting & FAQ
16. Contact & Support

---

## 1. Overview
InvoiceApp is intended to be a single-machine, local-first desktop application for Windows. It stores its data in a local SQLite database (file-based) and provides core invoice flows required by the client:
- Create customers and import legacy customer lists
- Create invoices with automatic numbering and line items
- Mark invoices Paid / Unpaid; detect Overdue
- Export printable PDF invoices
- Generate charts to show top customers by revenue
- Send automatic overdue email reminders via scheduled task

MVP goals: quick installation, minimal dependencies, solid UX for basic accounting workflows, and robust reminders.

---

## 2. Scope & MVP Feature List
**Required features (MVP):**
- Customer management (CRUD)
- Invoice creation (items, tax, subtotal, total)
- Automatic invoice numbering (INV{YEAR}-{zero-padded-id})
- Status tracking: Paid / Unpaid / Overdue
- Dashboard with charts (top customers by money)
- PDF invoice export (printable format)
- CSV import for customers
- Email reminders for overdue invoices (configurable SMTP)
- Local SQLite database
- Installer (ClickOnce/MSI) or single EXE publish

**Non-MVP / Nice-to-have (later):**
- Multi-user sync / server or cloud backup
- Payment gateway (Stripe/PayPal)
- Recurring invoices
- Role-based access & audit logs

---

## 3. Tech Stack & Libraries
- **Language / Framework:** C# (.NET 6 or .NET Framework 4.8) — use .NET 6 for future-proofing unless client requires legacy.
- **UI:** Windows Forms (WinForms)
- **Database:** SQLite (System.Data.SQLite)
- **PDF Generation:** iText7 for .NET or iTextSharp (legacy) — iText7 preferred.
- **Charts:** LiveCharts2 or ScottPlot (simple and easy to embed in WinForms)
- **Email:** MailKit (SMTP client)
- **ORM / Data Access:** Lightweight repository with `System.Data.SQLite` + Dapper (optional) or plain ADO.NET with prepared statements
- **Installer / Packaging:** ClickOnce (quick) or WiX / MSIX for production MSI
- **Logging:** Serilog (file sink) or simple rolling file logger

---

## 4. System Architecture
Single-tier desktop application; UI (WinForms) directly interacts with local data layer (SQLite) via a `DatabaseService`. Background operations (reminder sending) are executed by a small console utility `ReminderTask.exe` which can be scheduled via Windows Task Scheduler.

Key responsibilities:
- **Forms**: handle user interactions and present charts
- **Services**: encapsulate DB access, PDF generation, email sending
- **Models**: domain objects (Customer, Invoice, InvoiceItem, ReminderLog)
- **Utils**: numbering helper, CSV parser, config loader

Security boundaries are limited since this is a local app; nonetheless protect sensitive config (SMTP passwords) and use secure storage for credential data.

---

## 5. Project Structure
Top-level Visual Studio solution layout:

```
InvoiceApp.sln
InvoiceApp/
├─ InvoiceApp.csproj
├─ Program.cs
├─ App.config
├─ Forms/
│  ├─ MainForm.cs (+ MainForm.Designer.cs)
│  ├─ LoginForm.cs (optional)
│  ├─ CustomerForm.cs
│  ├─ InvoiceForm.cs
│  ├─ InvoiceViewForm.cs
│  ├─ ReportForm.cs
│  └─ SettingsForm.cs
├─ Models/
│  ├─ Customer.cs
│  ├─ Invoice.cs
│  ├─ InvoiceItem.cs
│  └─ ReminderLog.cs
├─ Services/
│  ├─ DatabaseService.cs
│  ├─ CustomerService.cs
│  ├─ InvoiceService.cs
│  ├─ PdfService.cs
│  └─ MailService.cs
├─ Utils/
│  ├─ NumberingHelper.cs
│  ├─ CsvImporter.cs
│  └─ Logger.cs
├─ Database/
│  └─ invoice_app.db (created at runtime in %APPDATA%\InvoiceApp\)
├─ Resources/
│  └─ images, icons
└─ Tools/
   └─ ReminderTask/ReminderTask.csproj (console app)
```

Notes:
- The `Database` folder in repo is empty; on first run the application will create the DB file in the user's AppData folder and initialize schema.
- `ReminderTask` console app uses the same `DatabaseService` to query overdue invoices and uses `MailService` to send reminders.

---

## 6. Database Schema (SQLite)
Execute schema on first run (SQL below):

```sql
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
```

---

## 7. Installation & Setup (Developer)

### Prerequisites
- Windows 10 / 11
- Visual Studio 2022 with .NET desktop development workload
- .NET 6 SDK (if using .NET 6 target)
- NuGet packages: `System.Data.SQLite`, `MailKit`, `Dapper` (optional), `LiveCharts.WinForms` (or `ScottPlot.WinForms`), `iText7` (or `iTextSharp`), `Serilog`.

### Steps
1. Clone the repository: `git clone <repo-url>`
2. Open `InvoiceApp.sln` in Visual Studio.
3. Restore NuGet packages (Visual Studio will do this automatically).
4. Configure `App.config` (see Section 8). For development you can keep defaults.
5. Build solution (Ctrl+Shift+B).
6. Run (F5) — app will detect missing DB and create `invoice_app.db` in `%APPDATA%\InvoiceApp`.
7. Create the first admin user from `Settings -> Users` or provide a seeded user in `Database/seed.sql`.

---

## 8. Configuration (App Settings)
Settings are stored in `App.config` or in a local JSON settings file saved in `%APPDATA%\InvoiceApp\settings.json` (recommended):

```json
{
  "DatabasePath": "%APPDATA%\\InvoiceApp\\invoice_app.db",
  "InvoicePrefix": "INV",
  "InvoiceYearMode": true,
  "Smtp": {
    "Host": "smtp.mailserver.com",
    "Port": 587,
    "UseSsl": true,
    "Username": "billing@yourdomain.com",
    "Password": "ENCRYPTED_OR_SECURE_STORAGE"
  },
  "Reminder": {
    "DaysAfterDue": 1,
    "MaxReminders": 3,
    "IntervalDays": 3
  }
}
```

**Important:** store SMTP password securely. For a simple MVP, storing encrypted password in settings is acceptable; production apps should use Windows Credential Manager or DPAPI.

---

## 9. Running the App (User)
1. Install the app via the provided installer (or copy the published EXE to the machine).
2. Launch InvoiceApp. On first run, an initial setup wizard will prompt for:
   - Default company name and logo (optional)
   - Admin account creation (email + password)
   - SMTP settings (so reminders can be sent later)
3. Create customers (or import CSV).
4. Create invoices: select customer, add line items, set due date, then Save & Generate PDF.
5. Use Dashboard to see top customers and revenue summary.
6. Schedule ReminderTask (see Section 10) for automated overdue reminders.

---

## 10. Sending Reminders (Automation)
We ship a console utility `Tools/ReminderTask/ReminderTask.exe` that uses the app settings and shared DB to check for overdue invoices and send reminders.

**Scheduling:** Use Windows Task Scheduler to run daily (e.g., 09:00 AM):
- Action: `C:\Program Files\InvoiceApp\ReminderTask.exe`
- Trigger: Daily 09:00
- Run with highest privileges: optional

**Reminder logic (MVP):**
- Query invoices where `DueDate < today` AND `Status = 'Unpaid'` AND `RemindersSent < MaxReminders`.
- Send email using MailService with templated text + link to PDF file path (no web link required).
- Increment `RemindersSent` or insert a row into `ReminderLog` with result.
- Respect `IntervalDays` between reminders.

Note: Reminders are conservative — do not send more than 1 reminder per invoice per configured interval.

---

## 11. Export / Backup / Import
- **Export Invoice to PDF:** `InvoiceViewForm -> Export PDF` uses `PdfService` (iText7) to render an HTML template and produce a PDF saved to `Documents\InvoiceApp\Invoices\INVXXXX.pdf` (user-configurable).
- **Backup DB:** Copy `%APPDATA%\InvoiceApp\invoice_app.db` to external storage; provide `Settings -> Export -> Backup DB` UI.
- **Import Customers:** `CustomerForm -> Import CSV` supports headers: `Name, Address, Postcode, Email, Phone`.
- **Export Reports:** Export chart data to CSV or Excel (CSV for MVP).

---

## 12. Security & Hardening
- Store DB in `%APPDATA%` per user; if multiple users share machine, consider shared folder with NTFS ACL.
- Use parameterized queries to prevent SQL injection.
- Hash user passwords with `PBKDF2` via `Rfc2898DeriveBytes` or use `PasswordHasher` from Microsoft.AspNetCore.Identity.
- Secure SMTP credentials with DPAPI (ProtectedData.Protect) or Windows Credential Manager.
- Validate CSV imports to avoid injection of large payloads.
- Limit attachments and enforce file size limits for generated PDFs.

---

## 13. Testing Checklist
- [ ] App creates DB file on first run and initializes schema.
- [ ] Admin user creation flow works.
- [ ] Create / Edit / Delete customers works and updates UI.
- [ ] Create invoice with multiple items, tax calculation, and total correctness.
- [ ] Invoice numbering increments and persists correctly after restarts.
- [ ] Mark invoice Paid updates status and dashboard.
- [ ] Overdue detection logic identifies invoices past due date.
- [ ] ReminderTask sends emails successfully (test with test SMTP server).
- [ ] PDF generation produces readable invoices and saved file path exists.
- [ ] CSV import handles malformed rows gracefully.

---

## 14. Future Roadmap (After MVP)
- Sync/Backup to cloud (OneDrive/Google Drive or custom server)
- Multi-machine sync / server component
- Payment gateway integration and webhook handling
- Advanced reporting and export to Excel (.xlsx)
- Multi-currency and tax rules
- Mobile companion app for on-the-go invoice viewing

---

## 15. Troubleshooting & FAQ
**Q: App says DB file cannot be created.**
A: Ensure the user has write permission to `%APPDATA%\InvoiceApp`. Run app as admin once to initialize.

**Q: Emails not sending.**
A: Check SMTP host/port/username/password. Test via `Settings -> Test SMTP`. Verify the machine allows outbound SMTP (some networks block port 25/587).

**Q: PDFs look broken or fonts missing.**
A: Ensure iText7 is available and the HTML invoice template uses system fonts or embeds fonts into PDF.

**Q: Reminders sent multiple times.**
A: Open `ReminderLog` and check timestamps. Configure `Reminder.IntervalDays` and `MaxReminders` appropriately.

---

## 16. Contact & Support
For development and maintenance reach out to:
- **Lead Engineer:** [Your Name] — email: lead-engineer@example.com
- **Support:** support@example.com

---

## Appendices
### A. Invoice Numbering Algorithm
Format: `{Prefix}{Year}-{NNNN}`
Example: `INV2025-0001`
Implementation: `InvoiceService.GetNextInvoiceNumber()` uses a DB transaction to SELECT MAX(Id) FOR UPDATE (or uses SQLite row locking) then increments to produce unique, race-free numbers.

### B. Sample Email Template (Reminder)
```
Subject: [Reminder] Invoice {InvoiceNumber} is overdue

Dear {CustomerName},

Our records show invoice {InvoiceNumber} (issued {IssueDate}, due {DueDate}) is outstanding. Amount due: {Total}.
Please arrange payment at your earliest convenience. A PDF copy is attached.

Thank you,
{CompanyName}
```

---

*End of README — InvoiceApp MVP (Senior Engineer)*

