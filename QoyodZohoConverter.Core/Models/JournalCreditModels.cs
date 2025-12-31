using System;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents a raw record of a journal credit applied to an invoice from the Qoyod export.
    /// </summary>
    public class QoyodJournalCreditAppliedInvoice
    {
        public DateTime? Date { get; set; }
        public string JournalNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Represents a raw record of a journal credit applied to a bill from the Qoyod export.
    /// </summary>
    public class QoyodJournalCreditAppliedBill
    {
        public DateTime? Date { get; set; }
        public string JournalNumber { get; set; }
        public DateTime? BillDate { get; set; }
        public string BillNumber { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Represents a formatted record for the Zoho "Journal Credits Applied to Invoices" CSV file.
    /// </summary>
    public class ZohoJournalCreditAppliedInvoice
    {
        public DateTime? Date { get; set; }
        public string JournalNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// Represents a formatted record for the Zoho "Journal Credits Applied to Bills" CSV file.
    /// </summary>
    public class ZohoJournalCreditAppliedBill
    {
        public DateTime? Date { get; set; }
        public string JournalNumber { get; set; }
        public DateTime? BillDate { get; set; }
        public string BillNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
