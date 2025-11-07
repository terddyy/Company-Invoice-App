using InvoiceApp.Services;

namespace InvoiceApp.Forms
{
    public partial class ReportsForm : Form
    {
        private CustomerService _customerService;
        private InvoiceService _invoiceService;
        private ScottPlot.WinForms.FormsPlot chartRevenue = null!;
        private DataGridView dgvTopCustomers = null!;
        private Label lblTotalRevenue = null!;
        private Label lblTotalOutstanding = null!;
        private Label lblOverdueCount = null!;

        public ReportsForm(CustomerService customerService, InvoiceService invoiceService)
        {
            _customerService = customerService;
            _invoiceService = invoiceService;
            InitializeComponent();
            LoadReports();
        }

        private void InitializeComponent()
        {
            this.Text = "Reports & Analytics";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title
            Label lblTitle = new Label
            {
                Text = "Reports & Analytics",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true
            };

            // Summary Panel
            Panel summaryPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(940, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.AliceBlue
            };

            lblTotalRevenue = new Label
            {
                Text = "Total Revenue: $0.00",
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };

            lblTotalOutstanding = new Label
            {
                Text = "Total Outstanding: $0.00",
                Location = new Point(20, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true
            };

            lblOverdueCount = new Label
            {
                Text = "Overdue Invoices: 0",
                Location = new Point(400, 20),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                AutoSize = true
            };

            summaryPanel.Controls.AddRange(new Control[] { lblTotalRevenue, lblTotalOutstanding, lblOverdueCount });

            // Top Customers Grid
            Label lblTopCustomers = new Label
            {
                Text = "Top Customers by Revenue:",
                Location = new Point(20, 180),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };

            dgvTopCustomers = new DataGridView
            {
                Location = new Point(20, 210),
                Size = new Size(450, 400),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Chart
            Label lblChart = new Label
            {
                Text = "Revenue Chart:",
                Location = new Point(490, 180),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true
            };

            chartRevenue = new ScottPlot.WinForms.FormsPlot
            {
                Location = new Point(490, 210),
                Size = new Size(470, 400)
            };

            // Add controls
            this.Controls.AddRange(new Control[] {
                lblTitle, summaryPanel, lblTopCustomers, dgvTopCustomers,
                lblChart, chartRevenue
            });
        }

        private void LoadReports()
        {
            try
            {
                // Load summary statistics
                decimal totalRevenue = _invoiceService.GetTotalRevenue();
                decimal totalOutstanding = _invoiceService.GetTotalOutstanding();
                var overdueInvoices = _invoiceService.GetOverdueInvoices();

                lblTotalRevenue.Text = $"Total Revenue: {totalRevenue:C}";
                lblTotalOutstanding.Text = $"Total Outstanding: {totalOutstanding:C}";
                lblOverdueCount.Text = $"Overdue Invoices: {overdueInvoices.Count}";

                // Load top customers
                var topCustomers = _customerService.GetTopCustomersByRevenue(10);
                
                var displayData = topCustomers.Select(tc => new
                {
                    Customer = tc.Customer.Name,
                    Email = tc.Customer.Email ?? "N/A",
                    Revenue = tc.TotalRevenue.ToString("C")
                }).ToList();

                dgvTopCustomers.DataSource = displayData;

                // Load chart
                LoadRevenueChart(topCustomers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading reports: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRevenueChart(List<(Models.Customer Customer, decimal TotalRevenue)> topCustomers)
        {
            if (topCustomers.Count == 0)
            {
                chartRevenue.Plot.Title("No revenue data available");
                chartRevenue.Refresh();
                return;
            }

            double[] values = topCustomers.Select(x => (double)x.TotalRevenue).ToArray();
            string[] labels = topCustomers.Select(x => x.Customer.Name).ToArray();

            // ScottPlot v5 API
            var bar = chartRevenue.Plot.Add.Bars(values);
            bar.Color = ScottPlot.Color.FromHex("#4682B4");

            chartRevenue.Plot.Title("Top Customers by Revenue");
            chartRevenue.Plot.Axes.Left.Label.Text = "Revenue ($)";
            chartRevenue.Plot.Axes.Bottom.Label.Text = "Customers";
            
            // Set custom tick labels
            double[] positions = Enumerable.Range(0, labels.Length).Select(x => (double)x).ToArray();
            chartRevenue.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(positions, labels);
            chartRevenue.Plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
            chartRevenue.Plot.Axes.Bottom.TickLabelStyle.Alignment = ScottPlot.Alignment.MiddleLeft;

            chartRevenue.Refresh();
        }
    }
}
