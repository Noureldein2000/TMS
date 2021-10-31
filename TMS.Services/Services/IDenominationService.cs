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
        PagedResult<DenominationDTO> GetDenominationsByServiceId(int serviceId, int page, int pageSize, string language);
        void AddDenomination(AddDenominationDTO denomination);
        void EditDenomination(DenominationDTO denomination);
        void ChangeStatus(int id);
        void EditDenominationServiceProvdier(DenominationServiceProviderDTO model);
        EditDenominationDTO GetDenominationById(int id);
        DenominationServiceProviderDTO GetDenominationServiceProviderById(int id);
        void ChangeDenominationServiceProviderStatus(int id);
    }
}
