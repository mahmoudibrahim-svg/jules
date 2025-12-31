using System.Collections.Generic;

namespace QoyodZohoConverter.Core.Models
{
    /// <summary>
    /// Represents a single account record from the Qoyod export data.
    /// This class is designed based on the 'A_Source_Data_Contract_Qoyod.md' document.
    /// </summary>
    public class QoyodAccount
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string Description { get; set; }
        public string ParentAccountCode { get; set; }
        public bool IsPayableReceivable { get; set; }

        // Navigation property for building hierarchy
        public List<QoyodAccount> Children { get; } = new List<QoyodAccount>();
        public QoyodAccount Parent { get; set; }
    }

    /// <summary>
    /// Represents a single account record formatted for import into Zoho Books.
    /// This class is designed based on the 'B_Target_Import_Contract_Zoho.md' document.
    /// </summary>
    public class ZohoAccount
    {
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string AccountCode { get; set; }
        public string Description { get; set; }
        public string ParentAccount { get; set; }
    }
}
