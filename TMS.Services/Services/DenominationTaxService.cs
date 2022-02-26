using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class DenominationTaxService : IDenominationTaxService
    {
        private readonly IBaseRepository<Denomination, int> _denominationRepository;
        private readonly IBaseRepository<DenominationTax, int> _denominationTaxRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DenominationTaxService(
            IBaseRepository<Denomination, int> denominationRepository,
            IBaseRepository<DenominationTax, int> denominationTaxRepository,
        IUnitOfWork unitOfWork
            )
        {
            _denominationRepository = denominationRepository;
            _denominationTaxRepository = denominationTaxRepository;
            _unitOfWork = unitOfWork;
        }

        public DenominationTaxesDTO AddDenominationTaxes(AddDenominationTaxesDTO model)
        {
            if (_denominationTaxRepository.Any(x => x.DenominationID == model.DenominationId && x.TaxID == model.TaxId))
            {
                throw new TMSException("Denomination-Taxes already exist", "-5");
            }

            var addedEntity = _denominationTaxRepository.Add(new DenominationTax
            {
                DenominationID = model.DenominationId,
                TaxID = model.TaxId
            });

            _unitOfWork.SaveChanges();

            var dto = _denominationTaxRepository.Getwhere(dc => dc.ID == addedEntity.ID).Select(x => new DenominationTaxesDTO
            {
                Id = x.ID,
                TaxId = x.TaxID,
                TaxTypeId = x.Tax.TaxTypeID,
                TaxTypeName = x.Tax.TaxType.Name,
                TaxValue = x.Tax.Value,
                PaymentModeId = x.Tax.PaymentModeID,
                PaymentMode = x.Tax.PaymentMode.Name,
                DenominationId = x.DenominationID,
                Range = $"{x.Tax.AmountFrom} - { x.Tax.AmountTo}",
                CreationDate = x.CreationDate
            }).FirstOrDefault();
            return dto;
        }

        public void DeleteDenominationTaxes(int id)
        {
            _denominationTaxRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<DenominationTaxesDTO> GetDeniminationTaxesByDenominationId(int denominationId, string language)
        {
            return _denominationTaxRepository.Getwhere(x => x.DenominationID == denominationId).Select(x => new DenominationTaxesDTO
            {
                Id = x.ID,
                TaxId = x.TaxID,
                TaxTypeId = x.Tax.TaxTypeID,
                TaxTypeName = language == "en" ? x.Tax.TaxType.Name : x.Tax.TaxType.ArName,
                TaxValue = x.Tax.Value,
                PaymentModeId = x.Tax.PaymentModeID,
                PaymentMode = language == "en" ? x.Tax.PaymentMode.Name : x.Tax.PaymentMode.ArName,
                DenominationId = x.DenominationID,
                AmountFrom = x.Tax.AmountFrom,
                AmountTo = x.Tax.AmountTo,
                CreationDate = x.CreationDate
            }).ToList();
        }
    }
}
