using System;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents a single customer payment transaction from the Qoyod AR account statement.
    /// </summary>
    public class QoyodPaymentTransaction
    {
        public DateTime PaymentDate { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; } // The invoice this payment is for
        public decimal Amount { get; set; }
        public string PaymentAccountNumber { get; set; } // The bank/cash account
    }

    /// <summary>
    /// Represents a single customer payment formatted for import into Zoho Books.
    /// The Zoho CSV for payments typically requires the invoice number to apply the payment.
    /// </summary>
    public class ZohoPayment
    {
        public string CustomerName { get; set; }
        public DateTime PaymentDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public string PaymentAccount { get; set; } // Name of the bank/cash account in Zoho
        public string ReferenceNumber { get; set; }
    }
}
