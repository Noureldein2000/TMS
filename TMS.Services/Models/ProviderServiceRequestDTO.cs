using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ProviderServiceRequestDTO
    {
        public int RequestTypeID { get; set; }
        public int ProviderServiceRequestStatusID { get; set; }
        public int? Brn { get; set; }
        public int DenominationID { get; set; }
        public int CreatedBy { get; set; }
        public string BillingAccount { get; set; }
    }
}
