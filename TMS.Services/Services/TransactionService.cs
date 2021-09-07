using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Repositories;


namespace TMS.Services.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBaseRepository<Request, int> _requests;
        private readonly IBaseRepository<Transaction, int> _transactions;
        private readonly IBaseRepository<AccountTransactionCommission, int> _accountTransactionCommission;
        private readonly IBaseRepository<AccountCommission, int> _accountCommission;
        private readonly IBaseRepository<AccountProfileCommission, int> _accountProfileCommission;
        private readonly IBaseRepository<DenominationCommission, int> _denominationCommission;
        //private readonly IBaseRepository<Invoice, int> _invoice;
        private readonly IUnitOfWork _unitOfWork;
        public TransactionService(IBaseRepository<Request, int> requests,
            IBaseRepository<Transaction, int> transactions,
            IBaseRepository<AccountTransactionCommission, int> accountTransactionCommission,
            IBaseRepository<AccountCommission, int> accountCommission,
            IBaseRepository<AccountProfileCommission, int> accountProfileCommission,
            IBaseRepository<DenominationCommission, int> denominationCommission,
            //IBaseRepository<Invoice, int> invoice,
            IUnitOfWork unitOfWork)
        {
            _requests = requests;
            _transactions = transactions;
            _accountTransactionCommission = accountTransactionCommission;
            _accountCommission = accountCommission;
            //_invoice = invoice;
            _unitOfWork = unitOfWork;
            _accountProfileCommission = accountProfileCommission;
            _denominationCommission = denominationCommission;
        }

        public void AddCommission(int transactionId, int? accountId, int denominationId, decimal originalAmount, int? accountProfileId)
        {
            var commission = GetTotalCommissions(denominationId, originalAmount, accountId, accountProfileId);
            _accountTransactionCommission.Add(new AccountTransactionCommission
            {
                Commission = commission,
                TransactionID = transactionId,
            });
            _unitOfWork.SaveChanges();
        }

        public void AddInvoice(int requestId, decimal amount, int userId, string billingAccount, decimal fees, string billingInfo)
        {

        }

        public int AddRequest(RequestDTO model)
        {
            //var sequence = _requests.Max(S => S.UUID);

            var sequence = "15151";
            var request = _requests.Add(new Request
            {
                AccountID = model.AccountId,
                Amount = model.Amount,
                ChannelID = model.ChannelID,
                BillingAccount = model.BillingAccount,
                ServiceDenominationID = model.DenominationId,
                StatusID = 1,
                UUID = model.HostTransactionId == "0" ? sequence : model.HostTransactionId
            });
            _unitOfWork.SaveChanges();
            return request.ID;
        }

        public int AddTransaction(int? accountIdFrom, decimal amount, int denominationId, decimal originalAmount, decimal fees, string originalTrx, int? accountIdTo, int? invoiceId, int? requestId)
        {
            var sequence = "15151";
            var transaction = _transactions.Add(new Transaction
            {
                TransactionID = $"MOMKN-{denominationId}-{sequence}",
                AccountIDFrom = accountIdFrom,
                Fees = fees,
                InvoiceID = invoiceId,
                OriginalAmount = originalAmount,
                RequestID = requestId,
                TotalAmount = amount,
                TransactionType = denominationId,
            });
            _unitOfWork.SaveChanges();
            return transaction.ID;
        }

        public decimal GetTotalCommissions(int denominationId, decimal originalAmount, int? accountId, int? accountProfileId)
        {
            var commission = GetAccountCommission(denominationId, originalAmount, accountId.Value);
            if (commission == 0)
            {
                var accountProfileCommission = GetAccountProfileCommission(denominationId, originalAmount, accountProfileId.Value);
                if (accountProfileCommission == 0)
                {
                    return GetDenominationCommission(denominationId, originalAmount);
                }
                return accountProfileCommission;
            }
            return commission;
        }

        private decimal GetAccountCommission(int denominationId, decimal originalAmount, int accountId)
        {
            return _accountCommission.Getwhere(a => a.DenominationID == denominationId
                && a.AccountID == accountId && a.Commission.Status == true
                && a.Commission.AmountFrom <= originalAmount
                && a.Commission.AmountTo >= originalAmount
                && a.Commission.StartDate <= DateTime.Today
                && a.Commission.EndDate >= DateTime.Today
                ).Select(s => new
                {
                    s.Commission.CommissionTypeID,
                    Commission = s.Commission.PaymentModeID == 1 ? s.Commission.Value
                  : s.Commission.PaymentModeID == 2 ? (s.Commission.Value *
                  (s.Denomination.Value > 0 ? (s.Denomination.Value * s.Denomination.Currency.Value) : originalAmount)) / 100
                  : 0
                }).ToList()
                .GroupBy(s => s.CommissionTypeID)
                  .Sum(s => s.Sum(ss => ss.Commission));

        }
        private decimal GetAccountProfileCommission(int denominationId, decimal originalAmount, int accountProfileId)
        {
            return _accountProfileCommission.Getwhere(a => a.AccountProfileDenomination.DenominationID == denominationId
                && a.AccountProfileDenomination.AccountProfileID == accountProfileId && a.Commission.Status == true
                && a.Commission.AmountFrom <= originalAmount
                && a.Commission.AmountTo >= originalAmount
                && a.Commission.StartDate <= DateTime.Today
                && a.Commission.EndDate >= DateTime.Today
                ).Select(s => new
                {
                    s.Commission.CommissionTypeID,
                    Commission = s.Commission.PaymentModeID == 1 ? s.Commission.Value
                  : s.Commission.PaymentModeID == 2 ? (s.Commission.Value *
                  (s.AccountProfileDenomination.Denomination.Value > 0 ? (s.AccountProfileDenomination.Denomination.Value * s.AccountProfileDenomination.Denomination.Currency.Value) : originalAmount)) / 100
                  : 0
                }).ToList()
                .GroupBy(s => s.CommissionTypeID)
                  .Sum(s => s.Sum(ss => ss.Commission));
        }
        private decimal GetDenominationCommission(int denominationId, decimal originalAmount)
        {
            return _denominationCommission.Getwhere(a => a.DenominationID == denominationId
                && a.Commission.Status == true
                && a.Commission.AmountFrom <= originalAmount
                && a.Commission.AmountTo >= originalAmount
                && a.Commission.StartDate <= DateTime.Today
                && a.Commission.EndDate >= DateTime.Today
                ).Select(s => new
                {
                    s.Commission.CommissionTypeID,
                    Commission = s.Commission.PaymentModeID == 1 ? s.Commission.Value
                  : s.Commission.PaymentModeID == 2 ? (s.Commission.Value *
                  (s.Denomination.Value > 0 ? (s.Denomination.Value * s.Denomination.Currency.Value) : originalAmount)) / 100
                  : 0
                }).ToList()
                .GroupBy(s => s.CommissionTypeID)
                  .Sum(s => s.Sum(ss => ss.Commission));
        }
        public void UpdateRequest(int? transactionId, int requestId, string RRN, int requestStatus, int userId, int providerServiceRequestId)
        {
            var request = _requests.GetById(requestId);
            //if(transactionId.HasValue)
            request.TransactionID = transactionId;
            request.RRN = RRN;
            request.StatusID = 2;
            request.UserID = userId;
            request.ProviderServiceRequestID = providerServiceRequestId;
            request.ResponseDate = DateTime.Now;
            _unitOfWork.SaveChanges();
        }

        public void UpdateRequestStatus(int requestId, int requestStatus)
        {
            var request = _requests.GetById(requestId);
            request.StatusID = requestStatus;
            _unitOfWork.SaveChanges();
        }

        public bool IsIntervalTransationExist(int accountId, int denominationId, string billingAccount, decimal amount)
        {
            var statusAllowd = new List<int?> { 1, 2, 3 };

            return _requests.Any(s => s.ServiceDenominationID == denominationId
                         && s.BillingAccount == billingAccount
                         && s.Amount == amount
                         && statusAllowd.Contains(s.StatusID)
                         && s.AccountID == accountId
                         && EF.Functions.DateDiffSecond(s.CreationDate, DateTime.Now) < s.Denomination.Interval * 60);
        }

        public bool IsRequestUUIDExist(int accountId, string UUID)
        {
            return _requests.Any(s => s.AccountID == accountId && s.UUID == UUID);
        }
    }
}
