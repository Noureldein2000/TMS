using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationServiceProvider :  BaseEntity<int>
    {
        public int DenominationID { get; set; }
        public int ServiceProviderID { get; set; }
        public decimal Balance { get; set; }
        public bool Status { get; set; }
        public string ProviderCode { get; set; }
        public decimal ProviderAmount { get; set; }
        public int? OldServiceID { get; set; }
        public bool ProviderHasFees { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        public virtual ICollection<ProviderServiceConfigeration> ProviderServiceConfigerations { get; set; }
    }
}
