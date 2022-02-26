using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IBaseRepository<ServiceProvider, int> _serviceProviderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceProviderService(
             IBaseRepository<ServiceProvider, int> serviceProviderRepository,
            IUnitOfWork unitOfWork
            )
        {
            _serviceProviderRepository = serviceProviderRepository;
            _unitOfWork = unitOfWork;
        }

        public ServiceProviderDTO AddServiceProviders(ServiceProviderDTO model)
        {
            var addedEntity = _serviceProviderRepository.Add(new ServiceProvider
            {
                Name = model.Name,
            });

            _unitOfWork.SaveChanges();
            model.Id = addedEntity.ID;
            return model;
        }

        public void DeleteServiceProviders(int id)
        {
            _serviceProviderRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void EditServiceProviders(ServiceProviderDTO model)
        {
            var current = _serviceProviderRepository.GetById(model.Id);
            current.Name = model.Name;
            _unitOfWork.SaveChanges();
        }

        public ServiceProviderDTO GetServiceProviderById(int id)
        {
            var entity = _serviceProviderRepository.GetById(id);
            return new ServiceProviderDTO() { Id = entity.ID, Name = entity.Name };
        }

        public PagedResult<ServiceProviderDTO> GetServiceProviders(int page, int pageSize)
        {
            var serviceProviders = _serviceProviderRepository.GetAll().Select(x => new
            {
                Id = x.ID,
                Name = x.Name,
                CreationDate = x.CreationDate
            }).ToList();

            var count = serviceProviders.Count();

            var resultList = serviceProviders.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<ServiceProviderDTO>
            {
                Results = resultList.Select(x => new ServiceProviderDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList(),
                PageCount = count
            };

        }
    }
}
