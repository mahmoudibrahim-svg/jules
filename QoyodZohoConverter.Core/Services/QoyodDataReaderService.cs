using OfficeOpenXml;
using QoyodZohoConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    public class QoyodDataReaderService
    {
        static QoyodDataReaderService()
        {
            ExcelPackage.License.SetNonCommercialOrganization("QoyodZohoConverter");
        }

        public QoyodDataReaderService()
        {
        }

        public virtual List<QoyodAccount> ReadChartOfAccounts(string filePath)
        {
            var accounts = new List<QoyodAccount>();
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Chart of Accounts file not found.", filePath);
            }

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var headerRow = FindHeaderRow(worksheet, new[] { "الرمز", "اسم الحساب", "نوعه" });
                var columns = GetHeaderColumns(worksheet, headerRow);
                var startRow = headerRow + 1;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    accounts.Add(new QoyodAccount
                    {
                        AccountCode = worksheet.Cells[row, columns["الرمز"]].Text,
                        AccountName = worksheet.Cells[row, columns["اسم الحساب"]].Text,
                        AccountType = worksheet.Cells[row, columns["نوعه"]].Text,
                        Description = worksheet.Cells[row, columns["الوصف"]].Text,
                        ParentAccountCode = ParseParentCode(worksheet.Cells[row, columns["الحساب الرئيسي"]].Text),
                        IsPayableReceivable = (worksheet.Cells[row, columns["حساب مدين/دائن"]].Text ?? "").Equals("نعم", StringComparison.OrdinalIgnoreCase)
                    });
                }
            }

            return accounts;
        }

        private string ParseParentCode(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return null;

            var parts = rawValue.Split('-');
            return parts[0].Trim();
        }

        public virtual List<QoyodJournalLine> ReadJournalEntries(IEnumerable<string> filePaths)
        {
            var journalLines = new List<QoyodJournalLine>();

            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists) continue;

                using (var package = new ExcelPackage(fileInfo))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var headerRow = FindHeaderRow(worksheet, new[] { "رقم القيد", "التاريخ", "مدين", "دائن", "الحساب" });
                    var columns = GetHeaderColumns(worksheet, headerRow);
                    var startRow = headerRow + 1;

                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        journalLines.Add(new QoyodJournalLine
                        {
                            JournalNumber = worksheet.Cells[row, columns["رقم القيد"]].Text,
                            JournalDate = DateTime.Parse(worksheet.Cells[row, columns["التاريخ"]].Text),
                            Notes = worksheet.Cells[row, columns["الملاحظات"]].Text,
                            ReferenceNumber = worksheet.Cells[row, columns["رقم المرجع"]].Text,
                            Description = worksheet.Cells[row, columns["البيان"]].Text,
                            Debit = decimal.Parse(worksheet.Cells[row, columns["مدين"]].Text),
                            Credit = decimal.Parse(worksheet.Cells[row, columns["دائن"]].Text),
                            Account = worksheet.Cells[row, columns["الحساب"]].Text,
                            Currency = worksheet.Cells[row, columns["العملة"]].Text
                        });
                    }
                }
            }

            return journalLines;
        }

        public virtual List<QoyodInvoiceTransaction> ReadInvoiceTransactions(string filePath)
        {
            var transactions = new List<QoyodInvoiceTransaction>();
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Invoice transaction file not found.", filePath);
            }

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var headerRow = FindHeaderRow(worksheet, new[] { "التاريخ", "اسم الحساب", "نوع الحركة", "رقم القيد", "دائن" });
                var columns = GetHeaderColumns(worksheet, headerRow);
                var startRow = headerRow + 1;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    var transactionType = worksheet.Cells[row, columns["نوع الحركة"]].Text;
                    if (transactionType == "فاتورة مبيعات")
                    {
                        transactions.Add(new QoyodInvoiceTransaction
                        {
                            InvoiceDate = DateTime.Parse(worksheet.Cells[row, columns["التاريخ"]].Text),
                            AccountName = worksheet.Cells[row, columns["اسم الحساب"]].Text,
                            Description = worksheet.Cells[row, columns["البيان"]].Text,
                            InvoiceNumber = worksheet.Cells[row, columns["رقم القيد"]].Text,
                            TotalAmount = decimal.Parse(worksheet.Cells[row, columns["دائن"]].Text)
                        });
                    }
                }
            }
            return transactions;
        }

        public virtual List<QoyodPaymentTransaction> ReadPaymentTransactions(string filePath)
        {
            var transactions = new List<QoyodPaymentTransaction>();
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Payment transaction file not found.", filePath);
            }

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var headerRow = FindHeaderRow(worksheet, new[] { "التاريخ", "جهة الاتصال", "نوع الحركة", "رقم الفاتورة", "دائن", "offset_account_id" });
                var columns = GetHeaderColumns(worksheet, headerRow);
                var startRow = headerRow + 1;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    var transactionType = worksheet.Cells[row, columns["نوع الحركة"]].Text;
                    if (transactionType == "customer_payment")
                    {
                        transactions.Add(new QoyodPaymentTransaction
                        {
                            PaymentDate = DateTime.Parse(worksheet.Cells[row, columns["التاريخ"]].Text),
                            CustomerName = worksheet.Cells[row, columns["جهة الاتصال"]].Text,
                            InvoiceNumber = worksheet.Cells[row, columns["رقم الفاتورة"]].Text,
                            Amount = decimal.Parse(worksheet.Cells[row, columns["دائن"]].Text),
                            PaymentAccountNumber = worksheet.Cells[row, columns["offset_account_id"]].Text
                        });
                    }
                }
            }
            return transactions;
        }

        public virtual List<QoyodBillTransaction> ReadBillTransactions(string filePath)
        {
            var transactions = new List<QoyodBillTransaction>();
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("Bill transaction file not found.", filePath);
            }

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var headerRow = FindHeaderRow(worksheet, new[] { "التاريخ", "اسم الحساب", "نوع الحركة", "المرجع", "دائن" });
                var columns = GetHeaderColumns(worksheet, headerRow);
                var startRow = headerRow + 1;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    var transactionType = worksheet.Cells[row, columns["نوع الحركة"]].Text;
                    if (transactionType == "bill")
                    {
                        transactions.Add(new QoyodBillTransaction
                        {
                            BillDate = DateTime.Parse(worksheet.Cells[row, columns["التاريخ"]].Text),
                            AccountName = worksheet.Cells[row, columns["اسم الحساب"]].Text,
                            Description = worksheet.Cells[row, columns["البيان"]].Text,
                            BillNumber = worksheet.Cells[row, columns["المرجع"]].Text,
                            TotalAmount = decimal.Parse(worksheet.Cells[row, columns["دائن"]].Text)
                        });
                    }
                }
            }
            return transactions;
        }

        public virtual List<QoyodJournalCreditAppliedInvoice> ReadAppliedInvoiceCredits(string filePath)
        {
            var records = new List<QoyodJournalCreditAppliedInvoice>();
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists) return records;

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var headerRow = FindHeaderRow(worksheet, new[] { "التاريخ", "رقم القيد", "تاريخ الفاتورة", "رقم الفاتورة", "المبلغ" });
                var columns = GetHeaderColumns(worksheet, headerRow);
                var startRow = headerRow + 1;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    records.Add(new QoyodJournalCreditAppliedInvoice
                    {
                        Date = GetNullableDateTime(worksheet.Cells[row, columns["التاريخ"]].Text),
                        JournalNumber = worksheet.Cells[row, columns["رقم القيد"]].Text,
                        InvoiceDate = GetNullableDateTime(worksheet.Cells[row, columns["تاريخ الفاتورة"]].Text),
                        InvoiceNumber = worksheet.Cells[row, columns["رقم الفاتورة"]].Text,
                        Amount = decimal.Parse(worksheet.Cells[row, columns["المبلغ"]].Text)
                    });
                }
            }
            return records;
        }

        private DateTime? GetNullableDateTime(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (DateTime.TryParse(text, out var date)) return date;
            return null;
        }

        public virtual List<QoyodJournalCreditAppliedBill> ReadAppliedBillCredits(string filePath)
        {
            var records = new List<QoyodJournalCreditAppliedBill>();
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists) return records;

            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var headerRow = FindHeaderRow(worksheet, new[] { "التاريخ", "رقم القيد", "تاريخ الفاتورة", "رقم الفاتورة", "المبلغ" });
                var columns = GetHeaderColumns(worksheet, headerRow);
                var startRow = headerRow + 1;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    records.Add(new QoyodJournalCreditAppliedBill
                    {
                        Date = GetNullableDateTime(worksheet.Cells[row, columns["التاريخ"]].Text),
                        JournalNumber = worksheet.Cells[row, columns["رقم القيد"]].Text,
                        BillDate = GetNullableDateTime(worksheet.Cells[row, columns["تاريخ الفاتورة"]].Text),
                        BillNumber = worksheet.Cells[row, columns["رقم الفاتورة"]].Text,
                        Amount = decimal.Parse(worksheet.Cells[row, columns["المبلغ"]].Text)
                    });
                }
            }
            return records;
        }

        private int FindHeaderRow(ExcelWorksheet worksheet, string[] expectedHeaders)
        {
            for (int row = 1; row <= 20 && row <= worksheet.Dimension.End.Row; row++)
            {
                var headerCells = new List<string>();
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    headerCells.Add(worksheet.Cells[row, col].Text?.Trim());
                }

                bool allHeadersFound = expectedHeaders.All(header => headerCells.Contains(header, StringComparer.OrdinalIgnoreCase));
                if (allHeadersFound)
                {
                    return row;
                }
            }
            throw new InvalidOperationException($"Could not find the expected header row in the worksheet. Expected headers: {string.Join(", ", expectedHeaders)}");
        }

        private Dictionary<string, int> GetHeaderColumns(ExcelWorksheet worksheet, int headerRow)
        {
            var columns = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[headerRow, col].Text?.Trim();
                if (!string.IsNullOrEmpty(header) && !columns.ContainsKey(header))
                {
                    columns[header] = col;
                }
            }
            return columns;
        }
    }
}
