# Source Data Contract (Qoyod) - Chart of Accounts

This document specifies the data structure of the Chart of Accounts export from Qoyod, based on the `QOYOD_Chart_of_Accounts.xlsx` file.

| Original Field Name (Arabic) | Field Name (Translated) | Data Type | Required? | Notes & Examples |
| :--- | :--- | :--- | :--- | :--- |
| **الرمز** | `Account Code` | `String` | **Yes** | The primary key for the account. Appears to be a numeric or alphanumeric code. Ex: `1`, `101`, `10101`. |
| **اسم الحساب** | `Account Name` | `String` | **Yes** | The name of the account. Ex: "الأصول", "النقدية". |
| **النوع** | `Account Type` | `String` | **Yes** | The Qoyod-specific account type. Ex: "الاصول", "النقدية ومافي حكمها". This will require mapping to Zoho's account types. |
| **الوصف** | `Description` | `String` | No | Description for the account. Ex: "Cash", "اساسي 68202355194000". |
| **Parent Account code** | `Parent Account Code` | `String` | No | The code of the parent account. This is the key for establishing account hierarchies. Ex: "101 - أصول متداولة". The tool will need to parse this to link child accounts to parents. |
| **يمكن الدفع والتحصيل بهذا الحساب** | `Is Payable/Receivable` | `String` | No | Indicates if the account can be used in payments. Ex: `Yes`, `No`. This might influence which accounts can be used in certain transaction types. |

---
### Key Identifiers & Relationships:

*   **Primary Key:** `Account Code` (الرمز) appears to be the unique identifier for each account.
*   **Linkage Key:** `Parent Account Code` is the key for building the hierarchy, linking a child account to its parent. The tool will need to parse this field to extract the parent's `Account Code`.
*   **Known Gaps:** The Qoyod export provides a rich hierarchy. The main challenge will be mapping the Qoyod `Account Type` to the stricter, predefined `Account Type` in Zoho Books. This will be a core function of the mapping screen in the application.

---
---

## Source Data Contract (Qoyod) - Sales Invoices

This contract is derived from `QOYOD_Account_Statement2.xlsx`. This file is an **account statement for a revenue account**, not a direct export of invoices. Transactions are identified by filtering the `النوع` (Type) column for "فاتورة مبيعات" (Sales Invoice).

| Original Field Name (Arabic) | Field Name (Translated) | Data Type | Required? | Notes & Examples |
| :--- | :--- | :--- | :--- | :--- |
| **التاريخ** | `Invoice Date` | `Date` | **Yes** | The date of the transaction. Ex: `2022-06-08`. |
| **النوع** | `Transaction Type` | `String` | **Yes** | Used to identify the transaction. Must be "فاتورة مبيعات". |
| **وصف العملية** | `Description` | `String` | **Yes** | Contains customer name and other details. Requires parsing. Ex: "يزيد الدوسري - 29199". |
| **المرجع** | `Invoice Number` | `String` | **Yes** | The unique invoice reference number. Ex: `INV2102583`. |
| **دائن** | `Total Amount` | `Number` | **Yes** | The total credit amount of the invoice. Ex: `500.00`. |
| **الحساب** | `Account Name`| `String` | **Yes** | The name of the revenue account. Ex: "ايرادات عاين زيارة". |

---
### Key Identifiers & Gaps:

*   **Primary Key:** `Invoice Number` (المرجع) serves as the unique ID for the invoice.
*   **CRITICAL GAP - No Line Items:** This export format is a **summary only**. It lacks individual line item details (product/service name, quantity, rate, tax per line).
*   **Implication:** The conversion tool can only create **single-line invoices** in Zoho Books. It will be necessary to use a placeholder "Item", set Quantity to 1, and use the `Total Amount` as the Rate. The `Description` field will be used for the line item description in Zoho. This is a significant constraint that must be addressed in the conversion rules and user guide.
