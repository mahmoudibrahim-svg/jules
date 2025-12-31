using NUnit.Framework;
using QoyodZohoConverter.Core.Models;
using QoyodZohoConverter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Tests
{
    [TestFixture]
    public class PaymentConverterServiceTests
    {
        [Test]
        public void Convert_ShouldApplyMappingsAndConvertPayments()
        {
            // Arrange
            var mappingService = new MappingService();
            mappingService.LoadAccountTypeMapping(new Dictionary<string, string>
            {
                { "1123", "Main Bank Account - ZB" } // Mapping Qoyod 'offset_account_id' to Zoho account name
            });

            var converterService = new PaymentConverterService(mappingService);

            var qoyodPayments = new List<QoyodPaymentTransaction>
            {
                new QoyodPaymentTransaction
                {
                    PaymentDate = new DateTime(2020, 5, 13),
                    CustomerName = "م. علي الشهري",
                    InvoiceNumber = "INV2005001",
                    Amount = 4050.0m,
                    PaymentAccountNumber = "1123"
                }
            };

            // Act
            var result = converterService.Convert(qoyodPayments).ToList();

            // Assert
            Assert.AreEqual(1, result.Count);

            var payment = result[0];
            Assert.AreEqual("م. علي الشهري", payment.CustomerName);
            Assert.AreEqual("INV2005001", payment.InvoiceNumber);
            Assert.AreEqual(4050.0m, payment.Amount);
            Assert.AreEqual("Main Bank Account - ZB", payment.PaymentAccount); // Verify mapping
        }
    }
}
