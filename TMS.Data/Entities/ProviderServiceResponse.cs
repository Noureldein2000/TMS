using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class ProviderServiceResponse : BaseEntity<int>
    {
        public int ProviderServiceRequestID { get; set; }
        public virtual ProviderServiceRequest ProviderServiceRequest { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual ICollection<ProviderServiceResponseParam> ProviderServiceResponseParams { get; set; }
        public virtual ICollection<InquiryBill> InquiryBills { get; set; }
    }
}
