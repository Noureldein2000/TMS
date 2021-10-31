using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationServiceProvidersModel
    {
        public int Id { get; set; }
        public int DenominationId { get; set; }
        public int ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public decimal Balance { get; set; }
        public bool Status { get; set; }
        public string ProviderCode { get; set; }
        public decimal ProviderAmount { get; set; }
        public int OldServiceId { get; set; }
        public bool ProviderHasFees { get; set; }
        public List<DenominationProviderConfigerationModel> DenominationProviderConfigurationModel { get; set; }
    }
}
