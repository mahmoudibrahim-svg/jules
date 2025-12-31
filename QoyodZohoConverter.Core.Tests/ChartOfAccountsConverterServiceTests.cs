using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class ChartOfAccountsConverterServiceTests
    {
        [Test]
        public void Convert_ShouldApplyMappingsAndPreserveHierarchy()
        {
            // Arrange
            var mappingService = new MappingService();
            mappingService.LoadAccountTypeMapping(new Dictionary<string, string>
            {
                { "الأصول المتداولة", "Other Current Asset" },
                { "النقدية ومافي حكمها", "Cash" }
            });

            var converterService = new ChartOfAccountsConverterService(mappingService);

            var qoyodAccounts = new List<QoyodAccount>
            {
                // Child account - should appear second in the output
                new QoyodAccount { AccountCode = "10101", AccountName = "النقدية", AccountType = "النقدية ومافي حكمها", ParentAccountCode = "101" },
                // Parent account - should appear first in the output
                new QoyodAccount { AccountCode = "101", AccountName = "أصول متداولة", AccountType = "الأصول المتداولة", ParentAccountCode = null },
                // Another root account
                new QoyodAccount { AccountCode = "400", AccountName = "Sales", AccountType = "Income", ParentAccountCode = null }
            };

            // Act
            var result = converterService.Convert(qoyodAccounts);

            // Assert
            Assert.AreEqual(3, result.Count);

            // 1. Check correct mapping and hierarchy for the parent
            var parent = result[0];
            Assert.AreEqual("أصول متداولة", parent.AccountName);
            Assert.AreEqual("Other Current Asset", parent.AccountType); // Mapped Type
            Assert.IsNull(parent.ParentAccount);

            // 2. Check correct mapping and hierarchy for the child
            var child = result[1];
            Assert.AreEqual("النقدية", child.AccountName);
            Assert.AreEqual("Cash", child.AccountType); // Mapped Type
            Assert.AreEqual("أصول متداولة", child.ParentAccount); // Parent name is correct

            // 3. Verify order: parent must come before child
            var parentIndex = result.FindIndex(a => a.AccountCode == "101");
            var childIndex = result.FindIndex(a => a.AccountCode == "10101");
            Assert.IsTrue(parentIndex < childIndex, "Parent account should be listed before child account.");
        }
    }
}
