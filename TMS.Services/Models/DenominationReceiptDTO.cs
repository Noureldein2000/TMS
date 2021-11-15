using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationReceiptDTO
    {
        public DenominationReceiptDataDTO DenominationReceiptDataDTO { get; set; }
        public List<DenominationReceiptParamDTO> DenominationReceiptParamDTOs { get; set; }
    }
}
