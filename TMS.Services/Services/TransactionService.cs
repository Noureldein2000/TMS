using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        private readonly IBaseRepository<AccountTypeProfileCommission, int> _accountTypeProfileCommission;
        private readonly IBaseRepository<DenominationCommission, int> _denominationCommission;
        private readonly IBaseRepository<TransactionReceipt, int> _transactionReceipt;
        private readonly IBaseRepository<PendingPaymentCard, int> _pendingPaymentCard;
        private readonly IBaseRepository<CardType, int> _cardType;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        //private readonly IBaseRepository<Invoice, int> _invoice;
        private readonly IUnitOfWork _unitOfWork;
        public TransactionService(IBaseRepository<Request, int> requests,
            IBaseRepository<Transaction, int> transactions,
            IBaseRepository<AccountTransactionCommission, int> accountTransactionCommission,
            IBaseRepository<AccountCommission, int> accountCommission,
            IBaseRepository<AccountTypeProfileCommission, int> accountTypeProfileCommission,
            IBaseRepository<DenominationCommission, int> denominationCommission,
            IBaseRepository<TransactionReceipt, int> transactionReceipt,
            IBaseRepository<PendingPaymentCard, int> pendingPaymentCard,
            IBaseRepository<CardType, int> cardType,
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
            _accountTypeProfileCommission = accountTypeProfileCommission;
            _denominationCommission = denominationCommission;
            _configuration = configuration;
            _context = context;
            _transactionReceipt = transactionReceipt;
            _pendingPaymentCard = pendingPaymentCard;
            _cardType = cardType;
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
            return _accountTypeProfileCommission.Getwhere(a => a.AccountTypeProfileDenomination.DenominationID == denominationId
                && a.AccountTypeProfileDenomination.AccountTypeProfileID == accountProfileId && a.Commission.Status == true
                && a.Commission.AmountFrom <= originalAmount
                && a.Commission.AmountTo >= originalAmount
                && a.Commission.StartDate <= DateTime.Today
                && a.Commission.EndDate >= DateTime.Today
                ).Select(s => new
                {
                    s.Commission.CommissionTypeID,
                    Commission = s.Commission.PaymentModeID == 1 ? s.Commission.Value
                  : s.Commission.PaymentModeID == 2 ? (s.Commission.Value *
                  (s.AccountTypeProfileDenomination.Denomination.Value > 0 ? (s.AccountTypeProfileDenomination.Denomination.Value * s.AccountTypeProfileDenomination.Denomination.Currency.Value) : originalAmount)) / 100
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
        public Root UpdateRequest(int? transactionId, int requestId, string RRN, RequestStatusCodeType requestStatus, int userId, int providerServiceRequestId)
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

        private Root AddTransactionRecipe(int transactionId)
        {
            var reciept = GetReceipt(transactionId);
            _transactionReceipt.Add(new TransactionReceipt
            {
                TransactionID = transactionId,
                Receipt = JsonConvert.SerializeObject(reciept)
            });
            return reciept;
            //_unitOfWork.SaveChanges();
        }
        //Denomination.DenominationReceiptData.FirstOrDefault().Title
        private Root GetReceipt(int transactionId)
        {
            var query = _transactions.Getwhere(s => s.ID == transactionId).Select(t => new
            {
                t.ID,
                t.Request.Denomination.DenominationReceiptData.Title,
                t.CreationDate,
                t.Request.ServiceDenominationID,
                t.InvoiceID,
                t.AccountIDFrom,
                t.TotalAmount,
                t.Request.BillingAccount,
                t.OriginalAmount,
                t.Request.Denomination.DenominationReceiptData.Disclaimer,
                t.Request.Denomination.DenominationReceiptData.Footer,
                parameterNames = t.Request.Denomination.DenominationReceiptParams.Select(s => new { s.Parameter.ArName, t.ReceiptBodyParams.Where(sd => sd.ParameterID == s.ParameterID).FirstOrDefault().Value }).ToList(),
                t.Request.Denomination.DenominationReceiptParams.FirstOrDefault().Bold,
                t.Request.Denomination.DenominationReceiptParams.FirstOrDefault().Alignment,

            }).FirstOrDefault();
            var root = new Root
            {
                title = new Title
                {
                    serviceName = query.Title
                },
                header = new Header
                {
                    data = new List<Datum>
                    {
                        new Datum{Key = "التاريخ", Value = query.CreationDate.ToString()},
                        new Datum{ Key ="كود الخدمه", Value =query.ServiceDenominationID.ToString()},
                        new Datum{Key = "رقم العمليه", Value = query.ID.ToString()},
                        new Datum{Key = "رقم الحساب", Value = query.AccountIDFrom.ToString()},
                        new Datum{Key = "المبلغ المدفوع", Value = query.TotalAmount.ToString()},
                    }
                },
                body = new Body
                {
                    data = new List<Datum>
                    {
                        new Datum{Key = "رقم العميل", Value = query.BillingAccount??""},
                        new Datum{Key = "المبلغ", Value = query.OriginalAmount.ToString()},
                    }
                },
                disclaimer = query.Disclaimer,
                footer = query.Footer
            };
            if (query.TotalAmount != query.OriginalAmount)
                foreach (var item in query.parameterNames)
                {
                    root.body.data.Add(new Datum { Key = item.ArName, Value = item.Value });
                }

            return root;
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
            catch (Exception ex)
            {
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

        public int AddInvoiceDonation(string providerName, string serviceName, string serviceId, decimal basicValue, decimal addedMoney, int status, decimal userId, string accountName, string mobile)
        {
            using var cmd = new SqlCommand("[Donation_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_name", providerName);
            cmd.Parameters.AddWithValue("@Service_name", serviceName);
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@account_name", accountName);
            cmd.Parameters.AddWithValue("@mobile", mobile);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceElectricityBill(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data, string mobile)
        {
            using var cmd = new SqlCommand("[electricity_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@account_code", accountCode);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@due_Date", dueDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@ProviderTransactionId", providerTransactionId);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@mobile_no", mobile);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceElectricityCard(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data, string mobile, string cardType, string cardData, int? requestId, string eCardmomknPaymentId)
        {
            using var cmd = new SqlCommand("[electricity_card_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@account_code", accountCode);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@due_Date", dueDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@ProviderTransactionId", providerTransactionId);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@mobile_no", mobile);
            cmd.Parameters.AddWithValue("@CardType", cardType);
            cmd.Parameters.AddWithValue("@CardData", cardData);
            cmd.Parameters.AddWithValue("@request_id", requestId);
            cmd.Parameters.AddWithValue("@ECardmomknPaymentId", eCardmomknPaymentId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceWaterCard(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data, string mobile, string cardType, string cardData, int? requestId, string eCardmomknPaymentId)
        {
            using var cmd = new SqlCommand("[Water_card_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@account_code", accountCode);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@due_Date", dueDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@ProviderTransactionId", providerTransactionId);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@mobile_no", mobile);
            cmd.Parameters.AddWithValue("@CardType", cardType);
            cmd.Parameters.AddWithValue("@CardData", cardData);
            cmd.Parameters.AddWithValue("@request_id", requestId);
            cmd.Parameters.AddWithValue("@ECardmomknPaymentId", eCardmomknPaymentId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceEtisalatEgypt(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName)
        {
            using var cmd = new SqlCommand("[Etisalat_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceEtisalatInternet(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName)
        {
            using var cmd = new SqlCommand("[Etisalat_int_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceITunes(string requestId, string pin, string serial, decimal basicValue, int userId, string productCode, string currency, decimal amountUAE, string validTo)
        {
            using var cmd = new SqlCommand("[ITunes_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@Pin", pin);
            cmd.Parameters.AddWithValue("@Serial", serial);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ProductCode", productCode);
            cmd.Parameters.AddWithValue("@Currency", currency);
            cmd.Parameters.AddWithValue("@AmountUAE", amountUAE);
            cmd.Parameters.AddWithValue("@ValidTo", validTo);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceSonyMicrosoft(string requestId, string pin, string serial, decimal basicValue, int userId, string productCode, int servId, decimal amountUSD, string currency, string validTo)
        {
            using var cmd = new SqlCommand("[Sony_Microsoft_send]");
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@Pin", pin);
            cmd.Parameters.AddWithValue("@Serial", serial);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ProductCode", productCode);
            cmd.Parameters.AddWithValue("@ServId", servId);
            cmd.Parameters.AddWithValue("@AmountUSD", amountUSD);
            cmd.Parameters.AddWithValue("@Currency", currency);
            cmd.Parameters.AddWithValue("@ValidTo", validTo);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceOneCard(string providerName, string serviceName, string servId, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string mobile, string secret)
        {
            using var cmd = new SqlCommand("[OneCard_egypt_send]");
            cmd.Parameters.AddWithValue("@provider_name", providerName);
            cmd.Parameters.AddWithValue("@Service_name", serviceName);
            cmd.Parameters.AddWithValue("@service_id", servId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@account_name", accountName);
            cmd.Parameters.AddWithValue("@ServId", servId);
            cmd.Parameters.AddWithValue("@mobile", mobile);
            cmd.Parameters.AddWithValue("@secret", secret);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceLinkDsl(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string mobile)
        {
            using var cmd = new SqlCommand("[Linkdsl_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@Mobile_Number", mobile);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceLinkDslReseller(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string mobile)
        {
            using var cmd = new SqlCommand("[Linkdsl_reseller_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@Mobile_Number", mobile);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceOrangeMobileEgypt(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName)
        {
            using var cmd = new SqlCommand("[Mobinil_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoicePetrotrade(string requestId, decimal basicValue, int userId, string mobile, string registerNo, string extraBillInfo, string FCRN, decimal fees)
        {
            using var cmd = new SqlCommand("[Petrotrade_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", requestId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@MobileNumber", mobile);
            cmd.Parameters.AddWithValue("@RegisterNo", registerNo);
            cmd.Parameters.AddWithValue("@ExtraBillInfo", extraBillInfo);
            cmd.Parameters.AddWithValue("@FCRN", FCRN);
            cmd.Parameters.AddWithValue("@Fees", fees);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceTelecomEgypt(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string msg, int requestId)
        {
            using var cmd = new SqlCommand("[Telecom_egypt_send_api]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@msg", msg);
            cmd.Parameters.AddWithValue("@request_id", requestId);
            return InitiateSqlCommand(cmd);
        }

        public int AddElectronicChargeLog(string providerCompany, string mobileNumber, float value, float totalAmount, string chargeStatus, int statusTransfer, int userId, int servId)
        {
            using var cmd = new SqlCommand("[InsertElectronicChargeLog]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_company", providerCompany);
            cmd.Parameters.AddWithValue("@mobile_number", mobileNumber);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@total_amount", totalAmount);
            cmd.Parameters.AddWithValue("@charge_status", chargeStatus);
            cmd.Parameters.AddWithValue("@status_transfer", statusTransfer);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ServId", servId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceElectronicCharge(string providerCompany, string mobileNumber, float value, float totalAmount, string chargeStatus, int statusTransfer, int userId, int logId, int servId)
        {
            using var cmd = new SqlCommand("[InsertElectronicCharge]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_company", providerCompany);
            cmd.Parameters.AddWithValue("@mobile_number", mobileNumber);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@total_amount", totalAmount);
            cmd.Parameters.AddWithValue("@charge_status", chargeStatus);
            cmd.Parameters.AddWithValue("@status_transfer", statusTransfer);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Log_Id", logId);
            cmd.Parameters.AddWithValue("@ServId", servId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceElectronicChargeProcBeeAwIncomeNew(int id, decimal value, int servId, int centerId, int userId, decimal invoicePrice, string mobileNumber)
        {
            using var cmd = new SqlCommand("[electronic_charge_proc_bee_aw_Income_new]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@ServId", servId);
            cmd.Parameters.AddWithValue("@center_id", centerId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@invoice_price", invoicePrice);
            cmd.Parameters.AddWithValue("@mobile_number", mobileNumber);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceUniversityBill(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data)
        {
            using var cmd = new SqlCommand("[electricity_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@account_code", accountCode);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@due_Date", dueDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@ProviderTransactionId", providerTransactionId);
            cmd.Parameters.AddWithValue("@data", data);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceValU(string requestId, decimal basicValue, int userId, string mobile, string passportNum, string SSN, string customerId)
        {
            using var cmd = new SqlCommand("[ValU_send]");
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@MobileNumber", mobile);
            cmd.Parameters.AddWithValue("@PassportNumber", passportNum);
            cmd.Parameters.AddWithValue("@SSN", SSN);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceVodafoneInternet(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName)
        {
            using var cmd = new SqlCommand("[vodafone_Int_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceVodafoneMobile(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName)
        {
            using var cmd = new SqlCommand("[vodafone_egypt_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            return InitiateSqlCommand(cmd);
        }

        public int AddElectronicChargeLogVoucher(string providerCompany, string mobileNumber, decimal value, string chargeStatus, int statusTransfer, int userId, int countVoucher)
        {
            using var cmd = new SqlCommand("[electronic_charge_proc_log_voucher]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_company", providerCompany);
            cmd.Parameters.AddWithValue("@mobile_number", mobileNumber);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@charge_status", chargeStatus);
            cmd.Parameters.AddWithValue("@status_transfer", statusTransfer);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@CountVoucher", countVoucher);
            return InitiateSqlCommand(cmd);
        }

        public int CheckVoucherValue(int serviceId, string provider, decimal value)
        {
            using var cmd = new SqlCommand("[CheckVoucherValue]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Service_ID", serviceId);
            cmd.Parameters.AddWithValue("@Provider", provider);
            cmd.Parameters.AddWithValue("@value", value);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceElectronicChargeNewVoucher(string providerCompany, string mobileNumber, decimal value, string chargeStatus, int statusTransfer, int userId, string masaryTransId, string voucherNumber, string serialNumber, int providerId, int servId, int logId)
        {
            using var cmd = new SqlCommand("[electronic_charge_proc_voucher_new]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_company", providerCompany);
            cmd.Parameters.AddWithValue("@mobile_number", mobileNumber);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@charge_status", chargeStatus);
            cmd.Parameters.AddWithValue("@status_transfer", statusTransfer);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@masary_trans_id", masaryTransId);
            cmd.Parameters.AddWithValue("@voucher_number", voucherNumber);
            cmd.Parameters.AddWithValue("@serial_number", serialNumber);
            cmd.Parameters.AddWithValue("@provider_id", providerId);
            cmd.Parameters.AddWithValue("@Service_ID", servId);
            cmd.Parameters.AddWithValue("@Log_Id", logId);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceElectronicChargeIncomeNewVoucher(string providerCompany, decimal value, int userId, string voucherNumber, int providerId, int servId, int id)
        {
            using var cmd = new SqlCommand("[electronic_charge_proc_voucher_Income_new]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_company", providerCompany);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@voucher_number", voucherNumber);
            cmd.Parameters.AddWithValue("@provider_id", providerId);
            cmd.Parameters.AddWithValue("@Service_ID", servId);
            cmd.Parameters.AddWithValue("@Id", id);
            return InitiateSqlCommand(cmd);
        }

        public TransactionDTO GetTransactionByBrn(int providerServiceRequestId)
        {
            return _transactions.Getwhere(x => x.Request.ProviderServiceRequestID == providerServiceRequestId).Select(x => new TransactionDTO
            {
                Id = x.ID,
                InvoiceId = x.InvoiceID,
                RequestId = x.RequestID,
                TotalAmount = x.TotalAmount
            }).FirstOrDefault();
        }


        public int ReturnInvoice(int invoiceId, int userPayedId, string reason)
        {
            using var cmd = new SqlCommand("[ReturnInvoice]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", invoiceId);
            cmd.Parameters.AddWithValue("@UserPayedId", userPayedId);
            cmd.Parameters.AddWithValue("@Reason", reason);
            return InitiateSqlCommand(cmd);
        }

        public void UpdateTransction(int id)
        {
            var current = _transactions.Getwhere(x => x.ID == id).FirstOrDefault();
            current.IsReversed = true;
            _unitOfWork.SaveChanges();
        }

        public int GetProviderRequestID(int transactionId, int accountId, int denominationID)
        {
            return (int)_requests.Getwhere(x => x.TransactionID == transactionId && x.AccountID == accountId && x.ServiceDenominationID == denominationID)
                .Select(x => x.ProviderServiceRequestID).FirstOrDefault();
        }
        public bool GetIsReversed(int transactionId, int accountId)
        {
            return _transactions.Getwhere(z => z.ID == transactionId && z.AccountIDFrom == accountId).Select(x => x.IsReversed).FirstOrDefault();
        }

        public int GetRequestID(int transactionId, int accountId)
        {
            return (int)_transactions.Getwhere(x => x.ID == transactionId && x.AccountIDFrom == accountId)
               .Select(x => x.RequestID).FirstOrDefault();
        }

        public int GetPendingPaymentCardStatus(int transactionId)
        {
            return _pendingPaymentCard.Getwhere(x => x.TransactionID == transactionId).Select(x => x.PengingPaymentCardStatusID).FirstOrDefault();
        }

        public void AddPendingPaymentCard(string paymentRefInfo, int transactionId, string cardType, string hostTransactionId, int? brn)
        {
            try
            {
                _pendingPaymentCard.Add(new PendingPaymentCard
                {
                    PaymentRefInfo = paymentRefInfo,
                    TransactionID = transactionId,
                    CardTypeID = _cardType.Getwhere(x => x.Name == cardType).Select(x => x.ID).FirstOrDefault(),
                    HostTransactionID = hostTransactionId,
                    Brn = brn,
                    PengingPaymentCardStatusID = 1
                });

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void UpdatePendingPaymentCardStatus(int transactionId, int pendingPaymentCardStatusID)
        {
            var currenr = _pendingPaymentCard.Getwhere(x => x.TransactionID == transactionId).FirstOrDefault();
            currenr.PengingPaymentCardStatusID = pendingPaymentCardStatusID;
            _unitOfWork.SaveChanges();
        }

        public int AddInvoiceTedataEgyptCharge(string teleCode, string teleNumber, decimal basicValue, decimal addedMoney, int status, int userId, string msg, string transInv, int fawBillerId, string serviceName)
        {
            using var cmd = new SqlCommand("[Tedata_egypt_charge]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@msg", msg);
            cmd.Parameters.AddWithValue("@TransInv", transInv);
            cmd.Parameters.AddWithValue("@faw_BillerId", fawBillerId);
            cmd.Parameters.AddWithValue("@ServiceName", serviceName);
            return InitiateSqlCommand(cmd);
        }
        public void TEDataLogUpdate(string chargeStatus, int requestId)
        {
            using var cmd = new SqlCommand("[TE_log_update]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@charge_status", chargeStatus);
            cmd.Parameters.AddWithValue("@Id", requestId);
            InitiateSqlCommand(cmd);
        }
        public int AddInvoiceTedataTest(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string msg, string transInv, string code, string messageTitle, int fawBillerId, string data, int servId)
        {
            using var cmd = new SqlCommand("[Tedata_test]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Tele_code", teleCode);
            cmd.Parameters.AddWithValue("@Tele_number", teleNumber);
            cmd.Parameters.AddWithValue("@Bill_date", billDate);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@added_money", addedMoney);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@AccountName", accountName);
            cmd.Parameters.AddWithValue("@msg", msg);
            cmd.Parameters.AddWithValue("@TransInv", transInv);
            cmd.Parameters.AddWithValue("@code", code);
            cmd.Parameters.AddWithValue("@message_title", messageTitle);
            cmd.Parameters.AddWithValue("@faw_BillerId", fawBillerId);
            cmd.Parameters.AddWithValue("@data", data);
            cmd.Parameters.AddWithValue("@ServId", servId);
            return InitiateSqlCommand(cmd);
        }
        public int AddTEDataProcLog(string providerCompany, string mobileNumber, decimal value, string chargeStatus, int statusTransfer, int userId, int servId, int transId)
        {
            using var cmd = new SqlCommand("[TE_proc_log]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@provider_company", providerCompany);
            cmd.Parameters.AddWithValue("@mobile_number", mobileNumber);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@charge_status", chargeStatus);
            cmd.Parameters.AddWithValue("@status_transfer", statusTransfer);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ServId", servId);
            cmd.Parameters.AddWithValue("@TransId", transId);
            return InitiateSqlCommand(cmd);
        }
        public int AddInvoiceSocialInsurance(int accountId, decimal basicValue, int userId, string BillingAccount, int serviceId, int servId, decimal value, decimal addedMoney)
        {
            using var cmd = new SqlCommand("[SocialInsurance_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CenterId", accountId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", value);
            cmd.Parameters.AddWithValue("@BillingAccount", BillingAccount);
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@ServId", servId);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@AddedMoney", addedMoney);
            return InitiateSqlCommand(cmd);
        }
        public int AddInvoiceTamkeenLoan(int accountId, decimal basicValue, int userId, string BillingAccount, int serviceId, int servId, decimal value, decimal addedMoney, string accountNumber, string providerTransactionID, string providerResponse, string branchNumber, string dueDate)
        {
            using var cmd = new SqlCommand("[TamkeenLoan_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CenterId", accountId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", value);
            cmd.Parameters.AddWithValue("@BillingAccount", BillingAccount);
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@ServId", servId);
            cmd.Parameters.AddWithValue("@value", value);
            cmd.Parameters.AddWithValue("@AddedMoney", addedMoney);
            cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
            cmd.Parameters.AddWithValue("@ProviderTransactionID", providerTransactionID);
            cmd.Parameters.AddWithValue("@Provider_Response", providerResponse);
            cmd.Parameters.AddWithValue("@BranchNumber", branchNumber);
            cmd.Parameters.AddWithValue("@DueDate", dueDate);
            return InitiateSqlCommand(cmd);
        }

        public int AddInvoiceTalabat(string requestId, decimal basicValue, int userId, int accountId, int serviceId, decimal addedMoney, string restaurantCode)
        {
            using var cmd = new SqlCommand("[Talabat_send]");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RequestId", requestId);
            cmd.Parameters.AddWithValue("@basic_value", basicValue);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@CenterId", accountId);
            cmd.Parameters.AddWithValue("@service_id", serviceId);
            cmd.Parameters.AddWithValue("@AddedMoney", addedMoney);
            cmd.Parameters.AddWithValue("@RestaurantCode", restaurantCode);
            return InitiateSqlCommand(cmd);
        }
    }
}
