using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationReceiptModel
    {
        public DenominationReceiptDataModel DenominationReceiptData { get; set; }
        public List<DenominationReceiptParamModel> DenominationReceiptParams { get; set; }
    }
}
