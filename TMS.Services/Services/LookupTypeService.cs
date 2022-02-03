using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class LookupTypeService : ILookupTypeService
    {
        private readonly IBaseRepository<FeesType, int> _feesTypeRepository;
        private readonly IBaseRepository<TaxType, int> _taxesTypeRepository;
        private readonly IBaseRepository<CommissionType, int> _commissionTypeRepository;
        private readonly IBaseRepository<ServiceType, int> _serviceTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LookupTypeService(
        IBaseRepository<FeesType, int> feesTypeRepository,
        IBaseRepository<TaxType, int> taxesTypeRepository,
        IBaseRepository<CommissionType, int> commissionTypeRepository,
        IBaseRepository<ServiceType, int> serviceTypeRepository,
        IUnitOfWork unitOfWork
            )
        {
            _feesTypeRepository = feesTypeRepository;
            _taxesTypeRepository = taxesTypeRepository;
            _commissionTypeRepository = commissionTypeRepository;
            _serviceTypeRepository = serviceTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public LookupTypeDTO AddLookupType(LookupTypeDTO dto)
        {
            dynamic addedEntity = null;

            switch (dto.IdentifierType)
            {
                case 1:
                    addedEntity = (FeesType)_feesTypeRepository.Add(new FeesType
                    {
                        Name = dto.Name,
                        ArName = dto.NameAr,
                    });
                    break;
                case 2:
                    addedEntity = (CommissionType)_commissionTypeRepository.Add(new CommissionType
                    {
                        Name = dto.Name,
                        ArName = dto.NameAr,
                    });
                    break;
                case 3:
                    addedEntity = (TaxType)_taxesTypeRepository.Add(new TaxType
                    {
                        Name = dto.Name,
                        ArName = dto.NameAr,
                    });
                    break;
            }

            _unitOfWork.SaveChanges();

            return MapToDto(addedEntity);
        }

        public IEnumerable<LookupTypeDTO> GetAllLookups(string language)
        {
            List<LookupTypeDTO> dtos;

            dtos = _feesTypeRepository.GetAll().Select(x => new LookupTypeDTO
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                IdentifierType = 1
            }).ToList();

            dtos.AddRange(_commissionTypeRepository.GetAll().Select(x => new LookupTypeDTO
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                IdentifierType = 2
            }).ToList());

            dtos.AddRange(_taxesTypeRepository.GetAll().Select(x => new LookupTypeDTO
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                IdentifierType = 3
            }).ToList());

            return dtos;
        }

        private LookupTypeDTO MapToDto(dynamic model)
        {
            return new LookupTypeDTO
            {
                Id = model.ID,
                Name = model.Name,
                NameAr = model.ArName,
            };
        }
    }
}
