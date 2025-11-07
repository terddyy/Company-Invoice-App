# InvoiceApp - Testing Checklist

Complete this checklist before deploying to production.

## ✅ Installation & First Run

- [ ] Application builds without errors
- [ ] Application starts successfully
- [ ] Database file created in %APPDATA%\InvoiceApp\
- [ ] Database schema initialized correctly
- [ ] Settings file created in %APPDATA%\InvoiceApp\
- [ ] Log directory created
- [ ] No errors in log files
- [ ] Main dashboard loads

## ✅ Customer Management

### Create Customer
- [ ] Can create new customer with all fields
- [ ] Can create customer with only Name (minimum)
- [ ] Customer appears in list immediately
- [ ] Customer ID auto-increments
- [ ] Created date populated correctly
- [ ] Form validation works (empty name rejected)

### Edit Customer
- [ ] Can select customer from list
- [ ] Fields populate correctly
- [ ] Can update all fields
- [ ] Changes saved to database
- [ ] List refreshes after save

### Delete Customer
- [ ] Can delete customer
- [ ] Confirmation dialog appears
- [ ] Customer removed from list
- [ ] Customer removed from database
- [ ] Cannot delete if invoices exist (or test cascade)

### CSV Import
- [ ] Can select CSV file via dialog
- [ ] Valid CSV imports successfully
- [ ] Shows count of imported customers
- [ ] Imported customers appear in list
- [ ] Handles malformed CSV gracefully
- [ ] Handles duplicate names
- [ ] Sample CSV file imports correctly

## ✅ Invoice Management

### Create Invoice
- [ ] Can create new invoice
- [ ] Customer dropdown populated
- [ ] Default dates set correctly (today + 30 days)
- [ ] Can add line items
- [ ] Line item totals calculate correctly
- [ ] Subtotal calculates correctly
- [ ] Tax added to subtotal
- [ ] Total calculates correctly
- [ ] Invoice saves successfully
- [ ] Invoice number auto-generated (INV{YEAR}-{NNNN})
- [ ] Invoice number unique
- [ ] Invoice number increments correctly

### Edit Invoice
- [ ] Can select and edit existing invoice
- [ ] All fields load correctly
- [ ] Line items display correctly
- [ ] Can add/remove items
- [ ] Totals recalculate on change
- [ ] Can update customer
- [ ] Can update dates
- [ ] Can update tax
- [ ] Changes saved correctly

### Invoice List
- [ ] All invoices display in grid
- [ ] Filter by "All" works
- [ ] Filter by "Paid" works
- [ ] Filter by "Unpaid" works
- [ ] Filter by "Overdue" works
- [ ] Double-click opens invoice
- [ ] Can select invoice from grid

### Mark as Paid
- [ ] Can mark invoice as paid
- [ ] Confirmation dialog appears
- [ ] Status updates in database
- [ ] Status updates in UI
- [ ] Dashboard reflects change
- [ ] Can mark as unpaid (reverse)

### Delete Invoice
- [ ] Can delete invoice
- [ ] Confirmation dialog appears
- [ ] Invoice and items deleted
- [ ] Removed from list
- [ ] Database cleaned up

### Overdue Detection
- [ ] Invoices past due date detected
- [ ] Status shows "Overdue" in list
- [ ] Overdue count on dashboard correct
- [ ] Refresh updates overdue status

## ✅ PDF Generation

### PDF Export
- [ ] "Export to PDF" button works
- [ ] PDF file created
- [ ] PDF saved to correct location
- [ ] Filename matches invoice number
- [ ] PDF opens automatically
- [ ] Success message displayed

### PDF Content
- [ ] Company name displayed
- [ ] Invoice number displayed
- [ ] Issue date displayed
- [ ] Due date displayed
- [ ] Status displayed
- [ ] Customer name displayed
- [ ] Customer address displayed
- [ ] Customer email displayed
- [ ] Customer phone displayed
- [ ] Line items table rendered
- [ ] Description column correct
- [ ] Quantity column correct
- [ ] Unit price column correct
- [ ] Line total column correct
- [ ] Subtotal displayed
- [ ] Tax displayed
- [ ] Total displayed (bold)
- [ ] Notes displayed (if present)
- [ ] Footer text displayed
- [ ] Professional appearance
- [ ] Readable fonts
- [ ] Proper alignment

