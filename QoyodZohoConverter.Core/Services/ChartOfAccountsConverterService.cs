using QoyodZohoConverter.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Handles the conversion of the Chart of Accounts from Qoyod to Zoho format.
    /// </summary>
    public class ChartOfAccountsConverterService
    {
        private readonly MappingService _mappingService;

        public ChartOfAccountsConverterService(MappingService mappingService)
        {
            _mappingService = mappingService;
        }

        /// <summary>
        /// Converts a list of Qoyod accounts to a list of Zoho accounts, applying mappings and preserving hierarchy.
        /// </summary>
        public List<ZohoAccount> Convert(IEnumerable<QoyodAccount> qoyodAccounts)
        {
            var qoyodAccountsList = qoyodAccounts.ToList();
            var zohoAccounts = new List<ZohoAccount>();
            var accountsDict = qoyodAccountsList.ToDictionary(a => a.AccountCode, a => a);

            // First, build the parent-child relationships in the source data
            foreach (var account in qoyodAccountsList)
            {
                if (!string.IsNullOrEmpty(account.ParentAccountCode) && accountsDict.TryGetValue(account.ParentAccountCode, out var parent))
                {
                    account.Parent = parent;
                    parent.Children.Add(account);
                }
            }

            // Now, process accounts in a way that ensures parents are added before children
            var processedAccounts = new HashSet<string>();
            foreach (var account in qoyodAccountsList)
            {
                AddAccountAndParents(account, zohoAccounts, accountsDict, processedAccounts);
            }

            return zohoAccounts;
        }

        private void AddAccountAndParents(QoyodAccount account, List<ZohoAccount> zohoAccounts, Dictionary<string, QoyodAccount> allAccounts, HashSet<string> processed)
        {
            if (account == null || processed.Contains(account.AccountCode))
            {
                return;
            }

            // Ensure parent is added first
            if (account.Parent != null)
            {
                AddAccountAndParents(account.Parent, zohoAccounts, allAccounts, processed);
            }

            // Add the current account
            zohoAccounts.Add(new ZohoAccount
            {
                AccountName = account.AccountName,
                AccountType = _mappingService.GetMappedAccountType(account.AccountType),
                AccountCode = account.AccountCode,
                Description = account.Description,
                ParentAccount = account.Parent?.AccountName
            });

            processed.Add(account.AccountCode);
        }
    }
}
