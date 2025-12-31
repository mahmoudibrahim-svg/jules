using QoyodZohoConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Handles the conversion of Journal Entries from Qoyod to Zoho format.
    /// </summary>
    public class JournalConverterService
    {
        private readonly MappingService _mappingService;

        public JournalConverterService(MappingService mappingService)
        {
            _mappingService = mappingService;
        }

        /// <summary>
        /// Converts a flat list of Qoyod journal lines into a list of Zoho journal lines.
        /// Also validates that each journal entry is balanced.
        /// </summary>
        public (List<ZohoJournalLine> ConvertedJournals, List<string> Errors) Convert(IEnumerable<QoyodJournalLine> qoyodLines)
        {
            var zohoLines = new List<ZohoJournalLine>();
            var errors = new List<string>();

            // Group lines by JournalNumber to reconstruct the original journal entries
            var journalGroups = qoyodLines.GroupBy(line => line.JournalNumber);

            foreach (var group in journalGroups)
            {
                var totalDebit = group.Sum(line => line.Debit);
                var totalCredit = group.Sum(line => line.Credit);

                // Validation: Ensure the journal entry is balanced
                if (totalDebit != totalCredit)
                {
                    errors.Add($"Journal number {group.Key} is unbalanced. Debit: {totalDebit}, Credit: {totalCredit}.");
                    continue; // Skip this unbalanced journal
                }

                // If balanced, convert each line
                foreach (var line in group)
                {
                    zohoLines.Add(new ZohoJournalLine
                    {
                        JournalDate = line.JournalDate,
                        Notes = line.Notes,
                        ReferenceNumber = line.ReferenceNumber,
                        Description = line.Description,
                        Account = _mappingService.GetMappedAccountType(line.Account), // Apply mapping
                        Debit = line.Debit,
                        Credit = line.Credit,
                        Currency = line.Currency
                    });
                }
            }

            return (zohoLines, errors);
        }
    }
}
