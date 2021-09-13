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
        Dictionary<string, string> GetProviderServiceResponseParam(int providerServiceRequestId, string parameterName, string language = "ar");
        Dictionary<string, decimal> GetProviderServiceRequestParam(int providerServiceRequestId, string parameterName, string language = "ar");
        int GetServiceBalanceType(int denominationId);
        IEnumerable<DenominationProviderConfigurationDTO> GetDenominationProviderConfigurationDetails(int denominationId);
        decimal GetCurrencyValue(int denominationId);
    }
}
