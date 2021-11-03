using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationParamDTO
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
