using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.Utils;

namespace InvoiceApp.Forms
{
    public partial class InvoiceForm : Form
    {
        private InvoiceService _invoiceService;
        private CustomerService _customerService;
        private AppSettings _settings;
        private Invoice? _invoice;

        private ComboBox cboCustomer = null!;
        private DateTimePicker dtpIssueDate = null!;
        private DateTimePicker dtpDueDate = null!;
        private DataGridView dgvItems = null!;
        private TextBox txtTax = null!;
        private Label lblSubtotal = null!;
        private Label lblTotal = null!;
        private TextBox txtNotes = null!;
        private Button btnAddItem = null!;
        private Button btnRemoveItem = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Button btnExportPdf = null!;
        private Button btnSendEmail = null!;

        public InvoiceForm(InvoiceService invoiceService, CustomerService customerService, AppSettings settings, Invoice? invoice)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _settings = settings;
            _invoice = invoice;
            
            InitializeComponent();
            LoadCustomers();
            
            if (_invoice != null)
            {
                LoadInvoiceData();
            }
            else
            {
                _invoice = new Invoice();
                _invoice.IssueDate = DateTime.Now;
                _invoice.DueDate = DateTime.Now.AddDays(30);
                dtpIssueDate.Value = _invoice.IssueDate;
                dtpDueDate.Value = _invoice.DueDate;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Invoice";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Customer
            Label lblCustomer = new Label
            {
                Text = "Customer:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            cboCustomer = new ComboBox
            {
                Location = new Point(120, 17),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Dates
            Label lblIssueDate = new Label
            {
                Text = "Issue Date:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            dtpIssueDate = new DateTimePicker
            {
                Location = new Point(120, 57),
                Size = new Size(200, 25)
            };

            Label lblDueDate = new Label
            {
                Text = "Due Date:",
                Location = new Point(20, 100),
                AutoSize = true
            };

            dtpDueDate = new DateTimePicker
            {
                Location = new Point(120, 97),
                Size = new Size(200, 25)
            };

            // Items Grid
            Label lblItems = new Label
            {
                Text = "Line Items:",
                Location = new Point(20, 140),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };

            dgvItems = new DataGridView
            {
                Location = new Point(20, 165),
                Size = new Size(840, 250),
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 300 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Quantity", Width = 100 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Unit Price", Width = 150 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { Name = "LineTotal", HeaderText = "Total", Width = 150, ReadOnly = true });

            btnAddItem = new Button
            {
                Text = "Add Item",
                Location = new Point(20, 425),
                Size = new Size(100, 30)
            };
            btnAddItem.Click += BtnAddItem_Click;

            btnRemoveItem = new Button
            {
                Text = "Remove Item",
                Location = new Point(130, 425),
                Size = new Size(100, 30)
            };
            btnRemoveItem.Click += BtnRemoveItem_Click;

            // Totals
            Label lblTaxLabel = new Label
            {
                Text = "Tax:",
                Location = new Point(600, 470),
                AutoSize = true
            };

            txtTax = new TextBox
            {
                Location = new Point(650, 467),
                Size = new Size(100, 25),
                Text = "0.00"
            };
            txtTax.TextChanged += RecalculateTotals;

            lblSubtotal = new Label
            {
                Text = "Subtotal: $0.00",
                Location = new Point(600, 505),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };

            lblTotal = new Label
            {
                Text = "Total: $0.00",
                Location = new Point(600, 535),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };

            // Notes
            Label lblNotesLabel = new Label
            {
                Text = "Notes:",
                Location = new Point(20, 475),
                AutoSize = true
            };

            txtNotes = new TextBox
            {
                Location = new Point(20, 500),
                Size = new Size(400, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Action Buttons
            btnSave = new Button
            {
                Text = "Save Invoice",
                Location = new Point(20, 600),
                Size = new Size(120, 40),
                BackColor = Color.LightGreen
            };
            btnSave.Click += BtnSave_Click;

            btnExportPdf = new Button
            {
                Text = "Export to PDF",
                Location = new Point(150, 600),
                Size = new Size(120, 40)
            };
            btnExportPdf.Click += BtnExportPdf_Click;

            btnSendEmail = new Button
            {
                Text = "Send Email",
                Location = new Point(280, 600),
                Size = new Size(120, 40),
                BackColor = Color.LightBlue
            };
            btnSendEmail.Click += BtnSendEmail_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(740, 600),
                Size = new Size(120, 40)
            };
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            this.Controls.AddRange(new Control[] {
                lblCustomer, cboCustomer,
                lblIssueDate, dtpIssueDate,
                lblDueDate, dtpDueDate,
                lblItems, dgvItems,
                btnAddItem, btnRemoveItem,
                lblTaxLabel, txtTax,
                lblSubtotal, lblTotal,
                lblNotesLabel, txtNotes,
                btnSave, btnExportPdf, btnSendEmail, btnCancel
            });
        }

        private void LoadCustomers()
        {
            var customers = _customerService.GetAllCustomers();
            cboCustomer.DataSource = customers;
            cboCustomer.DisplayMember = "Name";
            cboCustomer.ValueMember = "Id";
        }

        private void LoadInvoiceData()
        {
            if (_invoice == null) return;

            this.Text = $"Invoice - {_invoice.InvoiceNumber}";
            cboCustomer.SelectedValue = _invoice.CustomerId;
            dtpIssueDate.Value = _invoice.IssueDate;
            dtpDueDate.Value = _invoice.DueDate;
            txtTax.Text = _invoice.Tax.ToString("F2");
            txtNotes.Text = _invoice.Notes ?? "";

            foreach (var item in _invoice.Items)
            {
                dgvItems.Rows.Add(item.Description, item.Quantity, item.UnitPrice, item.LineTotal);
            }

            UpdateTotals();
        }

        private void BtnAddItem_Click(object? sender, EventArgs e)
        {
            dgvItems.Rows.Add("New Item", 1, 0.00, 0.00);
        }

        private void BtnRemoveItem_Click(object? sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                dgvItems.Rows.RemoveAt(dgvItems.SelectedRows[0].Index);
                UpdateTotals();
            }
        }

        private void RecalculateTotals(object? sender, EventArgs e)
        {
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            decimal subtotal = 0;

            foreach (DataGridViewRow row in dgvItems.Rows)
            {
                if (row.Cells["Quantity"].Value != null && row.Cells["UnitPrice"].Value != null)
                {
                    decimal qty = Convert.ToDecimal(row.Cells["Quantity"].Value);
                    decimal price = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                    decimal lineTotal = qty * price;
                    row.Cells["LineTotal"].Value = lineTotal.ToString("F2");
                    subtotal += lineTotal;
                }
            }

            decimal tax = 0;
            if (decimal.TryParse(txtTax.Text, out decimal taxValue))
            {
                tax = taxValue;
            }

            decimal total = subtotal + tax;

            lblSubtotal.Text = $"Subtotal: {subtotal:C}";
            lblTotal.Text = $"Total: {total:C}";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cboCustomer.SelectedValue == null)
                {
                    MessageBox.Show("Please select a customer.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvItems.Rows.Count == 0)
                {
                    MessageBox.Show("Please add at least one line item.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_invoice == null)
                    _invoice = new Invoice();

                _invoice.CustomerId = (int)cboCustomer.SelectedValue;
                _invoice.IssueDate = dtpIssueDate.Value;
                _invoice.DueDate = dtpDueDate.Value;
                _invoice.Tax = decimal.Parse(txtTax.Text);
                _invoice.Notes = txtNotes.Text;

                _invoice.Items.Clear();
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    var item = new InvoiceItem
                    {
                        Description = row.Cells["Description"].Value?.ToString() ?? "",
                        Quantity = Convert.ToDecimal(row.Cells["Quantity"].Value ?? 1),
                        UnitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value ?? 0)
                    };
                    item.CalculateLineTotal();
                    _invoice.Items.Add(item);
                }

                _invoice.RecalculateTotals();

                if (_invoice.Id == 0)
                {
                    _invoiceService.CreateInvoice(_invoice);
                    MessageBox.Show("Invoice created successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _invoiceService.UpdateInvoice(_invoice);
                    MessageBox.Show("Invoice updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving invoice: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportPdf_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_invoice == null || _invoice.Id == 0)
                {
                    MessageBox.Show("Please save the invoice first.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pdfService = new PdfService();
            string pdfPath = pdfService.GenerateInvoicePdf(_invoice);

            MessageBox.Show($"PDF exported successfully!\n\n{pdfPath}", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);                // Open PDF
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSendEmail_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_invoice == null || _invoice.Id == 0)
                {
                    MessageBox.Show("Please save the invoice first.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(_invoice.Customer?.Email))
                {
                    MessageBox.Show("The selected customer does not have an email address.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1. Generate PDF
                var pdfService = new PdfService();
                string pdfPath = pdfService.GenerateInvoicePdf(_invoice);

                // 2. Send Email
                var mailService = new MailService();
                mailService.Configure(
                    _settings.Smtp.Host,
                    _settings.Smtp.Port,
                    _settings.Smtp.UseSsl,
                    _settings.Smtp.Username,
                    _settings.Smtp.Password,
                    _settings.Company.Email,
                    _settings.Company.Name
                );

                bool success = mailService.SendInvoiceEmail(_invoice, pdfPath);

                if (success)
                {
                    MessageBox.Show($"Invoice sent successfully to {_invoice.Customer.Email}!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to send email. Please check your SMTP settings.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending email: {ex.Message}", "Error",
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
