using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMS.Services.SOFClientAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Infrastructure.Utils;
using TMS.Services.BusinessLayer;
using TMS.Services.Models;
using TMS.Services.Repositories;
using TMS.Services.Services;
using TMS.Services.Models.SwaggerModels;

namespace TMS.Services.ProviderLayer
{
    public class Cancel : IBaseProvider
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
        public Cancel(
           IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITransactionService transactionService
            )
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
        }

        public FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            throw new NotImplementedException();
        }

        public Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            string printedReciept = "";
            decimal totaAmount = 0;
            decimal amount = 0;
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
            var brnPayment = _providerService.GetMaxProviderServiceRequest(payModel.Brn, Infrastructure.RequestType.Payment);

            var denomaotionServicerProvider = _denominationService.GetDenominationServiceProvider(id);

            if (denomination.ServiceID == 20 || denomination.ServiceID == 57)
            {

                //Get Fees Amount
                FeesAmounts FA = new FeesAmounts();
                FA.Amount = _providerService.GetProviderServiceResponseParams(brnFees, "amountFees", "ar").Select(x => x.Value).FirstOrDefault();
                FA.CurrentCode = _providerService.GetProviderServiceResponseParams(brnFees, "currentCode", "ar").Select(x => x.Value).FirstOrDefault();
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
                totaAmount = amount + fees;

                //Logging Client Request
                await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
                  payModel.Brn,
                  LoggingType.CustomerRequest);

                var serviceConfiguration = _denominationService.GetServiceConfiguration(id);
                var providerResponseParams = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "en", "billNumber",
              "billRecId", "paymentRefInfo", "arabicName");

                var billNumber = providerResponseParams.Where(s => s.ProviderName == "billNumber").Select(s => s.Value).FirstOrDefault().ToString();
                var billRecId = providerResponseParams.Where(s => s.ProviderName == "billRecId").Select(s => s.Value).FirstOrDefault().ToString();
                var paymentRefInfo = providerResponseParams.Where(s => s.ProviderName == "paymentRefInfo").Select(s => s.Value).FirstOrDefault().ToString();
                var accountName = providerResponseParams.Where(s => s.ProviderName == "arabicName").Select(s => s.Value).FirstOrDefault().ToString();
                var cardType = _providerService.GetProviderServiceRequestParams(payModel.Brn, "en", "CardType").Where(x => x.Key == "CardType").Select(x => x.Value).FirstOrDefault();

                var switchRequestDto = new CancelDTO
                {
                    //transactionId = providerServiceRequestId.ToString(),
                    TransactionId = int.Parse("200"),
                    BillNumber = billNumber,
                    BillRecId = billRecId,
                    PaymentRefInfo = paymentRefInfo,
                    PaymentAmounts = _PayList,
                    FeesAmounts = _FeeList,
                    BillerId = denomaotionServicerProvider.ProviderCode,
                    BillingAcccount = payModel.BillingAccount,
                    MomknPaymentId = payModel.MomknPaymentId,
                    CardType = cardType
                };

                var switchEndPoint = new SwitchEndPointDTO
                {
                    URL = serviceConfiguration.URL,
                    TimeOut = serviceConfiguration.TimeOut,
                    UserName = serviceConfiguration.UserName,
                    UserPassword = serviceConfiguration.UserPassword
                };


                await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                  providerServiceRequestId,
                  LoggingType.ProviderRequest);

                var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.cancelPayment.ToString(), "Basic ", UrlType.Custom);

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                if (Validates.CheckJSON(response))
                {
                    JObject o = JObject.Parse(response);

                    var oldTransaction = _transactionService.GetTransactionByBrn(payModel.Brn);

                    // call Return invoice SP to another data base system
                    paymentResponse.InvoiceId = _transactionService.ReturnInvoice((int)oldTransaction.InvoiceId, userId, response);

                    // Refund sof
                    await _accountsApi.ApiAccountsReturnBalanceFromAccountIdToAccountIdBalancesAmountPutAsync(26457, payModel.AccountId, (double?)oldTransaction.TotalAmount);

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

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

                var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

                var switchRequestDto = new CancelITunes
                {
                    //transactionId = providerServiceRequestId.ToString(),
                    TransactionId = providerServiceRequestId.ToString(),
                    ProductCode = denomination.OldDenominationID,
                    ReferenceTransactionId = payModel.MomknPaymentId.ToString()
                };

                var switchEndPoint = new SwitchEndPointDTO
                {
                    URL = serviceConfiguration.URL,
                    TimeOut = serviceConfiguration.TimeOut,
                    UserName = serviceConfiguration.UserName,
                    UserPassword = serviceConfiguration.UserPassword
                };


                await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                  providerServiceRequestId,
                  LoggingType.ProviderRequest);

                var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.cancel.ToString(), "Basic ", UrlType.Custom);

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                if (Validates.CheckJSON(response))
                {
                    JObject o = JObject.Parse(response);

                    var oldTransaction = _transactionService.GetTransactionByBrn(payModel.Brn);

                    // call Return invoice SP to another data base system
                    paymentResponse.InvoiceId = _transactionService.ReturnInvoice((int)oldTransaction.InvoiceId, userId, response);

                    // Refund sof
                    await _accountsApi.ApiAccountsReturnBalanceFromAccountIdToAccountIdBalancesAmountPutAsync(26457, payModel.AccountId, (double?)oldTransaction.TotalAmount);

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                }
                else
                    throw new TMSException("DupplicatedRefund", "38");
            }
            else
                throw new TMSException("RequestNotFound", "14");

            paymentResponse.Code = 200;
            paymentResponse.Message = "Success";
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.AvailableBalance = (decimal)balance.TotalAvailableBalance - totalAmount;
            paymentResponse.Receipt = new List<Root> {
                JsonConvert.DeserializeObject<Root>(printedReciept)
            };

            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;

        }

        public async Task Execute(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            await Pay(payModel, userId, id, totalAmount, fees, serviceProviderId);
        }
    }
}
