using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class InquiryRequestModel
    {
        public InquiryRequestModel()
        {
            Data = new List<DataModel>();
        }

        public string BillingAccount { get; set; }
        public string Version { get; set; }
        public string ServiceListVersion { get; set; }
        public List<DataModel> Data { get; set; }
    }
}
