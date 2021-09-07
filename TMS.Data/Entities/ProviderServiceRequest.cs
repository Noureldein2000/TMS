using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ProviderServiceRequest : BaseEntity<int>
    {
        public int RequestTypeID { get; set; }
        public virtual RequestType RequestType { get; set; }
        public int ProviderServiceRequestStatusID { get; set; }
        public virtual ProviderServiceRequestStatus ProviderServiceRequestStatus { get; set; }
        public int? Brn { get; set; }
        public int DenominationID { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string BillingAccount { get; set; }
        public virtual ICollection<ProviderServiceResponse> ProviderServiceResponses { get; set; }
        public virtual ICollection<ReceiptBodyParam> ReceiptBodyParams { get; set; }
    }
}
