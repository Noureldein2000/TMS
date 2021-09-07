using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class DenominationService : IDenominationService
    {
        private readonly IBaseRepository<Denomination, int> _denominationRepository;
        //private readonly IBaseRepository<Service, int> _serviceRepository;
        private readonly IBaseRepository<DenominationServiceProvider, int> _denominationServiceProviderRepository;
        private readonly IBaseRepository<ServiceConfigeration, int> _serviceConfigurationRepository;
        private readonly IBaseRepository<ProviderServiceResponseParam, int> _providerServiceResponseParamRepository;
        private readonly IBaseRepository<ProviderServiceRequestParam, int> _providerServiceRequestParamRepository;
        //private readonly IBaseRepository<Parameter, int> _parameters;
        public DenominationService(
             IBaseRepository<DenominationServiceProvider, int> denominationServiceProviderRepository,
            IBaseRepository<ServiceConfigeration, int> serviceConfigurationRepository,
            IBaseRepository<Denomination, int> denominationRepository,
            //IBaseRepository<Service, int> serviceRepository,
            IBaseRepository<ProviderServiceRequestParam, int> providerServiceRequestParamRepository,
            IBaseRepository<ProviderServiceResponseParam, int> providerServiceResponseParamRepository
            //IBaseRepository<Parameter, int> parameters
            )
        {
            _denominationServiceProviderRepository = denominationServiceProviderRepository;
            _serviceConfigurationRepository = serviceConfigurationRepository;
            _denominationRepository = denominationRepository;
            //_serviceRepository = serviceRepository;
            _providerServiceResponseParamRepository = providerServiceResponseParamRepository;
            //_parameters = parameters;
            _providerServiceRequestParamRepository = providerServiceRequestParamRepository;
        }

        public DenominationDTO GetDenomination(int id)
        {
            var denomination = _denominationRepository.Getwhere(d => d.ID == id)
                .Select(denomination => new DenominationDTO
                {
                    APIValue = denomination.APIValue,
                    CurrencyID = (int)denomination.CurrencyID,
                    ServiceID = denomination.ServiceID,
                    Status = denomination.Status,
                    ClassType = denomination.ClassType,
                    ServiceProviderId = denomination.DenominationServiceProviders.Select(s => s.ServiceProviderID).FirstOrDefault(),
                    Interval = denomination.Interval,
                    MaxValue = denomination.MaxValue,
                    MinValue = denomination.MinValue,
                    Inquirable = denomination.Inquirable,
                    Value = denomination.Value,
                    BillPaymentModeID = denomination.BillPaymentModeID,
                    PaymentModeID = denomination.PaymentModeID
                }).FirstOrDefault();
            if (denomination == null)
                throw new TMSException("", "");

            return denomination;

        }

        public DenominationClassType GetDenominationClassType(int id)
        {
            return _denominationRepository.Getwhere(d => d.ID == id).Select(d => d.ClassType).FirstOrDefault();
        }

        public DenominationServiceProviderDTO GetDenominationServiceProvider(int denominationId)
        {
            var denominatiaon = _denominationServiceProviderRepository.Getwhere(s => s.DenominationID == denominationId)
                 .Select(s => new DenominationServiceProviderDTO
                 {
                     Id = s.ID,
                     ServiceProviderId = s.ServiceProviderID
                 }).FirstOrDefault();
            return denominatiaon;
        }

        public Dictionary<string, string> GetProviderServiceResponseParam(int providerServiceRequestId, string parameterName, string language = "ar")
        {
            var resp = _providerServiceResponseParamRepository.Getwhere(s => s.Parameter.ProviderName == parameterName
           && s.ProviderServiceResponse.ProviderServiceRequestID == providerServiceRequestId).Include(s => s.Parameter).FirstOrDefault();

            return new Dictionary<string, string>
            {
                { language == "ar" ? resp.Parameter.ArName : resp.Parameter.Name, resp.Value }
            };
            //resp.Select(s => new Dictionary<string, decimal>
            //{
            //    { language == "ar" ? s.Parameter.ArName : s.Parameter.Name, decimal.Parse(s.Value) }
            //}).FirstOrDefault();

        }
        public Dictionary<string, decimal> GetProviderServiceRequestParam(int providerServiceRequestId, string parameterName, string language = "ar")
        {
            return _providerServiceRequestParamRepository.Getwhere(s => s.Parameter.ProviderName == parameterName
            && s.ProviderServiceRequestID == providerServiceRequestId).Select(s => new Dictionary<string, decimal>
            {
                { language == "ar" ? s.Parameter.ArName : s.Parameter.Name, decimal.Parse(s.Value) }
            }).FirstOrDefault();

        }
        public ServiceClassType GetServiceClassType(int id)
        {
            return _denominationRepository.Getwhere(d => d.ID == id).Select(d => d.Service.ClassType).FirstOrDefault();
        }

        public SwitchEndPointDTO GetServiceConfiguration(int denominationId)
        {
            return _serviceConfigurationRepository.Getwhere(d => d.ProviderServiceConfigerations
           .Any(p => p.DenominationServiceProvider.DenominationID == denominationId
               && p.DenominationServiceProvider.Status == true))
               .Select(ui => new SwitchEndPointDTO
               {
                   URL = ui.URL,
                   TimeOut = ui.TimeOut,
                   UserName = ui.UserName,
                   UserPassword = ui.UserPassword,
                   ServiceConfigParms = ui.ServiceConfigParms.Select(s => new ServiceConfigParmsDTO { Name = s.Name, Value = s.Value }).ToList()
               }).FirstOrDefault();
        }

        public int GetServiceBalanceType(int denominationId)
        {
            return _denominationRepository.Getwhere(s => s.ID == denominationId)
                .Select(s => s.Service.ServiceBalanceTypes.Select(b => b.BalanceTypeID).FirstOrDefault()).FirstOrDefault();
        }
    }
}
