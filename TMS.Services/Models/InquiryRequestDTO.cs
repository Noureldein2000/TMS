using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class InquiryRequestDTO
    {
        public string Language { get; set; } = "ar-EG";
        public string BillingAccount { get; set; }
        public string Version { get; set; }
        public string ServiceListVersion { get; set; }
        public List<DataDTO> Data { get; set; }
    }
}
