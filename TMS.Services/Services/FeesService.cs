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
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class FeesService : IFeesService
    {
        private readonly IBaseRepository<Fee, int> _fees;
        private readonly IBaseRepository<AccountFee, int> _accountFees;
        private readonly IBaseRepository<AccountTypeProfileFee, int> _accountTypeProfileFee;
        private readonly IBaseRepository<DenominationFee, int> _denominationFee;
        private readonly IUnitOfWork _unitOfWork;

        //private readonly ApplicationDbContext _context;
        public FeesService(
            IBaseRepository<Fee, int> fees,
            IBaseRepository<AccountFee, int> accountFees,
            IBaseRepository<AccountTypeProfileFee, int> accountTypeProfileFee,
            IBaseRepository<DenominationFee, int> denominationFee,
            IUnitOfWork unitOfWork)
        {
            _fees = fees;
            _accountFees = accountFees;
            _accountTypeProfileFee = accountTypeProfileFee;
            _denominationFee = denominationFee;
            _unitOfWork = unitOfWork;
        }
        public void AddFee(FeesDTO fee)
        {
            _fees.Add(new Fee
            {
                FeesTypeID = fee.FeesTypeID,
                Value = fee.Value,
                PaymentModeID = fee.PaymentModeID,
                Status = fee.Status,
                AmountFrom = fee.AmountFrom,
                AmountTo = fee.AmountTo,
                CreatedBy = fee.CreatedBy,
                StartDate = fee.StartDate,
                EndDate = fee.EndDate
            });

            _unitOfWork.SaveChanges();
        }
        public void ChangeStatus(int id)
        {
            var current = _fees.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }
        public void DeleteFee(int id)
        {
            _fees.Delete(id);
            _unitOfWork.SaveChanges();
        }
        public void EditFee(FeesDTO fee)
        {
            var current = _fees.GetById(fee.ID);

            current.FeesTypeID = fee.FeesTypeID;
            current.Value = fee.Value;
            current.PaymentModeID = fee.PaymentModeID;
            current.AmountFrom = fee.AmountFrom;
            current.AmountTo = fee.AmountTo;
            current.StartDate = fee.StartDate;
            current.EndDate = fee.EndDate;
            //current.UpdatedBy = fee.CreatedBy;

            _unitOfWork.SaveChanges();
        }
        public IEnumerable<FeesDTO> GetAccountFees(int denominationId, decimal originalAmount, int accountId, out decimal sum, string language = "ar")
        {
            //var denominationIdParam = new SqlParameter("@DenominationID", denominationId);
            //var originalAmountParam = new SqlParameter("@OriginalAmount", originalAmount);
            //var accountIdParam = new SqlParameter("@AccountID", accountId);
            //var languageParam = new SqlParameter("@lang", language);
            //var toAccountIdParam = new SqlParameter("@ToAccountID", toAccountId);
            //var balanceTypeIdParam = new SqlParameter("@BalanceTypeID", balanceTypeId);

            //using (var cmd = _context.Database.GetDbConnection().CreateCommand())
            //{
            //    cmd.CommandText = "[dbo].[GetFees]";
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    if (cmd.Connection.State != ConnectionState.Open) cmd.Connection.Open();
            //    cmd.Parameters.Add(new SqlParameter("@DenominationID", denominationId));
            //    cmd.Parameters.Add(new SqlParameter("@OriginalAmount", originalAmount));
            //    cmd.Parameters.Add(new SqlParameter("@AccountID", accountId));
            //    cmd.Parameters.Add(new SqlParameter("@lang", language));
            //    cmd.Parameters.Add(new SqlParameter("@ToAccountID", toAccountId));
            //    cmd.Parameters.Add(new SqlParameter("@BalanceTypeID", balanceTypeId));
            //    DbDataReader rdr = cmd.ExecuteReader();
            //    while (rdr.Read())
            //    {
            //        var s = rdr.GetString(0);
            //        //obj_Service.Id = rdr["Id"].ToString();
            //    }
            //}
            //var result = _context.Database.ExecuteSqlCommand(" [dbo].[GetFees] @DenominationID, @OriginalAmount, @AccountID, @lang, @ToAccountID, @BalanceTypeID",
            //   denominationIdParam, originalAmountParam, accountIdParam, languageParam, toAccountIdParam, balanceTypeIdParam);
            var accountFees = _accountFees.Getwhere(s => s.DenominationID == denominationId &&
                             s.AccountID == accountId && s.Fee.Status == true
                             && s.Fee.AmountFrom <= originalAmount
                            && s.Fee.AmountTo >= originalAmount
                            && s.Fee.StartDate <= DateTime.Today
                            && s.Fee.EndDate >= DateTime.Today).Select(s => new
                            {
                                s.Fee.FeesTypeID,
                                FeesTypeName = language == "en" ? s.Fee.FeesType.Name : s.Fee.FeesType.ArName,
                                Fees = s.Fee.PaymentModeID == 1 ? s.Fee.Value
                                : s.Fee.PaymentModeID == 2 ? (s.Fee.Value *
                                (s.Denomination.Value > 0 ? (s.Denomination.Value * s.Denomination.Currency.Value) : originalAmount)) / 100
                                : 0
                            }).ToList()
                            .GroupBy(s => s.FeesTypeID)
                            .Select(s => new FeesDTO
                            {
                                FeesTypeID = s.Key,
                                FeesTypeName = s.Select(f => f.FeesTypeName).FirstOrDefault(),
                                Fees = s.Sum(f => f.Fees)
                            }).ToList();
            sum = accountFees.Sum(s => s.Fees);
            return accountFees;

        }
        public IEnumerable<FeesDTO> GetAccountProfileFees(int denominationId, decimal originalAmount, int accountProfileId, out decimal sum, string language = "ar")
        {
            var accountFees = _accountTypeProfileFee.Getwhere(s => s.AccountTypeProfileDenomination.DenominationID == denominationId &&
                           s.AccountTypeProfileDenomination.AccountTypeProfileID == accountProfileId && s.Fee.Status == true
                           && s.Fee.AmountFrom <= originalAmount
                          && s.Fee.AmountTo >= originalAmount
                          && s.Fee.StartDate <= DateTime.Today
                          && s.Fee.EndDate >= DateTime.Today).Select(s => new
                          {
                              s.Fee.FeesTypeID,
                              FeesTypeName = language == "en" ? s.Fee.FeesType.Name : s.Fee.FeesType.ArName,
                              Fees = s.Fee.PaymentModeID == 1 ? s.Fee.Value
                              : s.Fee.PaymentModeID == 2 ? (s.Fee.Value *
                              (s.AccountTypeProfileDenomination.Denomination.Value > 0 ? (s.AccountTypeProfileDenomination.Denomination.Value * s.AccountTypeProfileDenomination.Denomination.Currency.Value) : originalAmount)) / 100
                              : 0
                          }).ToList()
                          .GroupBy(s => s.FeesTypeID)
                          .Select(s => new FeesDTO
                          {
                              FeesTypeID = s.Key,
                              FeesTypeName = s.Select(f => f.FeesTypeName).FirstOrDefault(),
                              Fees = s.Sum(f => f.Fees)
                          }).ToList();
            sum = accountFees.Sum(s => s.Fees);
            return accountFees;
        }
        public IEnumerable<FeesDTO> GetDenominationFees(int denominationId, decimal originalAmount, out decimal sum, string language = "ar")
        {
            var denominationFees = _denominationFee.Getwhere(s => s.DenominationID == denominationId &&
                s.Fee.Status == true
               && s.Fee.AmountFrom <= originalAmount
              && s.Fee.AmountTo >= originalAmount
              && s.Fee.StartDate <= DateTime.Today
              && s.Fee.EndDate >= DateTime.Today).Select(s => new
              {
                  s.Fee.FeesTypeID,
                  FeesTypeName = language == "en" ? s.Fee.FeesType.Name : s.Fee.FeesType.ArName,
                  Fees = s.Fee.PaymentModeID == 1 ? s.Fee.Value
                  : s.Fee.PaymentModeID == 2 ? (s.Fee.Value *
                  (s.Denomination.Value > 0 ? (s.Denomination.Value * s.Denomination.Currency.Value) : originalAmount)) / 100
                  : 0
              }).ToList()
              .GroupBy(s => s.FeesTypeID)
              .Select(s => new FeesDTO
              {
                  FeesTypeID = s.Key,
                  FeesTypeName = s.Select(f => f.FeesTypeName).FirstOrDefault(),
                  Fees = s.Sum(f => f.Fees)
              }).ToList();
            sum = denominationFees.Sum(s => s.Fees);
            return denominationFees;
        }
        public FeesDTO GetFeeById(int id)
        {
            var current = _fees.Getwhere(fee => fee.ID == id).Select(fee => new FeesDTO
            {
                ID = fee.ID,
                FeesTypeID = fee.FeesTypeID,
                Value = fee.Value,
                PaymentModeID = fee.PaymentModeID,
                Status = fee.Status,
                AmountFrom = fee.AmountFrom,
                AmountTo = fee.AmountTo,
                StartDate = fee.StartDate,
                EndDate = fee.EndDate
            }).FirstOrDefault();
            return current;
        }
        public IEnumerable<FeesDTO> GetFees(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar")
        {
            var accountFees = GetAccountFees(denominationId, originalAmount, accountId, out sum, "ar").ToList();
            if (accountFees.Count == 0)
            {
                var accountProfileFees = GetAccountProfileFees(denominationId, originalAmount, accountProfileId, out sum, "ar").ToList();
                if (accountProfileFees.Count == 0)
                {
                    return GetDenominationFees(denominationId, originalAmount, out sum, "ar").ToList();
                }
                return accountProfileFees;
            }
            return accountFees;
        }
        public PagedResult<FeesDTO> GetFees(int page, int pageSize, string language)
        {
            var fees = _fees.Getwhere(x => true).Include(x => x.FeesType).Select(fee => new
            {
                ID = fee.ID,
                FeesTypeID = fee.FeesTypeID,
                FeesTypeName = language == "en" ? fee.FeesType.Name : fee.FeesType.ArName,
                Value = fee.Value,
                FeeRange = $"{fee.Value } [ {fee.AmountFrom} - { fee.AmountTo} ] {fee.PaymentMode.Name}",
                PaymentModeID = fee.PaymentModeID,
                PaymentModeName = language == "en" ? fee.PaymentMode.Name : fee.PaymentMode.ArName,
                Status = fee.Status,
                CreatedBy = fee.CreatedBy,
                AmountFrom = fee.AmountFrom,
                AmountTo = fee.AmountTo,
                StartDate = fee.StartDate,
                EndDate = fee.EndDate,
                CreationDate = fee.CreationDate
            }).ToList();

            var count = fees.Count();

            var resultList = fees.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<FeesDTO>
            {
                Results = resultList.Select(fee => new FeesDTO
                {
                    ID = fee.ID,
                    FeesTypeID = fee.FeesTypeID,
                    FeesTypeName = fee.FeesTypeName,
                    Value = fee.Value,
                    FeeRange = fee.FeeRange,
                    PaymentModeID = fee.PaymentModeID,
                    PaymentModeName = fee.PaymentModeName,
                    Status = fee.Status,
                    CreatedBy = fee.CreatedBy,
                    AmountFrom = fee.AmountFrom,
                    AmountTo = fee.AmountTo,
                    StartDate = fee.StartDate,
                    EndDate = fee.EndDate
                }).ToList(),
                PageCount = count
            };
        }
    }
}
