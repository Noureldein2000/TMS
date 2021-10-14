using System;
using System.Collections.Generic;
using System.Text;
using TMS.Infrastructure;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDenominationService
    {
        SwitchEndPointDTO GetServiceConfiguration(int denominationId);
        DenominationDTO GetDenomination(int id);
        DenominationServiceProviderDTO GetDenominationServiceProvider(int denominationId);
        ServiceClassType GetServiceClassType(int id);
        DenominationClassType GetDenominationClassType(int id);
        int GetServiceBalanceType(int denominationId);
        IEnumerable<DenominationProviderConfigurationDTO> GetDenominationProviderConfigurationDetails(int denominationId);
        decimal GetCurrencyValue(int denominationId);
        IEnumerable<DenominationDTO> GetDenominationsByServiceId(int serviceId);
        IEnumerable<ServiceDTO> GetServices();
    }
}
