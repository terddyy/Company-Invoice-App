using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.Utils;

namespace InvoiceApp.Forms
{
    public partial class InvoiceListForm : Form
    {
        private InvoiceService _invoiceService;
        private CustomerService _customerService;
        private AppSettings _settings;
        private DataGridView dgvInvoices;
        private Button btnNew;
        private Button btnView;
        private Button btnMarkPaid;
        private Button btnDelete;
        private Button btnRefresh;
        private ComboBox cboFilter;

        public InvoiceListForm(InvoiceService invoiceService, CustomerService customerService, AppSettings settings)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _settings = settings;
            InitializeComponent();
            LoadInvoices();
        }

        private void InitializeComponent()
        {
            this.Text = "Invoices";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Filter
            Label lblFilter = new Label
            {
                Text = "Filter:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            cboFilter = new ComboBox
            {
                Location = new Point(70, 17),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboFilter.Items.AddRange(new object[] { "All", "Paid", "Unpaid", "Overdue" });
            cboFilter.SelectedIndex = 0;
            cboFilter.SelectedIndexChanged += CboFilter_SelectedIndexChanged;

            // DataGridView
            dgvInvoices = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(1050, 420),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvInvoices.DoubleClick += DgvInvoices_DoubleClick;

            // Buttons
            btnNew = new Button
            {
                Text = "New Invoice",
                Location = new Point(20, 500),
                Size = new Size(120, 35)
            };
            btnNew.Click += BtnNew_Click;

            btnView = new Button
            {
                Text = "View/Edit",
                Location = new Point(150, 500),
                Size = new Size(120, 35)
            };
            btnView.Click += BtnView_Click;

            btnMarkPaid = new Button
            {
                Text = "Mark as Paid",
                Location = new Point(280, 500),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen
            };
            btnMarkPaid.Click += BtnMarkPaid_Click;

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(410, 500),
                Size = new Size(120, 35),
                BackColor = Color.LightCoral
            };
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(950, 500),
                Size = new Size(120, 35)
            };
            btnRefresh.Click += BtnRefresh_Click;

            // Add controls
            this.Controls.AddRange(new Control[] {
                lblFilter, cboFilter,
                dgvInvoices,
                btnNew, btnView, btnMarkPaid, btnDelete, btnRefresh
            });
        }

        private void LoadInvoices(string filter = "All")
        {
            try
            {
                var invoices = _invoiceService.GetAllInvoices();

                // Apply filter
                switch (filter)
                {
                    case "Paid":
                        invoices = invoices.Where(i => i.Status == "Paid").ToList();
                        break;
                    case "Unpaid":
                        invoices = invoices.Where(i => i.Status == "Unpaid").ToList();
                        break;
                    case "Overdue":
                        invoices = invoices.Where(i => i.IsOverdue()).ToList();
                        break;
                }

                var displayData = invoices.Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    Customer = i.Customer?.Name ?? "Unknown",
                    IssueDate = i.IssueDate.ToString("yyyy-MM-dd"),
                    DueDate = i.DueDate.ToString("yyyy-MM-dd"),
                    Total = i.Total.ToString("C"),
                    i.Status
                }).ToList();

                dgvInvoices.DataSource = displayData;
                dgvInvoices.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading invoices: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            LoadInvoices(cboFilter.SelectedItem?.ToString() ?? "All");
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            var invoiceForm = new InvoiceForm(_invoiceService, _customerService, _settings, null);
            if (invoiceForm.ShowDialog() == DialogResult.OK)
            {
                LoadInvoices(cboFilter.SelectedItem?.ToString() ?? "All");
            }
        }

        private void BtnView_Click(object? sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int invoiceId = (int)dgvInvoices.SelectedRows[0].Cells["Id"].Value;
            var invoice = _invoiceService.GetInvoiceById(invoiceId);

            if (invoice != null)
            {
                var invoiceForm = new InvoiceForm(_invoiceService, _customerService, _settings, invoice);
                if (invoiceForm.ShowDialog() == DialogResult.OK)
                {
                    LoadInvoices(cboFilter.SelectedItem?.ToString() ?? "All");
                }
            }
        }

        private void DgvInvoices_DoubleClick(object? sender, EventArgs e)
        {
            BtnView_Click(sender, e);
        }

        private void BtnMarkPaid_Click(object? sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int invoiceId = (int)dgvInvoices.SelectedRows[0].Cells["Id"].Value;
            string invoiceNumber = dgvInvoices.SelectedRows[0].Cells["InvoiceNumber"].Value.ToString() ?? "";

            var result = MessageBox.Show($"Mark invoice {invoiceNumber} as paid?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _invoiceService.MarkInvoiceAsPaid(invoiceId);
                    MessageBox.Show("Invoice marked as paid!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInvoices(cboFilter.SelectedItem?.ToString() ?? "All");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int invoiceId = (int)dgvInvoices.SelectedRows[0].Cells["Id"].Value;
            string invoiceNumber = dgvInvoices.SelectedRows[0].Cells["InvoiceNumber"].Value.ToString() ?? "";

            var result = MessageBox.Show($"Are you sure you want to delete invoice {invoiceNumber}?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _invoiceService.DeleteInvoice(invoiceId);
                    MessageBox.Show("Invoice deleted!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInvoices(cboFilter.SelectedItem?.ToString() ?? "All");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            _invoiceService.UpdateOverdueStatus();
            LoadInvoices(cboFilter.SelectedItem?.ToString() ?? "All");
        }
    }
}
