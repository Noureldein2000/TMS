using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Services.Models
{
    public class LookupTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public LookupType IdentifierType { get; set; }
    }
}
