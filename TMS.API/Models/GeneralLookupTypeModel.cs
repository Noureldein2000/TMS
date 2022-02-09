using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Infrastructure;

namespace TMS.API.Models
{
    public class GeneralLookupTypeModel
    {
        public int Id{ get; set; }
        public string Name{ get; set; }
        public string NameAr{ get; set; }
        public LookupType IdentifierType { get; set; }
    }
}
