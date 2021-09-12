using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Services.Models
{
    public class ProviderServiceRequestDTO
    {
        public RequestType RequestTypeID { get; set; }
        public ProviderServiceRequestStatusType ProviderServiceRequestStatusID { get; set; }
        public int? Brn { get; set; }
        public int DenominationID { get; set; }
        public int CreatedBy { get; set; }
        public string BillingAccount { get; set; }
    }
}
