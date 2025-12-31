using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Provides services to perform a "dry run" of the conversion process.
    /// </summary>
    public class DryRunService
    {
        private readonly ValidationService _validationService;
        private readonly QoyodDataReaderService _dataReaderService;

        public DryRunService(ValidationService validationService, QoyodDataReaderService dataReaderService)
        {
            _validationService = validationService;
            _dataReaderService = dataReaderService;
        }

        /// <summary>
        /// Performs a dry run by reading data from files and generating a summary report.
        /// </summary>
        public DryRunResult PerformDryRun(
            string invoiceFilePath,
            string billFilePath,
            IEnumerable<string> journalFilePaths,
            string appliedInvoiceCreditsFilePath,
            string appliedBillCreditsFilePath)
        {
            var result = new DryRunResult();

            // 1. Read Data
            var allInvoices = _dataReaderService.ReadInvoiceTransactions(invoiceFilePath);
            var allBills = _dataReaderService.ReadBillTransactions(billFilePath);
            var allJournalLines = _dataReaderService.ReadJournalEntries(journalFilePaths);
            var appliedInvoiceCredits = _dataReaderService.ReadAppliedInvoiceCredits(appliedInvoiceCreditsFilePath);
            var appliedBillCredits = _dataReaderService.ReadAppliedBillCredits(appliedBillCreditsFilePath);

            // Instantiate converter for credit logic
            var creditConverter = new JournalCreditConverterService();
            var (convertedInvoiceCredits, invoiceCreditWarnings) = creditConverter.ConvertAppliedInvoiceCredits(appliedInvoiceCredits, allJournalLines);
            var (convertedBillCredits, billCreditWarnings) = creditConverter.ConvertAppliedBillCredits(appliedBillCredits, allJournalLines);

            // 2. Perform Validations
            var duplicateInvoices = _validationService.FindDuplicateInvoiceNumbers(allInvoices).ToList();
            if (duplicateInvoices.Any())
            {
                result.ValidationMessages.Add($"Warning: Found {duplicateInvoices.Count} duplicate invoice numbers: {string.Join(", ", duplicateInvoices)}");
            }

            var duplicateBills = _validationService.FindDuplicateBillNumbers(allBills).ToList();
            if (duplicateBills.Any())
            {
                result.ValidationMessages.Add($"Warning: Found {duplicateBills.Count} duplicate bill numbers: {string.Join(", ", duplicateBills)}");
            }

            // Group journals to check for balance issues
            var journalGroups = allJournalLines.GroupBy(j => j.JournalNumber);
            foreach (var group in journalGroups)
            {
                if (group.Sum(j => j.Debit) != group.Sum(j => j.Credit))
                {
                    result.ValidationMessages.Add($"Error: Journal {group.Key} is unbalanced.");
                }
            }

            // Validate applied credits
            var invoiceCreditErrors = _validationService.ValidateAppliedInvoiceCredits(appliedInvoiceCredits);
            result.AppliedCreditsErrors.AddRange(invoiceCreditErrors);

            var billCreditErrors = _validationService.ValidateAppliedBillCredits(appliedBillCredits);
            result.AppliedCreditsErrors.AddRange(billCreditErrors);

            // 3. Populate Summary
            result.TotalInvoicesToConvert = allInvoices.Count();
            result.TotalBillsToConvert = allBills.Count();
            result.AppliedInvoiceCreditsCount = convertedInvoiceCredits.Count();
            result.AppliedBillCreditsCount = convertedBillCredits.Count();

            // The converter service now returns "warnings" for missing dates.
            result.MissingJournalDatesForAppliedCredits.AddRange(invoiceCreditWarnings);
            result.MissingJournalDatesForAppliedCredits.AddRange(billCreditWarnings);

            // Additional summary stats can be added, e.g., total journals
            result.ValidationMessages.Insert(0, $"Found {journalGroups.Count()} journals to process.");


            return result;
        }
    }
}
