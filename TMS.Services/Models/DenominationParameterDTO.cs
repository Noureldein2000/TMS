using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
   public class DenominationParameterDTO
    {
        public int Id { get; set; }
        public int DenominationID { get; set; }
        public int Sequence { get; set; }
        public bool Optional { get; set; }
        public int DenominationParamID { get; set; }
        public string Value { get; set; }
        public string ValueList { get; set; }
        public string ValidationExpression { get; set; }
        public string ValidationMessage { get; set; }
        public string ValidationMessageAr { get; set; }
    }
}
