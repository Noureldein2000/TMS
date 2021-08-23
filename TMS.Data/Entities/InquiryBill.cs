using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class InquiryBill : BaseEntity<int>
    {
        public decimal Amount { get; set; }
        public int Sequence { get; set; }
        public int ProviderServiceResponseID { get; set; }
        public virtual ProviderServiceResponse ProviderServiceResponse { get; set; }
        public virtual ICollection<InquiryBillDetails> InquiryBillDetails { get; set; }
    }
}
