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
            switch (dto.IdentifierType)
            {
                case LookupType.FeesType:
                    var fee = _feesTypeRepository.Add(new FeesType
                    {
                        Name = dto.Name,
                        ArName = dto.NameAr,
                    });
                    dto.Id = fee.ID;
                    break;
                case LookupType.CommissionType:
                    var commission = _commissionTypeRepository.Add(new CommissionType
                    {
                        Name = dto.Name,
                        ArName = dto.NameAr,
                    });
                    dto.Id = commission.ID;
                    break;
                case LookupType.TaxesType:
                    var tax = _taxesTypeRepository.Add(new TaxType
                    {
                        Name = dto.Name,
                        ArName = dto.NameAr,
                    });
                    dto.Id = tax.ID;
                    break;
                default:
                    throw new TMSException("Not supported type", "-1");
            }

            _unitOfWork.SaveChanges();
            return dto;
        }

        public void DeleteLookupType(int id, LookupType identifier)
        {
            switch (identifier)
            {
                case LookupType.FeesType:
                    _feesTypeRepository.Delete(id);
                    break;
                case LookupType.CommissionType:
                    _commissionTypeRepository.Delete(id);
                    break;
                case LookupType.TaxesType:
                    _taxesTypeRepository.Delete(id);
                    break;
            }

            _unitOfWork.SaveChanges();
        }

        public void EditLookupType(LookupTypeDTO dto)
        {
            switch (dto.IdentifierType)
            {
                case LookupType.FeesType:
                    var fee = _feesTypeRepository.GetById(dto.Id);
                    fee.Name = dto.Name;
                    fee.ArName = dto.NameAr;
                    break;
                case LookupType.CommissionType:
                    var commission = _commissionTypeRepository.GetById(dto.Id);
                    commission.Name = dto.Name;
                    commission.ArName = dto.NameAr;
                    break;
                case LookupType.TaxesType:
                    var tax = _taxesTypeRepository.GetById(dto.Id);
                    tax.Name = dto.Name;
                    tax.ArName = dto.NameAr;
                    break;
                default:
                    throw new TMSException("Not supported type", "-1");
            }
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<LookupTypeDTO> GetAllLookups(string language)
        {
            var dtos = new List<LookupTypeDTO>();

            var fees = _feesTypeRepository.GetAll().Select(x => new LookupTypeDTO
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                IdentifierType = LookupType.FeesType
            }).ToList();
            if(fees != null && fees.Count > 0)
            {
                dtos.AddRange(fees);
            }

            var commessions = _commissionTypeRepository.GetAll().Select(x => new LookupTypeDTO
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                IdentifierType = LookupType.CommissionType
            }).ToList();
            if (commessions != null && commessions.Count > 0)
            {
                dtos.AddRange(commessions);
            }
            
            var taxes = _taxesTypeRepository.GetAll().Select(x => new LookupTypeDTO
            {
                Id = x.ID,
                Name = x.Name,
                NameAr = x.ArName,
                IdentifierType = LookupType.TaxesType
            }).ToList();
            if (taxes != null && taxes.Count > 0)
            {
                dtos.AddRange(taxes);
            }

            return dtos;
        }

        public LookupTypeDTO GetLookupTypeById(int id, LookupType identifier)
        {
            switch (identifier)
            {
                case LookupType.FeesType:
                    return MapToDto(_feesTypeRepository.GetById(id));
                case LookupType.CommissionType:
                    return MapToDto(_commissionTypeRepository.GetById(id));
                case LookupType.TaxesType:
                    return MapToDto(_taxesTypeRepository.GetById(id));
                default:
                    throw new TMSException("Not supported type", "-1");
            }
        }

        private LookupTypeDTO MapToDto(ILookupType model)
        {
            return new LookupTypeDTO
            {
                NameAr = model.ArName,
                Id = model.ID,
                Name = model.Name,
            };
        }
    }
}
