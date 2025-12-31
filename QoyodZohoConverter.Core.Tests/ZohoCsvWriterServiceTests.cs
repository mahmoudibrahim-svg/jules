using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System.Collections.Generic;
using System.IO;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class ZohoCsvWriterServiceTests
    {
        [Test]
        public void WriteChartOfAccounts_ShouldProduceCorrectCsvFile()
        {
            // Arrange
            var service = new ZohoCsvWriterService();
            var accounts = new List<ZohoAccount>
            {
                new ZohoAccount
                {
                    AccountName = "Sales",
                    AccountType = "Income",
                    AccountCode = "4000",
                    Description = "Product Sales",
                    ParentAccount = null
                },
                new ZohoAccount
                {
                    AccountName = "Main Bank Account",
                    AccountType = "Bank",
                    AccountCode = "1010",
                    Description = "",
                    ParentAccount = null
                }
            };
            var tempFilePath = Path.GetTempFileName();

            // Act
            service.WriteChartOfAccounts(tempFilePath, accounts);

            // Assert
            var writtenContent = File.ReadAllText(tempFilePath);
            var expectedHeader = "\"Account Name\",\"Account Type\",\"Account Code\",\"Description\",\"Parent Account\"";
            var expectedLine1 = "\"Sales\",\"Income\",\"4000\",\"Product Sales\",\"\"";
            var expectedLine2 = "\"Main Bank Account\",\"Bank\",\"1010\",\"\",\"\"";

            var lines = writtenContent.Replace("\r", "").Split('\n');

            Assert.AreEqual(expectedHeader, lines[0]);
            Assert.AreEqual(expectedLine1, lines[1]);
            Assert.AreEqual(expectedLine2, lines[2]);

            // Clean up
            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteInvoices_ShouldProduceCorrectCsvFile()
        {
            // Arrange
            var service = new ZohoCsvWriterService();
            var invoices = new List<ZohoInvoice>
            {
                new ZohoInvoice { CustomerName = "Test Customer", InvoiceNumber = "INV-001", InvoiceDate = new System.DateTime(2023,1,15), DueDate = new System.DateTime(2023,1,30), ItemName = "Test Item", Account = "Sales", Quantity = 1, Rate = 100 }
            };
            var tempFilePath = Path.GetTempFileName();

            // Act
            service.WriteInvoices(tempFilePath, invoices);

            // Assert
            var writtenContent = File.ReadAllText(tempFilePath);
            var expectedHeader = "\"Customer Name\",\"Invoice Number\",\"Invoice Date\",\"Due Date\",\"Item Name\",\"Account\",\"Quantity\",\"Rate\",\"Tax Name\",\"Currency Code\"";
            var expectedLine = "\"Test Customer\",\"INV-001\",\"2023-01-15\",\"2023-01-30\",\"Test Item\",\"Sales\",\"1\",\"100\",\"\",\"\"";
            var lines = writtenContent.Replace("\r", "").Split('\n');
            Assert.AreEqual(expectedHeader, lines[0]);
            Assert.AreEqual(expectedLine, lines[1]);

            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteJournalEntries_ShouldProduceCorrectCsvFile()
        {
            // Arrange
            var service = new ZohoCsvWriterService();
            var journals = new List<ZohoJournalLine>
            {
                new ZohoJournalLine { JournalDate = new System.DateTime(2023,1,1), Account = "Bank", Debit = 100, Credit = 0 },
                new ZohoJournalLine { JournalDate = new System.DateTime(2023,1,1), Account = "Sales", Debit = 0, Credit = 100 }
            };
            var tempFilePath = Path.GetTempFileName();

            // Act
            service.WriteJournalEntries(tempFilePath, journals);

            // Assert
            var writtenContent = File.ReadAllText(tempFilePath);
            var expectedHeader = "\"Journal Date\",\"Notes\",\"Reference Number\",\"Account\",\"Debit\",\"Credit\",\"Description\",\"Currency\"";
            var expectedLine1 = "\"2023-01-01\",\"\",\"\",\"Bank\",\"100\",\"0\",\"\",\"\"";
            var expectedLine2 = "\"2023-01-01\",\"\",\"\",\"Sales\",\"0\",\"100\",\"\",\"\"";
            var lines = writtenContent.Replace("\r", "").Split('\n');
            Assert.AreEqual(expectedHeader, lines[0]);
            Assert.AreEqual(expectedLine1, lines[1]);
            Assert.AreEqual(expectedLine2, lines[2]);

            File.Delete(tempFilePath);
        }

        [Test]
        public void WritePayments_ShouldProduceCorrectCsvFile()
        {
            // Arrange
            var service = new ZohoCsvWriterService();
            var payments = new List<ZohoPayment>
            {
                new ZohoPayment { CustomerName = "Test Customer", PaymentDate = new System.DateTime(2023,1,20), InvoiceNumber = "INV-001", Amount = 100, PaymentAccount = "Bank", ReferenceNumber = "REF-123" }
            };
            var tempFilePath = Path.GetTempFileName();

            // Act
            service.WritePayments(tempFilePath, payments);

            // Assert
            var writtenContent = File.ReadAllText(tempFilePath);
            var expectedHeader = "\"Customer Name\",\"Payment Date\",\"Invoice Number\",\"Amount\",\"Payment Account\",\"Reference#\"";
            var expectedLine = "\"Test Customer\",\"2023-01-20\",\"INV-001\",\"100\",\"Bank\",\"REF-123\"";
            var lines = writtenContent.Replace("\r", "").Split('\n');
            Assert.AreEqual(expectedHeader, lines[0]);
            Assert.AreEqual(expectedLine, lines[1]);

            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteJournalCreditsAppliedInvoices_ShouldProduceCorrectCsvFile()
        {
            var service = new ZohoCsvWriterService();
            var credits = new List<ZohoJournalCreditAppliedInvoice>
            {
                new ZohoJournalCreditAppliedInvoice { Date = new System.DateTime(2023, 2, 1), JournalNumber = "JV-001", InvoiceDate = new System.DateTime(2023, 1, 15), InvoiceNumber = "INV-100", Amount = 50.75m },
                new ZohoJournalCreditAppliedInvoice { Date = null, JournalNumber = "JV-002, containing comma", InvoiceDate = null, InvoiceNumber = "INV-101 \"with quotes\"", Amount = 100 }
            };
            var tempFilePath = Path.GetTempFileName();

            service.WriteJournalCreditsAppliedInvoices(tempFilePath, credits);

            var writtenContent = File.ReadAllText(tempFilePath);
            var expectedHeader = "\"Date\",\"Journal Number\",\"Invoice Date\",\"Invoice Number\",\"Amount\"";
            var expectedLine1 = "\"2023-02-01\",\"JV-001\",\"2023-01-15\",\"INV-100\",\"50.75\"";
            var expectedLine2 = "\"\",\"JV-002, containing comma\",\"\",\"INV-101 \"\"with quotes\"\"\",\"100\"";

            var lines = writtenContent.Replace("\r", "").Split('\n');

            Assert.AreEqual(expectedHeader, lines[0]);
            Assert.AreEqual(expectedLine1, lines[1]);
            Assert.AreEqual(expectedLine2, lines[2]);

            File.Delete(tempFilePath);
        }

        [Test]
        public void WriteJournalCreditsAppliedBills_ShouldProduceCorrectCsvFile()
        {
            var service = new ZohoCsvWriterService();
            var credits = new List<ZohoJournalCreditAppliedBill>
            {
                new ZohoJournalCreditAppliedBill { Date = new System.DateTime(2023, 3, 1), JournalNumber = "JV-003", BillDate = new System.DateTime(2023, 2, 20), BillNumber = "BILL-200", Amount = 150.25m },
                new ZohoJournalCreditAppliedBill { Date = null, JournalNumber = "JV-004\nwith newline", BillDate = null, BillNumber = "BILL-201", Amount = 200 }
            };
            var tempFilePath = Path.GetTempFileName();

            service.WriteJournalCreditsAppliedBills(tempFilePath, credits);

            var writtenContent = File.ReadAllText(tempFilePath);

            // Use a golden string approach for multi-line content
            var expectedContent =
@"""Date"",""Journal Number"",""Bill Date"",""Bill Number"",""Amount""
""2023-03-01"",""JV-003"",""2023-02-20"",""BILL-200"",""150.25""
"""",""JV-004
with newline"","""",""BILL-201"",""200""
";

            // Normalize line endings for cross-platform consistency
            Assert.AreEqual(expectedContent.Replace("\r\n", "\n"), writtenContent.Replace("\r\n", "\n"));

            File.Delete(tempFilePath);
        }
    }
}
