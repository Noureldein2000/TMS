using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationParamModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public string ParamKey { get; set; }
        public int ValueModeID { get; set; }
        public string ValueModeName { get; set; }
        public int ValueTypeID { get; set; }
        public string ValueTypeName { get; set; }
    }
}
