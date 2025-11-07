# InvoiceApp - Deployment Guide

## Overview

This guide covers building and deploying InvoiceApp for production use on Windows machines.

## Prerequisites

- Visual Studio 2022 with .NET Desktop Development workload
- .NET 6.0 SDK
- Windows 10/11 for testing

## Build for Production

### Method 1: Visual Studio Publish (Recommended)

1. **Open Solution**
   ```
   Open InvoiceApp.sln in Visual Studio 2022
   ```

2. **Set Release Configuration**
   - Change build configuration from "Debug" to "Release"
   - Click on dropdown at top: Debug → Release

3. **Publish the Application**
   - Right-click on `InvoiceApp` project
   - Select "Publish..."
   - Choose "Folder" as target
   - Set target location: `bin\Release\net6.0\publish\`
   - Click "Publish"

4. **Publish ReminderTask**
   - Right-click on `ReminderTask` project
   - Select "Publish..."
   - Choose "Folder" as target
   - Set target location: `Tools\ReminderTask\bin\Release\net6.0\publish\`
   - Click "Publish"

### Method 2: Command Line Build

```powershell
# Navigate to solution directory
cd "d:\TERD\c#_freelance"

# Restore packages
dotnet restore

# Build in Release mode
dotnet build --configuration Release

# Publish main application
dotnet publish InvoiceApp\InvoiceApp.csproj `
  --configuration Release `
  --output .\publish\InvoiceApp `
  --self-contained false `
  --runtime win-x64

# Publish ReminderTask
dotnet publish Tools\ReminderTask\ReminderTask.csproj `
  --configuration Release `
  --output .\publish\ReminderTask `
  --self-contained false `
  --runtime win-x64
```

### Method 3: Self-Contained Deployment

For machines without .NET 6 Runtime:

```powershell
# Publish main application (self-contained)
dotnet publish InvoiceApp\InvoiceApp.csproj `
  --configuration Release `
  --output .\publish\InvoiceApp-Standalone `
  --self-contained true `
  --runtime win-x64 `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true

# Publish ReminderTask (self-contained)
dotnet publish Tools\ReminderTask\ReminderTask.csproj `
  --configuration Release `
  --output .\publish\ReminderTask-Standalone `
  --self-contained true `
  --runtime win-x64 `
  -p:PublishSingleFile=true
```

## Package for Distribution

### Option 1: Zip Archive (Simple)

1. Create distribution folder structure:
   ```
   InvoiceApp-v1.0/
   ├── InvoiceApp.exe
   ├── *.dll (all dependencies)
   ├── ReminderTask/
   │   ├── ReminderTask.exe
   │   └── *.dll
   ├── Samples/
   │   └── sample_customers.csv
   ├── README.md
   ├── QUICKSTART.md
   └── LICENSE.txt
   ```

2. Create ZIP archive:
   ```powershell
   Compress-Archive -Path .\InvoiceApp-v1.0 -DestinationPath InvoiceApp-v1.0.zip
   ```

### Option 2: Windows Installer (MSI) with WiX

1. **Install WiX Toolset**
   - Download from: https://wixtoolset.org/
   - Install WiX Toolset Build Tools

2. **Create WiX Project**
   - Add new WiX Setup Project to solution
   - Configure Product.wxs file
   - Reference InvoiceApp project output

3. **Build Installer**
   ```
   msbuild InvoiceAppSetup.wixproj /p:Configuration=Release
   ```

### Option 3: ClickOnce Deployment

1. **Configure ClickOnce**
   - Right-click InvoiceApp project
   - Properties → Publish
   - Set Publishing Folder URL
   - Set Installation URL (optional)
   - Configure prerequisites (.NET 6)

2. **Publish**
   - Click "Publish Now"
   - Files will be in `bin\Release\app.publish\`

3. **Distribute**
   - Upload to web server or file share
   - Users click setup.exe to install

## Installation on Target Machine

### Using Published Folder

1. **Copy Files**
   - Copy entire publish folder to target machine
   - Suggested location: `C:\Program Files\InvoiceApp\`

2. **Create Shortcuts**
   ```powershell
   # Desktop shortcut
   $WshShell = New-Object -comObject WScript.Shell
   $Shortcut = $WshShell.CreateShortcut("$Home\Desktop\InvoiceApp.lnk")
   $Shortcut.TargetPath = "C:\Program Files\InvoiceApp\InvoiceApp.exe"
   $Shortcut.Save()
   ```

3. **First Run**
   - Run InvoiceApp.exe
   - Database will be created in %APPDATA%\InvoiceApp\
   - Configure settings

### Using Installer (MSI/ClickOnce)

1. Run the installer
2. Follow installation wizard
3. Launch from Start Menu

## Post-Installation Configuration

### 1. Schedule Reminder Task

**Manual Setup:**

1. Open Task Scheduler (`taskschd.msc`)
2. Create Basic Task:
   - Name: "InvoiceApp Reminders"
   - Description: "Send overdue invoice reminders"
   - Trigger: Daily at 9:00 AM
   - Action: Start a program
   - Program: `C:\Program Files\InvoiceApp\ReminderTask\ReminderTask.exe`
   - Start in: `C:\Program Files\InvoiceApp\ReminderTask\`

**Automated Setup (PowerShell script):**

```powershell
# Create scheduled task
$action = New-ScheduledTaskAction -Execute "C:\Program Files\InvoiceApp\ReminderTask\ReminderTask.exe" -WorkingDirectory "C:\Program Files\InvoiceApp\ReminderTask\"
$trigger = New-ScheduledTaskTrigger -Daily -At 9:00AM
$principal = New-ScheduledTaskPrincipal -UserId "$env:USERDOMAIN\$env:USERNAME" -LogonType Interactive
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
$task = New-ScheduledTask -Action $action -Principal $principal -Trigger $trigger -Settings $settings -Description "Send overdue invoice reminders"

