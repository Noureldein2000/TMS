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
        private readonly IBaseRepository<AccountFee, int> _accountFees;
        private readonly IBaseRepository<AccountProfileFee, int> _accountProfileFee;
        private readonly IBaseRepository<DenominationFee, int> _denominationFee;

        //private readonly ApplicationDbContext _context;
        public FeesService(
            IBaseRepository<AccountFee, int> accountFees,
            IBaseRepository<AccountProfileFee, int> accountProfileFee,
            IBaseRepository<DenominationFee, int> denominationFee
            )
        {
            _accountFees = accountFees;
            _accountProfileFee = accountProfileFee;
            _denominationFee = denominationFee;
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
            var accountFees = _accountProfileFee.Getwhere(s => s.AccountProfileDenomination.DenominationID == denominationId &&
                           s.AccountProfileDenomination.AccountProfileID == accountProfileId && s.Fee.Status == true
                           && s.Fee.AmountFrom <= originalAmount
                          && s.Fee.AmountTo >= originalAmount
                          && s.Fee.StartDate <= DateTime.Today
                          && s.Fee.EndDate >= DateTime.Today).Select(s => new
                          {
                              s.Fee.FeesTypeID,
                              FeesTypeName = language == "en" ? s.Fee.FeesType.Name : s.Fee.FeesType.ArName,
                              Fees = s.Fee.PaymentModeID == 1 ? s.Fee.Value
                              : s.Fee.PaymentModeID == 2 ? (s.Fee.Value *
                              (s.AccountProfileDenomination.Denomination.Value > 0 ? (s.AccountProfileDenomination.Denomination.Value * s.AccountProfileDenomination.Denomination.Currency.Value) : originalAmount)) / 100
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
    }
}
