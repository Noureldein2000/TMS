using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Infrastructure;

namespace TMS.API.Models
{
    public class AddDenominationModel
    {
        public DenominationModel Denomination { get; set; }
        public DenominationServiceProvidersModel DenominationServiceProviders { get; set; }
        public DenominationProviderConfigerationModel DenominationProviderConfigeration { get; set; }
        public ServiceConfigerationModel ServiceConfigeration { get; set; }
        public ServiceConfigParmsModel ServiceConfigParms { get; set; }
    }
}
