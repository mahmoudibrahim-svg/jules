using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Provides services to convert Qoyod bill-related transactions into Zoho Bill format.
    /// </summary>
    public class BillConverterService
    {
        /// <summary>
        /// Converts a list of Qoyod AP transactions into a list of Zoho Bills.
        /// </summary>
        public IEnumerable<ZohoBill> ConvertToZohoBills(IEnumerable<QoyodBillTransaction> qoyodTransactions, string placeholderItemName)
        {
            var zohoBills = new List<ZohoBill>();

            // We will assume that the source data for this function is pre-filtered to be bills.
            // A more robust implementation would filter by a 'Transaction Type' field if available.
            foreach (var trx in qoyodTransactions)
            {
                var zohoBill = new ZohoBill
                {
                    BillNumber = trx.BillNumber,
                    BillDate = trx.BillDate,
                    DueDate = trx.BillDate, // Set DueDate same as BillDate as per spec
                    VendorName = ParseVendorName(trx.Description), // Helper method to extract vendor name
                    Account = trx.AccountName,

                    // Apply single-line workaround
                    ItemName = placeholderItemName,
                    Quantity = 1,
                    Rate = trx.TotalAmount,

                    TaxName = null,
                    CurrencyCode = "SAR" // Assuming SAR as default
                };
                zohoBills.Add(zohoBill);
            }

            return zohoBills;
        }

        /// <summary>
        /// Parses the vendor name from the Qoyod description field.
        /// </summary>
        private string ParseVendorName(string description)
        {
            // This is a placeholder implementation. The actual parsing logic will depend
            // on the consistent patterns in the real data.
            // For now, we will return the description as is, assuming it's the vendor.
            return description.Trim();
        }
    }
}
