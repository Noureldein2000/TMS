using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationReceiptParamModel
    {
        public int Id { get; set; }
        public int DenominationID { get; set; }
        public int ParameterID { get; set; }
        public string ParameterName { get; set; }
        public bool Bold { get; set; }
        public int Alignment { get; set; }
        public bool Status { get; set; }
    }
}
