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

        public void AddAccountCommission(AccountCommissionDTO model)
        {
            _accountCommission.Add(new AccountCommission
            {
                AccountID = model.AccountId,
                DenominationID = model.DenomiinationId,
                CommissionID = model.CommissionId
            });

            _unitOfWork.SaveChanges();
        }

        public void DeleteAccountCommission(int id)
        {
            _accountCommission.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<CommissionDTO> GetAccountCommission(int denominationId, decimal originalAmount, int accountId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public PagedResult<AccountCommissionDTO> GetAccountCommissionByAccountId(int accountId, int pageNumber, int pageSize, string language = "ar")
        {
            var accountFeesLst = _accountCommission.Getwhere(x => x.AccountID == accountId).Select(x => new
            {
                Id = x.ID,
                CommissionId = x.CommissionID,
                CommissionTypeId = x.Commission.CommissionTypeID,
                CommissionTypeName = language == "en" ? x.Commission.CommissionType.Name : x.Commission.CommissionType.ArName,
                CommissionValue = x.Commission.Value,
                PaymentModeId = x.Commission.PaymentModeID,
                PaymentModeName = language == "en" ? x.Commission.PaymentMode.Name : x.Commission.PaymentMode.ArName,
                DenominationId = x.DenominationID,
                DenominationFullName = x.Denomination.Service.Name + " - " + x.Denomination.Name,
                CreationDate = x.CreationDate
            });

            var count = accountFeesLst.Count();

            var resultList = accountFeesLst.OrderByDescending(ar => ar.CreationDate)
          .Skip(pageNumber - 1).Take(pageSize)
          .ToList();

            //return accountLst;
            return new PagedResult<AccountCommissionDTO>
            {
                Results = resultList.Select(x => new AccountCommissionDTO
                {
                    Id = x.Id,
                    CommissionId = x.CommissionId,
                    CommissionTypeId = x.CommissionTypeId,
                    CommissionTypeName = x.CommissionTypeName,
                    CommissionValue = x.CommissionValue,
                    PaymentModeId = x.PaymentModeId,
                    PaymentMode = x.PaymentModeName,
                    DenomiinationId = x.DenominationId,
                    DenominationFullName = x.DenominationFullName,
                    CreationDate = x.CreationDate
                }).ToList(),
                PageCount = count
            };
        }

        public IEnumerable<CommissionDTO> GetAccountProfileCommission(int denominationId, decimal originalAmount, int accountProfileId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetCommission(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetCommission()
        {
            return _commission.Getwhere(x => true).Include(x => x.CommissionType).Select(com => new CommissionDTO()
            {
                ID = com.ID,
                CommissionTypeID = com.CommissionTypeID,
                CommissionTypeName = com.CommissionType.ArName,
                Value = com.Value,
                CommissionRange = com.Value + " [" + com.AmountFrom.ToString() + " - " + com.AmountTo + "] " + com.PaymentMode.Name,
                PaymentModeID = com.PaymentModeID,
                Status = com.Status,
                AmountFrom = com.AmountFrom,
                AmountTo = com.AmountTo
            }).ToList();
        }

        public IEnumerable<CommissionDTO> GetDenominationCommission(int denominationId, decimal originalAmount, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }
    }
}
