using System.Collections.Generic;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents the result of a pre-conversion dry run.
    /// Contains a summary of the operations and a list of any validation issues found.
    /// </summary>
    public class DryRunResult
    {
        public int TotalInvoicesToConvert { get; set; }
        public int TotalBillsToConvert { get; set; }
        public int AppliedInvoiceCreditsCount { get; set; }
        public int AppliedBillCreditsCount { get; set; }
        public List<string> MissingJournalDatesForAppliedCredits { get; } = new List<string>();
        public List<string> AppliedCreditsErrors { get; } = new List<string>();
        public List<string> ValidationMessages { get; } = new List<string>();

        public bool HasIssues => ValidationMessages.Count > 0 || AppliedCreditsErrors.Count > 0;
    }
}
