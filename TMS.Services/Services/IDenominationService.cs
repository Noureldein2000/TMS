﻿using System;
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
        DenominationServiceProviderDTO EditDenominationServiceProvdier(DenominationServiceProviderDTO model);
        DenominationServiceProviderDTO AddDenominationServiceProvdier(DenominationServiceProviderDTO model);
        EditDenominationDTO GetDenominationById(int id);
        DenominationServiceProviderDTO GetDenominationServiceProviderById(int id);
        void ChangeDenominationServiceProviderStatus(int id);
        DenominationParameterDTO GetDenominationParameterById(int id);
        DenominationParameterDTO EditDenominationParameter(DenominationParameterDTO model);
        DenominationParameterDTO AddDenominationParameter(DenominationParameterDTO model);
        void DeleteDenominationParameter(int id);
        void EditDenominationReceiptData(DenominationReceiptDataDTO model);
        DenominationReceiptParamDTO GetDenominationReceiptParamById(int id);
        void EditDenominationReceiptParam(DenominationReceiptParamDTO model);
        void ChangeDenominationReceiptParamStatus(int id);
        void DeleteDenominationReceiptParam(int id);
        void EditDenominationReceipt(DenominationReceiptDTO model);
    }
}
