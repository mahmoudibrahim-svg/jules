using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Provides services to convert Qoyod invoice-related transactions into Zoho Invoice format.
    /// </summary>
    public class InvoiceConverterService
    {
        /// <summary>
        /// Converts a list of Qoyod account statement lines into a list of ZohoInvoices.
        /// </summary>
        /// <param name="qoyodTransactions">The raw list of transactions from a Qoyod revenue account statement.</param>
        /// <param name="placeholderItemName">The default item name to use for the single invoice line.</param>
        /// <returns>A list of ZohoInvoice objects ready for CSV generation.</returns>
        public IEnumerable<ZohoInvoice> ConvertToZohoInvoices(IEnumerable<QoyodInvoiceTransaction> qoyodTransactions, string placeholderItemName)
        {
            var zohoInvoices = new List<ZohoInvoice>();

            // As per Rule 2.1, filter only for transactions that are sales invoices.
            // This assumes the source data might contain other transaction types like credit notes.
            var salesInvoiceTransactions = qoyodTransactions
                .Where(t => t.Description.Contains("فاتورة مبيعات")); // This logic will be refined based on actual data.

            foreach (var trx in salesInvoiceTransactions)
            {
                var zohoInvoice = new ZohoInvoice
                {
                    // Rule 2.2: Map fields and apply workarounds
                    InvoiceNumber = trx.InvoiceNumber,
                    InvoiceDate = trx.InvoiceDate,
                    DueDate = trx.InvoiceDate, // Set DueDate same as InvoiceDate as per spec
                    CustomerName = ParseCustomerName(trx.Description), // Helper method to extract customer name
                    Account = trx.AccountName,

                    // Rule 2.2 Workaround for single-line invoices
                    ItemName = placeholderItemName,
                    Quantity = 1,
                    Rate = trx.TotalAmount,

                    // Tax and Currency are not available in the source file
                    TaxName = null,
                    CurrencyCode = "SAR" // Assuming SAR as default, could be configurable
                };
                zohoInvoices.Add(zohoInvoice);
            }

            return zohoInvoices;
        }

        /// <summary>
        /// Parses the customer name from the Qoyod description field.
        /// This is a basic implementation and may need to be made more robust.
        /// </summary>
        private string ParseCustomerName(string description)
        {
            // Assumes format "Customer Name - Other Details"
            var parts = description.Split('-');
            if (parts.Length > 0)
            {
                return parts[0].Trim();
            }
            return "Unknown Customer";
        }
    }
}
