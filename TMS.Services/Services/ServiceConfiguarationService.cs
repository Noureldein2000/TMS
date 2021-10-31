using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class ServiceConfiguarationService : IServiceConfiguarationService
    {
        private readonly IBaseRepository<ServiceConfigeration, int> _serviceConfigurationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceConfiguarationService(
            IBaseRepository<ServiceConfigeration, int> serviceConfigurationRepository,
            IUnitOfWork unitOfWork
            )
        {
            _serviceConfigurationRepository = serviceConfigurationRepository;
            _unitOfWork = unitOfWork;
        }
        public ServiceConfigerationDTO AddServiceConfiguartions(ServiceConfigerationDTO model)
        {
            var addedEntity = _serviceConfigurationRepository.Add(new ServiceConfigeration
            {
                URL = model.URL,
                TimeOut = model.TimeOut,
                UserName = model.UserName,
                UserPassword = model.UserPassword,
            });

            if (model.ServiceConfigParms?.Count > 0 && model.ServiceConfigParms != null)
            {
                addedEntity.ServiceConfigParms = model.ServiceConfigParms.Select(x => new ServiceConfigParms
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList();
            }

            _unitOfWork.SaveChanges();
            return MapToDTO(addedEntity);
        }

        public void EditServiceConfiguartions(ServiceConfigerationDTO model)
        {
            var current = _serviceConfigurationRepository.GetById(model.Id);

            current.TimeOut = model.TimeOut;
            current.URL = model.URL;
            current.UserName = model.UserName;
            current.UserPassword = model.UserPassword;

            if (model.ServiceConfigParms?.Count > 0 && model.ServiceConfigParms != null)
            {
                current.ServiceConfigParms = model.ServiceConfigParms.Select(x => new ServiceConfigParms
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList();
            }

            _unitOfWork.SaveChanges();

        }

        public ServiceConfigerationDTO GetServiceConfiguartionById(int id)
        {
            return _serviceConfigurationRepository.Getwhere(x => x.ID == id).Select(sc => new ServiceConfigerationDTO
            {
                Id = sc.ID,
                URL = sc.URL,
                TimeOut = sc.TimeOut,
                UserName = sc.UserName,
                UserPassword = sc.UserPassword,
                ServiceConfigParms = sc.ServiceConfigParms.Select(x => new ServiceConfigParmsDTO
                {
                    Id = x.ID,
                    Name = x.Name,
                    Value = x.Value
                }).ToList()
            }).FirstOrDefault();
        }

        public PagedResult<ServiceConfigerationDTO> GetServiceConfiguartions(int page, int pageSize)
        {
            var serviceConfiguarations = _serviceConfigurationRepository.GetAll()
                .Select(sc => new
                {
                    Id = sc.ID,
                    Url = sc.URL,
                    TimeOut = sc.TimeOut,
                    Username = sc.UserName,
                    UserPassword = sc.UserPassword,
                    CreationDate = sc.CreationDate
                });

            var count = serviceConfiguarations.Count();
            var resultList = serviceConfiguarations.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<ServiceConfigerationDTO>
            {
                Results = resultList.Select(sc => new ServiceConfigerationDTO
                {
                    Id = sc.Id,
                    URL = sc.Url,
                    TimeOut = sc.TimeOut,
                    UserName = sc.Username,
                    UserPassword = sc.UserPassword,
                }).ToList(),
                PageCount = count
            };
        }

        private ServiceConfigerationDTO MapToDTO(ServiceConfigeration model)
        {
            return new ServiceConfigerationDTO
            {
                Id = model.ID,
                URL = model.URL,
                TimeOut = model.TimeOut,
                UserName = model.UserName,
                UserPassword = model.UserPassword,
                ServiceConfigParms = model.ServiceConfigParms?.Select(x => MapToDTO(x)).ToList()
            };
        }
        private ServiceConfigParmsDTO MapToDTO(ServiceConfigParms model)
        {
            return new ServiceConfigParmsDTO
            {
                Id = model.ID,
                Name = model.Name,
                Value = model.Value
            };
        }
    }
}

