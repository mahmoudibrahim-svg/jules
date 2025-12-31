using OfficeOpenXml;
using QoyodZohoConverter.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

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
                var startRow = 4;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    accounts.Add(new QoyodAccount
                    {
                        AccountCode = worksheet.Cells[row, 1].Text,
                        AccountName = worksheet.Cells[row, 2].Text,
                        AccountType = worksheet.Cells[row, 3].Text,
                        Description = worksheet.Cells[row, 4].Text,
                        ParentAccountCode = ParseParentCode(worksheet.Cells[row, 5].Text),
                        IsPayableReceivable = worksheet.Cells[row, 6].Text.Equals("Yes", StringComparison.OrdinalIgnoreCase)
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
                    var startRow = 2;

                    for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                    {
                        journalLines.Add(new QoyodJournalLine
                        {
                            JournalNumber = worksheet.Cells[row, 3].Text,
                            JournalDate = DateTime.Parse(worksheet.Cells[row, 4].Text),
                            Notes = worksheet.Cells[row, 5].Text,
                            ReferenceNumber = worksheet.Cells[row, 6].Text,
                            Description = worksheet.Cells[row, 7].Text,
                            Debit = decimal.Parse(worksheet.Cells[row, 8].Text),
                            Credit = decimal.Parse(worksheet.Cells[row, 9].Text),
                            Account = worksheet.Cells[row, 10].Text,
                            Currency = worksheet.Cells[row, 14].Text
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
                var startRow = 6;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    var transactionType = worksheet.Cells[row, 3].Text;
                    if (transactionType == "فاتورة مبيعات")
                    {
                        transactions.Add(new QoyodInvoiceTransaction
                        {
                            InvoiceDate = DateTime.Parse(worksheet.Cells[row, 1].Text),
                            AccountName = worksheet.Cells[row, 2].Text,
                            Description = worksheet.Cells[row, 4].Text,
                            InvoiceNumber = worksheet.Cells[row, 5].Text,
                            TotalAmount = decimal.Parse(worksheet.Cells[row, 7].Text)
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
                var startRow = 3;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    var transactionType = worksheet.Cells[row, 8].Text;
                    if (transactionType == "customer_payment")
                    {
                        transactions.Add(new QoyodPaymentTransaction
                        {
                            PaymentDate = DateTime.Parse(worksheet.Cells[row, 1].Text),
                            CustomerName = worksheet.Cells[row, 3].Text,
                            InvoiceNumber = worksheet.Cells[row, 10].Text,
                            Amount = decimal.Parse(worksheet.Cells[row, 12].Text),
                            PaymentAccountNumber = worksheet.Cells[row, 6].Text
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
                var startRow = 3;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    var transactionType = worksheet.Cells[row, 8].Text;
                    if (transactionType == "bill")
                    {
                        transactions.Add(new QoyodBillTransaction
                        {
                            BillDate = DateTime.Parse(worksheet.Cells[row, 1].Text),
                            AccountName = worksheet.Cells[row, 2].Text,
                            Description = worksheet.Cells[row, 3].Text,
                            BillNumber = worksheet.Cells[row, 10].Text,
                            TotalAmount = decimal.Parse(worksheet.Cells[row, 12].Text)
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
                var startRow = 2;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    records.Add(new QoyodJournalCreditAppliedInvoice
                    {
                        Date = GetNullableDateTime(worksheet.Cells[row, 1].Text),
                        JournalNumber = worksheet.Cells[row, 2].Text,
                        InvoiceDate = GetNullableDateTime(worksheet.Cells[row, 3].Text),
                        InvoiceNumber = worksheet.Cells[row, 4].Text,
                        Amount = decimal.Parse(worksheet.Cells[row, 5].Text)
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
                var startRow = 2;

                for (int row = startRow; row <= worksheet.Dimension.End.Row; row++)
                {
                    records.Add(new QoyodJournalCreditAppliedBill
                    {
                        Date = GetNullableDateTime(worksheet.Cells[row, 1].Text),
                        JournalNumber = worksheet.Cells[row, 2].Text,
                        BillDate = GetNullableDateTime(worksheet.Cells[row, 3].Text),
                        BillNumber = worksheet.Cells[row, 4].Text,
                        Amount = decimal.Parse(worksheet.Cells[row, 5].Text)
                    });
                }
            }
            return records;
        }
    }
}
