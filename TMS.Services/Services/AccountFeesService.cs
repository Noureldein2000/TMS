using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using TMS.Data;
using TMS.Data.Entities;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Repositories;


namespace TMS.Services.Services
{
    public class AccountFeesService : IAccountFeesService
    {
        private readonly IBaseRepository<Fee, int> _fees;
        private readonly IBaseRepository<AccountFee, int> _accountFees;
        private readonly IUnitOfWork _unitOfWork;

        //private readonly ApplicationDbContext _context;
        public AccountFeesService(
            IBaseRepository<Fee, int> fees,
            IBaseRepository<AccountFee, int> accountFees,
            IUnitOfWork unitOfWork)
        {
            _fees = fees;
            _accountFees = accountFees;
            _unitOfWork = unitOfWork;
        }
        public void AddAccountFees(AccountFeesDTO model)
        {
            if (_accountFees.Any(x => x.AccountID == model.AccountId && x.FeesID == model.FeesId && x.DenominationID == model.DenomiinationId))
            {
                throw new TMSException("Account-Fees already exist", "-5");
            }

            _accountFees.Add(new AccountFee
            {
                AccountID = model.AccountId,
                DenominationID = model.DenomiinationId,
                FeesID = model.FeesId
            });

            _unitOfWork.SaveChanges();
        }

        public void DeleteAccountFees(int id)
        {
            _accountFees.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public PagedResult<AccountFeesDTO> GetAccountFeesByAccountId(int accountId, int pageNumber, int pageSize, string language = "ar")
        {
            var accountFeesLst = _accountFees.Getwhere(x => x.AccountID == accountId).Select(x => new
            {
                Id = x.ID,
                FeesId = x.FeesID,
                FeesTypeId = x.Fee.FeesTypeID,
                FeesTypeName = language == "en" ? x.Fee.FeesType.Name : x.Fee.FeesType.ArName,
                FeesValue = x.Fee.Value,
                AmountFrom = x.Fee.AmountFrom,
                AmountTo = x.Fee.AmountTo,
                PaymentModeId = x.Fee.PaymentModeID,
                PaymentModeName = language == "en" ? x.Fee.PaymentMode.Name : x.Fee.PaymentMode.ArName,
                DenominationId = x.DenominationID,
                DenominationFullName = x.Denomination.Service.Name + " - " + x.Denomination.Name,
                CreationDate = x.CreationDate
            });

            var count = accountFeesLst.Count();

            var resultList = accountFeesLst.OrderByDescending(ar => ar.CreationDate)
          .Skip(pageNumber - 1).Take(pageSize)
          .ToList();

            //return accountLst;
            return new PagedResult<AccountFeesDTO>
            {
                Results = resultList.Select(x => new AccountFeesDTO
                {
                    Id = x.Id,
                    FeesId = x.FeesId,
                    FeesTypeId = x.FeesTypeId,
                    FeesTypeName = x.FeesTypeName,
                    AmountFrom = x.AmountFrom,
                    AmountTo = x.AmountTo,
                    FeesValue = x.FeesValue,
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
