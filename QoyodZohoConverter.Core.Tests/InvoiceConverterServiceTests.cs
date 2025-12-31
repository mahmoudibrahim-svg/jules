using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class InvoiceConverterServiceTests
    {
        [Test]
        public void ConvertToZohoInvoices_ShouldConvertSalesInvoicesCorrectly()
        {
            // Arrange
            var service = new InvoiceConverterService();
            var qoyodTransactions = new List<QoyodInvoiceTransaction>
            {
                new QoyodInvoiceTransaction
                {
                    InvoiceDate = new DateTime(2022, 6, 8),
                    Description = "يزيد الدوسري - فاتورة مبيعات 29199", // Added "فاتورة مبيعات" to match the filter
                    InvoiceNumber = "INV2102583",
                    TotalAmount = 500.00m,
                    AccountName = "ايرادات عاين زيارة"
                },
                new QoyodInvoiceTransaction
                {
                    InvoiceDate = new DateTime(2022, 6, 10),
                    Description = "احمد محمد - فاتورة مبيعات 29265", // Added "فاتورة مبيعات" to match the filter
                    InvoiceNumber = "INV2102591",
                    TotalAmount = 750.50m,
                    AccountName = "ايرادات عاين زيارة"
                },
                new QoyodInvoiceTransaction
                {
                    InvoiceDate = new DateTime(2022, 6, 11),
                    Description = "Some Other Transaction - إشعار دائن", // This should be ignored
                    InvoiceNumber = "CRN12345",
                    TotalAmount = 100.00m,
                    AccountName = "ايرادات عاين زيارة"
                }
            };
            string placeholderItemName = "Migrated Sale";

            // Act
            var result = service.ConvertToZohoInvoices(qoyodTransactions, placeholderItemName).ToList();

            // Assert
            // 1. Check that only the sales invoices were converted
            Assert.AreEqual(2, result.Count);

            // 2. Check the details of the first converted invoice
            var firstInvoice = result[0];
            Assert.AreEqual("INV2102583", firstInvoice.InvoiceNumber);
            Assert.AreEqual(new DateTime(2022, 6, 8), firstInvoice.InvoiceDate);
            Assert.AreEqual(new DateTime(2022, 6, 8), firstInvoice.DueDate); // Rule: DueDate = InvoiceDate
            Assert.AreEqual("يزيد الدوسري", firstInvoice.CustomerName); // Rule: Parse Customer Name
            Assert.AreEqual("ايرادات عاين زيارة", firstInvoice.Account);
            Assert.AreEqual(placeholderItemName, firstInvoice.ItemName); // Rule: Placeholder Item
            Assert.AreEqual(1, firstInvoice.Quantity); // Rule: Quantity = 1
            Assert.AreEqual(500.00m, firstInvoice.Rate); // Rule: Rate = TotalAmount
            Assert.AreEqual("SAR", firstInvoice.CurrencyCode);
        }
    }
}
