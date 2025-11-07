# InvoiceApp - Quick Start Guide

## 🚀 Getting Started

### Step 1: Build the Application

**Using Visual Studio:**
1. Open `InvoiceApp.sln` in Visual Studio 2022
2. Right-click on the solution and select "Restore NuGet Packages"
3. Press `Ctrl+Shift+B` to build the solution
4. Press `F5` to run the application

**Expected Output:**
- The application will create a database file at: `%APPDATA%\InvoiceApp\invoice_app.db`
- The main dashboard window will appear

### Step 2: Configure Settings

1. Click on **Settings** menu
2. Fill in:
   - **Company Name:** Your company name
   - **Company Email:** Your email address
   - **SMTP Host:** Your email server (e.g., smtp.gmail.com)
   - **SMTP Port:** Usually 587
   - **Username:** Your email
   - **Password:** Your email password or app password
3. Click **Test Connection** to verify SMTP settings
4. Click **Save**

### Step 3: Add Customers

**Option A: Import from CSV**
1. Click **Customers** menu
2. Click **Import CSV** button
3. Select `sample_customers.csv` from the project folder
4. Click Open
5. Confirm import

**Option B: Add Manually**
1. Click **Customers** menu
2. Click **New Customer**
3. Fill in customer details:
   - Name (required)
   - Address
   - Postcode
   - Email
   - Phone
4. Click **Save**

### Step 4: Create Your First Invoice

1. Click **Invoices** menu
2. Click **New Invoice**
3. Select a customer from dropdown
4. Set Issue Date and Due Date
5. Click **Add Item** to add line items:
   - Description: Product/Service name
   - Quantity: Number of units
   - Unit Price: Price per unit
   - Total: Calculated automatically
6. Enter Tax amount (if applicable)
7. Add notes (optional)
8. Click **Save Invoice**

### Step 5: Export Invoice to PDF

1. After saving, click **Export to PDF**
2. The PDF will be saved to: `Documents\InvoiceApp\Invoices\`
3. The PDF will automatically open

### Step 6: View Dashboard

1. Close the invoice window
2. The main dashboard shows:
   - Total Revenue
   - Outstanding Amount
   - Overdue Invoice Count
   - Top 10 Customers Chart

### Step 7: Mark Invoice as Paid

1. Click **Invoices** menu
2. Select an invoice from the list
3. Click **Mark as Paid**
4. Confirm

### Step 8: Set Up Automated Reminders

1. Build the ReminderTask project:
   ```
   cd Tools\ReminderTask
   dotnet build
   ```

2. Schedule with Windows Task Scheduler:
   - Open Task Scheduler
   - Create Basic Task
   - Name: "InvoiceApp Reminders"
   - Trigger: Daily at 9:00 AM
   - Action: Start a program
   - Program: `d:\TERD\c#_freelance\Tools\ReminderTask\bin\Debug\net6.0\ReminderTask.exe`
   - Finish

3. Test manually:
   ```powershell
   cd "d:\TERD\c#_freelance\Tools\ReminderTask\bin\Debug\net6.0"
   .\ReminderTask.exe
   ```

## 📊 Features Overview

### Dashboard
- Real-time statistics
- Top customers by revenue chart
- Quick overview of business health

### Customer Management
- Add, edit, delete customers
- Import customers from CSV
- Search and filter
- Track customer revenue

### Invoice Management
- Automatic invoice numbering (INV2025-0001)
- Multiple line items support
- Tax calculation
- Status tracking (Paid/Unpaid/Overdue)
- PDF export

### Reports & Analytics
- Total revenue
- Outstanding amounts
- Overdue invoices
- Top customers chart
- Export data

### Email Reminders
- Automatic overdue detection
- Configurable reminder rules
- Email tracking
- Prevent duplicate reminders

## 📁 File Locations

| Item | Location |
|------|----------|
| Database | `%APPDATA%\InvoiceApp\invoice_app.db` |
| Settings | `%APPDATA%\InvoiceApp\settings.json` |
| Logs | `%APPDATA%\InvoiceApp\Logs\` |
| PDFs | `Documents\InvoiceApp\Invoices\` |

## ⚙️ Configuration

### Invoice Settings
- **Prefix:** INV (customizable)
- **Year Mode:** Include year in invoice number
- **Numbering:** Automatic sequential numbering

### Reminder Settings
- **Days After Due:** Days to wait after due date before first reminder (default: 1)
- **Max Reminders:** Maximum number of reminders to send (default: 3)
- **Interval Days:** Days between reminders (default: 3)

## 🔧 Troubleshooting

### Application won't start
- Ensure .NET 6 Runtime is installed
- Check Windows Event Viewer for errors
- Run as Administrator

### Database errors
- Check write permissions to %APPDATA%
- Delete database file to reset (warning: data loss)
- Check disk space

### PDF generation fails
- Ensure Documents folder exists
- Check disk space
- Verify iText7 package installed

### Emails not sending
- Verify SMTP settings in Settings menu
- Test connection
- Check firewall/antivirus settings
- For Gmail: Use App Password, not regular password

### Charts not showing
- Rebuild solution
- Ensure ScottPlot package installed
- Check for errors in logs

## 💡 Tips & Best Practices

1. **Backup regularly:** Copy the database file to a safe location
2. **Test SMTP before going live:** Use the Test Connection button
3. **Import customers in bulk:** Use CSV import for efficiency
4. **Review logs:** Check %APPDATA%\InvoiceApp\Logs\ for issues
5. **Set realistic reminder intervals:** Don't spam customers
6. **Keep invoice numbers sequential:** Don't delete invoices once created
7. **Export PDFs regularly:** Keep copies outside the application

## 📞 Next Steps

1. Customize company information in Settings
2. Import or create customers
3. Create test invoices
4. Generate and review PDFs
5. Configure and test email reminders
6. Schedule automated reminder task
7. Start using in production!

## 🎯 Common Workflows

### Monthly Invoicing
1. Create invoices for all customers
2. Export PDFs
3. Email invoices to customers
4. Track payments
5. Run automated reminders for overdue

### End of Month Reporting
1. Go to Reports menu
2. Review top customers
3. Check outstanding amounts
4. Export data if needed
5. Follow up on overdue invoices

### Customer Onboarding
1. Add customer via Customers menu
2. Create first invoice
3. Export and send PDF
4. Track payment
5. Monitor in dashboard

---

**Congratulations!** You're now ready to use InvoiceApp to manage your invoices efficiently. 🎉
