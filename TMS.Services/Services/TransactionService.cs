using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TMS.Data;
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
        private readonly IBaseRepository<TransactionReceipt, int> _transactionReceipt;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        //private readonly IBaseRepository<Invoice, int> _invoice;
        private readonly IUnitOfWork _unitOfWork;
        public TransactionService(IBaseRepository<Request, int> requests,
            IBaseRepository<Transaction, int> transactions,
            IBaseRepository<AccountTransactionCommission, int> accountTransactionCommission,
            IBaseRepository<AccountCommission, int> accountCommission,
            IBaseRepository<AccountProfileCommission, int> accountProfileCommission,
            IBaseRepository<DenominationCommission, int> denominationCommission,
            IBaseRepository<TransactionReceipt, int> transactionReceipt,
            IConfiguration configuration,
            ApplicationDbContext context,
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
            _configuration = configuration;
            _context = context;
            _transactionReceipt = transactionReceipt;
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



        public int AddRequest(RequestDTO model)
        {
            var p = new SqlParameter("@result", SqlDbType.Int);
            p.Direction = ParameterDirection.Output;
            _context.Database.ExecuteSqlRaw("set @result = next value for Request_seq", p);
            var nextVal = (int)p.Value;

            var request = _requests.Add(new Request
            {
                AccountID = model.AccountId,
                Amount = model.Amount,
                ChannelID = model.ChannelID,
                BillingAccount = model.BillingAccount,
                ServiceDenominationID = model.DenominationId,
                StatusID = 1,
                UUID = model.HostTransactionId == "0" ? nextVal.ToString() : model.HostTransactionId,
                ResponseDate = DateTime.Now
            });
            _unitOfWork.SaveChanges();
            return request.ID;
        }

        public int AddTransaction(int? accountIdFrom, decimal amount, int denominationId, decimal originalAmount, decimal fees, string originalTrx, int? accountIdTo, int? invoiceId, int? requestId)
        {
            var p = new SqlParameter("@result", SqlDbType.Int);
            p.Direction = ParameterDirection.Output;
            _context.Database.ExecuteSqlRaw("set @result = next value for Transaction_Seq", p);
            var nextVal = (int)p.Value;

            var transaction = _transactions.Add(new Transaction
            {
                TransactionID = $"MOMKN-{denominationId}-{nextVal}",
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
        public string UpdateRequest(int? transactionId, int requestId, string RRN, RequestStatusCodeType requestStatus, int userId, int providerServiceRequestId)
        {
            var request = _requests.GetById(requestId);
            //if(transactionId.HasValue)
            request.TransactionID = transactionId;
            request.RRN = RRN;
            request.StatusID = (int)requestStatus;
            request.UserID = userId;
            request.ProviderServiceRequestID = providerServiceRequestId;
            request.ResponseDate = DateTime.Now;

            //if (transactionId.HasValue)
            var reciept = AddTransactionRecipe(transactionId.Value);

            _unitOfWork.SaveChanges();
            return reciept;
        }

        public void UpdateRequestStatus(int requestId, RequestStatusCodeType requestStatus)
        {
            var request = _requests.GetById(requestId);
            request.StatusID = (int)requestStatus;
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

        private string AddTransactionRecipe(int transactionId)
        {
            var reciept = GetReceipt(transactionId);
            _transactionReceipt.Add(new TransactionReceipt
            {
                TransactionID = transactionId,
                Receipt = reciept
            });
            return reciept;
            //_unitOfWork.SaveChanges();
        }
        //Denomination.DenominationReceiptData.FirstOrDefault().Title
        private string GetReceipt(int transactionId)
        {
            var query = _transactions.Getwhere(s => s.ID == transactionId).Select(t => new
            {
                t.Request.Denomination.DenominationReceiptData.FirstOrDefault().Title,
                t.CreationDate,
                t.Request.ServiceDenominationID,
                t.InvoiceID,
                t.AccountIDFrom,
                t.TotalAmount,
                t.Request.BillingAccount,
                t.OriginalAmount,
                t.Request.Denomination.DenominationReceiptData.FirstOrDefault().Disclaimer,
                t.Request.Denomination.DenominationReceiptData.FirstOrDefault().Footer,
                t.Request.Denomination.DenominationReceiptParams.FirstOrDefault().Parameter.ArName,
                t.ReceiptBodyParams.FirstOrDefault().Value,
                t.Request.Denomination.DenominationReceiptParams.FirstOrDefault().Bold,
                t.Request.Denomination.DenominationReceiptParams.FirstOrDefault().Alignment,

            }).FirstOrDefault();
            return "{'title':{ 'serviceName':'" + query.Title + "'}," +
                "header:{" +
                 "data:[" +
                "{ 'Key':'التاريخ','Value' :'" + query.CreationDate + "'}," +
                "{ 'Key':'كود الخدمه','Value' :'" + query.ServiceDenominationID + "'}," +
                "{ 'Key':'رقم العمليه','Value' :'" + query.InvoiceID + "'}," +
                "{ 'Key':'رقم الحساب','Value' :'" + query.AccountIDFrom + "'}," +
                "{ 'Key':'المبلغ المدفوع','Value':'" + query.TotalAmount + "'}" +
                "]}," +
                "body:{" +
                "data:[{ 'Key':'رقم العميل','Value' :'" + query.BillingAccount + "'}," +
                    "{'Key':'" + query.ArName + "','Value' :'" + query.Value + "','Bold' :'" + query.Bold + "','Alignment' :'" + query.Alignment + "'}," +
                    "{ 'Key':'المبلغ','Value' :'" + query.OriginalAmount + "'}]}," +
                    "'disclaimer':'" + query.Disclaimer + "','footer':'" + query.Footer + "'}";
        }
        public int AddInvoiceBTech(int requestId, decimal amount, int userId, string billingAccount, decimal fees, string billingInfo)
        {
            using var cmd = new SqlCommand("[BTech_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", amount);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@PaymentCode", billingAccount);
            cmd.Parameters.AddWithValue("@Fees", fees);
            cmd.Parameters.AddWithValue("@BillInfo", billingInfo);
            return InitiateSqlCommand(cmd);
        }
        public int AddInvoiceEducationService(int requestId, decimal basic_value, int userId, string ssn, decimal fees, int subServId, string customerName, string fcrn)
        {
            using var cmd = new SqlCommand("[EducationServices_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", basic_value);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@SSN", ssn);
            cmd.Parameters.AddWithValue("@Fees", fees);
            cmd.Parameters.AddWithValue("@SubServId", subServId);
            cmd.Parameters.AddWithValue("@CustomerName", customerName);
            cmd.Parameters.AddWithValue("@FCRN", fcrn);
            return InitiateSqlCommand(cmd);
        }
        private int InitiateSqlCommand(SqlCommand cmd)
        {
            var sqlConnString = _configuration.GetConnectionString("OldConnectionString");
            //"Data Source=164.160.104.66;User ID=Ebram;Password =P@$$w0rd123;Initial Catalog=momkentest;Integrated Security=false;Min Pool Size=5;Max Pool Size=30000;Connect Timeout=10000;";
            using var conn = new SqlConnection(sqlConnString);
            cmd.Connection = conn;
            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
            return 1;
        }

        public int AddInvoiceCashUTopUp(int requestId, decimal amount, int userId, string currency, string holderName)
        {
            using var cmd = new SqlCommand("[CashUTopUp_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", amount);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Currency", currency);
            cmd.Parameters.AddWithValue("@HolderName", holderName);
            return InitiateSqlCommand(cmd);
        }


        public string GetTransactionReceipt(int transactionId)
        {
            return _transactionReceipt.Getwhere(s => s.TransactionID == transactionId).Select(s => s.Receipt).FirstOrDefault();
        }

        public int AddInvoiceCashU(int requestId, decimal amount, int userId, string currency, string cardNumber, string serial, DateTime creationDate, DateTime expirationDate, int denominationId)
        {
            using var cmd = new SqlCommand("[CashU_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", amount);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Currency", currency);
            cmd.Parameters.AddWithValue("@CardNumber", cardNumber);
            cmd.Parameters.AddWithValue("@Serial", serial);
            cmd.Parameters.AddWithValue("@CreationDate", creationDate);
            cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate);
            cmd.Parameters.AddWithValue("@DenominationId", denominationId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceCashIn(int requestId, decimal amount, int userId, string payment_ref_number, int serviceId, string accountName)
        {
            using var cmd = new SqlCommand("[CashIn_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", amount);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@payment_ref_number", payment_ref_number);
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            return InitiateSqlCommand(cmd);
        }
    }
}
