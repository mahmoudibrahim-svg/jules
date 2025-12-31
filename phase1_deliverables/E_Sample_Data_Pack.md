# E) Sample Data Pack Specification

To ensure the conversion tool is robust, accurate, and handles all required scenarios, a comprehensive and anonymized sample data pack is required for development and testing. Please provide the following Qoyod export files, ensuring they contain the specified edge cases.

**IMPORTANT:** All data must be anonymized. Please replace real names, company names, and any other sensitive information with realistic but fictional data.

---
### 1. Chart of Accounts Export (`QOYOD_Chart_of_Accounts.xlsx`)
*   **Required Scenarios:**
    *   [ ] At least three levels of account hierarchy (e.g., Grandparent -> Parent -> Child).
    *   [ ] Accounts that are marked as "Payable/Receivable" (`يمكن الدفع والتحصيل بهذا الحساب` = Yes).
    *   [ ] Accounts that are *not* marked as "Payable/Receivable" (`يمكن الدفع والتحصيل بهذا الحساب` = No).
    *   [ ] Accounts with special characters or long names in the `Account Name`.

---
### 2. General Journal Exports (`Journal00.xlsx`, etc.)
*   **Required Scenarios:**
    *   [ ] A standard, two-line journal entry (one debit, one credit).
    *   [ ] A multi-line journal entry (e.g., one debit, multiple credits).
    *   [ ] A journal entry that includes a `Contact Name`.
    *   [ ] A journal entry that has a `Reference Number`.
    *   [ ] A journal entry that affects a control account like Accounts Receivable or Accounts Payable (this will help test validation rules).

---
### 3. Revenue Account Statement (`QOYOD_Account_Statement2.xlsx`)
*   **Required Scenarios (to test Invoice & Credit Note logic):**
    *   [ ] A simple "فاتورة مبيعات" (Sales Invoice) transaction.
    *   [ ] An "إشعار دائن" (Credit Note) transaction.
    *   [ ] An invoice that is later fully credited by a credit note (please ensure they can be linked by the `Description` or another field if possible).
    *   [ ] An invoice that has a partial credit note applied.

---
### 4. AP / Liability Account Statement (e.g., `ZOHO_AP_Account_Transactions.xlsx` equivalent from Qoyod)
*   **Required Scenarios (to test Bill & Vendor Credit logic):**
    *   [ ] A standard "فاتورة مشتريات" (Purchase Bill) transaction, if this type exists in the export.
    *   [ ] A payment transaction applied to a bill.
    *   [ ] A vendor credit transaction.

---

This sample data is the foundation for the next phases of development. The more comprehensive it is, the more reliable the final application will be.
