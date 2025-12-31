using QoyodZohoConverter.Core.Services;
using QoyodZohoConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

// --- Step 1: Define File Paths ---
// IMPORTANT: Replace the placeholder paths below with the actual paths to your Qoyod export files.
// Example: string invoiceFilePath = @"C:\Users\YourUser\Documents\Qoyod\Invoices.xlsx";

string basePath = @"C:\Path\To\Your\Files"; // <--- REPLACE THIS BASE PATH

string invoiceFilePath = Path.Combine(basePath, "Invoices.xlsx");
string billFilePath = Path.Combine(basePath, "Bills.xlsx");
string appliedInvoiceCreditsFilePath = Path.Combine(basePath, "AppliedInvoiceCredits.xlsx");
string appliedBillCreditsFilePath = Path.Combine(basePath, "AppliedBillCredits.xlsx");
// For journal files, you can list multiple paths.
IEnumerable<string> journalFilePaths = new string[]
{
    Path.Combine(basePath, "Journals1.xlsx"),
    Path.Combine(basePath, "Journals2.xlsx")
};


// --- Step 2: Run the Conversion Dry Run ---

Console.WriteLine("Starting conversion dry run...");

try
{
    var dataReader = new QoyodDataReaderService();
    var validationService = new ValidationService();
    var dryRunService = new DryRunService(validationService, dataReader);

    var result = dryRunService.PerformDryRun(
        invoiceFilePath,
        billFilePath,
        journalFilePaths,
        appliedInvoiceCreditsFilePath,
        appliedBillCreditsFilePath
    );

    // --- Step 3: Print the Report ---
    Console.WriteLine("\n--- Dry Run Report ---");
    Console.WriteLine($"Total Invoices to Convert: {result.TotalInvoicesToConvert}");
    Console.WriteLine($"Total Bills to Convert: {result.TotalBillsToConvert}");
    Console.WriteLine($"Applied Invoice Credits Found: {result.AppliedInvoiceCreditsCount}");
    Console.WriteLine($"Applied Bill Credits Found: {result.AppliedBillCreditsCount}");

    if (result.HasIssues)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n--- ISSUES FOUND ---");

        if (result.ValidationMessages.Any())
        {
            Console.WriteLine("\n[General Validation Messages]");
            foreach (var msg in result.ValidationMessages)
            {
                Console.WriteLine($"- {msg}");
            }
        }

        if (result.MissingJournalDatesForAppliedCredits.Any())
        {
            Console.WriteLine("\n[Warnings: Missing Journal Dates for Applied Credits]");
            Console.WriteLine("The following applied credits are missing a date and could not find a matching journal entry to infer the date from. The output file will have an empty date for these records.");
            foreach (var warning in result.MissingJournalDatesForAppliedCredits)
            {
                Console.WriteLine($"- {warning}");
            }
        }

        if (result.AppliedCreditsErrors.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n[Errors: Applied Credits Validation]");
            foreach (var error in result.AppliedCreditsErrors)
            {
                Console.WriteLine($"- {error}");
            }
        }

        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nNo critical issues found. The data appears to be ready for conversion.");
        Console.ResetColor();
    }

}
catch (FileNotFoundException ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nERROR: A required file was not found.");
    Console.WriteLine($"Please check the path: {ex.FileName}");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nAn unexpected error occurred: {ex.Message}");
    Console.ResetColor();
}

Console.WriteLine("\n--- End of Report ---");
Console.WriteLine("Press any key to exit.");
Console.ReadKey();
