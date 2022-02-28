using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Services.Models
{
    public class DenominationReceiptParamDTO
    {
        public int Id { get; set; }
        public int DenominationID { get; set; }
        public int ParameterID { get; set; }
        public string ParameterName { get; set; }
        public bool Bold { get; set; }
        public int Alignment { get; set; }
        public bool Status { get; set; }
        public FontSize FontSize { get; set; }
    }
}
