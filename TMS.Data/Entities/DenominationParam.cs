using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class DenominationParam : BaseEntity<int>
    {
        public string Label { get; set; }
        public string Title { get; set; }
        public int ValueModeID { get; set; }
        public virtual DenominationParamValueMode DenominationParamValueMode { get; set; }
        public int ValueTypeID { get; set; }
        public virtual DenominationParamValueType DenominationParamValueType { get; set; }
        public string ParamKey { get; set; }
        public virtual ICollection<DenominationParameter> DenominationParameters { get; set; }
    }
}
