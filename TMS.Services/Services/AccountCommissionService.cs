using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class AccountCommissionService : IAccountCommissionService
    {
        private readonly IBaseRepository<AccountCommission, int> _accountCommission;
        private readonly IUnitOfWork _unitOfWork;

        public AccountCommissionService(
            IBaseRepository<AccountCommission, int> accountCommission,
            IUnitOfWork unitOfWork)
        {
            _accountCommission = accountCommission;
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
    }
}
