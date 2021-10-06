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
        private readonly IBaseRepository<Service, int> _serviceRepository;
        private readonly IBaseRepository<DenominationProviderConfiguration, int> _denominationProviderConfigurationRepository;
        private readonly IBaseRepository<DenominationServiceProvider, int> _denominationServiceProviderRepository;
        private readonly IBaseRepository<ServiceConfigeration, int> _serviceConfigurationRepository;

        public DenominationService(
             IBaseRepository<DenominationServiceProvider, int> denominationServiceProviderRepository,
            IBaseRepository<ServiceConfigeration, int> serviceConfigurationRepository,
            IBaseRepository<Denomination, int> denominationRepository,
            IBaseRepository<Service, int> serviceRepository,
            IBaseRepository<DenominationProviderConfiguration, int> denominationProviderConfigurationRepository
            )
        {
            _denominationServiceProviderRepository = denominationServiceProviderRepository;
            _serviceConfigurationRepository = serviceConfigurationRepository;
            _denominationRepository = denominationRepository;
            _serviceRepository = serviceRepository;
            _denominationProviderConfigurationRepository = denominationProviderConfigurationRepository;
        }

        public DenominationDTO GetDenomination(int id)
        {
            var denomination = _denominationRepository.Getwhere(d => d.ID == id).Include(s => s.Service).ThenInclude(x => x.ServiceEntity)
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
                    PaymentModeID = denomination.PaymentModeID,
                    OldDenominationID = denomination.OldDenominationID,
                    ServiceEntity = denomination.Service.ServiceEntity.ArName
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
                     ServiceProviderId = s.ServiceProviderID,
                     ProviderCode = s.ProviderCode,
                     ProviderHasFees = s.ProviderHasFees
                 }).FirstOrDefault();
            return denominatiaon;
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

        public IEnumerable<DenominationProviderConfigurationDTO> GetDenominationProviderConfigurationDetails(int denominationId)
        {
            return _denominationProviderConfigurationRepository.Getwhere(s => s.DenominationServiceProvider.DenominationID == denominationId)
                 .Select(s => new DenominationProviderConfigurationDTO
                 {
                     ID = s.ID,
                     Name = s.Name,
                     DenominationProviderID = s.DenominationProviderID,
                     Value = s.Value,
                 }).ToList();
        }

        public decimal GetCurrencyValue(int denominationId)
        {
            return _denominationRepository.Getwhere(x => x.ID == denominationId).Include(x => x.Currency).Select(x => x.Currency.Value).FirstOrDefault();
        }

        public IEnumerable<DenominationProviderConfigurationDTO> GetServiceDenomination(int denominationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DenominationDTO> GetDenominationsByServiceId(int serviceId)
        {
            var denomination = _denominationRepository.Getwhere(d => d.ServiceID == serviceId).Include(s => s.Service).ThenInclude(x => x.ServiceEntity)
               .Select(denomination => new DenominationDTO
               {
                   Id = denomination.ID,
                   Name = denomination.Name,
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
                   PaymentModeID = denomination.PaymentModeID,
                   OldDenominationID = denomination.OldDenominationID,
                   ServiceEntity = denomination.Service.ServiceEntity.ArName
               }).ToList();

            if (denomination == null)
                throw new TMSException("", "");

            return denomination;
        }

        public IEnumerable<ServiceDTO> GetServices()
        {
            return _serviceRepository.GetAll().Select(service => new ServiceDTO()
            {
                Id = service.ID,
                Name = service.Name,
                ArName = service.ArName,
                Code = service.Code,
                PathClass = service.PathClass,
                ServiceEntityID = service.ServiceEntityID,
                ServiceTypeID = service.ServiceTypeID
            }).ToList();
        }
    }
}
