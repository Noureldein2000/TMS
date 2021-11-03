using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationParameterModel
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
    }
}
