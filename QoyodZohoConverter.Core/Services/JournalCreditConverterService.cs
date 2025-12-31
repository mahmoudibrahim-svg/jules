using QoyodZohoConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Handles the conversion and date defaulting logic for applied journal credits.
    /// </summary>
    public class JournalCreditConverterService
    {
        /// <summary>
        /// Converts raw applied invoice credits to the Zoho format, applying date defaulting.
        /// </summary>
        /// <returns>A tuple containing the converted records and a list of warnings.</returns>
        public (List<ZohoJournalCreditAppliedInvoice> Converted, List<string> Warnings) ConvertAppliedInvoiceCredits(
            IEnumerable<QoyodJournalCreditAppliedInvoice> rawCredits,
            IEnumerable<QoyodJournalLine> allJournalLines)
        {
            var journalDateLookup = BuildJournalDateLookup(allJournalLines);
            var converted = new List<ZohoJournalCreditAppliedInvoice>();
            var warnings = new List<string>();

            foreach (var credit in rawCredits)
            {
                var date = credit.Date;
                if (!date.HasValue)
                {
                    if (journalDateLookup.TryGetValue(credit.JournalNumber, out var journalDate))
                    {
                        date = journalDate;
                    }
                    else
                    {
                        warnings.Add($"Could not find Journal Date for JournalNumber: {credit.JournalNumber}.");
                    }
                }

                converted.Add(new ZohoJournalCreditAppliedInvoice
                {
                    Date = date,
                    JournalNumber = credit.JournalNumber,
                    InvoiceDate = credit.InvoiceDate,
                    InvoiceNumber = credit.InvoiceNumber,
                    Amount = credit.Amount
                });
            }
            return (converted, warnings);
        }

        private Dictionary<string, DateTime> BuildJournalDateLookup(IEnumerable<QoyodJournalLine> allJournalLines)
        {
            return allJournalLines
                .GroupBy(line => line.JournalNumber)
                .ToDictionary(g => g.Key, g => g.First().JournalDate);
        }

        /// <summary>
        /// Converts raw applied bill credits to the Zoho format, applying date defaulting.
        /// </summary>
        public (List<ZohoJournalCreditAppliedBill> Converted, List<string> Warnings) ConvertAppliedBillCredits(
            IEnumerable<QoyodJournalCreditAppliedBill> rawCredits,
            IEnumerable<QoyodJournalLine> allJournalLines)
        {
            var journalDateLookup = BuildJournalDateLookup(allJournalLines);
            var converted = new List<ZohoJournalCreditAppliedBill>();
            var warnings = new List<string>();

            foreach (var credit in rawCredits)
            {
                var date = credit.Date;
                if (!date.HasValue)
                {
                    if (journalDateLookup.TryGetValue(credit.JournalNumber, out var journalDate))
                    {
                        date = journalDate;
                    }
                    else
                    {
                        warnings.Add($"Could not find Journal Date for JournalNumber: {credit.JournalNumber}.");
                    }
                }

                converted.Add(new ZohoJournalCreditAppliedBill
                {
                    Date = date,
                    JournalNumber = credit.JournalNumber,
                    BillDate = credit.BillDate,
                    BillNumber = credit.BillNumber,
                    Amount = credit.Amount
                });
            }
            return (converted, warnings);
        }
    }
}
