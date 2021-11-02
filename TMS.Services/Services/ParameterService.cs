using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IBaseRepository<Parameter, int> _serviceParameterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ParameterService(
            IBaseRepository<Parameter, int> serviceParameterRepository,
            IUnitOfWork unitOfWork
            )
        {
            _serviceParameterRepository = serviceParameterRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddParameter(ParameterDTO model)
        {
            _serviceParameterRepository.Add(new Parameter
            {
                Name = model.Name,
                ArName = model.NameAr,
                ProviderName = model.ProviderName
            });

            _unitOfWork.SaveChanges();
        }

        public void DeleteParameter(int id)
        {
            _serviceParameterRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void EditParameter(ParameterDTO model)
        {
            var current = _serviceParameterRepository.GetById(model.Id);
            current.Name = model.Name;
            current.ArName = model.NameAr;
            current.ProviderName = model.ProviderName;
            _unitOfWork.SaveChanges();

        }

        public ParameterDTO GetParamterById(int id)
        {
            return _serviceParameterRepository.Getwhere(x => x.ID == id).Select(x => new ParameterDTO()
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                ProviderName = x.ProviderName
            }).FirstOrDefault();
        }

        public PagedResult<ParameterDTO> GetParamters(int page, int pageSize, string language)
        {
            var paramters = _serviceParameterRepository.GetAll().Select(x => new
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                ProviderName = x.ProviderName,
                CreationDate = x.CreationDate
            });

            var count = paramters.Count();

            var resultList = paramters.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<ParameterDTO>
            {
                Results = resultList.Select(x => new ParameterDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    NameAr = x.NameAr,
                    ProviderName = x.ProviderName,
                }).ToList(),
                PageCount = count
            };
        }
    }
}
