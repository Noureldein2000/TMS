using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Parameter : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public string ProviderName { get; set; }
        public virtual ICollection<ProviderServiceRequestParams> ProviderServiceRequestParams { get; set; }
        public virtual ICollection<ProviderServiceResponseParam> ProviderServiceResponseParams { get; set; }
        public virtual ICollection<ReceiptBodyParam> ReceiptBodyParams { get; set; }
        public virtual ICollection<InquiryBillDetails> InquiryBillDetails { get; set; }
    }
}