Register-ScheduledTask -TaskName "InvoiceApp Reminders" -InputObject $task
```

### 2. Firewall Configuration

Allow outbound SMTP (port 587):

```powershell
New-NetFirewallRule -DisplayName "InvoiceApp SMTP" `
  -Direction Outbound `
  -Protocol TCP `
  -LocalPort 587 `
  -Action Allow
```

### 3. User Training

- Provide QUICKSTART.md guide
- Demo basic workflows
- Configure SMTP settings together
- Import sample customer data
- Create test invoice
- Generate PDF
- Test email reminder

## Upgrade/Update Procedure

### Manual Update

1. **Backup Data**
   ```powershell
   # Backup database
   $appData = "$env:APPDATA\InvoiceApp"
   Copy-Item "$appData\invoice_app.db" "$appData\invoice_app.db.backup"
   Copy-Item "$appData\settings.json" "$appData\settings.json.backup"
   ```

2. **Stop Application**
   - Close InvoiceApp if running
   - Disable scheduled task temporarily

3. **Replace Files**
   - Replace executable and DLLs with new version
   - Keep database and settings files

4. **Test**
   - Launch new version
   - Verify database compatibility
   - Check all features

5. **Re-enable Scheduled Task**

### Automated Update Script

```powershell
# update-invoiceapp.ps1
param(
    [string]$NewVersionPath,
    [string]$InstallPath = "C:\Program Files\InvoiceApp"
)

# Backup
$appData = "$env:APPDATA\InvoiceApp"
Copy-Item "$appData\invoice_app.db" "$appData\invoice_app.db.backup" -Force
Copy-Item "$appData\settings.json" "$appData\settings.json.backup" -Force

# Stop processes
Stop-Process -Name "InvoiceApp" -ErrorAction SilentlyContinue

# Update files (preserve database and settings)
Copy-Item "$NewVersionPath\*" $InstallPath -Recurse -Force -Exclude @("*.db", "settings.json")

Write-Host "Update complete. Please restart InvoiceApp."
```

## Uninstall Procedure

### Manual Uninstall

1. **Remove Scheduled Task**
   ```powershell
   Unregister-ScheduledTask -TaskName "InvoiceApp Reminders" -Confirm:$false
   ```

2. **Delete Application Files**
   ```powershell
   Remove-Item "C:\Program Files\InvoiceApp" -Recurse -Force
   ```

3. **Optional: Remove User Data**
   ```powershell
   # WARNING: This deletes all invoices and customer data!
   Remove-Item "$env:APPDATA\InvoiceApp" -Recurse -Force
   ```

4. **Remove Shortcuts**
   ```powershell
   Remove-Item "$Home\Desktop\InvoiceApp.lnk" -ErrorAction SilentlyContinue
   ```

## Multi-User Deployment

For shared/networked installations:

### Shared Database Approach

1. **Central Database Location**
   - Place database on network share
   - Example: `\\FileServer\InvoiceApp\invoice_app.db`

2. **Modify Settings**
   - Each user's settings.json should point to shared DB:
   ```json
   {
     "DatabasePath": "\\\\FileServer\\InvoiceApp\\invoice_app.db"
   }
   ```

3. **Permissions**
   - All users need read/write access to database file
   - Set NTFS permissions accordingly

4. **Locking Considerations**
   - SQLite supports concurrent reads
   - Writes are serialized
   - Good for small teams (< 10 users)

### Cloud Backup

For automatic backups to cloud storage:

```powershell
# Scheduled task to backup to OneDrive
$source = "$env:APPDATA\InvoiceApp\invoice_app.db"
$dest = "$env:OneDrive\Backups\InvoiceApp\invoice_app-$(Get-Date -Format 'yyyy-MM-dd').db"
Copy-Item $source $dest
```

## Deployment Checklist

- [ ] Build in Release configuration
- [ ] Test on clean machine
- [ ] Verify .NET 6 Runtime requirement
- [ ] Package all dependencies
- [ ] Include README and QUICKSTART
- [ ] Create shortcuts/installers
- [ ] Test database creation
- [ ] Test PDF generation
- [ ] Test SMTP with real server
- [ ] Schedule reminder task
- [ ] Configure firewall rules
- [ ] Train users
- [ ] Document backup procedure
- [ ] Plan update/upgrade path

## Support & Maintenance

### Log Files

Monitor these locations for issues:
- Application logs: `%APPDATA%\InvoiceApp\Logs\app-*.log`
- Reminder logs: `%APPDATA%\InvoiceApp\Logs\reminder-*.log`

### Common Issues

| Issue | Solution |
|-------|----------|
| Database locked | Close all InvoiceApp instances |
| SMTP auth failed | Verify credentials, use app password |
| PDF not opening | Install PDF reader (Adobe, Edge) |
| Charts not showing | Reinstall/repair .NET Runtime |
| Reminders not sending | Check Task Scheduler, verify logs |

### Backup Strategy

**Daily:** Automatic backup via scheduled task
**Weekly:** Manual backup to external drive
**Monthly:** Cloud backup verification

### Performance Tuning

- Database size > 100MB: Consider archiving old invoices
- Many customers (1000+): Add indexes if queries slow
- Network database: Consider local cache

---

**Ready for Production!** Follow this guide to deploy InvoiceApp successfully. 🚀
