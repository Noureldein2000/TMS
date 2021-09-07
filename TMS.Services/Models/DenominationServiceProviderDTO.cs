using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationServiceProviderDTO
    {
        public int Id { get; set; }
        public int DenominationId { get; set; }
        public int ServiceProviderId { get; set; }
        public decimal Balance { get; set; }
        public int Status { get; set; }
        public string ProviderCode { get; set; }
        public decimal ProviderAmount { get; set; }
        public int OldServiceId { get; set; }
        public bool ProviderHasFees { get; set; }
    }
}
