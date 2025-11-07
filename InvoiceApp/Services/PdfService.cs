using iText.Kernel.Pdf;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using InvoiceApp.Models;
using InvoiceApp.Utils;
using System.IO;

namespace InvoiceApp.Services
{
    public class PdfService
    {
        private readonly string _outputPath;
        private readonly AppSettings _settings;

        // Professional color scheme
        private static readonly DeviceRgb HeaderBlue = new DeviceRgb(0, 123, 255);
        private static readonly DeviceRgb LightGray = new DeviceRgb(245, 245, 245);
        private static readonly DeviceRgb DarkGray = new DeviceRgb(100, 100, 100);
        private static readonly DeviceRgb White = new DeviceRgb(255, 255, 255);

        public PdfService()
        {
            _outputPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "InvoiceApp",
                "Invoices"
            );

            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }

            _settings = AppSettings.Load();
        }

        public string GenerateInvoicePdf(Invoice invoice, string? companyName = null)
        {
            if (invoice.Customer == null)
            {
                throw new ArgumentException("Invoice must have a customer");
            }

            string fileName = $"{invoice.InvoiceNumber}.pdf";
            string filePath = System.IO.Path.Combine(_outputPath, fileName);

            using (PdfWriter writer = new PdfWriter(filePath))
            using (PdfDocument pdf = new PdfDocument(writer))
            {
                pdf.SetDefaultPageSize(PageSize.A4);
                using (Document document = new Document(pdf))
                {
                    // Set margins
                    document.SetMargins(0, 0, 0, 0);

                    // Add header section with blue background
                    AddHeader(document, invoice);

                    // Add body content with proper margins
                    document.SetMargins(40, 40, 40, 40);

                    // Add company and customer details
                    AddCompanyAndCustomerDetails(document, invoice);

                    // Add items table
                    AddItemsTable(document, invoice);

                    // Add payment details
                    AddPaymentDetails(document, invoice);

                    // Add notes if present
                    AddNotes(document, invoice);

                    // Add footer
                    AddFooter(document);
                }
            }

            return filePath;
        }

        private void AddHeader(Document document, Invoice invoice)
        {
            // Create header table with blue background
            Table headerTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }));
            headerTable.SetWidth(UnitValue.CreatePercentValue(100));
            headerTable.SetBackgroundColor(HeaderBlue);
            headerTable.SetBorder(Border.NO_BORDER);

            // Left side - INVOICE title
            Cell leftCell = new Cell()
                .Add(new Paragraph("INVOICE")
                    .SetFontSize(32)
                    .SetBold()
                    .SetFontColor(White)
                    .SetMarginBottom(0))
                .SetBorder(Border.NO_BORDER)
                .SetPadding(40)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            headerTable.AddCell(leftCell);

            // Right side - Invoice number and date
            Paragraph invoiceInfo = new Paragraph()
                .Add(new Text($"INVOICE #\n")
                    .SetFontSize(10)
                    .SetBold())
                .Add(new Text($"{invoice.InvoiceNumber}\n\n")
                    .SetFontSize(14)
                    .SetBold())
                .Add(new Text("INVOICE DATE\n")
                    .SetFontSize(10)
                    .SetBold())
                .Add(new Text($"{invoice.IssueDate:dd/MM/yyyy}")
                    .SetFontSize(12));

            Cell rightCell = new Cell()
                .Add(invoiceInfo)
                .SetBorder(Border.NO_BORDER)
                .SetPadding(40)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetFontColor(White)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE);
            headerTable.AddCell(rightCell);

            document.Add(headerTable);
        }

        private void AddCompanyAndCustomerDetails(Document document, Invoice invoice)
        {
            // Create table for FROM and BILL TO sections
            Table detailsTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1 }));
            detailsTable.SetWidth(UnitValue.CreatePercentValue(100));
            detailsTable.SetMarginTop(30);
            detailsTable.SetBorder(Border.NO_BORDER);

            // FROM section
            Paragraph fromPara = new Paragraph()
                .Add(new Text("FROM\n")
                    .SetFontSize(10)
                    .SetBold()
                    .SetFontColor(DarkGray))
                .Add(new Text($"{_settings.Company.Name}\n")
                    .SetFontSize(11)
                    .SetBold());

            if (!string.IsNullOrEmpty(_settings.Company.Address))
                fromPara.Add(new Text($"{_settings.Company.Address},\n").SetFontSize(10));
            
            if (!string.IsNullOrEmpty(_settings.Company.City))
                fromPara.Add(new Text($"{_settings.Company.City},\n").SetFontSize(10));
            
            if (!string.IsNullOrEmpty(_settings.Company.PostCode))
                fromPara.Add(new Text($"{_settings.Company.PostCode}").SetFontSize(10));
            
            if (!string.IsNullOrEmpty(_settings.Company.Country))
                fromPara.Add(new Text($",\n{_settings.Company.Country}").SetFontSize(10));

            Cell fromCell = new Cell()
                .Add(fromPara)
                .SetBorder(Border.NO_BORDER)
                .SetPaddingRight(20);
            detailsTable.AddCell(fromCell);

            // BILL TO section
            Paragraph billToPara = new Paragraph()
                .Add(new Text("BILL TO\n")
                    .SetFontSize(10)
                    .SetBold()
                    .SetFontColor(DarkGray))
                .Add(new Text($"{invoice.Customer!.Name}\n")
                    .SetFontSize(11)
                    .SetBold());

            if (!string.IsNullOrEmpty(invoice.Customer.Address))
            {
                string[] addressLines = invoice.Customer.Address.Split(',');
                foreach (var line in addressLines)
                {
                    billToPara.Add(new Text($"{line.Trim()},\n").SetFontSize(10));
                }
            }
            
            if (!string.IsNullOrEmpty(invoice.Customer.Postcode))
                billToPara.Add(new Text($"{invoice.Customer.Postcode}").SetFontSize(10));

            Cell billToCell = new Cell()
                .Add(billToPara)
                .SetBorder(Border.NO_BORDER)
                .SetPaddingRight(20);
            detailsTable.AddCell(billToCell);

            // Invoice total (top right)
            Paragraph totalPara = new Paragraph()
                .Add(new Text("INVOICE TOTAL\n")
                    .SetFontSize(10)
                    .SetBold()
                    .SetFontColor(DarkGray))
                .Add(new Text($"£{invoice.Total:N2}")
                    .SetFontSize(18)
                    .SetBold()
                    .SetFontColor(HeaderBlue));

            Cell totalCell = new Cell()
                .Add(totalPara)
                .SetBorder(Border.NO_BORDER)
                .SetTextAlignment(TextAlignment.RIGHT);
            detailsTable.AddCell(totalCell);

            document.Add(detailsTable);
        }

        private void AddItemsTable(Document document, Invoice invoice)
        {
            // Add spacing
            document.Add(new Paragraph().SetMarginTop(20));

            // Create items table
            Table itemsTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1 }));
            itemsTable.SetWidth(UnitValue.CreatePercentValue(100));

            // Header row with blue background
            Cell descHeaderCell = new Cell()
                .Add(new Paragraph("DESCRIPTION")
                    .SetFontSize(10)
                    .SetBold()
                    .SetFontColor(White))
                .SetBackgroundColor(HeaderBlue)
                .SetBorder(Border.NO_BORDER)
                .SetPadding(10)
                .SetTextAlignment(TextAlignment.LEFT);
            itemsTable.AddHeaderCell(descHeaderCell);

            Cell amountHeaderCell = new Cell()
                .Add(new Paragraph("AMOUNT")
                    .SetFontSize(10)
                    .SetBold()
                    .SetFontColor(White))
                .SetBackgroundColor(HeaderBlue)
                .SetBorder(Border.NO_BORDER)
                .SetPadding(10)
                .SetTextAlignment(TextAlignment.RIGHT);
            itemsTable.AddHeaderCell(amountHeaderCell);

            // Add items
            bool alternateRow = false;
            foreach (var item in invoice.Items)
            {
                DeviceRgb bgColor = alternateRow ? LightGray : White;

                // Description cell
                Cell descCell = new Cell()
                    .Add(new Paragraph(item.Description)
                        .SetFontSize(10))
                    .SetBackgroundColor(bgColor)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(10)
                    .SetBorderBottom(new SolidBorder(new DeviceRgb(230, 230, 230), 1));
                itemsTable.AddCell(descCell);

                // Amount cell
                Cell amountCell = new Cell()
                    .Add(new Paragraph($"{item.LineTotal:N2}")
                        .SetFontSize(10))
                    .SetBackgroundColor(bgColor)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(10)
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorderBottom(new SolidBorder(new DeviceRgb(230, 230, 230), 1));
                itemsTable.AddCell(amountCell);

                alternateRow = !alternateRow;
            }

            document.Add(itemsTable);

            // Add total row
            Table totalTable = new Table(UnitValue.CreatePercentArray(new float[] { 3, 1 }));
            totalTable.SetWidth(UnitValue.CreatePercentValue(100));
            totalTable.SetMarginTop(10);

            Cell totalLabelCell = new Cell()
                .Add(new Paragraph("TOTAL")
                    .SetFontSize(12)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER)
                .SetPadding(10)
                .SetBackgroundColor(LightGray);
            totalTable.AddCell(totalLabelCell);

            Cell totalAmountCell = new Cell()
                .Add(new Paragraph($"£{invoice.Total:N2}")
                    .SetFontSize(12)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER)
                .SetPadding(10)
                .SetBackgroundColor(LightGray);
            totalTable.AddCell(totalAmountCell);

            document.Add(totalTable);
        }

        private void AddPaymentDetails(Document document, Invoice invoice)
        {
            // Calculate days
            int daysDiff = (invoice.DueDate - invoice.IssueDate).Days;
            
            // Payment details section
            document.Add(new Paragraph().SetMarginTop(30));

            Paragraph paymentHeader = new Paragraph($"{daysDiff} DAYS")
                .SetFontSize(11)
                .SetBold()
                .SetMarginBottom(10);
            document.Add(paymentHeader);

            // Bank details
            Paragraph paymentDetails = new Paragraph()
                .SetFontSize(9)
                .SetFontColor(DarkGray);

            paymentDetails.Add(new Text("Payment Details\n").SetBold());

            if (!string.IsNullOrEmpty(_settings.Company.BankTitle))
                paymentDetails.Add(new Text($"{_settings.Company.BankTitle}\n"));
            else if (!string.IsNullOrEmpty(_settings.Company.Name))
                paymentDetails.Add(new Text($"{_settings.Company.Name}\n"));

            if (!string.IsNullOrEmpty(_settings.Company.BankName))
                paymentDetails.Add(new Text($"{_settings.Company.BankName}\n"));

            if (!string.IsNullOrEmpty(_settings.Company.SortCode))
                paymentDetails.Add(new Text($"Sort Code {_settings.Company.SortCode}\n"));

            if (!string.IsNullOrEmpty(_settings.Company.AccountNumber))
                paymentDetails.Add(new Text($"Account Number {_settings.Company.AccountNumber}"));

            document.Add(paymentDetails);
        }

        private void AddNotes(Document document, Invoice invoice)
        {
            // Only add notes section if there are notes
            if (!string.IsNullOrEmpty(invoice.Notes))
            {
                // Add spacing
                document.Add(new Paragraph().SetMarginTop(20));

                // Notes header
                Paragraph notesHeader = new Paragraph("Notes")
                    .SetFontSize(11)
                    .SetBold()
                    .SetMarginBottom(5);
                document.Add(notesHeader);

                // Notes content
                Paragraph notesContent = new Paragraph(invoice.Notes)
                    .SetFontSize(9)
                    .SetFontColor(DarkGray)
                    .SetMarginBottom(10);
                document.Add(notesContent);
            }
        }

        private void AddFooter(Document document)
        {
            document.Add(new Paragraph().SetMarginTop(30));
            
            Paragraph footer = new Paragraph("Thank You For Your Business")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(DarkGray)
                .SetItalic();
            
            document.Add(footer);
        }

        public string GetOutputPath()
        {
            return _outputPath;
        }
    }
}
