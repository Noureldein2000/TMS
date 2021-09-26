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

namespace TMS.Services.ProviderLayer
{
    public class Topup : IBaseProvider
    {
        private readonly IDenominationService _denominationService;
        private readonly IProviderService _providerService;
        private readonly ISwitchService _switchService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ILoggingService _loggingService;
        private readonly IDbMessageService _dbMessageService;
        private readonly IFeesService _feesService;
        private readonly ITransactionService _transactionService;
        private readonly IStringLocalizer<ServiceLanguageResource> _localizer;
        private readonly IAccountsApi _accountsApi;
        public Topup(
           IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITransactionService transactionService,
           IStringLocalizer<ServiceLanguageResource> localizer
            )
        {
            _denominationService = denominationService;
            _providerService = providerService;
            _switchService = switchService;
            _localizer = localizer;
            _inquiryBillService = inquiryBillService;
            _loggingService = loggingService;
            _dbMessageService = dbMessageService;
            _feesService = feesService;
            _transactionService = transactionService;
            _accountsApi = new AccountsApi("http://localhost:5000");
        }

        public FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            var feeResponse = new FeesResponseDTO();
            decimal ProviderFees = 0;


            var Ds = _denominationService.GetDenominationServiceProvider(id);
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Fees,
                BillingAccount = null,
                Brn = feesModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            if (feesModel.Data != null)
                foreach (var item in feesModel.Data)
                {
                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = item.Key,
                        ProviderServiceRequestID = providerServiceRequestId,
                        Value = item.Value
                    });
                }

            if (Ds.ProviderHasFees)
            {
                ProviderFees = 0;
            }
            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
            feeResponse.Fees = Math.Round(feesAmount + ProviderFees, 3);
            feeResponse.TotalAmount = feesModel.Amount + feeResponse.Fees;

            var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
            {
                ProviderServiceRequestID = providerServiceRequestId,
                TotalAmount = feeResponse.TotalAmount
            });

            if (feesList.Count > 0)
            {
                foreach (var item in feesList)
                {
                    if (item.Fees != 0)
                    {
                        feeResponse.Data.Add(new DataDTO
                        {
                            Key = item.FeesTypeName,
                            Value = item.Fees.ToString("0.000")
                        });
                        _providerService.AddProviderServiceResponseParam(
                            new ProviderServiceResponseParamDTO
                            {
                                ParameterName = item.FeesTypeName,
                                ServiceRequestID = providerServiceResponseId,
                                Value = item.Fees.ToString("0.000")
                            });
                        _inquiryBillService.AddReceiptBodyParam(
                           new ReceiptBodyParamDTO
                           {
                               ParameterName = item.FeesTypeName,
                               ProviderServiceRequestID = feesModel.Brn,
                               TransactionID = 0,
                               Value = item.Fees.ToString("0.000")
                           });
                    }
                }
            }
            else
            {
                feeResponse.Amount = feesModel.Amount;
                feeResponse.TotalAmount = feesModel.Amount + feesAmount;
                feeResponse.Fees = feesAmount;

                if (feesAmount.ToString() != "0.000")
                {
                    _providerService.AddProviderServiceResponseParam(
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "Service Fees",// "Service Fees",
                            ServiceRequestID = providerServiceResponseId,
                            Value = feesAmount.ToString("0.000")
                        });
                    _inquiryBillService.AddReceiptBodyParam(
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = "Service Fees",// "Service Fees",
                           ProviderServiceRequestID = feesModel.Brn,
                           TransactionID = 0,
                           Value = feesAmount.ToString("0.000")
                       });
                }
            }

            _inquiryBillService.AddInquiryBill(new InquiryBillDTO
            {
                Amount = feesModel.Amount,
                ProviderServiceResponseID = providerServiceResponseId,
                Sequence = 1
            });

            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

            feeResponse.Brn = feesModel.Brn == 0 ? providerServiceRequestId : feesModel.Brn;
            feeResponse.Code = 200.ToString();
            feeResponse.Message = _localizer["Success"].Value;
            return feeResponse;
        }

        public Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            string printedReciept = "";

            var denomination = _denominationService.GetDenomination(id);
            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);

            var DPCList = _denominationService.GetDenominationProviderConfigurationDetails(id);

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
            {
                ParameterName = "Client Number",
                ProviderServiceRequestID = providerServiceRequestId,
                Value = payModel.BillingAccount
            });
            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
            {
                ParameterName = "Total Amount",
                ProviderServiceRequestID = providerServiceRequestId,
                Value = totalAmount.ToString()
            });
            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
            {
                ParameterName = "Fees",
                ProviderServiceRequestID = providerServiceRequestId,
                Value = fees.ToString()
            });

            var newRequestId = _transactionService.AddRequest(new RequestDTO
            {
                AccountId = payModel.AccountId,
                Amount = payModel.Amount,
                BillingAccount = payModel.BillingAccount,
                ChannelID = payModel.ChannelId,
                DenominationId = id,
                HostTransactionId = payModel.HostTransactionID
            });

            //int query_result = Convert.ToInt32(DB.ExecuteScalar(System.Configuration.ConfigurationManager.ConnectionStrings["MomknConnection"].ConnectionString, "InsertElectronicChargeLog",
            //                                                                 GetProviderName(DS.DenominationID), _PDTO.BillingAccount, _PDTO.Amount, _TotalAmount, "", 0,
            //                                                                _LoginedUser.UserId, DS.OldServiceId).ToString());

            //Add New Request For Old Version
            int queryResult = _transactionService.AddElectronicChargeLog(GetProviderName(denomination.ServiceEntity), payModel.BillingAccount, (float)payModel.Amount, (float)totalAmount, "", 0, userId, denominationServiceProviderDetails.OldServiceId);

            // check balance 

            var serviceBalanceTypeId = _denominationService.GetServiceBalanceType(id);
            var balance = await _accountsApi.ApiAccountsAccountIdBalancesBalanceTypeIdGetAsync(payModel.AccountId, serviceBalanceTypeId);
            if (balance == null || ((decimal)balance.TotalAvailableBalance < totalAmount && (decimal)balance.TotalAvailableBalance != 0))
                throw new TMSException(_localizer["BalanceError"].Value, "-5");

            // post to hold
            await _accountsApi.ApiAccountsAccountIdBalancesBalanceTypeIdRequestsRequestIdPostAsync(payModel.AccountId, newRequestId, 1,
                new Models.SwaggerModels.HoldBalanceModel
                (
                    amount: (double)totalAmount
                ));

            //Logging Client Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
              payModel.Brn,
              LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchBodyRequest = new TopUpDTO
            {
                RequestID = newRequestId.ToString(),
                RequestDate = DateTime.Now.ToString(),
                Phone = payModel.BillingAccount,
                Amount = payModel.Amount.ToString(),
                Password = serviceConfiguration.UserPassword,
                UserName = serviceConfiguration.UserName,
                ProviderID = DPCList.Where(t => t.Name == "providerID").Select(z => z.Value).FirstOrDefault(),
                TerminalID = DPCList.Where(t => t.Name == "providerID").Select(z => z.Value).FirstOrDefault(),
                ServiceID = "8"
            };

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };


            await _loggingService.Log($"{JsonConvert.SerializeObject(switchBodyRequest)}:{JsonConvert.SerializeObject(switchEndPoint)}",
              providerServiceRequestId,
              LoggingType.ProviderRequest);


            var response = _switchService.Connect(switchBodyRequest, switchEndPoint, "", "Basic ");

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);
                if (o["code"].ToString() == "200" || o["code"].ToString() == "-300" || response.Contains("The operation has timed out") || string.IsNullOrEmpty(response) || response.Contains("underlying connection was closed"))
                {

                    paymentResponse.InvoiceId = _transactionService.AddInvoiceElectronicCharge(GetProviderName(denomination.ServiceEntity), payModel.BillingAccount, (float)payModel.Amount, (float)totalAmount, "", 0, userId, queryResult, denominationServiceProviderDetails.OldServiceId);


                    var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                    paymentResponse.TransactionId = transactionId;

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    // confirm sof
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                        new List<int?> { transactionId });

                    _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);

                    if (o["code"].ToString() == "-300")
                        _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Pending, userId, payModel.Brn);
                    else
                        printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                    // add commission
                    _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

                    try
                    {
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceElectronicChargeProcBeeAwIncomeNew(
                           paymentResponse.InvoiceId, payModel.Amount, denominationServiceProviderDetails.OldServiceId, userId, userId, totalAmount, payModel.BillingAccount);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);

                    _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);

                    // GET MESSAGE PROVIDER ID
                    var message = _dbMessageService.GetMainStatusCodeMessage(statusCode: GetData.GetCode(response), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);
                }
            }
            else
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                // GET MESSAGE PROVIDER ID
                var message = _dbMessageService.GetMainStatusCodeMessage(statusCode: GetData.GetCode(response), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            paymentResponse.Code = 200;
            paymentResponse.Message = _localizer["Success"].Value;
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.AvailableBalance = (decimal)balance.TotalAvailableBalance - totalAmount;
            paymentResponse.Receipt = new List<Root> {
                JsonConvert.DeserializeObject<Root>(printedReciept)
            };
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }

        private string GetProviderName(string serviceEntity)
        {
            switch (serviceEntity)
            {
                case "اورنج":
                    return "موبينيل";
                case "فودافون":
                    return "موبينيل";
                case "إتصالات":
                    return "إتصالات";
                default:
                    return "المصرية WE";
            }
        }
    }
}
