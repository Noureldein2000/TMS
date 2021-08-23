using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class InquiryBillDetails : BaseEntity<int>
    {
        public int InquiryBillID { get; set; }
        public virtual InquiryBill InquiryBill { get; set; }
        public int ParameterID { get; set; }
        public virtual Parameter Parameter { get; set; }
        public string  Value { get; set; }
    }
}
