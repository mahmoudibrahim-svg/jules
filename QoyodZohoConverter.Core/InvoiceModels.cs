using System;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents a single sales invoice transaction from the Qoyod account statement.
    /// Based on 'A_Source_Data_Contract_Qoyod.md' for Sales Invoices.
    /// </summary>
    public class QoyodInvoiceTransaction
    {
        public DateTime InvoiceDate { get; set; }
        public string Description { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string AccountName { get; set; }
        public string CustomerName { get; set; } // To be parsed from Description
    }

    /// <summary>
    /// Represents a single sales invoice formatted for import into Zoho Books.
    /// Based on the preliminary structure in 'B_Target_Import_Contract_Zoho.md'.
    /// </summary>
    public class ZohoInvoice
    {
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public string ItemName { get; set; } // Will be a placeholder
        public string Account { get; set; }
        public int Quantity { get; set; } // Will be hardcoded to 1
        public decimal Rate { get; set; } // Will be the TotalAmount from Qoyod
        public string TaxName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
