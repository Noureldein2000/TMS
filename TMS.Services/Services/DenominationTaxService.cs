using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
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

        public void AddDenominationTaxes(AddDenominationTaxesDTO model)
        {
            _denominationTaxRepository.Add(new DenominationTax
            {
                DenominationID = model.DenominationId,
                TaxID = model.TaxId
            });
            _unitOfWork.SaveChanges();
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
                DenominationFullName = x.Denomination.Service.Name + " - " + x.Denomination.Name,
                CreationDate = x.CreationDate
            }).ToList();
        }
    }
}
