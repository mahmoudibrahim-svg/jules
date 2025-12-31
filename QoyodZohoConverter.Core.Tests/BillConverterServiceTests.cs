using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class BillConverterServiceTests
    {
        [Test]
        public void ConvertToZohoBills_ShouldConvertBillsCorrectly()
        {
            // Arrange
            var service = new BillConverterService();
            var qoyodTransactions = new List<QoyodBillTransaction>
            {
                new QoyodBillTransaction
                {
                    BillDate = new DateTime(2021, 1, 17),
                    Description = "شركة افق( فيديو تسويقى)",
                    BillNumber = "16261626",
                    TotalAmount = 3500.0m,
                    AccountName = "الموردين"
                },
                new QoyodBillTransaction
                {
                    BillDate = new DateTime(2020, 12, 23),
                    Description = "شركة فيريتاس",
                    BillNumber = "29552955",
                    TotalAmount = 17250.0m,
                    AccountName = "الموردين"
                }
            };
            string placeholderItemName = "Migrated Purchase";

            // Act
            var result = service.ConvertToZohoBills(qoyodTransactions, placeholderItemName).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);

            // Check the details of the first converted bill
            var firstBill = result[0];
            Assert.AreEqual("16261626", firstBill.BillNumber);
            Assert.AreEqual(new DateTime(2021, 1, 17), firstBill.BillDate);
            Assert.AreEqual(new DateTime(2021, 1, 17), firstBill.DueDate);
            Assert.AreEqual("شركة افق( فيديو تسويقى)", firstBill.VendorName);
            Assert.AreEqual("الموردين", firstBill.Account);
            Assert.AreEqual(placeholderItemName, firstBill.ItemName);
            Assert.AreEqual(1, firstBill.Quantity);
            Assert.AreEqual(3500.0m, firstBill.Rate);
            Assert.AreEqual("SAR", firstBill.CurrencyCode);
        }
    }
}
