using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class TaxService : ITaxService
    {
        private readonly IBaseRepository<Tax, int> _taxes;
        private readonly IBaseRepository<DenominationTax, int> _denominationTax;
        private readonly IUnitOfWork _unitOfWork;

        public TaxService(
            IBaseRepository<Tax, int> taxes,
            IBaseRepository<DenominationTax, int> denominationTax,
            IUnitOfWork unitOfWork)
        {
            _taxes = taxes;
            _denominationTax = denominationTax;
            _unitOfWork = unitOfWork;
        }

        public void AddTax(TaxesDTO tax)
        {
            _taxes.Add(new Tax
            {
                TaxTypeID = tax.TaxesTypeID,
                Value = tax.Value,
                PaymentModeID = tax.PaymentModeID,
                Status = tax.Status,
                AmountFrom = tax.AmountFrom,
                AmountTo = tax.AmountTo,
                CreatedBy = tax.CreatedBy,
                StartDate = tax.StartDate,
                EndDate = tax.EndDate
            });

            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(int id)
        {
            var current = _taxes.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public void DeleteTax(int id)
        {
            _taxes.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void EditTax(TaxesDTO tax)
        {
            var current = _taxes.GetById(tax.ID);

            current.TaxTypeID = tax.TaxesTypeID;
            current.Value = tax.Value;
            current.PaymentModeID = tax.PaymentModeID;
            current.AmountFrom = tax.AmountFrom;
            current.AmountTo = tax.AmountTo;
            current.StartDate = tax.StartDate;
            current.EndDate = tax.EndDate;
            current.UpdatedBy = tax.CreatedBy;

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<TaxesDTO> GetDenominationTaxes(int denominationId, decimal originalAmount, out decimal sum, string language = "ar")
        {
            var denominationTaxes = _denominationTax.Getwhere(s => s.DenominationID == denominationId &&
                 s.Tax.Status == true
                && s.Tax.AmountFrom <= originalAmount
               && s.Tax.AmountTo >= originalAmount
               && s.Tax.StartDate <= DateTime.Today
               && s.Tax.EndDate >= DateTime.Today).Select(s => new
               {
                   s.Tax.TaxTypeID,
                   TaxTypeName = language == "en" ? s.Tax.TaxType.Name : s.Tax.TaxType.ArName,
                   Taxes = s.Tax.PaymentModeID == 1 ? s.Tax.Value
                   : s.Tax.PaymentModeID == 2 ? (s.Tax.Value *
                   (s.Denomination.Value > 0 ? (s.Denomination.Value * s.Denomination.Currency.Value) : originalAmount)) / 100
                   : 0
               }).ToList()
               .GroupBy(s => s.TaxTypeID)
               .Select(s => new TaxesDTO
               {
                   TaxesTypeID = s.Key,
                   TaxesTypeName = s.Select(f => f.TaxTypeName).FirstOrDefault(),
                   Taxes = s.Sum(f => f.Taxes)
               }).ToList();
            sum = denominationTaxes.Sum(s => s.Taxes);
            return denominationTaxes;
        }

        public TaxesDTO GetTaxById(int id)
        {
            var current = _taxes.Getwhere(tax => tax.ID == id).Select(tax => new TaxesDTO
            {
                ID = tax.ID,
                TaxesTypeID = tax.TaxTypeID,
                Value = tax.Value,
                PaymentModeID = tax.PaymentModeID,
                Status = tax.Status,
                AmountFrom = tax.AmountFrom,
                AmountTo = tax.AmountTo,
                StartDate = tax.StartDate,
                EndDate = tax.EndDate
            }).FirstOrDefault();
            return current;
        }

        public IEnumerable<TaxesDTO> GetTaxes(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar")
        {
            return GetDenominationTaxes(denominationId, originalAmount, out sum, "ar").ToList();
        }

        public PagedResult<TaxesDTO> GetTaxes(int page, int pageSize, string language)
        {
            var taxes = _taxes.Getwhere(x => true).Include(x => x.TaxType).Select(tax => new
            {
                ID = tax.ID,
                TaxesTypeID = tax.TaxTypeID,
                TaxesTypeName = language == "en" ? tax.TaxType.Name : tax.TaxType.ArName,
                Value = tax.Value,
                TaxRange = tax.Value + " [" + tax.AmountFrom.ToString() + " - " + tax.AmountTo + "] " + tax.PaymentMode.Name,
                PaymentModeID = tax.PaymentModeID,
                PaymentModeName = language == "en" ? tax.PaymentMode.Name : tax.PaymentMode.ArName,
                Status = tax.Status,
                CreatedBy = tax.CreatedBy,
                AmountFrom = tax.AmountFrom,
                AmountTo = tax.AmountTo,
                StartDate = tax.StartDate,
                EndDate = tax.EndDate,
                CreationDate = tax.CreationDate
            }).ToList();

            var count = taxes.Count();

            var resultList = taxes.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<TaxesDTO>
            {
                Results = resultList.Select(tax => new TaxesDTO
                {
                    ID = tax.ID,
                    TaxesTypeID = tax.TaxesTypeID,
                    TaxesTypeName = tax.TaxesTypeName,
                    Value = tax.Value,
                    TaxRange = tax.TaxRange,
                    PaymentModeID = tax.PaymentModeID,
                    PaymentModeName = tax.PaymentModeName,
                    Status = tax.Status,
                    CreatedBy = tax.CreatedBy,
                    AmountFrom = tax.AmountFrom,
                    AmountTo = tax.AmountTo,
                    StartDate = tax.StartDate,
                    EndDate = tax.EndDate
                }).ToList(),
                PageCount = count
            };
        }
    }
}
