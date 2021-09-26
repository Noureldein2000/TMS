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
    public class OrangeInternet : IBaseProvider
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
        public OrangeInternet(
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
                    feeResponse.Data.Add(new DataDTO
                    {
                        Key = item.FeesTypeName,
                        Value = item.Fees.ToString("0.000")
                    });

                    if (item.Fees.ToString("0.000") != "0.000")
                    {
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
            feeResponse.Brn = feesModel.Brn;
            feeResponse.Code = 200;
            feeResponse.Message = "Success";
            return feeResponse;
        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            int count = 1;
            decimal totalAmount = 0;

            var denomation = _denominationService.GetDenominationServiceProvider(id);

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Inquiry,
                BillingAccount = inquiryModel.BillingAccount,
                Brn = null,
                CreatedBy = userId,
                DenominationID = id
            });

            //Logging Client Request
            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                providerServiceRequestId,
                LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };

            //Logging Provider Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(switchEndPoint)}",
               providerServiceRequestId,
               LoggingType.ProviderRequest);


            string url = switchEndPoint.URL + "requestID=" + providerServiceRequestId + "&telephoneCode=" + Validates.GetCodeAndTelephone(inquiryModel.BillingAccount)[0] + "&phone=" + Validates.GetCodeAndTelephone(inquiryModel.BillingAccount)[1] + "&userName=" + switchEndPoint.UserName + "&password=" + switchEndPoint.UserPassword;

            var response = _switchService.Connect(switchEndPoint, url);

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);
                if (o["code"].ToString() == "200")
                {

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                    {
                        ProviderServiceRequestID = providerServiceRequestId,
                        TotalAmount = totalAmount
                    });

                    inquiryResponse.Invoices = new List<InvoiceDTO>
                {
                    new InvoiceDTO
                    {
                        Amount = decimal.Parse(o["Total_Amount"].ToString()),
                        Sequence = 1
                    }
                };

                    totalAmount += inquiryResponse.Invoices.Sum(x => x.Amount);

                    inquiryResponse.Brn = providerServiceResponseId;
                    inquiryResponse.TotalAmount = totalAmount;

                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = "arabicName",
                        Value = o["customerName"].ToString()
                    });

                    //Add InquiryBill
                    foreach (var item in inquiryResponse.Invoices)
                    {
                        _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                        {
                            Amount = item.Amount,
                            ProviderServiceResponseID = providerServiceResponseId,
                            Sequence = count
                        });
                        count++;
                    }

                    //Add Value To Receipt
                    _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                    {
                        ParameterName = "arabicName",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = o["customerName"].ToString()
                    });
                }
                else
                {
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);
                }
            }
            else
            {
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            inquiryResponse.Code = 200;
            inquiryResponse.Message = "Success";

            //Logging Client Response
            await _loggingService.Log(JsonConvert.SerializeObject(inquiryResponse), providerServiceRequestId, LoggingType.CustomerResponse);
            return inquiryResponse;
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            string printedReciept = "";

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            foreach (var item in payModel.Data)
            {
                _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                {
                    ParameterName = item.Key,
                    ProviderServiceRequestID = providerServiceRequestId,
                    Value = item.Value,

                });
            }

            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);

            var newRequestId = _transactionService.AddRequest(new RequestDTO
            {
                AccountId = payModel.AccountId,
                Amount = payModel.Amount,
                BillingAccount = payModel.BillingAccount,
                ChannelID = payModel.ChannelId,
                DenominationId = id,
                HostTransactionId = payModel.HostTransactionID
            });
            // check balance 

            var serviceBalanceTypeId = _denominationService.GetServiceBalanceType(id);
            var balance = await _accountsApi.ApiAccountsAccountIdBalancesBalanceTypeIdGetAsync(payModel.AccountId, serviceBalanceTypeId);
            if (balance == null || ((decimal)balance.TotalAvailableBalance < totalAmount && (decimal)balance.TotalAvailableBalance != 0))
                throw new TMSException("BalanceError", "-5");

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

            var switchBodyRequest = new PaymentOrangeInternet
            {
                RequestID = newRequestId.ToString(),
                Phone = Validates.GetCodeAndTelephone(payModel.BillingAccount)[1],
                TelephoneCode = Validates.GetCodeAndTelephone(payModel.BillingAccount)[0],
                UserName = serviceConfiguration.UserName,
                Password = serviceConfiguration.UserPassword,
                Amount = payModel.Amount.ToString(),
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

            var transactionId = 0;
            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);
                if (o["Code"].ToString() != "200")
                {

                    // send add invoice to another data base system
                    if (denominationServiceProviderDetails.DenominationId == 60)
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceLinkDsl(
                            Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1],
                            DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "", "");
                    else
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceLinkDslReseller(
                            Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1],
                            DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "", "");


                    transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                    paymentResponse.TransactionId = transactionId;

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    // confirm sof
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                        new List<int?> { transactionId });

                    _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                    printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                    // add commission
                    _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
                }
                else
                {
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);

                    _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);

                    // GET MESSAGE PROVIDER ID
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);
                }
            }
            else if (response.Contains("timed out"))
            {
                // send add invoice to another data base system
                if (denominationServiceProviderDetails.DenominationId == 60)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceLinkDsl(
                        Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1],
                        DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "", "");
                else
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceLinkDslReseller(
                        Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1],
                        DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "", "");

                transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Pending, userId, payModel.Brn);
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
            }
            else
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                // GET MESSAGE PROVIDER ID
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            paymentResponse.Code = 200;
            paymentResponse.Message = "Success";
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.TransactionId = transactionId;
            paymentResponse.AvailableBalance = (decimal)balance.TotalAvailableBalance - totalAmount;
            paymentResponse.Receipt = new List<Root> {
                JsonConvert.DeserializeObject<Root>(printedReciept)
            };
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }
    }
}
