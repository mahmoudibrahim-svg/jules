using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class JournalConverterServiceTests
    {
        [Test]
        public void Convert_ShouldProcessBalancedJournalsAndReportUnbalancedOnes()
        {
            // Arrange
            var mappingService = new MappingService();
            mappingService.LoadAccountTypeMapping(new Dictionary<string, string>
            {
                { "رواتب واجور", "Salaries and Wages" },
                { "رواتب وبدلات مستحقة", "Accrued Salaries" },
                { "مصرف الانماء", "Alinma Bank" }
            });

            var converterService = new JournalConverterService(mappingService);

            var qoyodLines = new List<QoyodJournalLine>
            {
                // Balanced Journal #1
                new QoyodJournalLine { JournalNumber = "JR01", JournalDate = new DateTime(2023, 1, 1), Account = "رواتب واجور", Debit = 5000, Credit = 0 },
                new QoyodJournalLine { JournalNumber = "JR01", JournalDate = new DateTime(2023, 1, 1), Account = "رواتب وبدلات مستحقة", Debit = 0, Credit = 5000 },

                // Unbalanced Journal #2
                new QoyodJournalLine { JournalNumber = "JR02", JournalDate = new DateTime(2023, 1, 2), Account = "مصرف الانماء", Debit = 1000, Credit = 0 },
                new QoyodJournalLine { JournalNumber = "JR02", JournalDate = new DateTime(2023, 1, 2), Account = "رواتب واجور", Debit = 0, Credit = 900 }, // Mismatch

                // Balanced Journal #3
                new QoyodJournalLine { JournalNumber = "JR03", JournalDate = new DateTime(2023, 1, 3), Account = "مصرف الانماء", Debit = 200, Credit = 0 },
                new QoyodJournalLine { JournalNumber = "JR03", JournalDate = new DateTime(2023, 1, 3), Account = "رواتب واجور", Debit = 0, Credit = 200 }
            };

            // Act
            var (convertedJournals, errors) = converterService.Convert(qoyodLines);

            // Assert
            // 1. Check that only balanced journals were converted (2 lines from JR01 + 2 lines from JR03)
            Assert.AreEqual(4, convertedJournals.Count);

            // 2. Check that the unbalanced journal was reported as an error
            Assert.AreEqual(1, errors.Count);
            Assert.IsTrue(errors[0].Contains("JR02 is unbalanced"));

            // 3. Verify mapping for a sample converted line from JR01
            var convertedLine = convertedJournals.First(j => j.Debit == 5000);
            Assert.AreEqual("Salaries and Wages", convertedLine.Account);
            Assert.AreEqual(new DateTime(2023, 1, 1), convertedLine.JournalDate);
        }
    }
}
