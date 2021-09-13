using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ProviderServiceResponseParamDTO
    {
        public int ServiceRequestID { get; set; }
        public string ProviderName { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
    }
}
