using InvoiceApp.Services;
using InvoiceApp.Utils;

namespace InvoiceApp.Forms
{
    public partial class MainForm : Form
    {
        private DatabaseService _db = null!;
        private CustomerService _customerService = null!;
        private InvoiceService _invoiceService = null!;
        private AppSettings _settings = null!;

        private ScottPlot.WinForms.FormsPlot chartTopCustomers = null!;
        private Label lblTotalRevenue = null!;
        private Label lblOutstanding = null!;
        private Label lblOverdue = null!;
        private MenuStrip mainMenu = null!;
        private ToolStripMenuItem customersToolStripMenuItem = null!;
        private ToolStripMenuItem invoicesToolStripMenuItem = null!;
        private ToolStripMenuItem reportsToolStripMenuItem = null!;
        private ToolStripMenuItem settingsToolStripMenuItem = null!;

        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.Text = "InvoiceApp - Dashboard";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Main Menu
            mainMenu = new MenuStrip();
            customersToolStripMenuItem = new ToolStripMenuItem("Customers");
            customersToolStripMenuItem.Click += CustomersMenuItem_Click;

            invoicesToolStripMenuItem = new ToolStripMenuItem("Invoices");
            invoicesToolStripMenuItem.Click += InvoicesMenuItem_Click;

            reportsToolStripMenuItem = new ToolStripMenuItem("Reports");
            reportsToolStripMenuItem.Click += ReportsMenuItem_Click;

            settingsToolStripMenuItem = new ToolStripMenuItem("Settings");
            settingsToolStripMenuItem.Click += SettingsMenuItem_Click;

            mainMenu.Items.AddRange(new ToolStripItem[] {
                customersToolStripMenuItem,
                invoicesToolStripMenuItem,
                reportsToolStripMenuItem,
                settingsToolStripMenuItem
            });

            this.MainMenuStrip = mainMenu;
            this.Controls.Add(mainMenu);

            // Dashboard Panel
            Panel dashboardPanel = new Panel
            {
                Location = new Point(20, 50),
                Size = new Size(940, 600),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Title
            Label lblDashboard = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            dashboardPanel.Controls.Add(lblDashboard);

            // Stats Panel
            Panel statsPanel = new Panel
            {
                Location = new Point(10, 60),
                Size = new Size(920, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightBlue
            };

            lblTotalRevenue = new Label
            {
                Text = "Total Revenue: $0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblOutstanding = new Label
            {
                Text = "Outstanding: $0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 40),
                AutoSize = true
            };

            lblOverdue = new Label
            {
                Text = "Overdue Invoices: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(350, 15),
                AutoSize = true,
                ForeColor = Color.DarkRed
            };

            statsPanel.Controls.AddRange(new Control[] { lblTotalRevenue, lblOutstanding, lblOverdue });
            dashboardPanel.Controls.Add(statsPanel);

            // Chart
            chartTopCustomers = new ScottPlot.WinForms.FormsPlot
            {
                Location = new Point(10, 160),
                Size = new Size(920, 420)
            };
            dashboardPanel.Controls.Add(chartTopCustomers);

            this.Controls.Add(dashboardPanel);
        }

        private void InitializeServices()
        {
            _db = new DatabaseService();
            _customerService = new CustomerService(_db);
            _invoiceService = new InvoiceService(_db, _customerService);
            _settings = AppSettings.Load();
        }

        private void LoadDashboard()
        {
            try
            {
                // Update statistics
                decimal totalRevenue = _invoiceService.GetTotalRevenue();
                decimal outstanding = _invoiceService.GetTotalOutstanding();
                var overdueInvoices = _invoiceService.GetOverdueInvoices();

                lblTotalRevenue.Text = $"Total Revenue: {totalRevenue:C}";
                lblOutstanding.Text = $"Outstanding: {outstanding:C}";
                lblOverdue.Text = $"Overdue Invoices: {overdueInvoices.Count}";

                // Load top customers chart
                LoadTopCustomersChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTopCustomersChart()
        {
            var topCustomers = _customerService.GetTopCustomersByRevenue(10);

            if (topCustomers.Count == 0)
            {
                chartTopCustomers.Plot.Title("No customer data yet");
                chartTopCustomers.Refresh();
                return;
            }

            double[] values = topCustomers.Select(x => (double)x.TotalRevenue).ToArray();
            string[] labels = topCustomers.Select(x => x.Customer.Name).ToArray();

            // ScottPlot v5 API
            var barPlot = chartTopCustomers.Plot.Add.Bars(values);
            chartTopCustomers.Plot.Axes.Left.Label.Text = "Revenue ($)";
            chartTopCustomers.Plot.Axes.Bottom.Label.Text = "Customers";
            chartTopCustomers.Plot.Title("Top 10 Customers by Revenue");
            
            // Set custom tick labels
            double[] positions = Enumerable.Range(0, labels.Length).Select(x => (double)x).ToArray();
            chartTopCustomers.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(positions, labels);
            chartTopCustomers.Plot.Axes.Bottom.TickLabelStyle.Rotation = 45;
            chartTopCustomers.Plot.Axes.Bottom.TickLabelStyle.Alignment = ScottPlot.Alignment.MiddleLeft;

            chartTopCustomers.Refresh();
        }

        private void CustomersMenuItem_Click(object? sender, EventArgs e)
        {
            var customerForm = new CustomerForm(_customerService);
            customerForm.ShowDialog();
            LoadDashboard(); // Refresh dashboard after closing
        }

        private void InvoicesMenuItem_Click(object? sender, EventArgs e)
        {
            var invoiceListForm = new InvoiceListForm(_invoiceService, _customerService, _settings);
            invoiceListForm.ShowDialog();
            LoadDashboard(); // Refresh dashboard after closing
        }

        private void ReportsMenuItem_Click(object? sender, EventArgs e)
        {
            var reportsForm = new ReportsForm(_customerService, _invoiceService);
            reportsForm.ShowDialog();
        }

        private void SettingsMenuItem_Click(object? sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(_settings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                _settings = AppSettings.Load(); // Reload settings
            }
        }
    }
}
