# Quick Start: Professional Invoice Generation

## 5-Minute Setup Guide

### Step 1: Configure Company Details (First Time Setup)

1. **Launch the application**
   ```powershell
   cd "d:\TERD\c#_freelance\InvoiceApp\bin\Release\net6.0-windows\"
   .\InvoiceApp.exe
   ```

2. **Open Settings**
   - Click on the **Settings** button from the main menu

3. **Fill in Company Details** (Tab 1)
   ```
   Company Name: Your Company Limited
   Address: 123 Business Street
   City: London
   Post Code: SW1A 1AA
   Country: United Kingdom
   Email: info@yourcompany.com
   Phone: +44 20 1234 5678
   
   Bank Details:
   Bank Name: Your Bank
   Account Title: Your Company Limited
   Account Number: 12345678
   Sort Code: 12-34-56
   
   Default Payment Terms: 7 days
   ```

4. **Save Settings**
   - Click **Save Settings** button
   - Settings are now stored and will be used for all future invoices

### Step 2: Create Your First Professional Invoice

1. **Create/Select a Customer**
   - Go to the Customers section
   - Add a new customer with complete address details

2. **Create New Invoice**
   - Click **New Invoice**
   - Select the customer
   - Set issue date and due date (or use defaults)

3. **Add Invoice Items**
   - Click **Add Item** button
   - Enter description (e.g., "Website Design Services")
   - Enter quantity (e.g., 1)
   - Enter unit price (e.g., 500.00)
   - Line total will be calculated automatically

4. **Save Invoice**
   - Click **Save** button
   - Invoice is now saved in the database

5. **Export Professional PDF**
   - Click **Export PDF** button
   - PDF will be generated with:
     - ✅ Blue professional header
     - ✅ Your company details
     - ✅ Customer details  
     - ✅ Items table with clean formatting
     - ✅ Payment terms and bank details
     - ✅ Professional footer

6. **View PDF**
   - PDF opens automatically
   - Saved in: `Documents\InvoiceApp\Invoices\[InvoiceNumber].pdf`

## What Your Invoice Will Look Like

```
┌─────────────────────────────────────────────────────────┐
│  INVOICE              INVOICE #         510             │ ← Blue Header
│                       INVOICE DATE      01/09/2025      │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  FROM                 BILL TO              INVOICE     │
│  Your Company Ltd     Customer Name         TOTAL      │
│  123 Business St      Customer Address    £500.00      │ ← Your Details
│  London, SW1A 1AA     City, Postcode                   │
│                                                         │
├─────────────────────────────────────────────────────────┤
│  DESCRIPTION                              AMOUNT        │
│  ───────────────────────────────────────────────────   │
│  Website Design Services                   500.00      │ ← Items Table
│  Hosting Setup                             50.00       │
│  ───────────────────────────────────────────────────   │
│  TOTAL                                    £550.00      │
│                                                         │
│  7 DAYS                                                 │
│  Payment Details                                        │ ← Bank Details
│  Your Company Limited                                   │
│  Your Bank                                              │
│  Sort Code 12-34-56                                     │
│  Account Number 12345678                                │
│                                                         │
│         Thank You For Your Business                     │ ← Footer
└─────────────────────────────────────────────────────────┘
```

## Key Features

### ✅ What's Working

| Feature | Status | Description |
|---------|--------|-------------|
| Professional Design | ✅ Ready | Blue header, clean layout |
| Company Details | ✅ Ready | Full address and contact info |
| Customer Details | ✅ Ready | Formatted address display |
| Items Table | ✅ Ready | Clean 2-column layout |
| Bank Details | ✅ Ready | Payment information included |
| Payment Terms | ✅ Ready | Automatically calculated |
| Auto-save Location | ✅ Ready | Documents folder |
| Settings Persistence | ✅ Ready | JSON configuration |

### 🎨 Design Elements

- **Color Scheme**: Professional blue (#007BFF) with gray accents
- **Typography**: Clean, hierarchical font sizes
- **Layout**: Responsive to content length
- **Spacing**: Proper margins and padding throughout
- **Readability**: High contrast, alternating row colors

## Common Tasks

### Update Company Information
```
Settings → Company Details → Edit Fields → Save Settings
```

### Change Payment Terms
```
Settings → Company Details → Default Payment Terms → Set Days → Save
```

### Update Bank Details
```
Settings → Company Details → Bank Section → Update Fields → Save
```

### Export Multiple Invoices
```
1. Open Invoice List
2. Select each invoice
3. Click "Export PDF" for each
4. All PDFs saved to Documents\InvoiceApp\Invoices\
```

## Tips for Best Results

### 1. Complete Address Information
Always fill in complete addresses for:
- ✅ Your company (in Settings)
- ✅ Your customers (in Customer Management)

### 2. Use Descriptive Item Names
Instead of: "Service"
Better: "Website Design and Development Services"

### 3. Keep Bank Details Updated
- Verify account numbers are correct
- Update sort codes if you change banks
- Use official account title

### 4. Consistent Payment Terms
- Set default in Settings
- Override per-invoice if needed
- Common terms: 7, 14, 30 days

### 5. Review Before Sending
- Always open the PDF after generation
- Check all details are correct
- Verify calculations are accurate

## File Locations

### Invoice PDFs
```
C:\Users\[YourUsername]\Documents\InvoiceApp\Invoices\
```

### Configuration File
```
C:\Users\[YourUsername]\AppData\Roaming\InvoiceApp\settings.json
```

### Database
```
C:\Users\[YourUsername]\AppData\Roaming\InvoiceApp\invoices.db
```

## Build and Run

### Development Build
```powershell
cd "d:\TERD\c#_freelance"
dotnet build InvoiceApp.sln
```

### Release Build
```powershell
cd "d:\TERD\c#_freelance"
dotnet build InvoiceApp.sln --configuration Release
```

### Run Application
```powershell
cd "d:\TERD\c#_freelance\InvoiceApp\bin\Release\net6.0-windows\"
.\InvoiceApp.exe
```

## Troubleshooting

### Issue: Invoice looks incomplete
**Fix**: Go to Settings → Company Details and fill in ALL fields

### Issue: Bank details not showing
**Fix**: Settings → Company Details → Bank section → Fill in details

### Issue: PDF won't open
**Fix**: Check Documents folder permissions or install a PDF reader

### Issue: Colors look different
**Normal**: Display calibration varies. PDF colors are standard.

## Next Steps

After generating your first professional invoice:

1. ✅ **Test the complete flow** (Create → Save → Export → View)
2. ✅ **Configure email settings** (Settings → Email tab)
3. ✅ **Set up payment reminders** (Settings → Reminders tab)
4. ✅ **Import customer data** (Customers → Import CSV)
5. ✅ **Create invoice templates** (Save common items)

## Support

For issues or questions:
- Review `INVOICE_PDF_ENHANCEMENT.md` for detailed documentation
- Check `IMPLEMENTATION_SUMMARY.md` for technical details
- Review code comments in `Services/PdfService.cs`

---

**You're all set!** Start creating professional invoices that will impress your clients. 🎉
