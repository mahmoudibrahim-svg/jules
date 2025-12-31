using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class ValidationServiceTests
    {
        [Test]
        public void FindDuplicateInvoiceNumbers_ShouldReturnOnlyDuplicateNumbers()
        {
            // Arrange
            var service = new ValidationService();
            var qoyodTransactions = new List<QoyodInvoiceTransaction>
            {
                new QoyodInvoiceTransaction { InvoiceNumber = "INV001" },
                new QoyodInvoiceTransaction { InvoiceNumber = "INV002" },
                new QoyodInvoiceTransaction { InvoiceNumber = "INV001" }, // Duplicate
                new QoyodInvoiceTransaction { InvoiceNumber = "INV003" },
                new QoyodInvoiceTransaction { InvoiceNumber = "INV002" }, // Duplicate
                new QoyodInvoiceTransaction { InvoiceNumber = "INV002" }  // Triplicate
            };

            // Act
            var duplicates = service.FindDuplicateInvoiceNumbers(qoyodTransactions).ToList();

            // Assert
            Assert.AreEqual(2, duplicates.Count, "Should find two distinct duplicate invoice numbers.");
            Assert.IsTrue(duplicates.Contains("INV001"));
            Assert.IsTrue(duplicates.Contains("INV002"));
            Assert.IsFalse(duplicates.Contains("INV003"));
        }

        [Test]
        public void FindDuplicateBillNumbers_ShouldReturnOnlyDuplicateNumbers()
        {
            // Arrange
            var service = new ValidationService();
            var qoyodTransactions = new List<QoyodBillTransaction>
            {
                new QoyodBillTransaction { BillNumber = "BILL-A" },
                new QoyodBillTransaction { BillNumber = "BILL-B" },
                new QoyodBillTransaction { BillNumber = "BILL-C" },
                new QoyodBillTransaction { BillNumber = "BILL-A" }, // Duplicate
            };

            // Act
            var duplicates = service.FindDuplicateBillNumbers(qoyodTransactions).ToList();

            // Assert
            Assert.AreEqual(1, duplicates.Count);
            Assert.AreEqual("BILL-A", duplicates[0]);
        }

        [Test]
        public void ValidateAppliedInvoiceCredits_WithValidData_ShouldReturnNoErrors()
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 50 },
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-002", InvoiceNumber = "INV-101", Amount = 100 }
            };
            var errors = service.ValidateAppliedInvoiceCredits(credits);
            Assert.IsEmpty(errors);
        }

        [TestCase((string?)null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ValidateAppliedInvoiceCredits_MissingJournalNumber_ShouldReturnError(string? journalNumber)
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = journalNumber, InvoiceNumber = "INV-100", Amount = 50 }
            };
            var errors = service.ValidateAppliedInvoiceCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("missing a JournalNumber"));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void ValidateAppliedInvoiceCredits_InvalidAmount_ShouldReturnError(decimal amount)
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = amount }
            };
            var errors = service.ValidateAppliedInvoiceCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("non-positive Amount"));
        }

        [Test]
        public void ValidateAppliedInvoiceCredits_DuplicatePair_ShouldReturnError()
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 50 },
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 75 }
            };
            var errors = service.ValidateAppliedInvoiceCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Duplicate applied credit"));
        }

        [Test]
        public void ValidateAppliedInvoiceCredits_DuplicatePairCaseInsensitive_ShouldReturnError()
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "jv-001", InvoiceNumber = "inv-100", Amount = 50 },
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 75 }
            };
            var errors = service.ValidateAppliedInvoiceCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Duplicate applied credit"));
        }

        [Test]
        public void ValidateAppliedBillCredits_WithValidData_ShouldReturnNoErrors()
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-001", BillNumber = "BILL-100", Amount = 50 },
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-002", BillNumber = "BILL-101", Amount = 100 }
            };
            var errors = service.ValidateAppliedBillCredits(credits);
            Assert.IsEmpty(errors);
        }

        [TestCase((string?)null)]
        [TestCase("")]
        [TestCase(" ")]
        public void ValidateAppliedBillCredits_MissingJournalNumber_ShouldReturnError(string? journalNumber)
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = journalNumber, BillNumber = "BILL-100", Amount = 50 }
            };
            var errors = service.ValidateAppliedBillCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("missing a JournalNumber"));
        }

        [TestCase(0)]
        [TestCase(-10)]
        public void ValidateAppliedBillCredits_InvalidAmount_ShouldReturnError(decimal amount)
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-001", BillNumber = "BILL-100", Amount = amount }
            };
            var errors = service.ValidateAppliedBillCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("non-positive Amount"));
        }

        [Test]
        public void ValidateAppliedBillCredits_DuplicatePair_ShouldReturnError()
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-001", BillNumber = "BILL-100", Amount = 50 },
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-001", BillNumber = "BILL-100", Amount = 75 }
            };
            var errors = service.ValidateAppliedBillCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Duplicate applied credit"));
        }

        [Test]
        public void ValidateAppliedBillCredits_DuplicatePairCaseInsensitive_ShouldReturnError()
        {
            var service = new ValidationService();
            var credits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = "jv-001", BillNumber = "bill-100", Amount = 50 },
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-001", BillNumber = "BILL-100", Amount = 75 }
            };
            var errors = service.ValidateAppliedBillCredits(credits);
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("Duplicate applied credit"));
        }
    }
}
