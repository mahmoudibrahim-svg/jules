# Qoyod to Zoho Books Conversion Tool - User & Developer Guide

## 1. Introduction

This document provides a comprehensive guide to the **Qoyod to Zoho Books Conversion Core Library**. This library is a complete, tested, and robust backend engine designed to accurately migrate your accounting data from Qoyod export files to import-ready CSV files for Zoho Books.

**IMPORTANT NOTE:** This is a **backend library**, not a standalone application. A graphical user interface (GUI) is required to use this library. This guide explains the intended workflow for the accountant and provides the necessary technical details for a developer to integrate the library into a new or existing UI.

## 2. The Accountant's Workflow

This is the step-by-step process a user will follow in the final application.

### Step 1: Prepare Your Data

Export the following `.xlsx` files from Qoyod and place them in a single folder:

*   **Chart of Accounts:** `QOYOD_Chart_of_Accounts.xlsx`
*   **Journal Entries:** `Journal00.xlsx`, `Journal01.xlsx`, etc.
*   **Revenue Account Statement:** `QOYOD_Account_Statement2.xlsx` (source for Invoices)
*   **AP Account Statement:** `ZOHO_AP_Account_Transactions.xlsx` (source for Bills)

### Step 2: Configure Mappings (One-Time Setup)

The application will require a simple configuration file (e.g., `mapping.json`) to translate your Qoyod accounts.

*   **Chart of Accounts Mapping:** You must map every unique Qoyod `Account Type` to a valid Zoho `Account Type`.
    *   *Example:* Map `"النقدية ومافي حكمها"` from Qoyod to `"Cash"` in Zoho.

### Step 3: Perform a "Dry Run"

This is a crucial preview step. The application will use the core library to:
1.  Read all your source files from the specified folder.
2.  Perform all validations (unbalanced journals, duplicate invoices, etc.).
3.  Generate a summary report showing:
    *   How many Accounts, Invoices, Bills, and Journals were found.
    *   A list of any errors or warnings (e.g., "Error: Journal JR02 is unbalanced.").

### Step 4: Generate Zoho Import Files

After a successful dry run, the application will use the library to generate the final CSV files in a local output folder.

### Step 5: Import into Zoho Books & Reconcile

Import the generated CSV files into Zoho Books in the correct order and use the Reconciliation Checklist below to verify the migration.

---
## 3. The Developer's Guide: Integrating the Core Library

The backend logic is in the `QoyodZohoConverter.Core` library. To build the application, create a UI project (e.g., WPF, WinUI) and reference this core project.

### Full Usage Workflow:

```csharp
// 1. Instantiate all services (Dependency Injection is recommended)
var dataReader = new QoyodDataReaderService();
var validationService = new ValidationService();
var mappingService = new MappingService();
var csvWriter = new ZohoCsvWriterService();

// 2. Load User-Defined Mappings
var accountMappings = new Dictionary<string, string> { { "النقدية ومافي حكمها", "Cash" }, /* ... more mappings */ };
mappingService.LoadAccountTypeMapping(accountMappings);

// 3. Perform the Dry Run and Display Results
var dryRunService = new DryRunService(validationService, dataReader);
var dryRunResult = dryRunService.PerformDryRun("path/to/invoices.xlsx", "path/to/bills.xlsx", new[] { "path/to/journals.xlsx" });

if (dryRunResult.HasIssues) {
    // Display dryRunResult.ValidationMessages to the user and stop.
    return;
}

// 4. If Dry Run is clean, proceed with full conversion
// Read
var qoyodAccounts = dataReader.ReadChartOfAccounts("path/to/coa.xlsx");
var qoyodInvoices = dataReader.ReadInvoiceTransactions("path/to/invoices.xlsx");
// ... read other files

// Convert
var coaConverter = new ChartOfAccountsConverterService(mappingService);
var zohoAccounts = coaConverter.Convert(qoyodAccounts);

var invoiceConverter = new InvoiceConverterService(); // Doesn't need mapping service directly
var zohoInvoices = invoiceConverter.ConvertToZohoInvoices(qoyodInvoices, "Placeholder Item");
// ... convert other data types

// 5. Write the final CSV files
csvWriter.WriteChartOfAccounts("output/Zoho_Accounts.csv", zohoAccounts);
csvWriter.WriteInvoices("output/Zoho_Invoices.csv", zohoInvoices);
// ... write other files
```

---
## 4. Reconciliation & Acceptance Criteria

After importing, you **must** perform these checks.

| Report / Area | Reconciliation Check | Acceptance Criteria |
| :--- | :--- | :--- |
| **Trial Balance** | Compare closing balances of key control accounts. | Balances must match **exactly**. |
| **AR/AP Aging** | Compare total AR/AP balances. | Totals must match the Trial Balance. |
| **Bank Balances**| Verify closing bank account balances. | Balances must match **exactly**. |