## ✅ Email Functionality

### SMTP Configuration
- [ ] Can open settings form
- [ ] Can enter SMTP details
- [ ] Can save settings
- [ ] Settings persist after restart
- [ ] "Test Connection" button works
- [ ] Success message on valid config
- [ ] Error message on invalid config

### Send Invoice Email (if implemented)
- [ ] Can send invoice via email
- [ ] Email received
- [ ] PDF attached
- [ ] Subject line correct
- [ ] Body text correct
- [ ] From address correct
- [ ] To address correct

## ✅ Dashboard & Reports

### Dashboard
- [ ] Total revenue calculated correctly
- [ ] Outstanding amount correct
- [ ] Overdue count correct
- [ ] Top customers chart displays
- [ ] Chart shows correct data
- [ ] Chart labels readable
- [ ] Chart updates after invoice changes

### Reports Form
- [ ] Reports form opens
- [ ] Summary statistics correct
- [ ] Top customers grid populated
- [ ] Revenue chart displays
- [ ] Chart shows correct data
- [ ] Values match database

### Analytics
- [ ] Revenue calculation correct (paid only)
- [ ] Outstanding calculation correct
- [ ] Customer ranking correct
- [ ] All amounts formatted correctly

## ✅ Settings

### Application Settings
- [ ] Can update company name
- [ ] Can update company email
- [ ] Can update invoice prefix
- [ ] Settings save correctly
- [ ] Settings load on restart

### SMTP Settings
- [ ] Can update SMTP host
- [ ] Can update port
- [ ] Can toggle SSL
- [ ] Can update username
- [ ] Can update password
- [ ] Password masked in UI
- [ ] Test connection validates settings

### Reminder Settings
- [ ] Can update days after due
- [ ] Can update max reminders
- [ ] Can update interval days
- [ ] Settings save correctly
- [ ] Settings used by ReminderTask

## ✅ Reminder Task

### Build & Run
- [ ] ReminderTask project builds
- [ ] Can run from command line
- [ ] No runtime errors
- [ ] Connects to database
- [ ] Loads settings correctly

### Reminder Logic
- [ ] Finds overdue invoices
- [ ] Respects DaysAfterDue setting
- [ ] Respects MaxReminders limit
- [ ] Respects IntervalDays setting
- [ ] Skips invoices without email
- [ ] Skips paid invoices
- [ ] Logs sent reminders
- [ ] Updates ReminderLog table

### Email Sending
- [ ] Emails sent successfully
- [ ] Email content correct
- [ ] Invoice details included
- [ ] Customer name correct
- [ ] Amount correct
- [ ] Dates formatted correctly

### Logging
- [ ] Reminder log file created
- [ ] Success events logged
- [ ] Failure events logged
- [ ] Error details captured
- [ ] Log file rotates daily

## ✅ Data Integrity

### Database
- [ ] All tables created
- [ ] Foreign keys enforced
- [ ] No data corruption
- [ ] Transactions work correctly
- [ ] Concurrent access handled

### Validation
- [ ] Empty customer name rejected
- [ ] Empty invoice items rejected
- [ ] Invalid dates rejected
- [ ] Negative amounts rejected
- [ ] Duplicate invoice numbers prevented

### Calculations
- [ ] Line totals = Qty × Price
- [ ] Subtotal = Sum of line totals
- [ ] Total = Subtotal + Tax
- [ ] All decimals precise to 2 places

## ✅ Error Handling

### User Errors
- [ ] Missing customer shows warning
- [ ] Missing line items shows warning
- [ ] Invalid CSV shows error
- [ ] Friendly error messages
- [ ] No application crashes

### System Errors
- [ ] Database locked handled gracefully
- [ ] Network errors handled
- [ ] File access errors handled
- [ ] SMTP errors handled
- [ ] Errors logged to file

