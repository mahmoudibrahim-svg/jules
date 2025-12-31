using System;
using System.Collections.Generic;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents a single line item within a Qoyod journal entry export.
    /// A complete journal entry consists of multiple lines with the same JournalNumber.
    /// </summary>
    public class QoyodJournalLine
    {
        public string JournalNumber { get; set; }
        public DateTime JournalDate { get; set; }
        public string Notes { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Account { get; set; }
        public string Currency { get; set; }
    }

    /// <summary>
    // Represents a single line item formatted for a Zoho Books journal import.
    /// A complete journal entry will be represented by multiple rows in the CSV file.
    /// </summary>
    public class ZohoJournalLine
    {
        public DateTime JournalDate { get; set; }
        public string Notes { get; set; }
        public string ReferenceNumber { get; set; }
        public string Account { get; set; } // This will be the mapped account name
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
    }
}
