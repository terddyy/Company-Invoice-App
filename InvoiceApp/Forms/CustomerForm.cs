using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.Utils;

namespace InvoiceApp.Forms
{
    public partial class CustomerForm : Form
    {
        private CustomerService _customerService;
        private ListBox lstCustomers;
        private TextBox txtName;
        private TextBox txtAddress;
        private TextBox txtPostcode;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtNotes;
        private Button btnNew;
        private Button btnSave;
        private Button btnDelete;
        private Button btnImport;
        private Customer? _selectedCustomer;

        public CustomerForm(CustomerService customerService)
        {
            _customerService = customerService;
            InitializeComponent();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.Text = "Customer Management";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Customer List
            Label lblList = new Label
            {
                Text = "Customers:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            lstCustomers = new ListBox
            {
                Location = new Point(20, 45),
                Size = new Size(300, 450)
            };
            lstCustomers.SelectedIndexChanged += LstCustomers_SelectedIndexChanged;

            // Detail Panel
            Label lblDetails = new Label
            {
                Text = "Customer Details:",
                Location = new Point(340, 20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };

            Label lblName = new Label { Text = "Name:", Location = new Point(340, 60), AutoSize = true };
            txtName = new TextBox { Location = new Point(450, 57), Size = new Size(400, 25) };

            Label lblAddress = new Label { Text = "Address:", Location = new Point(340, 95), AutoSize = true };
            txtAddress = new TextBox { Location = new Point(450, 92), Size = new Size(400, 25) };

            Label lblPostcode = new Label { Text = "Postcode:", Location = new Point(340, 130), AutoSize = true };
            txtPostcode = new TextBox { Location = new Point(450, 127), Size = new Size(200, 25) };

            Label lblEmail = new Label { Text = "Email:", Location = new Point(340, 165), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(450, 162), Size = new Size(400, 25) };

            Label lblPhone = new Label { Text = "Phone:", Location = new Point(340, 200), AutoSize = true };
            txtPhone = new TextBox { Location = new Point(450, 197), Size = new Size(200, 25) };

            Label lblNotes = new Label { Text = "Notes:", Location = new Point(340, 235), AutoSize = true };
            txtNotes = new TextBox
            {
                Location = new Point(450, 232),
                Size = new Size(400, 80),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            btnNew = new Button
            {
                Text = "New Customer",
                Location = new Point(340, 330),
                Size = new Size(120, 35)
            };
            btnNew.Click += BtnNew_Click;

            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(470, 330),
                Size = new Size(120, 35)
            };
            btnSave.Click += BtnSave_Click;

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(600, 330),
                Size = new Size(120, 35),
                BackColor = Color.LightCoral
            };
            btnDelete.Click += BtnDelete_Click;

            btnImport = new Button
            {
                Text = "Import CSV",
                Location = new Point(730, 330),
                Size = new Size(120, 35)
            };
            btnImport.Click += BtnImport_Click;

            // Add controls
            this.Controls.AddRange(new Control[] {
                lblList, lstCustomers,
                lblDetails, lblName, txtName,
                lblAddress, txtAddress,
                lblPostcode, txtPostcode,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblNotes, txtNotes,
                btnNew, btnSave, btnDelete, btnImport
            });
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                lstCustomers.Items.Clear();
                
                foreach (var customer in customers)
                {
                    lstCustomers.Items.Add(customer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LstCustomers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstCustomers.SelectedItem is Customer customer)
            {
                _selectedCustomer = customer;
                txtName.Text = customer.Name;
                txtAddress.Text = customer.Address ?? "";
                txtPostcode.Text = customer.Postcode ?? "";
                txtEmail.Text = customer.Email ?? "";
                txtPhone.Text = customer.Phone ?? "";
                txtNotes.Text = customer.Notes ?? "";
            }
        }

        private void BtnNew_Click(object? sender, EventArgs e)
        {
            _selectedCustomer = null;
            lstCustomers.ClearSelected();
            txtName.Clear();
            txtAddress.Clear();
            txtPostcode.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtNotes.Clear();
            txtName.Focus();
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Customer name is required.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var customer = _selectedCustomer ?? new Customer();
                customer.Name = txtName.Text.Trim();
                customer.Address = string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim();
                customer.Postcode = string.IsNullOrWhiteSpace(txtPostcode.Text) ? null : txtPostcode.Text.Trim();
                customer.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                customer.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();
                customer.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();

                if (_selectedCustomer == null)
                {
                    _customerService.CreateCustomer(customer);
                    MessageBox.Show("Customer created successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _customerService.UpdateCustomer(customer);
                    MessageBox.Show("Customer updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadCustomers();
                BtnNew_Click(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to delete.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete customer '{_selectedCustomer.Name}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _customerService.DeleteCustomer(_selectedCustomer.Id);
                    MessageBox.Show("Customer deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                    BtnNew_Click(null, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Select CSV file to import";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var customers = CsvImporter.ImportCustomersFromCsv(openFileDialog.FileName);
                        int imported = _customerService.ImportCustomersFromCsv(customers);
                        
                        MessageBox.Show($"Successfully imported {imported} customers!", "Import Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        LoadCustomers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing CSV: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
