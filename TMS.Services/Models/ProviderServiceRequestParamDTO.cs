using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ProviderServiceRequestParamDTO
    {
        public int ProviderServiceRequestID { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
    }
}
