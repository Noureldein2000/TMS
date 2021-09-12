using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ReceiptBodyParamDTO
    {
        public int ProviderServiceRequestID { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public int? TransactionID { get; set; }
    }
}
