using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;

namespace TMS.Data.Entities
{
    public interface ILookupType
    {
        public int ID { get; set; }
        string Name { get; set; }
        string ArName { get; set; }
        LookupType IdentifierType { get; set; }
    }
}
