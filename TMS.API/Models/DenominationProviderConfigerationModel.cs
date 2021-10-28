using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DenominationProviderConfigerationModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int DenominationProviderID { get; set; }
    }
}
