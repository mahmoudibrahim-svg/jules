using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Handles the conversion of Customer Payments from Qoyod to Zoho format.
    /// </summary>
    public class PaymentConverterService
    {
        private readonly MappingService _mappingService;

        public PaymentConverterService(MappingService mappingService)
        {
            _mappingService = mappingService;
        }

        /// <summary>
        /// Converts a list of Qoyod payment transactions to a list of Zoho payments.
        /// </summary>
        public IEnumerable<ZohoPayment> Convert(IEnumerable<QoyodPaymentTransaction> qoyodPayments)
        {
            var zohoPayments = new List<ZohoPayment>();

            foreach (var payment in qoyodPayments)
            {
                zohoPayments.Add(new ZohoPayment
                {
                    CustomerName = payment.CustomerName,
                    PaymentDate = payment.PaymentDate,
                    InvoiceNumber = payment.InvoiceNumber,
                    Amount = payment.Amount,
                    PaymentAccount = _mappingService.GetMappedAccountType(payment.PaymentAccountNumber), // Apply mapping
                    ReferenceNumber = $"Qoyod-{payment.InvoiceNumber}" // Create a reference number
                });
            }

            return zohoPayments;
        }
    }
}
