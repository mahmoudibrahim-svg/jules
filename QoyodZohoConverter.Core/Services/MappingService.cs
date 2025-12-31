using System.Collections.Generic;
using System.Linq;

namespace QoyodZohoConverter.Core.Services
{
    /// <summary>
    /// Manages the mapping between Qoyod and Zoho entities.
    /// </summary>
    public class MappingService
    {
        private Dictionary<string, string> _accountTypeMapping = new Dictionary<string, string>();

        /// <summary>
        /// Loads the account type mapping from a user-defined source.
        /// In the final application, this would read from a JSON configuration file.
        /// </summary>
        public void LoadAccountTypeMapping(Dictionary<string, string> mapping)
        {
            _accountTypeMapping = mapping;
        }

        /// <summary>
        /// Gets the mapped Zoho account type for a given Qoyod account type.
        /// </summary>
        /// <returns>The mapped Zoho account type, or the original type if no mapping is found.</returns>
        public string GetMappedAccountType(string qoyodAccountType)
        {
            if (_accountTypeMapping.TryGetValue(qoyodAccountType, out var mappedType))
            {
                return mappedType;
            }
            // Return the original type as a fallback, but the UI should prevent this.
            return qoyodAccountType;
        }
    }
}
