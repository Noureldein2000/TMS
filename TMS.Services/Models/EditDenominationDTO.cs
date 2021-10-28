using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
   public class EditDenominationDTO
    {
        public DenominationDTO Denomination { get; set; }
        public List<DenominationServiceProviderDTO> DenominationServiceProvidersDto { get; set; }
    }
}
