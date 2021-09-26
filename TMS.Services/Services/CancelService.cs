using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Infrastructure.Utils;
using TMS.Services.Models;
using TMS.Services.Models.SwaggerModels;
using TMS.Services.Repositories;
using TMS.Services.SOFClientAPIs;

namespace TMS.Services.Services
{
    public class CancelService : ICancelService
    {

        private readonly IDenominationService _denominationService;
        private readonly IProviderService _providerService;
        private readonly ISwitchService _switchService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ILoggingService _loggingService;
        private readonly IDbMessageService _dbMessageService;
        private readonly IFeesService _feesService;
        private readonly ITransactionService _transactionService;
        private readonly IAccountsApi _accountsApi;
        private readonly IUnitOfWork _unitOfWork;
        public CancelService(
             IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITransactionService transactionService,
            IUnitOfWork unitOfWork)
        {
            _denominationService = denominationService;
            _providerService = providerService;
            _switchService = switchService;
            _inquiryBillService = inquiryBillService;
            _loggingService = loggingService;
            _dbMessageService = dbMessageService;
            _feesService = feesService;
            _transactionService = transactionService;
            _accountsApi = new AccountsApi("http://localhost:5000");
            _unitOfWork = unitOfWork;
        }
        public async Task<PaymentResponseDTO> Cancel(CancelDTO payModel, int userId, int id, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            string printedReciept = "";
            decimal totalAmount = 0;
            decimal amount = 0;
            bool isCancelled;
            var _FeeList = new List<FeesAmounts>();
            var _PayList = new List<PaymentCardAmounts>();
            AccountBalanceResponseModel balance = null;
            var denomination = _denominationService.GetDenomination(id);

            //Get BillingAccount For Inquiry Brn
            var billingAccount = _providerService.GetProviderServiceRequestBillingAccount(payModel.Brn, userId, id);

            //Add ProviderServiceRequest
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Cancel,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            //Get Max ProviderServiceRequestFees
            var brnFees = _providerService.GetMaxProviderServiceRequest(payModel.Brn, Infrastructure.RequestType.Fees);

            var denomaotionServicerProvider = _denominationService.GetDenominationServiceProvider(id);

            if (denomination.ServiceID == 20 || denomination.ServiceID == 57)
            {
                //Get Fees Amount
                FeesAmounts FA = new FeesAmounts();
                FA.Amount = _providerService.GetProviderServiceResponseParams(brnFees, "ar", "amountFees").Select(x => x.Value).FirstOrDefault();
                FA.CurrentCode = _providerService.GetProviderServiceResponseParams(brnFees, "ar", "currentCode").Select(x => x.Value).FirstOrDefault();
                _FeeList.Add(FA);
                fees = decimal.Parse(FA.Amount);

                //Get Payment Bills 
                var IBList = _inquiryBillService.GetInquiryBillSequence(brnFees);
                foreach (var item in IBList)
                {
                    var IBDList = _inquiryBillService.GetInquiryBillDetails(brnFees, item.Sequence);
                    if (IBDList != null)
                    {
                        PaymentCardAmounts PA = new PaymentCardAmounts();

                        //PA.amount = IBDList
                        PA.CurrentCode = IBDList.FirstOrDefault(t => t.ParameterId == 15).Value;
                        PA.Sequence = item.Sequence.ToString();

                        PA.Amount = item.Amount.ToString();
                        amount += item.Amount;

                        _PayList.Add(PA);
                    }

                }
                totalAmount = amount + fees;

                //Logging Client Request
                await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
                  payModel.Brn,
                  LoggingType.CustomerRequest);

                string response = CallCancellProvider(new CancellProviderDTO
                {
                    ServiceID = denomination.ServiceID,
                    FeesAmounts = _FeeList,
                    PayCardAmounts = _PayList,
                    BillingAccount = billingAccount,
                    Brn = payModel.Brn,
                    DenomationId = id,
                    MomknPaymentId = payModel.MomknPaymentId,
                    OldDenominationID = denomination.OldDenominationID,
                    ProviderCode = denomaotionServicerProvider.ProviderCode,
                    ProviderServiceRequestId = providerServiceRequestId
                }, out isCancelled);

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                if (Validates.CheckJSON(response))
                {
                    JObject o = JObject.Parse(response);

                    var oldTransaction = _transactionService.GetTransactionByBrn(payModel.Brn);

                    if (oldTransaction != null)
                    {
                        paymentResponse.InvoiceId = _transactionService.ReturnInvoice((int)oldTransaction.InvoiceId, userId, response);

                        var transactionId = _transactionService.AddTransaction(null, totalAmount, id, payModel.Amount, fees, oldTransaction.Id.ToString(), payModel.AccountId, paymentResponse.InvoiceId, oldTransaction.RequestId);
                        paymentResponse.TransactionId = transactionId;

                        _transactionService.UpdateTransction(oldTransaction.Id);

                        // Delete sof
                        await _accountsApi.ApiAccountsReturnBalanceFromAccountIdToAccountIdBalancesAmountPutAsync(26457, payModel.AccountId, (double)totalAmount);

                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                        _transactionService.UpdatePendingPaymentCardStatus(oldTransaction.Id, 2);
                    }

                }
                else if (response.Contains("timed out"))
                {
                    throw new TMSException("PendingTrx", "3");
                }
                else
                {
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);
                }
            }
            else if (denomination.ServiceID == 36)
            {
                //Logging Client Request
                await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
                 payModel.Brn,
                 LoggingType.CustomerRequest);

