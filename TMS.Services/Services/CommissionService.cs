using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Models.SwaggerModels;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly IBaseRepository<Commission, int> _commission;
        private readonly IBaseRepository<AccountCommission, int> _accountCommission;
        private readonly IBaseRepository<AccountProfileCommission, int> _accountProfileCommission;
        private readonly IBaseRepository<DenominationCommission, int> _denominationCommission;
        private readonly IUnitOfWork _unitOfWork;

        public CommissionService(
            IBaseRepository<Commission, int> commission,
            IBaseRepository<AccountCommission, int> accountCommission,
            IBaseRepository<AccountProfileCommission, int> accountProfileCommission,
            IBaseRepository<DenominationCommission, int> denominationCommission,
            IUnitOfWork unitOfWork)
        {
            _commission = commission;
            _accountCommission = accountCommission;
            _accountProfileCommission = accountProfileCommission;
            _denominationCommission = denominationCommission;
            _unitOfWork = unitOfWork;
        }

        public void AddCommission(CommissionDTO commission)
        {
            _commission.Add(new Commission
            {
                CommissionTypeID = commission.CommissionTypeID,
                Value = commission.Value,
                PaymentModeID = commission.PaymentModeID,
                Status = commission.Status,
                AmountFrom = commission.AmountFrom,
                AmountTo = commission.AmountTo,
                CreatedBy = commission.CreatedBy,
                StartDate = commission.StartDate,
                EndDate = commission.EndDate
            });

            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(int id)
        {
            var current = _commission.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public void DeleteCommission(int id)
        {
            _commission.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void EditCommission(CommissionDTO commission)
        {
            var current = _commission.GetById(commission.ID);

            current.CommissionTypeID = commission.CommissionTypeID;
            current.Value = commission.Value;
            current.PaymentModeID = commission.PaymentModeID;
            current.AmountFrom = commission.AmountFrom;
            current.AmountTo = commission.AmountTo;
            current.StartDate = commission.StartDate;
            current.EndDate = commission.EndDate;
            current.UpdatedBy = commission.CreatedBy;

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<CommissionDTO> GetAccountCommission(int denominationId, decimal originalAmount, int accountId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetAccountProfileCommission(int denominationId, decimal originalAmount, int accountProfileId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetCommission(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public PagedResult<CommissionDTO> GetCommission(int page, int pageSize, string language)
        {
            var commissions = _commission.Getwhere(x => true).Include(x => x.CommissionType).Select(com => new
            {
                ID = com.ID,
                CommissionTypeID = com.CommissionTypeID,
                CommissionTypeName = language == "en" ? com.CommissionType.Name : com.CommissionType.ArName,
                Value = com.Value,
                CommissionRange = com.Value + " [" + com.AmountFrom.ToString() + " - " + com.AmountTo + "] " + com.PaymentMode.Name,
                PaymentModeID = com.PaymentModeID,
                PaymentModeName = language == "en" ? com.PaymentMode.Name : com.PaymentMode.ArName,
                Status = com.Status,
                CreatedBy = com.CreatedBy,
                AmountFrom = com.AmountFrom,
                AmountTo = com.AmountTo,
                StartDate = com.StartDate,
                EndDate = com.EndDate,
                CreationDate = com.CreationDate
            }).ToList();

            var count = commissions.Count();

            var resultList = commissions.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<CommissionDTO>
            {
                Results = resultList.Select(com => new CommissionDTO
                {
                    ID = com.ID,
                    CommissionTypeID = com.CommissionTypeID,
                    CommissionTypeName = com.CommissionTypeName,
                    Value = com.Value,
                    CommissionRange = com.CommissionRange,
                    PaymentModeID = com.PaymentModeID,
                    PaymentModeName = com.PaymentModeName,
                    Status = com.Status,
                    CreatedBy = com.CreatedBy,
                    AmountFrom = com.AmountFrom,
                    AmountTo = com.AmountTo,
                    StartDate = com.StartDate,
                    EndDate = com.EndDate
                }).ToList(),
                PageCount = count
            };
        }

        public CommissionDTO GetCommissionById(int id)
        {
            return _commission.Getwhere(com => com.ID == id).Select(com => new CommissionDTO
            {
                ID = com.ID,
                CommissionTypeID = com.CommissionTypeID,
                Value = com.Value,
                PaymentModeID = com.PaymentModeID,
                Status = com.Status,
                AmountFrom = com.AmountFrom,
                AmountTo = com.AmountTo,
                StartDate = com.StartDate,
                EndDate = com.EndDate
            }).FirstOrDefault();
        }

        public IEnumerable<CommissionDTO> GetDenominationCommission(int denominationId, decimal originalAmount, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }
    }
}
