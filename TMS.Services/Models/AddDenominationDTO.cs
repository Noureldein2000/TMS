using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class AddDenominationDTO
    {
        public DenominationDTO Denomination { get; set; }
        public DenominationServiceProviderDTO DenominationServiceProvidersDto { get; set; }
        public ServiceConfigerationDTO ServiceConfigerationDto { get; set; }
        public DenominationParameterDTO DenominationParameter  { get; set; }
    }
}