                string response = CallCancellProvider(new CancellProviderDTO
                {
                    ServiceID = denomination.ServiceID,
                    FeesAmounts = _FeeList,
                    PayCardAmounts = _PayList,
                    BillingAccount = billingAccount,
                    Brn = payModel.Brn,
                    DenomationId = id,
                    MomknPaymentId = payModel.MomknPaymentId,
                    OldDenominationID = denomination.OldDenominationID,
                    ProviderCode = denomaotionServicerProvider.ProviderCode,
                    ProviderServiceRequestId = providerServiceRequestId
                }, out isCancelled);

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                if (Validates.CheckJSON(response))
                {
                    JObject o = JObject.Parse(response);

                    var oldTransaction = _transactionService.GetTransactionByBrn(payModel.Brn);
                    if (oldTransaction != null)
                    {
                        paymentResponse.InvoiceId = _transactionService.ReturnInvoice((int)oldTransaction.InvoiceId, userId, response);

                        var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, oldTransaction.Id.ToString(), null, paymentResponse.InvoiceId, oldTransaction.RequestId);
                        paymentResponse.TransactionId = transactionId;

                        _transactionService.UpdateTransction(oldTransaction.Id);

                        // Return Balance sof
                        await _accountsApi.ApiAccountsReturnBalanceFromAccountIdToAccountIdBalancesAmountPutAsync(26457, payModel.AccountId, (double)oldTransaction.TotalAmount);

                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                        _transactionService.UpdatePendingPaymentCardStatus(transactionId, 2);
                    }

                }
                else
                    throw new TMSException("DupplicatedRefund", "38");
            }
            else
                throw new TMSException("RequestNotFound", "14");

            paymentResponse.Code = 200;
            paymentResponse.Message = "Success";
            paymentResponse.ServerDate = DateTime.Now.ToString();
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }

        public string CallCancellProvider(CancellProviderDTO model, out bool isCancelled)
        {
            var response = "";

            var serviceConfiguration = _denominationService.GetServiceConfiguration(model.DenomationId);

            if (model.ServiceID == 20 || model.ServiceID == 57)
            {

                var providerResponseParams = _providerService.GetProviderServiceResponseParams(model.Brn, language: "en", "billNumber",
              "billRecId", "paymentRefInfo", "arabicName");

                var billNumber = providerResponseParams.Where(s => s.ProviderName == "billNumber").Select(s => s.Value).FirstOrDefault().ToString();
                var billRecId = providerResponseParams.Where(s => s.ProviderName == "billRecId").Select(s => s.Value).FirstOrDefault().ToString();
                var paymentRefInfo = providerResponseParams.Where(s => s.ProviderName == "paymentRefInfo").Select(s => s.Value).FirstOrDefault().ToString();
                var accountName = providerResponseParams.Where(s => s.ProviderName == "arabicName").Select(s => s.Value).FirstOrDefault().ToString();
                var cardType = _providerService.GetProviderServiceRequestParams(model.Brn, "en", "CardType").Where(x => x.Key == "CardType").Select(x => x.Value).FirstOrDefault();

                var switchRequestDto = new CancelModel
                {
                    //transactionId = providerServiceRequestId.ToString(),
                    TransactionId = "200",
                    BillNumber = billNumber,
                    BillRecId = billRecId,
                    PaymentRefInfo = paymentRefInfo,
                    PaymentAmounts = model.PayCardAmounts,
                    FeesAmounts = model.FeesAmounts,
                    BillerId = model.ProviderCode,
                    BillingAcccount = model.BillingAccount,
                    MomknPaymentId = model.MomknPaymentId,
                    CardType = cardType
                };

                var switchEndPoint = new SwitchEndPointDTO
                {
                    URL = serviceConfiguration.URL,
                    TimeOut = serviceConfiguration.TimeOut,
                    UserName = serviceConfiguration.UserName,
                    UserPassword = serviceConfiguration.UserPassword
                };


                _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                model.ProviderServiceRequestId,
                LoggingType.ProviderRequest);

                response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.cancelPayment.ToString(), "Basic ");
                isCancelled = Validates.CheckJSON(response);
            }
            else if (model.ServiceID == 36)
            {

                var switchRequestDto = new CancelITunes
                {
                    //transactionId = providerServiceRequestId.ToString(),
                    TransactionId = model.ProviderServiceRequestId.ToString(),
                    ProductCode = model.OldDenominationID,
                    ReferenceTransactionId = model.MomknPaymentId.ToString()
                };

                var switchEndPoint = new SwitchEndPointDTO
                {
                    URL = serviceConfiguration.URL,
                    TimeOut = serviceConfiguration.TimeOut,
                    UserName = serviceConfiguration.UserName,
                    UserPassword = serviceConfiguration.UserPassword
                };


                _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                model.ProviderServiceRequestId,
                LoggingType.ProviderRequest);

                response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.cancel.ToString(), "Basic ");
                isCancelled = Validates.CheckJSON(response);
            }
            else
                throw new TMSException("RequestNotFound", "14");

            return response;
        }
    }
}
