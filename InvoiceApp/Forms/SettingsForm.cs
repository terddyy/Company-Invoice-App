using InvoiceApp.Services;
using InvoiceApp.Utils;

namespace InvoiceApp.Forms
{
    public partial class SettingsForm : Form
    {
        private AppSettings _settings;
        private TabControl tabControl = null!;
        
        // Company Settings
        private TextBox txtCompanyName = null!;
        private TextBox txtCompanyAddress = null!;
        private TextBox txtCompanyCity = null!;
        private TextBox txtCompanyPostCode = null!;
        private TextBox txtCompanyCountry = null!;
        private TextBox txtCompanyEmail = null!;
        private TextBox txtCompanyPhone = null!;
        private TextBox txtBankName = null!;
        private TextBox txtAccountNumber = null!;
        private TextBox txtSortCode = null!;
        private TextBox txtBankTitle = null!;
        private NumericUpDown numPaymentTerms = null!;
        
        // SMTP Settings
        private TextBox txtSmtpHost = null!;
        private TextBox txtSmtpPort = null!;
        private CheckBox chkUseSsl = null!;
        private TextBox txtSmtpUsername = null!;
        private TextBox txtSmtpPassword = null!;
        private Button btnTestSmtp = null!;
        
        // Reminder Settings
        private TextBox txtReminderDaysAfterDue = null!;
        private TextBox txtReminderMaxReminders = null!;
        private TextBox txtReminderIntervalDays = null!;
        
        private Button btnSave = null!;
        private Button btnCancel = null!;

        public SettingsForm(AppSettings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new Size(700, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Create tab control
            tabControl = new TabControl
            {
                Location = new Point(10, 10),
                Size = new Size(660, 540)
            };

            // Create tabs
            TabPage companyTab = CreateCompanyTab();
            TabPage smtpTab = CreateSmtpTab();
            TabPage reminderTab = CreateReminderTab();

            tabControl.TabPages.Add(companyTab);
            tabControl.TabPages.Add(smtpTab);
            tabControl.TabPages.Add(reminderTab);

            // Action Buttons
            btnSave = new Button
            {
                Text = "Save Settings",
                Location = new Point(450, 560),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(560, 560),
                Size = new Size(100, 35),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] { tabControl, btnSave, btnCancel });
        }

        private TabPage CreateCompanyTab()
        {
            TabPage tab = new TabPage("Company Details");
            int y = 20;

            // Company Information Section
            Label lblCompanyInfo = new Label
            {
                Text = "Company Information",
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };
            tab.Controls.Add(lblCompanyInfo);
            y += 35;

            AddLabelAndTextBox(tab, "Company Name:", ref txtCompanyName, ref y);
            AddLabelAndTextBox(tab, "Address:", ref txtCompanyAddress, ref y);
            AddLabelAndTextBox(tab, "City:", ref txtCompanyCity, ref y);
            AddLabelAndTextBox(tab, "Post Code:", ref txtCompanyPostCode, ref y);
            AddLabelAndTextBox(tab, "Country:", ref txtCompanyCountry, ref y);
            AddLabelAndTextBox(tab, "Email:", ref txtCompanyEmail, ref y);
            AddLabelAndTextBox(tab, "Phone:", ref txtCompanyPhone, ref y);

            y += 15;

            // Bank Details Section
            Label lblBankInfo = new Label
            {
                Text = "Bank Details (for invoices)",
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };
            tab.Controls.Add(lblBankInfo);
            y += 35;

            AddLabelAndTextBox(tab, "Bank Name:", ref txtBankName, ref y);
            AddLabelAndTextBox(tab, "Account Title:", ref txtBankTitle, ref y);
            AddLabelAndTextBox(tab, "Account Number:", ref txtAccountNumber, ref y);
            AddLabelAndTextBox(tab, "Sort Code:", ref txtSortCode, ref y);

            y += 15;

            // Payment Terms
            Label lblPayment = new Label
            {
                Text = "Default Payment Terms:",
                Location = new Point(30, y),
                AutoSize = true
            };
            numPaymentTerms = new NumericUpDown
            {
                Location = new Point(200, y - 3),
                Size = new Size(80, 25),
                Minimum = 1,
                Maximum = 365,
                Value = 7
            };
            Label lblDays = new Label
            {
                Text = "days",
                Location = new Point(290, y),
                AutoSize = true
            };
            tab.Controls.AddRange(new Control[] { lblPayment, numPaymentTerms, lblDays });

            return tab;
        }

