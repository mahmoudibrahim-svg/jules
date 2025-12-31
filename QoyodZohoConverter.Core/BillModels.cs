using System;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents a single vendor bill transaction from the Qoyod account statement.
    /// This is based on the structure observed in 'ZOHO_AP_Account_Transactions.xlsx',
    /// assuming it represents the Qoyod source data for payables.
    /// </summary>
    public class QoyodBillTransaction
    {
        public DateTime BillDate { get; set; }
        public string Description { get; set; }
        public string BillNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string AccountName { get; set; }
        public string VendorName { get; set; } // To be parsed from Description
    }

    /// <summary>
    /// Represents a single vendor bill formatted for import into Zoho Books.
    /// This is a preliminary structure and must be verified with an official sample file.
    /// </summary>
    public class ZohoBill
    {
        public string VendorName { get; set; }
        public string BillNumber { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime DueDate { get; set; }
        public string ItemName { get; set; } // Will be a placeholder
        public string Account { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public string TaxName { get; set; }
        public string CurrencyCode { get; set; }
    }
}
