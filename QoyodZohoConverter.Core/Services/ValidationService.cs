using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Provides services for validating Qoyod data before conversion.
    /// </summary>
    public class ValidationService
    {
        /// <summary>
        /// Finds duplicate invoice numbers from a collection of Qoyod invoice transactions.
        /// </summary>
        /// <param name="invoiceTransactions">A collection of invoice transactions.</param>
        /// <returns>An IEnumerable of strings containing the invoice numbers that are duplicated.</returns>
        public virtual IEnumerable<string> FindDuplicateInvoiceNumbers(IEnumerable<QoyodInvoiceTransaction> invoiceTransactions)
        {
            return invoiceTransactions
                .GroupBy(t => t.InvoiceNumber)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
        }

        /// <summary>
        /// Finds duplicate bill numbers from a collection of Qoyod bill transactions.
        /// </summary>
        /// <param name="billTransactions">A collection of bill transactions.</param>
        /// <returns>An IEnumerable of strings containing the bill numbers that are duplicated.</returns>
        public virtual IEnumerable<string> FindDuplicateBillNumbers(IEnumerable<QoyodBillTransaction> billTransactions)
        {
            return billTransactions
                .GroupBy(t => t.BillNumber)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
        }

        /// <summary>
        /// Validates a collection of applied invoice credits.
        /// </summary>
        /// <returns>A list of string messages describing any validation errors.</returns>
        public virtual List<string> ValidateAppliedInvoiceCredits(IEnumerable<QoyodJournalCreditAppliedInvoice> rows)
        {
            var errors = new List<string>();
            var seenPairs = new HashSet<(string, string)>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.JournalNumber))
                    errors.Add("Validation Error: A record in the Applied Invoice Credits file is missing a JournalNumber.");
                if (string.IsNullOrWhiteSpace(row.InvoiceNumber))
                    errors.Add($"Validation Error: JournalCredit for Journal '{row.JournalNumber}' is missing an InvoiceNumber.");
                if (row.Amount <= 0)
                    errors.Add($"Validation Error: JournalCredit for Journal '{row.JournalNumber}' has a non-positive Amount.");

                var pair = ((row.JournalNumber ?? "").Trim().ToLowerInvariant(), (row.InvoiceNumber ?? "").Trim().ToLowerInvariant());
                if (!seenPairs.Add(pair))
                {
                    errors.Add($"Validation Error: Duplicate applied credit found for Journal '{row.JournalNumber}' and Invoice '{row.InvoiceNumber}'.");
                }
            }
            return errors;
        }

        /// <summary>
        /// Validates a collection of applied bill credits.
        /// </summary>
        public virtual List<string> ValidateAppliedBillCredits(IEnumerable<QoyodJournalCreditAppliedBill> rows)
        {
            var errors = new List<string>();
            var seenPairs = new HashSet<(string, string)>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.JournalNumber))
                    errors.Add("Validation Error: A record in the Applied Bill Credits file is missing a JournalNumber.");
                if (string.IsNullOrWhiteSpace(row.BillNumber))
                    errors.Add($"Validation Error: JournalCredit for Journal '{row.JournalNumber}' is missing a BillNumber.");
                if (row.Amount <= 0)
                    errors.Add($"Validation Error: JournalCredit for Journal '{row.JournalNumber}' has a non-positive Amount.");

                var pair = ((row.JournalNumber ?? "").Trim().ToLowerInvariant(), (row.BillNumber ?? "").Trim().ToLowerInvariant());
                if (!seenPairs.Add(pair))
                {
                    errors.Add($"Validation Error: Duplicate applied credit found for Journal '{row.JournalNumber}' and Bill '{row.BillNumber}'.");
                }
            }
            return errors;
        }
    }
}
