using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationParameter : BaseEntity<int>
    {
        public int DenominationID { get; set; }
        public virtual Denomination Denomination { get; set; }
        public int Sequence { get; set; }
        public bool Optional { get; set; }
        public int DenominationParamID { get; set; }
        public virtual DenominationParam DenominationParam { get; set; }
        public string Value { get; set; }
        public string ValueList { get; set; }
        public string ValidationExpression { get; set; }
        public string ValidationMessage { get; set; }
        public string ValidationMessageAr { get; set; }
    }
}
