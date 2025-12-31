using CsvHelper;
using CsvHelper.Configuration;
using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Service for writing data to Zoho-compliant CSV files.
    /// </summary>
    public class ZohoCsvWriterService
    {
        /// <summary>
        /// Writes a list of ZohoAccount objects to a CSV file.
        /// </summary>
        public void WriteChartOfAccounts(string filePath, IEnumerable<ZohoAccount> accounts)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoAccountMap>();
                csv.WriteRecords(accounts);
            }
        }

        /// <summary>
        /// Writes a list of ZohoInvoice objects to a CSV file.
        /// </summary>
        public void WriteInvoices(string filePath, IEnumerable<ZohoInvoice> invoices)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoInvoiceMap>();
                csv.WriteRecords(invoices);
            }
        }

        /// <summary>
        /// Writes a list of ZohoBill objects to a CSV file.
        /// </summary>
        public void WriteBills(string filePath, IEnumerable<ZohoBill> bills)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoBillMap>();
                csv.WriteRecords(bills);
            }
        }

        /// <summary>
        /// Writes a list of ZohoJournalLine objects to a CSV file.
        /// </summary>
        public void WriteJournalEntries(string filePath, IEnumerable<ZohoJournalLine> journalLines)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoJournalLineMap>();
                csv.WriteRecords(journalLines);
            }
        }

        /// <summary>
        /// Writes a list of ZohoJournalCreditAppliedInvoice objects to a CSV file.
        /// </summary>
        public void WriteJournalCreditsAppliedInvoices(string filePath, IEnumerable<ZohoJournalCreditAppliedInvoice> rows)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoJournalCreditAppliedInvoiceMap>();
                csv.WriteRecords(rows);
            }
        }

        /// <summary>
        /// Writes a list of ZohoJournalCreditAppliedBill objects to a CSV file.
        /// </summary>
        public void WriteJournalCreditsAppliedBills(string filePath, IEnumerable<ZohoJournalCreditAppliedBill> rows)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoJournalCreditAppliedBillMap>();
                csv.WriteRecords(rows);
            }
        }

        /// <summary>
        /// Writes a list of ZohoPayment objects to a CSV file.
        /// </summary>
        public void WritePayments(string filePath, IEnumerable<ZohoPayment> payments)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (args) => true
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.Context.RegisterClassMap<ZohoPaymentMap>();
                csv.WriteRecords(payments);
            }
        }
    }

    #region CsvClassMaps

    public sealed class ZohoAccountMap : ClassMap<ZohoAccount>
    {
        public ZohoAccountMap()
        {
            Map(m => m.AccountName).Name("Account Name");
            Map(m => m.AccountType).Name("Account Type");
            Map(m => m.AccountCode).Name("Account Code");
            Map(m => m.Description).Name("Description");
            Map(m => m.ParentAccount).Name("Parent Account");
        }
    }

    public sealed class ZohoInvoiceMap : ClassMap<ZohoInvoice>
    {
        public ZohoInvoiceMap()
        {
            Map(m => m.CustomerName).Name("Customer Name");
            Map(m => m.InvoiceNumber).Name("Invoice Number");
            Map(m => m.InvoiceDate).Name("Invoice Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.DueDate).Name("Due Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.ItemName).Name("Item Name");
            Map(m => m.Account).Name("Account");
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.Rate).Name("Rate");
            Map(m => m.TaxName).Name("Tax Name");
            Map(m => m.CurrencyCode).Name("Currency Code");
        }
    }

    public sealed class ZohoBillMap : ClassMap<ZohoBill>
    {
        public ZohoBillMap()
        {
            Map(m => m.VendorName).Name("Vendor Name");
            Map(m => m.BillNumber).Name("Bill Number");
            Map(m => m.BillDate).Name("Bill Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.DueDate).Name("Due Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.ItemName).Name("Item Name");
            Map(m => m.Account).Name("Account");
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.Rate).Name("Rate");
            Map(m => m.TaxName).Name("Tax Name");
            Map(m => m.CurrencyCode).Name("Currency Code");
        }
    }

    public sealed class ZohoJournalLineMap : ClassMap<ZohoJournalLine>
    {
        public ZohoJournalLineMap()
        {
            Map(m => m.JournalDate).Name("Journal Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.Notes).Name("Notes");
            Map(m => m.ReferenceNumber).Name("Reference Number");
            Map(m => m.Account).Name("Account");
            Map(m => m.Debit).Name("Debit");
            Map(m => m.Credit).Name("Credit");
            Map(m => m.Description).Name("Description");
            Map(m => m.Currency).Name("Currency");
        }
    }

    public sealed class ZohoPaymentMap : ClassMap<ZohoPayment>
    {
        public ZohoPaymentMap()
        {
            Map(m => m.CustomerName).Name("Customer Name");
            Map(m => m.PaymentDate).Name("Payment Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.InvoiceNumber).Name("Invoice Number");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.PaymentAccount).Name("Payment Account");
            Map(m => m.ReferenceNumber).Name("Reference#");
        }
    }

    public sealed class ZohoJournalCreditAppliedInvoiceMap : ClassMap<ZohoJournalCreditAppliedInvoice>
    {
        public ZohoJournalCreditAppliedInvoiceMap()
        {
            Map(m => m.Date).Name("Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.JournalNumber).Name("Journal Number");
            Map(m => m.InvoiceDate).Name("Invoice Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.InvoiceNumber).Name("Invoice Number");
            Map(m => m.Amount).Name("Amount");
        }
    }

    public sealed class ZohoJournalCreditAppliedBillMap : ClassMap<ZohoJournalCreditAppliedBill>
    {
        public ZohoJournalCreditAppliedBillMap()
        {
            Map(m => m.Date).Name("Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.JournalNumber).Name("Journal Number");
            Map(m => m.BillDate).Name("Bill Date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.BillNumber).Name("Bill Number");
            Map(m => m.Amount).Name("Amount");
        }
    }

    #endregion
}
