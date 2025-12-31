# Transaction Classification & Conversion Specification

This document outlines the deterministic rules for converting Qoyod export data into Zoho Books import files.

**Verification Date:** 2023-10-27

---
## Rule Set 1: Chart of Accounts

**Objective:** To map the Qoyod Chart of Accounts to the Zoho Books Chart of Accounts, preserving hierarchy.

*   **1.1. Field Mapping:**
    *   Qoyod `اسم الحساب` (`Account Name`) -> Zoho `Account Name`
    *   Qoyod `الرمز` (`Account Code`) -> Zoho `Account Code`
    *   Qoyod `الوصف` (`Description`) -> Zoho `Description`

*   **1.2. Account Type Mapping (Critical):**
    *   The application will present a user interface (UI) for mapping.
    *   The UI will display a list of all unique `النوع` (`Account Type`) values from the Qoyod file.
    *   For each Qoyod type, the user **must** select a corresponding valid Zoho Books `Account Type` from a dropdown list.
    *   This mapping will be saved to a configuration file for reuse. The application will not proceed without a complete mapping.

*   **1.3. Hierarchy Reconstruction:**
    *   The application will read all Qoyod accounts into memory.
    *   It will then iterate through the accounts and use the `Parent Account Code` field to build the parent-child relationships.
    *   When generating the Zoho import file, the application will ensure that parent accounts are listed before their corresponding child accounts, as required by Zoho.

---
## Rule Set 2: Sales Invoices

**Objective:** To convert Qoyod revenue account statement entries into importable Zoho Books invoices.

*   **2.1. Transaction Identification:**
    *   A transaction from a Qoyod account statement file (like `QOYOD_Account_Statement2.xlsx`) will be classified as a **Sales Invoice** if and only if the `النوع` (`Transaction Type`) column is equal to "فاتورة مبيعات".

*   **2.2. Single-Line Invoice Creation (Workaround for Data Gap):**
    *   Due to the lack of line-item detail in the Qoyod export, all Zoho invoices will be created with a **single summary line item**.
    *   **Customer Name:** The application will attempt to parse the `وصف العملية` (`Description`) field to extract the customer's name. This may require a configurable pattern (e.g., text before the first hyphen).
    *   **Invoice Number:** Maps directly from `المرجع` (`Invoice Number`).
    *   **Invoice Date:** Maps directly from `التاريخ` (`Invoice Date`).
    *   **Due Date:** Will be set to be the same as the `Invoice Date` as this information is not available in the source file.
    *   **Item Name (Placeholder):** A default, placeholder Item Name (e.g., "Migrated Sale") must be configured in the application. This item must be pre-created in Zoho Books by the user.
    *   **Account:** Maps from the `الحساب` (`Account Name`) field. This account name must exist in the mapped Chart of Accounts.
    *   **Quantity:** Will be hardcoded to `1`.
    *   **Rate:** Will be set to the value from the `دائن` (`Total Amount`) column.
    *   **Tax:** Tax information is not present in the source file, so no tax will be applied. This is a known limitation.

*   **2.3. Data Validation:**
    *   The tool will validate that the `Invoice Number` is unique.
    *   It will verify that the `Account Name` exists in the Chart of Accounts mapping.
    *   It will produce an error if the `Total Amount` is not a valid number.
