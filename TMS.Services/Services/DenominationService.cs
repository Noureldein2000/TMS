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
        private readonly IBaseRepository<DenominationParameter, int> _denominationParamter;
        private readonly IBaseRepository<DenominationReceiptData, int> _denominationRecepitData;
        private readonly IBaseRepository<DenominationReceiptParam, int> _denominationRecepitParam;
        private readonly IBaseRepository<Restaurant, int> _resturant;
        private readonly IUnitOfWork _unitOfWork;

        public DenominationService(
             IBaseRepository<DenominationServiceProvider, int> denominationServiceProviderRepository,
            IBaseRepository<ServiceConfigeration, int> serviceConfigurationRepository,
            IBaseRepository<Denomination, int> denominationRepository,
            IBaseRepository<Service, int> serviceRepository,
            IBaseRepository<DenominationProviderConfiguration, int> denominationProviderConfigurationRepository,
            IBaseRepository<DenominationParameter, int> denominationParamter,
            IBaseRepository<DenominationReceiptData, int> denominationRecepitData,
            IBaseRepository<DenominationReceiptParam, int> denominationRecepitParam,
            IBaseRepository<Restaurant, int> resturant,
            IUnitOfWork unitOfWork
            )
        {
            _denominationServiceProviderRepository = denominationServiceProviderRepository;
            _serviceConfigurationRepository = serviceConfigurationRepository;
            _denominationRepository = denominationRepository;
            _serviceRepository = serviceRepository;
            _denominationProviderConfigurationRepository = denominationProviderConfigurationRepository;
            _denominationParamter = denominationParamter;
            _denominationRecepitData = denominationRecepitData;
            _denominationRecepitParam = denominationRecepitParam;
            _resturant = resturant;
            _unitOfWork = unitOfWork;
        }

        public DenominationDTO GetDenomination(int id)
        {
            var denomination = _denominationRepository.Getwhere(d => d.ID == id).Include(s => s.Service).ThenInclude(x => x.ServiceEntity)
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
                    ServiceEntity = denomination.Service.ServiceEntity.ArName,
                    ServiceCategoryID = (int)denomination.ServiceCategoryID,
                    PathClass = denomination.PathClass
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
                     ProviderHasFees = s.ProviderHasFees,
                     Balance = s.Balance,
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

        public PagedResult<DenominationDTO> GetDenominationsByServiceId(int serviceId, int page, int pageSize, string language)
        {
            var denomination = _denominationRepository.Getwhere(d => d.ServiceID == serviceId).Include(s => s.Service).ThenInclude(x => x.ServiceEntity)
               .Select(denomination => new
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
                   BillPaymentModeName = language == "en" ? denomination.BillPaymentMode.Name : denomination.BillPaymentMode.ArName,
                   PaymentModeID = denomination.PaymentModeID,
                   PaymentModeName = language == "en" ? denomination.PaymentMode.Name : denomination.PaymentMode.ArName,
                   OldDenominationID = denomination.OldDenominationID,
                   ServiceEntity = denomination.Service.ServiceEntity.ArName,
                   CreationDate = denomination.CreationDate
               });

            var count = denomination.Count();

            var resultList = denomination.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<DenominationDTO>
            {
                Results = resultList.Select(denomination => new DenominationDTO
                {
                    Id = denomination.Id,
                    Name = denomination.Name,
                    APIValue = denomination.APIValue,
                    CurrencyID = (int)denomination.CurrencyID,
                    ServiceID = denomination.ServiceID,
                    Status = denomination.Status,
                    ClassType = denomination.ClassType,
                    ServiceProviderId = denomination.ServiceProviderId,
                    Interval = denomination.Interval,
                    MaxValue = denomination.MaxValue,
                    MinValue = denomination.MinValue,
                    Inquirable = denomination.Inquirable,
                    Value = denomination.Value,
                    BillPaymentModeID = denomination.BillPaymentModeID,
                    BillPaymentModeName = denomination.BillPaymentModeName,
                    PaymentModeID = denomination.PaymentModeID,
                    PaymentModeName = denomination.PaymentModeName,
                    OldDenominationID = denomination.OldDenominationID,
                    ServiceEntity = denomination.ServiceEntity
                }).ToList(),
                PageCount = count
            };
        }

        public void AddDenomination(AddDenominationDTO model)
        {

            _denominationRepository.Add(new Denomination
            {
                Name = model.Denomination.Name,
                APIValue = model.Denomination.APIValue,
                CurrencyID = (int)model.Denomination.CurrencyID,
                ServiceID = model.Denomination.ServiceID,
                Status = model.Denomination.Status,
                ClassType = model.Denomination.ClassType,
                Interval = model.Denomination.Interval,
                MaxValue = model.Denomination.MaxValue,
                MinValue = model.Denomination.MinValue,
                Inquirable = model.Denomination.Inquirable,
                Value = model.Denomination.Value,
                BillPaymentModeID = model.Denomination.BillPaymentModeID,
                PaymentModeID = model.Denomination.PaymentModeID,
                OldDenominationID = model.Denomination.OldDenominationID,
                DenominationServiceProviders = new List<DenominationServiceProvider>()
                {
                    new DenominationServiceProvider
                    {
                        Balance=model.DenominationServiceProvidersDto.Balance,
                        ProviderAmount=model.DenominationServiceProvidersDto.ProviderAmount,
                        ProviderCode=model.DenominationServiceProvidersDto.ProviderCode,
                        ProviderHasFees=model.DenominationServiceProvidersDto.ProviderHasFees,
                        OldServiceID=model.DenominationServiceProvidersDto.OldServiceId,
                        ServiceProviderID=model.DenominationServiceProvidersDto.ServiceProviderId,
                        Status=model.DenominationServiceProvidersDto.Status,
                        ProviderServiceConfigerations=new ProviderServiceConfigeration
                        {
                          ServiceConfigerationID = model.ServiceConfigerationDto.Id
                        }
                    }
                },
                DenominationParameters = new List<DenominationParameter>()
                {
                    new DenominationParameter
                    {
                       Optional=model.DenominationParameter.Optional,
                       Sequence=model.DenominationParameter.Sequence,
                       ValidationExpression=model.DenominationParameter.ValidationExpression,
                       ValidationMessage=model.DenominationParameter.ValidationMessage,
                       DenominationParamID=model.DenominationParameter.DenominationParamID,
                       Value=model.DenominationParameter.Value,
                       ValueList=model.DenominationParameter.ValueList,
                    }

                },
                DenominationReceiptData = new DenominationReceiptData
                {
                    Title = model.DenominationReceiptData.Title,
                    Disclaimer = model.DenominationReceiptData.Disclaimer,
                    Footer = model.DenominationReceiptData.Footer
                },
                DenominationReceiptParams = model.DenominationReceiptParams.Select(x => new DenominationReceiptParam
                {
                    ParameterID = x.ParameterID,
                    Bold = x.Bold,
                    Alignment = x.Alignment,
                    Status = x.Status
                }).ToList()
            });

            _unitOfWork.SaveChanges();

            if (!string.IsNullOrEmpty(model.DenominationServiceProvidersDto.DenominationProviderConfigurationDto?.Select(x => x.Name).FirstOrDefault()))
            {
                var addedServiceConfiguration = _denominationProviderConfigurationRepository.Add(new DenominationProviderConfiguration
                {
                    DenominationProviderID = _denominationServiceProviderRepository.GetAll().Max(x => x.ID),
                    Name = model.DenominationServiceProvidersDto.DenominationProviderConfigurationDto.Select(x => x.Name).FirstOrDefault(),
                    Value = model.DenominationServiceProvidersDto.DenominationProviderConfigurationDto.Select(x => x.Value).FirstOrDefault(),
                });

                _unitOfWork.SaveChanges();
            }
        }

        public void EditDenomination(DenominationDTO denomination)
        {
            var current = _denominationRepository.GetById(denomination.Id);

            current.ID = denomination.Id;
            current.Name = denomination.Name;
            current.APIValue = denomination.APIValue;
            current.CurrencyID = (int)denomination.CurrencyID;
            current.ServiceID = denomination.ServiceID;
            current.Status = denomination.Status;
            current.ClassType = denomination.ClassType;
            current.Interval = denomination.Interval;
            current.MaxValue = denomination.MaxValue;
            current.MinValue = denomination.MinValue;
            current.Inquirable = denomination.Inquirable;
            current.Value = denomination.Value;
            current.BillPaymentModeID = denomination.BillPaymentModeID;
            current.PaymentModeID = denomination.PaymentModeID;
            current.OldDenominationID = denomination.OldDenominationID;
            //current.ServiceCategoryID = denomination.ServiceCategoryID;
            current.PathClass = denomination.PathClass;

            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(int id)
        {
            var current = _denominationRepository.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public DenominationServiceProviderDTO EditDenominationServiceProvdier(DenominationServiceProviderDTO model)
        {
            var current = _denominationServiceProviderRepository.Getwhere(x => x.ID == model.Id).FirstOrDefault();

            current.Balance = model.Balance;
            current.ProviderAmount = model.ProviderAmount;
            current.ProviderCode = model.ProviderCode;
            current.ProviderHasFees = model.ProviderHasFees;
            current.OldServiceID = (int)model.OldServiceId;
            current.ServiceProviderID = model.ServiceProviderId;
            current.ProviderServiceConfigerations.ServiceConfigerationID = model.ServiceConfigerationId;
            //current.Status = model.Status;

            _unitOfWork.SaveChanges();

            return _denominationServiceProviderRepository.Getwhere(x => x.ID == current.ID).Select(d => new DenominationServiceProviderDTO
            {
                Id = d.ID,
                DenominationId = d.DenominationID,
                Balance = d.Balance,
                ProviderAmount = d.ProviderAmount,
                ProviderCode = d.ProviderCode,
                ProviderHasFees = d.ProviderHasFees,
                OldServiceId = (int)d.OldServiceID,
                ServiceProviderId = d.ServiceProviderID,
                ServiceProviderName = d.ServiceProvider.Name,
                Status = d.Status,
            }).FirstOrDefault();
        }

        public EditDenominationDTO GetDenominationById(int id)
        {
            var editDenominationModel = _denominationRepository.Getwhere(d => d.ID == id).Include(s => s.DenominationServiceProviders)
                .Select(x => new EditDenominationDTO
                {
                    Denomination = new DenominationDTO
                    {
                        Id = x.ID,
                        Name = x.Name,
                        APIValue = x.APIValue,
                        CurrencyID = (int)x.CurrencyID,
                        ServiceID = x.ServiceID,
                        Status = x.Status,
                        ClassType = x.ClassType,
                        Interval = x.Interval,
                        MaxValue = x.MaxValue,
                        MinValue = x.MinValue,
                        Inquirable = x.Inquirable,
                        Value = x.Value,
                        BillPaymentModeID = x.BillPaymentModeID,
                        PaymentModeID = x.PaymentModeID,
                        OldDenominationID = x.OldDenominationID,
                        ServiceEntity = x.Service.ServiceEntity.ArName,
                        ServiceCategoryID = (int)x.ServiceCategoryID,
                        PathClass = x.PathClass
                    },
                    DenominationServiceProvidersDto = x.DenominationServiceProviders.Select(dsp => new DenominationServiceProviderDTO
                    {
                        Id = dsp.ID,
                        Balance = dsp.Balance,
                        DenominationId = dsp.DenominationID,
                        OldServiceId = (int)dsp.OldServiceID,
                        ProviderAmount = dsp.ProviderAmount,
                        ProviderCode = dsp.ProviderCode,
                        ProviderHasFees = dsp.ProviderHasFees,
                        Status = dsp.Status,
                        ServiceProviderId = dsp.ServiceProviderID,
                        ServiceProviderName = dsp.ServiceProvider.Name,
                        DenominationProviderConfigurationDto = dsp.DenominationProviderConfigerations.Select(psc => new DenominationProviderConfigurationDTO
                        {
                            ID = psc.ID,
                            Name = psc.Name,
                            Value = psc.Value,
                            DenominationProviderID = psc.DenominationProviderID
                        }).ToList()
                    }).ToList(),
                    DenominationParameterDTOs = x.DenominationParameters.Select(dp => new DenominationParameterDTO
                    {
                        Id = dp.ID,
                        DenominationID = dp.DenominationID,
                        Optional = dp.Optional,
                        Sequence = dp.Sequence,
                        ValidationExpression = dp.ValidationExpression,
                        ValidationMessage = dp.ValidationMessage,
                        DenominationParamID = dp.DenominationParamID,
                        Value = dp.Value,
                        ValueList = dp.ValueList
                    }).ToList(),
                    DenominationRecepitDTO = new DenominationReceiptDTO
                    {
                        DenominationReceiptDataDTO = new DenominationReceiptDataDTO
                        {
                            Id = x.DenominationReceiptData.ID,
                            Title = x.DenominationReceiptData.Title,
                            Footer = x.DenominationReceiptData.Footer,
                            Disclaimer = x.DenominationReceiptData.Disclaimer
                        },
                        DenominationReceiptParamDTOs = x.DenominationReceiptParams.Select(x => new DenominationReceiptParamDTO
                        {
                            ParameterID = x.ParameterID,
                            ParameterName = x.Parameter.ProviderName,
                            Bold = x.Bold,
                            Alignment = x.Alignment,
                            Status = x.Status,
                            Id = x.ID,
                            DenominationID = x.DenominationID
                        }).ToList()
                    }
                }).FirstOrDefault();

            if (editDenominationModel == null)
                throw new TMSException("", "");

            return editDenominationModel;
        }

        public DenominationServiceProviderDTO GetDenominationServiceProviderById(int id)
        {
            var denominatiaon = _denominationServiceProviderRepository.Getwhere(s => s.ID == id)
                .Select(dsp => new DenominationServiceProviderDTO
                {
                    Id = dsp.ID,
                    Balance = dsp.Balance,
                    DenominationId = dsp.DenominationID,
                    OldServiceId = (int)dsp.OldServiceID,
                    ProviderAmount = dsp.ProviderAmount,
                    ProviderCode = dsp.ProviderCode,
                    ProviderHasFees = dsp.ProviderHasFees,
                    Status = dsp.Status,
                    ServiceProviderId = dsp.ServiceProviderID,
                    ServiceProviderName = dsp.ServiceProvider.Name,
                    ServiceConfigerationId = dsp.ProviderServiceConfigerations.ServiceConfigerationID,
                    DenominationProviderConfigurationDto = dsp.DenominationProviderConfigerations.Select(psc => new DenominationProviderConfigurationDTO
                    {
                        ID = psc.ID,
                        Name = psc.Name,
                        Value = psc.Value,
                        DenominationProviderID = psc.DenominationProviderID
                    }).ToList()
                }).FirstOrDefault();

            return denominatiaon;
        }

        public void ChangeDenominationServiceProviderStatus(int id)
        {
            var current = _denominationServiceProviderRepository.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public DenominationParameterDTO GetDenominationParameterById(int id)
        {
            var denominatiaon = _denominationParamter.Getwhere(s => s.ID == id)
               .Select(dp => new DenominationParameterDTO
               {
                   Id = dp.ID,
                   DenominationID = dp.DenominationID,
                   Optional = dp.Optional,
                   Sequence = dp.Sequence,
                   ValidationExpression = dp.ValidationExpression,
                   ValidationMessage = dp.ValidationMessage,
                   DenominationParamID = dp.DenominationParamID,
                   Value = dp.Value,
                   ValueList = dp.ValueList
               }).FirstOrDefault();

            return denominatiaon;
        }

        public DenominationParameterDTO EditDenominationParameter(DenominationParameterDTO dp)
        {
            var current = _denominationParamter.GetById(dp.Id);

            current.Optional = dp.Optional;
            current.Sequence = dp.Sequence;
            current.ValidationExpression = dp.ValidationExpression;
            current.ValidationMessage = dp.ValidationMessage;
            current.DenominationParamID = dp.DenominationParamID;
            current.Value = dp.Value;
            current.ValueList = dp.ValueList;

            _unitOfWork.SaveChanges();

            return new DenominationParameterDTO
            {
                Id = current.ID,
                DenominationID = current.DenominationID,
                Optional = current.Optional,
                Sequence = current.Sequence,
                ValidationExpression = current.ValidationExpression,
                ValidationMessage = current.ValidationMessage,
                DenominationParamID = current.DenominationParamID,
                Value = current.Value,
                ValueList = current.ValueList
            };
        }

        public void DeleteDenominationParameter(int id)
        {
            _denominationParamter.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void EditDenominationReceiptData(DenominationReceiptDataDTO model)
        {
            var current = _denominationRecepitData.GetById(model.Id);
            current.Title = model.Title;
            current.Disclaimer = model.Disclaimer;
            current.Footer = model.Footer;
            _unitOfWork.SaveChanges();
        }

        public DenominationReceiptParamDTO GetDenominationReceiptParamById(int id)
        {
            return _denominationRecepitParam.Getwhere(x => x.ID == id).Select(x => new DenominationReceiptParamDTO()
            {
                Id = x.ID,
                DenominationID = x.DenominationID,
                Alignment = x.Alignment,
                Bold = x.Bold,
                ParameterID = x.ParameterID,
                Status = x.Status
            }).FirstOrDefault();
        }

        public void EditDenominationReceiptParam(DenominationReceiptParamDTO model)
        {
            var current = _denominationRecepitParam.GetById(model.Id);
            current.Alignment = model.Alignment;
            current.Bold = model.Bold;
            current.ParameterID = model.ParameterID;

            _unitOfWork.SaveChanges();
        }

        public void ChangeDenominationReceiptParamStatus(int id)
        {
            var current = _denominationRecepitParam.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public DenominationServiceProviderDTO AddDenominationServiceProvdier(DenominationServiceProviderDTO model)
        {
            var addedEntity = _denominationServiceProviderRepository.Add(new DenominationServiceProvider
            {
                DenominationID = model.DenominationId,
                Balance = model.Balance,
                ProviderAmount = model.ProviderAmount,
                ProviderCode = model.ProviderCode,
                ProviderHasFees = model.ProviderHasFees,
                OldServiceID = (int)model.OldServiceId,
                ServiceProviderID = model.ServiceProviderId,
                Status = model.Status,
                ProviderServiceConfigerations = new ProviderServiceConfigeration
                {
                    ServiceConfigerationID = model.ServiceConfigerationId
                }
            });

            _unitOfWork.SaveChanges();

            var id = addedEntity.ID;

            return _denominationServiceProviderRepository.Getwhere(x => x.ID == id).Select(d => new DenominationServiceProviderDTO
            {
                Id = d.ID,
                DenominationId = d.DenominationID,
                Balance = d.Balance,
                ProviderAmount = d.ProviderAmount,
                ProviderCode = d.ProviderCode,
                ProviderHasFees = d.ProviderHasFees,
                OldServiceId = (int)d.OldServiceID,
                ServiceProviderId = d.ServiceProviderID,
                ServiceProviderName = d.ServiceProvider.Name,
                Status = d.Status,
                ServiceConfigerationId = d.ProviderServiceConfigerations.ServiceConfigerationID
            }).FirstOrDefault();
        }

        public DenominationParameterDTO AddDenominationParameter(DenominationParameterDTO dp)
        {
            var addedEntity = _denominationParamter.Add(new DenominationParameter
            {
                Optional = dp.Optional,
                DenominationID = dp.DenominationID,
                Sequence = dp.Sequence,
                ValidationExpression = dp.ValidationExpression,
                ValidationMessage = dp.ValidationMessage,
                DenominationParamID = dp.DenominationParamID,
                Value = dp.Value,
                ValueList = dp.ValueList
            });

            _unitOfWork.SaveChanges();

            return new DenominationParameterDTO
            {
                Id = addedEntity.ID,
                DenominationID = addedEntity.DenominationID,
                Optional = addedEntity.Optional,
                Sequence = addedEntity.Sequence,
                ValidationExpression = addedEntity.ValidationExpression,
                ValidationMessage = addedEntity.ValidationMessage,
                DenominationParamID = addedEntity.DenominationParamID,
                Value = addedEntity.Value,
                ValueList = addedEntity.ValueList
            };
        }

        public void EditDenominationReceipt(DenominationReceiptDTO model)
        {
            var current = _denominationRecepitData.GetById(model.DenominationReceiptDataDTO.Id);
            current.Title = model.DenominationReceiptDataDTO.Title;
            current.Disclaimer = model.DenominationReceiptDataDTO.Disclaimer;
            current.Footer = model.DenominationReceiptDataDTO.Footer;

            _unitOfWork.SaveChanges();
            //DenomotionRevcpit param>>>>TODO Ebram


        }

        public void DeleteDenominationReceiptParam(int id)
        {
            _denominationRecepitParam.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public PagedResult<DenominationDTO> SearchDenominations(string serviceName, string serviceCode, string denomninationName, string denomniationCode, int page, int pageSize, string language)
        {
            var denomination = _denominationRepository.Getwhere(d =>
               (string.IsNullOrEmpty(serviceName) || (d.Service.Name.Contains(serviceName) || d.Service.ArName.Contains(serviceName)))
            && (string.IsNullOrEmpty(serviceCode) || d.Service.ID == int.Parse(serviceCode))
            && (string.IsNullOrEmpty(denomninationName) || d.Name.Contains(denomninationName))
            && (string.IsNullOrEmpty(denomniationCode) || d.ID == int.Parse(denomniationCode))
            ).Include(s => s.Service).ThenInclude(x => x.ServiceEntity)
               .Select(denomination => new
               {
                   Id = denomination.ID,
                   Name = denomination.Name,
                   APIValue = denomination.APIValue,
                   CurrencyID = (int)denomination.CurrencyID,
                   ServiceID = denomination.ServiceID,
                   ServiceName = language == "en" ? denomination.Service.Name : denomination.Service.ArName,
                   Status = denomination.Status,
                   ClassType = denomination.ClassType,
                   ServiceProviderId = denomination.DenominationServiceProviders.Select(s => s.ServiceProviderID).FirstOrDefault(),
                   Interval = denomination.Interval,
                   MaxValue = denomination.MaxValue,
                   MinValue = denomination.MinValue,
                   Inquirable = denomination.Inquirable,
                   Value = denomination.Value,
                   BillPaymentModeID = denomination.BillPaymentModeID,
                   BillPaymentModeName = language == "en" ? denomination.BillPaymentMode.Name : denomination.BillPaymentMode.ArName,
                   PaymentModeID = denomination.PaymentModeID,
                   PaymentModeName = language == "en" ? denomination.PaymentMode.Name : denomination.PaymentMode.ArName,
                   OldDenominationID = denomination.OldDenominationID,
                   ServiceEntity = denomination.Service.ServiceEntity.ArName,
                   CreationDate = denomination.CreationDate
               });

            var count = denomination.Count();

            var resultList = denomination.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<DenominationDTO>
            {
                Results = resultList.Select(denomination => new DenominationDTO
                {
                    Id = denomination.Id,
                    Name = denomination.Name,
                    APIValue = denomination.APIValue,
                    CurrencyID = (int)denomination.CurrencyID,
                    ServiceID = denomination.ServiceID,
                    ServiceName = denomination.ServiceName,
                    Status = denomination.Status,
                    ClassType = denomination.ClassType,
                    ServiceProviderId = denomination.ServiceProviderId,
                    Interval = denomination.Interval,
                    MaxValue = denomination.MaxValue,
                    MinValue = denomination.MinValue,
                    Inquirable = denomination.Inquirable,
                    Value = denomination.Value,
                    BillPaymentModeID = denomination.BillPaymentModeID,
                    BillPaymentModeName = denomination.BillPaymentModeName,
                    PaymentModeID = denomination.PaymentModeID,
                    PaymentModeName = denomination.PaymentModeName,
                    OldDenominationID = denomination.OldDenominationID,
                    ServiceEntity = denomination.ServiceEntity
                }).ToList(),
                PageCount = count
            };
        }

        public RestaurantDTO GetResturants(string code,string language)
        {
            return _resturant.Getwhere(x=>x.RestaurantCode==code).Select(x => new RestaurantDTO
            {
                ID = x.ID,
                RestaurantCode = x.RestaurantCode,
                RestaurantName = language == "en" ? x.RestaurantName : x.RestaurantNameAr,
                CreationDate = x.CreationDate
            }).FirstOrDefault();
        }
    }
}

