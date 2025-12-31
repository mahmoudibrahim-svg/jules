using NUnit.Framework;
using Moq;
using QoyodZohoConverter.Core.Services;
using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class DryRunServiceTests
    {
        private Mock<ValidationService> _mockValidationService;
        private Mock<QoyodDataReaderService> _mockDataReaderService;
        private DryRunService _dryRunService;

        [SetUp]
        public void Setup()
        {
            _mockValidationService = new Mock<ValidationService>();
            _mockDataReaderService = new Mock<QoyodDataReaderService>();
            _dryRunService = new DryRunService(_mockValidationService.Object, _mockDataReaderService.Object);
        }

        [Test]
        public void PerformDryRun_WithAppliedCredits_ShouldReturnCorrectCountsAndWarningMessages()
        {
            // Arrange
            // Corrected model names to match what the data reader service returns
            var invoices = new List<QoyodInvoiceTransaction> { new QoyodInvoiceTransaction { InvoiceNumber = "INV-001" } };
            var bills = new List<QoyodBillTransaction> { new QoyodBillTransaction { BillNumber = "BILL-001" } };
            var journals = new List<QoyodJournalLine> { new QoyodJournalLine { JournalNumber = "JV-001", JournalDate = new DateTime(2023, 1, 1) } };

            // Added valid Amount to test data
            var appliedInvoiceCredits = new List<QoyodJournalCreditAppliedInvoice> { new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-001", Date = null, Amount = 100 } };
            var appliedBillCredits = new List<QoyodJournalCreditAppliedBill> { new QoyodJournalCreditAppliedBill { JournalNumber = "JV-002", BillNumber = "BILL-001", Date = null, Amount = 200 } };

            _mockDataReaderService.Setup(s => s.ReadInvoiceTransactions(It.IsAny<string>())).Returns(invoices);
            _mockDataReaderService.Setup(s => s.ReadBillTransactions(It.IsAny<string>())).Returns(bills);
            _mockDataReaderService.Setup(s => s.ReadJournalEntries(It.IsAny<IEnumerable<string>>())).Returns(journals);
            _mockDataReaderService.Setup(s => s.ReadAppliedInvoiceCredits(It.IsAny<string>())).Returns(appliedInvoiceCredits);
            _mockDataReaderService.Setup(s => s.ReadAppliedBillCredits(It.IsAny<string>())).Returns(appliedBillCredits);

            // Corrected the generic type in the validation service setup
            _mockValidationService.Setup(v => v.FindDuplicateInvoiceNumbers(It.IsAny<List<QoyodInvoiceTransaction>>())).Returns(new List<string>());
            _mockValidationService.Setup(v => v.FindDuplicateBillNumbers(It.IsAny<List<QoyodBillTransaction>>())).Returns(new List<string>());
            _mockValidationService.Setup(v => v.ValidateAppliedInvoiceCredits(It.IsAny<IEnumerable<QoyodJournalCreditAppliedInvoice>>())).Returns(new List<string>());
            _mockValidationService.Setup(v => v.ValidateAppliedBillCredits(It.IsAny<IEnumerable<QoyodJournalCreditAppliedBill>>())).Returns(new List<string>());


            // Act
            var result = _dryRunService.PerformDryRun("dummy_inv.xlsx", "dummy_bill.xlsx", new[] { "dummy_journal.xlsx" }, "dummy_inv_credits.xlsx", "dummy_bill_credits.xlsx");

            // Assert
            Assert.AreEqual(1, result.TotalInvoicesToConvert);
            Assert.AreEqual(1, result.TotalBillsToConvert);
            Assert.AreEqual(1, result.AppliedInvoiceCreditsCount);
            Assert.AreEqual(1, result.AppliedBillCreditsCount);

            // The `ValidationMessages` list will contain a "Found X journals to process" message, so HasIssues will be true.
            Assert.IsTrue(result.HasIssues);
            Assert.IsEmpty(result.AppliedCreditsErrors);

            Assert.AreEqual(1, result.MissingJournalDatesForAppliedCredits.Count);
            Assert.IsTrue(result.MissingJournalDatesForAppliedCredits[0].Contains("JV-002"));
        }

        [Test]
        public void PerformDryRun_Integration_WithInvalidCreditData_ShouldPopulateAppliedCreditsErrors()
        {
            // Arrange
            var dataReader = new Mock<QoyodDataReaderService>();
            var validationService = new ValidationService(); // Use a real instance
            var dryRunService = new DryRunService(validationService, dataReader.Object);

            var creditsWithDuplicates = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 50 },
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 75 } // Duplicate pair
            };

            dataReader.Setup(s => s.ReadInvoiceTransactions(It.IsAny<string>())).Returns(new List<QoyodInvoiceTransaction>());
            dataReader.Setup(s => s.ReadBillTransactions(It.IsAny<string>())).Returns(new List<QoyodBillTransaction>());
            dataReader.Setup(s => s.ReadJournalEntries(It.IsAny<IEnumerable<string>>())).Returns(new List<QoyodJournalLine>());
            dataReader.Setup(s => s.ReadAppliedInvoiceCredits(It.IsAny<string>())).Returns(creditsWithDuplicates);
            dataReader.Setup(s => s.ReadAppliedBillCredits(It.IsAny<string>())).Returns(new List<QoyodJournalCreditAppliedBill>());

            // Act
            var result = dryRunService.PerformDryRun("dummy", "dummy", new string[]{}, "dummy", "dummy");

            // Assert
            Assert.IsNotEmpty(result.AppliedCreditsErrors);
            Assert.AreEqual(1, result.AppliedCreditsErrors.Count);
            Assert.IsTrue(result.AppliedCreditsErrors[0].Contains("Duplicate applied credit"));
        }
    }
}
