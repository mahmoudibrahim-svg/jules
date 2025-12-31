using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class JournalCreditConverterServiceTests
    {
        private JournalCreditConverterService _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new JournalCreditConverterService();
        }

        [Test]
        public void ConvertAppliedInvoiceCredits_WhenJournalEntryExists_ShouldSetCorrectDate()
        {
            // Arrange
            var appliedCredits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 50, Date = null } // Date is initially null
            };
            var journalEntries = new List<QoyodJournalLine>
            {
                new QoyodJournalLine { JournalNumber = "JV-001", JournalDate = new DateTime(2023, 1, 15) }
            };

            // Act
            var (converted, warnings) = _converter.ConvertAppliedInvoiceCredits(appliedCredits, journalEntries);

            // Assert
            Assert.AreEqual(1, converted.Count);
            Assert.AreEqual(new DateTime(2023, 1, 15), converted.First().Date);
            Assert.IsEmpty(warnings);
        }

        [Test]
        public void ConvertAppliedInvoiceCredits_WhenJournalEntryIsMissing_ShouldReturnNullDateAndAddWarning()
        {
            // Arrange
            var appliedCredits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-002", InvoiceNumber = "INV-101", Amount = 75, Date = null }
            };
            var journalEntries = new List<QoyodJournalLine>(); // Empty list

            // Act
            var (converted, warnings) = _converter.ConvertAppliedInvoiceCredits(appliedCredits, journalEntries);

            // Assert
            Assert.AreEqual(1, converted.Count);
            Assert.IsNull(converted.First().Date); // Date should be null
            Assert.AreEqual(1, warnings.Count);
            Assert.IsTrue(warnings.First().Contains("JV-002"));
        }

        [Test]
        public void ConvertAppliedBillCredits_WhenJournalEntryExists_ShouldSetCorrectDate()
        {
            // Arrange
            var appliedCredits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-003", BillNumber = "BILL-200", Amount = 100, Date = null }
            };
            var journalEntries = new List<QoyodJournalLine>
            {
                new QoyodJournalLine { JournalNumber = "JV-003", JournalDate = new DateTime(2023, 2, 20) }
            };

            // Act
            var (converted, warnings) = _converter.ConvertAppliedBillCredits(appliedCredits, journalEntries);

            // Assert
            Assert.AreEqual(1, converted.Count);
            Assert.AreEqual(new DateTime(2023, 2, 20), converted.First().Date);
            Assert.IsEmpty(warnings);
        }

        [Test]
        public void ConvertAppliedBillCredits_WhenJournalEntryIsMissing_ShouldReturnNullDateAndAddWarning()
        {
            // Arrange
            var appliedCredits = new List<QoyodJournalCreditAppliedBill>
            {
                new QoyodJournalCreditAppliedBill { JournalNumber = "JV-004", BillNumber = "BILL-201", Amount = 150, Date = null }
            };
            var journalEntries = new List<QoyodJournalLine>();

            // Act
            var (converted, warnings) = _converter.ConvertAppliedBillCredits(appliedCredits, journalEntries);

            // Assert
            Assert.AreEqual(1, converted.Count);
            Assert.IsNull(converted.First().Date); // Date should be null
            Assert.AreEqual(1, warnings.Count);
            Assert.IsTrue(warnings.First().Contains("JV-004"));
        }

        [Test]
        public void ConvertAppliedInvoiceCredits_WhenDateIsAlreadyPresent_ShouldKeepExistingDate()
        {
            // Arrange
            var existingDate = new DateTime(2023, 5, 5);
            var appliedCredits = new List<QoyodJournalCreditAppliedInvoice>
            {
                new QoyodJournalCreditAppliedInvoice { JournalNumber = "JV-001", InvoiceNumber = "INV-100", Amount = 50, Date = existingDate }
            };
            var journalEntries = new List<QoyodJournalLine>
            {
                new QoyodJournalLine { JournalNumber = "JV-001", JournalDate = new DateTime(2023, 1, 15) } // This date should be ignored
            };

            // Act
            var (converted, warnings) = _converter.ConvertAppliedInvoiceCredits(appliedCredits, journalEntries);

            // Assert
            Assert.AreEqual(1, converted.Count);
            Assert.AreEqual(existingDate, converted.First().Date); // Should be the original date, not the journal's date
            Assert.IsEmpty(warnings);
        }
    }
}
