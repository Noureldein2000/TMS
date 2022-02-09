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
    public class DenominationCommissionService : IDenominationCommissionService
    {
        private readonly IBaseRepository<DenominationCommission, int> _denominationCommission;
        private readonly IUnitOfWork _unitOfWork;
        public DenominationCommissionService(
            IBaseRepository<DenominationCommission, int> denominationCommission,
            IUnitOfWork unitOfWork)
        {
            _denominationCommission = denominationCommission;
            _unitOfWork = unitOfWork;
        }

        public void AddDenominationCommission(AddDenominationCommissionDTO model)
        {
            if (_denominationCommission.Any(x => x.DenominationID == model.DenominationId && x.CommissionID == model.CommissionId))
            {
                throw new TMSException("Denomination-Commission already exist", "-5");
            }

            _denominationCommission.Add(new DenominationCommission
            {
                DenominationID = model.DenominationId,
                CommissionID = model.CommissionId
            });
            _unitOfWork.SaveChanges();
        }

        public void DeleteDenominationCommission(int id)
        {
            _denominationCommission.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<DenominationCommissionDTO> GetDeniminationCommissionsByDenominationId(int denominationId, string language)
        {
            return _denominationCommission.Getwhere(x => x.DenominationID == denominationId).Select(x => new DenominationCommissionDTO
            {
                Id = x.ID,
                CommissionId = x.CommissionID,
                CommissionTypeId = x.Commission.CommissionTypeID,
                CommissionTypeName = language == "en" ? x.Commission.CommissionType.Name : x.Commission.CommissionType.ArName,
                CommissionValue = x.Commission.Value,
                PaymentModeId = x.Commission.PaymentModeID,
                PaymentMode = language == "en" ? x.Commission.PaymentMode.Name : x.Commission.PaymentMode.ArName,
                DenominationId = x.DenominationID,
                Range = $"{x.Commission.AmountFrom} - { x.Commission.AmountTo}",
                CreationDate = x.CreationDate
            }).ToList();
        }
    }
}