        private TabPage CreateSmtpTab()
        {
            TabPage tab = new TabPage("Email (SMTP)");
            int y = 20;

            Label lblSmtpInfo = new Label
            {
                Text = "SMTP Server Configuration",
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };
            tab.Controls.Add(lblSmtpInfo);
            y += 35;

            AddLabelAndTextBox(tab, "SMTP Host:", ref txtSmtpHost, ref y);

            Label lblPort = new Label
            {
                Text = "SMTP Port:",
                Location = new Point(30, y),
                AutoSize = true
            };
            txtSmtpPort = new TextBox
            {
                Location = new Point(200, y - 3),
                Size = new Size(100, 25)
            };
            tab.Controls.AddRange(new Control[] { lblPort, txtSmtpPort });
            y += 35;

            chkUseSsl = new CheckBox
            {
                Text = "Use SSL/TLS (Recommended)",
                Location = new Point(200, y),
                AutoSize = true,
                Checked = true
            };
            tab.Controls.Add(chkUseSsl);
            y += 40;

            AddLabelAndTextBox(tab, "Username:", ref txtSmtpUsername, ref y);

            Label lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(30, y),
                AutoSize = true
            };
            txtSmtpPassword = new TextBox
            {
                Location = new Point(200, y - 3),
                Size = new Size(400, 25),
                PasswordChar = '*'
            };
            tab.Controls.AddRange(new Control[] { lblPassword, txtSmtpPassword });
            y += 45;

            btnTestSmtp = new Button
            {
                Text = "Test SMTP Connection",
                Location = new Point(200, y),
                Size = new Size(180, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTestSmtp.Click += BtnTestSmtp_Click;
            tab.Controls.Add(btnTestSmtp);

            y += 50;

            // Help text
            Label lblHelp = new Label
            {
                Text = "Common SMTP settings:\n" +
                       "Gmail: smtp.gmail.com, Port 587, SSL enabled\n" +
                       "Outlook: smtp.office365.com, Port 587, SSL enabled\n" +
                       "Note: Gmail requires App Password if 2FA is enabled",
                Location = new Point(30, y),
                Size = new Size(580, 80),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9)
            };
            tab.Controls.Add(lblHelp);

            return tab;
        }

        private TabPage CreateReminderTab()
        {
            TabPage tab = new TabPage("Reminders");
            int y = 20;

            Label lblReminderInfo = new Label
            {
                Text = "Automatic Payment Reminders",
                Location = new Point(20, y),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };
            tab.Controls.Add(lblReminderInfo);
            y += 35;

            Label lblInfo = new Label
            {
                Text = "Configure when and how often to send payment reminders for overdue invoices.",
                Location = new Point(30, y),
                Size = new Size(580, 40),
                ForeColor = Color.Gray
            };
            tab.Controls.Add(lblInfo);
            y += 50;

            AddLabelAndTextBox(tab, "Days After Due Date:", ref txtReminderDaysAfterDue, ref y, 100);
            AddLabelAndTextBox(tab, "Maximum Reminders:", ref txtReminderMaxReminders, ref y, 100);
            AddLabelAndTextBox(tab, "Interval Between (Days):", ref txtReminderIntervalDays, ref y, 100);

            return tab;
        }

        private void AddLabelAndTextBox(TabPage tab, string labelText, ref TextBox textBox, ref int y, int textBoxWidth = 400)
        {
            Label label = new Label
            {
                Text = labelText,
                Location = new Point(30, y),
                AutoSize = true
            };
            textBox = new TextBox
            {
                Location = new Point(200, y - 3),
                Size = new Size(textBoxWidth, 25)
            };
            tab.Controls.AddRange(new Control[] { label, textBox });
            y += 35;
        }

