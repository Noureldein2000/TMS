using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly IBaseRepository<Service, int> _serviceRepository;
        private readonly IBaseRepository<Denomination, int> _denominationRepository;
        private readonly IBaseRepository<ServiceCategory, int> _serviceCategoryRepository;
        private readonly IBaseRepository<ServiceEntity, int> _serviceEntityRepository;
        private readonly IBaseRepository<ServiceType, int> _serviceTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(
        IBaseRepository<Denomination, int> denominationRepository,
        IBaseRepository<Service, int> serviceRepository,
        IBaseRepository<ServiceCategory, int> serviceCategoryRepository,
        IBaseRepository<ServiceEntity, int> serviceEntityRepository,
        IBaseRepository<ServiceType, int> serviceTypeRepository,
        IUnitOfWork unitOfWork
            )
        {
            _serviceRepository = serviceRepository;
            _denominationRepository = denominationRepository;
            _serviceCategoryRepository = serviceCategoryRepository;
            _serviceEntityRepository = serviceEntityRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddService(AdminServiceDTO service)
        {
            _serviceRepository.Add(new Service()
            {
                Name = service.Name,
                ArName = service.ArName,
                ServiceTypeID = service.ServiceTypeID,
                ServiceCategoryID = service.ServiceCategoryID,
                Status = service.Status,
                Code = service.Code,
                ServiceEntityID = service.ServiceEntityID,
                PathClass = service.PathClass,
                ClassType = service.ClassType
            });

            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(int id)
        {
            var current = _serviceRepository.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public void EditService(AdminServiceDTO service)
        {
            var current = _serviceRepository.GetById(service.Id);

            //current.Status = service.Status;
            current.Name = service.Name;
            current.ArName = service.ArName;
            current.ServiceEntityID = service.ServiceEntityID;
            current.ServiceTypeID = service.ServiceTypeID;
            current.ServiceCategoryID = service.ServiceCategoryID;
            current.Code = service.Code;
            current.PathClass = service.PathClass;
            current.ClassType = service.ClassType;

            _unitOfWork.SaveChanges();
        }

        public AdminServiceDTO GetServiceById(int id)
        {
            return _serviceRepository.Getwhere(x => x.ID == id).Select(x => new AdminServiceDTO
            {
                Id = x.ID,
                Name = x.Name,
                ArName = x.ArName,
                ServiceTypeID = x.ServiceTypeID,
                ServiceTypeName = x.ServiceType.Name,
                ServiceCategoryID = x.ServiceCategoryID,
                ServiceCategoryName = x.ServiceCategory.Name,
                Status = x.Status,
                Code = x.Code,
                ServiceEntityID = x.ServiceEntityID,
                ServiceEntityName = x.ServiceEntity.Name,
                PathClass = x.PathClass,
                ClassType = x.ClassType,
                CreationDate = x.CreationDate
            }).FirstOrDefault();
        }

        public IEnumerable<ServiceCategoryDTO> GetServiceCategories()
        {
            return _serviceCategoryRepository.GetAll().Select(x => new ServiceCategoryDTO
            {
                Id = x.ID,
                Name = x.Name,
                ArName = x.ArName
            });
        }

        public IEnumerable<ServiceEntityDTO> GetServiceEntities()
        {
            return _serviceEntityRepository.GetAll().Select(x => new ServiceEntityDTO
            {
                Id = x.ID,
                Name = x.Name,
                ArName = x.ArName
            });
        }

        public PagedResult<AdminServiceDTO> GetServices(int pageNumber, int pageSize, string language = "ar")
        {
            var serviceList = _serviceRepository.Getwhere(x => true).Select(x => new
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                ServiceTypeID = x.ServiceTypeID,
                ServiceTypeName = x.ServiceType.Name,
                ServiceCategoryID = x.ServiceCategoryID,
                ServiceCategoryName = language == "en" ? x.ServiceCategory.Name : x.ServiceCategory.ArName,
                Status = x.Status,
                Code = x.Code,
                ServiceEntityID = x.ServiceEntityID,
                ServiceEntityName = language == "en" ? x.ServiceEntity.Name : x.ServiceEntity.ArName,
                PathClass = x.PathClass,
                ClassType = x.ClassType,
                CreationDate = x.CreationDate
            });

            var count = serviceList.Count();

            var resultList = serviceList.OrderByDescending(ar => ar.CreationDate)
          .Skip(pageNumber - 1).Take(pageSize)
          .ToList();

            return new PagedResult<AdminServiceDTO>
            {
                Results = resultList.Select(x => new AdminServiceDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    ArName = x.NameAr,
                    ServiceTypeID = x.ServiceTypeID,
                    ServiceTypeName = x.ServiceTypeName,
                    ServiceCategoryID = x.ServiceCategoryID,
                    ServiceCategoryName = x.ServiceCategoryName,
                    Status = x.Status,
                    Code = x.Code,
                    ServiceEntityID = x.ServiceEntityID,
                    ServiceEntityName = x.ServiceEntityName,
                    PathClass = x.PathClass,
                    ClassType = x.ClassType,
                    CreationDate = x.CreationDate
                }).ToList(),
                PageCount = count
            };
        }

        public IEnumerable<ServiceTypesDTO> GetServiceTypes()
        {
            return _serviceTypeRepository.GetAll().Select(x => new ServiceTypesDTO
            {
                Id = x.ID,
                Name = x.Name,
            });
        }

    }
}