### Edge Cases
- [ ] Very long customer names
- [ ] Very large invoice amounts
- [ ] Many line items (100+)
- [ ] Special characters in names
- [ ] Empty database state
- [ ] First invoice creation
- [ ] Year rollover (invoice numbering)

## ✅ Performance

### Responsiveness
- [ ] Application starts < 3 seconds
- [ ] Forms open instantly
- [ ] Lists load quickly
- [ ] Charts render smoothly
- [ ] Database queries fast
- [ ] PDF generation < 2 seconds

### Scalability
- [ ] 100 customers perform well
- [ ] 1000 invoices perform well
- [ ] Large PDFs generate correctly
- [ ] Chart handles many data points

## ✅ User Experience

### Navigation
- [ ] Menu items clear
- [ ] Forms intuitive
- [ ] Buttons labeled clearly
- [ ] Consistent layout
- [ ] Logical workflow

### Visual Design
- [ ] Professional appearance
- [ ] Consistent fonts
- [ ] Good color choices
- [ ] Readable text
- [ ] Proper spacing
- [ ] Icons/images (if any)

### Usability
- [ ] Tab order logical
- [ ] Enter key submits forms
- [ ] Escape key cancels
- [ ] Double-click opens items
- [ ] Right-click menus (if any)
- [ ] Keyboard shortcuts work

## ✅ Security

### Data Protection
- [ ] Database in user's AppData
- [ ] No plaintext passwords (consider encryption)
- [ ] SQL injection prevention (parameterized queries)
- [ ] Input validation
- [ ] XSS prevention (if web views)

### SMTP Security
- [ ] SSL/TLS support
- [ ] Password not logged
- [ ] Credentials not exposed in UI
- [ ] Test connection doesn't leak creds

## ✅ Logging & Debugging

### Application Logs
- [ ] Startup logged
- [ ] Shutdown logged
- [ ] Errors logged
- [ ] Warnings logged
- [ ] Info events logged
- [ ] Log rotation works
- [ ] Logs readable

### Reminder Logs
- [ ] Task execution logged
- [ ] Reminders sent logged
- [ ] Failures logged
- [ ] Statistics logged

## ✅ Documentation

### User Documentation
- [ ] README.md complete
- [ ] QUICKSTART.md clear
- [ ] Sample data provided
- [ ] Screenshots (if applicable)

### Developer Documentation
- [ ] Code comments adequate
- [ ] README explains architecture
- [ ] Deployment guide complete
- [ ] Setup instructions clear

## ✅ Deployment

### Build
- [ ] Release build succeeds
- [ ] No warnings
- [ ] All dependencies included
- [ ] Correct target framework

### Installation
- [ ] Can install on clean machine
- [ ] .NET Runtime detected/installed
- [ ] Application runs after install
- [ ] Shortcuts created
- [ ] Uninstall works

### Scheduled Task
- [ ] Can create scheduled task
- [ ] Task runs at scheduled time
- [ ] Task runs with correct permissions
- [ ] Task logs output

## ✅ Backup & Recovery

### Backup
- [ ] Can locate database file
- [ ] Can copy database file
- [ ] Can export settings
- [ ] Backup procedure documented

### Recovery
- [ ] Can restore from backup
- [ ] Application works after restore
- [ ] Data intact after restore
- [ ] Settings preserved

## ✅ Final Checks

- [ ] No hardcoded paths
- [ ] No debug code
- [ ] No test data
- [ ] Version number set
- [ ] License file included
- [ ] All TODOs resolved
- [ ] Code reviewed
- [ ] Tested on target OS version
- [ ] Antivirus doesn't flag app
- [ ] Ready for production

---

## Test Results Summary

**Date Tested:** _________________

**Tested By:** _________________

**Pass Rate:** _____ / _____ (___%)

**Critical Issues:** _________________

**Minor Issues:** _________________

**Ready for Production:** ☐ Yes  ☐ No  ☐ With Fixes

**Notes:**
_________________________________________________________________
_________________________________________________________________
_________________________________________________________________

---

**Sign Off:**

Developer: _________________ Date: _________

Tester: _________________ Date: _________

Manager: _________________ Date: _________
