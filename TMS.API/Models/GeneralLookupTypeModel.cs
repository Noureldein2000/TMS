using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class GeneralLookupTypeModel
    {
        public int Id{ get; set; }
        public string Name{ get; set; }
        public string NameAr{ get; set; }
        public int IdentifierType { get; set; }
    }
}
