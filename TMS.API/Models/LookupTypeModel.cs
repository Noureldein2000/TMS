using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class LookupTypeModel
    {
        public List<GeneralLookupTypeModel> Fees { get; set; }
        public List<GeneralLookupTypeModel> Commissions { get; set; }
        public List<GeneralLookupTypeModel> Taxes { get; set; }
    }
}
