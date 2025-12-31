# Target Import Contract (Zoho Books) - Chart of Accounts

This document specifies the required CSV format for importing the Chart of Accounts into Zoho Books. The structure is based on the fields discovered in the `ZOHO_Chart_of_Accounts.xlsx` export file.

**Verification Date:** 2023-10-27

| Field Name | Data Type | Required? | Notes & Constraints |
| :--- | :--- | :--- | :--- |
| **Account Name** | `String` | **Yes** | The name of the account (e.g., "Sales", "Bank of America"). Must be unique. |
| **Account Type** | `String` | **Yes** | Must be one of the predefined Zoho Books account types (e.g., `Income`, `Other Current Asset`, `Cash`, `Bank`, `Fixed Asset`, `Accounts Receivable`, `Other Current Liability`, `Equity`, `Expense`). |
| **Account Code** | `String` | No | A unique code or number for the account (e.g., "4000", "1110"). |
| **Description** | `String` | No | A description for the account. |
| **Parent Account**| `String` | No | If this is a sub-account, provide the **name** of the parent account here. The parent account must already exist in Zoho Books or be defined earlier in the same import file. |

---
### Important Considerations:

*   **Import Order:** The Chart of Accounts must be imported *before* any transactional data (Invoices, Bills, Journals).
*   **Parent Accounts:** If using sub-accounts, ensure the parent accounts are defined before the child accounts in the import file.
*   **Account Types:** The `Account Type` field is critical and must match Zoho's predefined categories exactly. The conversion tool will need a mapping mechanism to ensure Qoyod account types are correctly translated to their Zoho equivalents.

---
---

## Target Import Contract (Zoho Books) - Sales Invoices

**_NOTE:_** _This structure is preliminary, based on common accounting import formats and analysis of Zoho's API documentation. It must be verified against the official Zoho Books sample import file before development._

**Verification Date:** 2023-10-27

### Invoice CSV Format

| Field Name | Data Type | Required? | Notes & Constraints |
| :--- | :--- | :--- | :--- |
| **Customer Name** | `String` | **Yes** | The name of the customer. Must match an existing customer in Zoho Books exactly. |
| **Invoice Number** | `String` | **Yes** | The unique identifier for the invoice. Zoho will reject duplicates. |
| **Invoice Date** | `Date` | **Yes** | Format must match Zoho's requirements (e.g., `YYYY-MM-DD` or `DD-MM-YYYY`). |
| **Due Date** | `Date` | **Yes** | The date the invoice payment is due. |
| **Item Name** | `String` | **Yes** | The name of the line item being sold. Must match an existing "Item" in Zoho Books. |
| **Account** | `String` | **Yes** | The income account to which the sale is credited (e.g., "Sales"). This must match an account from the imported Chart of Accounts. |
| **Quantity** | `Number` | **Yes** | The quantity of the item sold. |
| **Rate** | `Number` | **Yes** | The price per unit of the item. |
| **Tax Name** | `String` | No | The name of the tax rate to apply (e.g., "VAT 15%"). Must match a tax rate pre-configured in Zoho Books. |
| **Currency Code** | `String` | No | The currency of the transaction (e.g., "SAR"). If not provided, the base currency is assumed. |
