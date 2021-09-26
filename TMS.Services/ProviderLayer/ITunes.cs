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
    public class ITunes : IBaseProvider
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
        public ITunes(
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


            if (Ds.ProviderHasFees)
            {
                ProviderFees = 0;
            }

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

            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            var currency = _denominationService.GetCurrencyValue(id);
            feesModel.Amount = Math.Round(feesModel.Amount * currency, 3);
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
                    if (item.Fees.ToString("0.000") != "0.000")
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

            if (feesModel.Brn == 0)
                feeResponse.Brn = providerServiceRequestId;
            else
                feeResponse.Brn = feesModel.Brn;

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
            Root printedReciept = null;

            var denomination = _denominationService.GetDenomination(id);
            var DS = _denominationService.GetDenominationServiceProvider(id);

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
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

            var switchRequestDto = new ITunesDTO
            {
                TransactionId = newRequestId.ToString(),
                ProductCode = denomination.OldDenominationID
            };

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };


            await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)}:{JsonConvert.SerializeObject(switchEndPoint)}",
              providerServiceRequestId,
              LoggingType.ProviderRequest);


            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.printPin.ToString(), "Basic ");

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);
            int transactionId;

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);


                // send add invoice to another data base system
                if (denomination.ServiceID == 36)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceITunes(newRequestId.ToString(), o["pin"].ToString(), o["serial"].ToString(), totalAmount, userId, denomination.OldDenominationID, "UAE", payModel.Amount, o["validTo"].ToString());
                else
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceSonyMicrosoft(newRequestId.ToString(), o["pin"].ToString(), o["serial"].ToString(), totalAmount, userId, denomination.OldDenominationID, DS.OldServiceId, payModel.Amount, "UAE", o["validTo"].ToString());


                transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                var providerServiceResponeId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });

                _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                {
                    ParameterName = "Pin",
                    ServiceRequestID = providerServiceResponeId,
                    Value = o["pin"].ToString()
                }, new ProviderServiceResponseParamDTO
                {
                    ParameterName = "Serial",
                    ServiceRequestID = providerServiceResponeId,
                    Value = o["serial"].ToString()
                },
                new ProviderServiceResponseParamDTO
                {
                    ParameterName = "End Date",
                    ServiceRequestID = providerServiceResponeId,
                    Value = o["validTo"].ToString()
                },
                new ProviderServiceResponseParamDTO
                {
                    ParameterName = "ProviderTransactionId",
                    ServiceRequestID = providerServiceResponeId,
                    Value = o["providerTransactionId"].ToString()
                });

                //Add DataList To Client Response 
                var x = 0;
                paymentResponse.DataList.AddRange(new List<DataListDTO>
                {
                    new DataListDTO
                    {
                        Key = _localizer["Pin"].Value,
                        Value = o["pin"].ToString()
                    },
                    new DataListDTO
                    {
                        Key = _localizer["Serial"].Value,
                        Value = o["serial"].ToString()
                    },
                     new DataListDTO
                    {
                        Key = _localizer["End Date"].Value,
                        Value = o["validTo"].ToString()
                    }
                     ,
                     new DataListDTO
                    {
                        Key = _localizer["ProviderTransactionId"].Value,
                        Value = o["providerTransactionId"].ToString()
                    }
                });

                //Add Value To Receipt
                _inquiryBillService.AddReceiptBodyParam(
              new ReceiptBodyParamDTO
              {
                  ParameterName = "Pin",
                  ProviderServiceRequestID = providerServiceRequestId,
                  TransactionID = transactionId,
                  Value = o["pin"].ToString()
              },
              new ReceiptBodyParamDTO
              {
                  ParameterName = "Serial",
                  ProviderServiceRequestID = providerServiceRequestId,
                  TransactionID = transactionId,
                  Value = o["serial"].ToString()
              },
              new ReceiptBodyParamDTO
              {
                  ParameterName = "End Date",
                  ProviderServiceRequestID = providerServiceRequestId,
                  TransactionID = transactionId,
                  Value = o["validTo"].ToString()
              }
              ,
              new ReceiptBodyParamDTO
              {
                  ParameterName = "ProviderTransactionId",
                  ProviderServiceRequestID = providerServiceRequestId,
                  TransactionID = transactionId,
                  Value = o["providerTransactionId"].ToString()
              });

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
                var message = _dbMessageService.GetMainStatusCodeMessage(statusCode: GetData.GetCode(response), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            paymentResponse.Code = 200;
            paymentResponse.Message = _localizer["Success"].Value;
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.TransactionId = transactionId;
            paymentResponse.AvailableBalance = (decimal)balance.TotalAvailableBalance - totalAmount;
            paymentResponse.Receipt = new List<Root> {
               printedReciept
            };
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }
    }
}