        private void LoadSettings()
        {
            // Company details
            txtCompanyName.Text = _settings.Company.Name;
            txtCompanyAddress.Text = _settings.Company.Address;
            txtCompanyCity.Text = _settings.Company.City;
            txtCompanyPostCode.Text = _settings.Company.PostCode;
            txtCompanyCountry.Text = _settings.Company.Country;
            txtCompanyEmail.Text = _settings.Company.Email;
            txtCompanyPhone.Text = _settings.Company.Phone;
            
            // Bank details
            txtBankName.Text = _settings.Company.BankName;
            txtBankTitle.Text = _settings.Company.BankTitle;
            txtAccountNumber.Text = _settings.Company.AccountNumber;
            txtSortCode.Text = _settings.Company.SortCode;
            numPaymentTerms.Value = _settings.Company.DefaultPaymentTermsDays;
            
            // SMTP settings
            txtSmtpHost.Text = _settings.Smtp.Host;
            txtSmtpPort.Text = _settings.Smtp.Port.ToString();
            chkUseSsl.Checked = _settings.Smtp.UseSsl;
            txtSmtpUsername.Text = _settings.Smtp.Username;
            txtSmtpPassword.Text = _settings.Smtp.Password;
            
            // Reminder settings
            txtReminderDaysAfterDue.Text = _settings.Reminder.DaysAfterDue.ToString();
            txtReminderMaxReminders.Text = _settings.Reminder.MaxReminders.ToString();
            txtReminderIntervalDays.Text = _settings.Reminder.IntervalDays.ToString();
        }

        private void BtnTestSmtp_Click(object? sender, EventArgs e)
        {
            try
            {
                var mailService = new MailService();
                mailService.Configure(
                    txtSmtpHost.Text,
                    int.Parse(txtSmtpPort.Text),
                    chkUseSsl.Checked,
                    txtSmtpUsername.Text,
                    txtSmtpPassword.Text,
                    txtCompanyEmail.Text,
                    txtCompanyName.Text
                );

                if (mailService.TestConnection())
                {
                    MessageBox.Show("SMTP connection successful!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("SMTP connection failed. Please check your settings.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing SMTP: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Company details
                _settings.Company.Name = txtCompanyName.Text;
                _settings.Company.Address = txtCompanyAddress.Text;
                _settings.Company.City = txtCompanyCity.Text;
                _settings.Company.PostCode = txtCompanyPostCode.Text;
                _settings.Company.Country = txtCompanyCountry.Text;
                _settings.Company.Email = txtCompanyEmail.Text;
                _settings.Company.Phone = txtCompanyPhone.Text;
                
                // Bank details
                _settings.Company.BankName = txtBankName.Text;
                _settings.Company.BankTitle = txtBankTitle.Text;
                _settings.Company.AccountNumber = txtAccountNumber.Text;
                _settings.Company.SortCode = txtSortCode.Text;
                _settings.Company.DefaultPaymentTermsDays = (int)numPaymentTerms.Value;
                
                // SMTP settings
                _settings.Smtp.Host = txtSmtpHost.Text;
                _settings.Smtp.Port = int.Parse(txtSmtpPort.Text);
                _settings.Smtp.UseSsl = chkUseSsl.Checked;
                _settings.Smtp.Username = txtSmtpUsername.Text;
                _settings.Smtp.Password = txtSmtpPassword.Text;
                
                // Reminder settings
                _settings.Reminder.DaysAfterDue = int.Parse(txtReminderDaysAfterDue.Text);
                _settings.Reminder.MaxReminders = int.Parse(txtReminderMaxReminders.Text);
                _settings.Reminder.IntervalDays = int.Parse(txtReminderIntervalDays.Text);

                _settings.Save();

                MessageBox.Show("Settings saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
