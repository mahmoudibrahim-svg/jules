# Reconciliation & Acceptance Criteria

This document outlines the required checks and validation gates to ensure a successful and accurate data migration from Qoyod to Zoho Books.

---
## 1. Pre-Export Validation ("Stop-Ship" Conditions)

The conversion tool **must** perform the following checks *before* generating the final Zoho import files. If any of these conditions are met, the tool will halt the export and present a clear error report to the user.

*   **Unbalanced Journal Entries:** For any Journal Entry import file, the sum of all debits must exactly equal the sum of all credits.
*   **Unmapped Chart of Accounts:** Every account referenced in a transactional file (Invoices, Bills, Journals) must have a valid mapping to a Zoho account in the Chart of Accounts mapping configuration.
*   **Unmapped Contacts/Taxes:** (Applicable when implementing Bills/Invoices) Any customer, vendor, or tax code must be mapped.
*   **Missing Required Fields:** Every row in a generated import file must have all of Zoho's required fields populated (e.g., `Invoice Number`, `Invoice Date`, `Customer Name`).
*   **Invalid Data Formats:** Dates must be in a valid format; numerical fields must contain only numbers.

---
## 2. Post-Import Reconciliation Checklist

After successfully importing the generated CSV files into Zoho Books, the user must perform the following checks to verify the migration. The user should run a Trial Balance report in both Qoyod and Zoho Books for the same period end date.

| Report / Area | Reconciliation Check | Acceptance Criteria / Tolerance |
| :--- | :--- | :--- |
| **Trial Balance** | Compare the closing balances of key control accounts (e.g., Accounts Receivable, Accounts Payable, Bank Accounts, Sales, VAT Payable/Receivable) between Qoyod and Zoho Books. | Balances must match **exactly**. Any discrepancy indicates a fundamental error in the migration and must be investigated. |
| **AR Aging Summary** | Compare the total Accounts Receivable balance and the balances for each aging bucket (e.g., 0-30 days, 31-60 days) between the two systems. | Total AR must match the Trial Balance. Individual customer balances must match their respective totals from Qoyod. |
| **AP Aging Summary** | Compare the total Accounts Payable balance and the balances for each aging bucket between the two systems. | Total AP must match the Trial Balance. Individual vendor balances must match their respective totals from Qoyod. |
| **VAT / Tax Report** | (If applicable) Compare the total VAT collected and paid for the period. | Totals should match. *Note: Given the limitations of the Qoyod exports, this may require manual verification.* |
| **Bank Balances** | Verify that the closing balances for all bank and cash accounts in Zoho Books match the balances in Qoyod. | Balances must match **exactly**. |
| **Tolerance Rules** | | No tolerance is acceptable for rounding or currency differences in a properly executed migration. All values must be identical. |
