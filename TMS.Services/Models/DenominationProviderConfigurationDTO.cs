using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DenominationProviderConfigurationDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int DenominationProviderID { get; set; }
    }
}
