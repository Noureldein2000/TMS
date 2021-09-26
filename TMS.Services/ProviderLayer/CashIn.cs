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
    public class CashIn : IBaseProvider
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
        public CashIn(
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

            //Add ProviderServiceRequest
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Fees,
                BillingAccount = null,
                Brn = feesModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            // var providerFees = 0;
            var bills = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn);
            foreach (var item in bills)
            {
                feesModel.Amount = item.Amount;
            }

            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
            feeResponse.Fees = Math.Round(feesAmount, 3);
            feeResponse.TotalAmount = feesModel.Amount + feeResponse.Fees;

            //Add ProviderServiceResponse
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
            feeResponse.Brn = feesModel.Brn;
            feeResponse.Code = 200;
            feeResponse.Message = "Success";
            return feeResponse;
        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            decimal totalAmount;

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Inquiry,
                BillingAccount = inquiryModel.BillingAccount,
                Brn = null,
                CreatedBy = userId,
                DenominationID = id
            });

            if (inquiryModel.Data != null)
            {
                var billCount = inquiryModel.Data.Where(d => d.Key == "BillCount").FirstOrDefault();
                if (billCount != null)
                {
                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = "BillCount",
                        Value = billCount.Value,
                        ProviderServiceRequestID = providerServiceRequestId
                    });
                }
            }

            //Logging Client Request
            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                 providerServiceRequestId,
                 LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchRequestDto = new InquiryCashIn
            {
                BillReference = inquiryModel.BillingAccount,
                TransactionId = providerServiceRequestId
            };
            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };

            //Logging Provider Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
               providerServiceRequestId,
               LoggingType.ProviderRequest);

            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.inquiry.ToString(), "Basic ");

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);


            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);

                string inquiryReference = o["inquiryReference"].ToString();
                o = JObject.Parse(o["inquireData"].ToString());
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                totalAmount = decimal.Parse(o["cashinAmount"].ToString());

                string customerName = o["clientName"].ToString();

                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });

                _providerService.AddProviderServiceResponseParam(
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "billReferenceNumber",
                   ServiceRequestID = providerServiceResponseId,
                   Value = inquiryReference
               },
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "arabicName",
                   ServiceRequestID = providerServiceResponseId,
                   Value = customerName
               });

                inquiryResponse.Data.AddRange(new List<DataDTO>
                {
                    new DataDTO
                    {
                        Key = "arabicName",
                        Value = customerName
                    },
                    new DataDTO
                    {
                        Key = "billReferenceNumber",
                        Value = o["billReference"].ToString()
                    }
                });


                //Add InquiryBill
                var inquiryId = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                {
                    Amount = totalAmount,
                    ProviderServiceResponseID = providerServiceResponseId,
                    Sequence = 1
                });

                //Add Value To Receipt Body
                _inquiryBillService.AddReceiptBodyParam(
               new ReceiptBodyParamDTO
               {
                   ParameterName = "arabicName",
                   ProviderServiceRequestID = providerServiceRequestId,
                   TransactionID = 0,
                   Value = customerName
               });


                inquiryResponse.Brn = providerServiceRequestId;
                inquiryResponse.TotalAmount = totalAmount;
                inquiryResponse.Invoices = new List<InvoiceDTO>
                {
                    new InvoiceDTO
                    {
                        Amount = decimal.Parse(o["cashinAmount"].ToString()),
                        Sequence = 1
                    }
                };


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
            Root printedReciept = null;
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });
            var bills = _inquiryBillService.GetInquiryBillSequence(payModel.Brn);
            foreach (var item in bills)
            {
                payModel.Amount = item.Amount;
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

            var serviceBalanceTypeId = _denominationService.GetServiceBalanceType(id);
            var balance = await _accountsApi.ApiAccountsAccountIdBalancesBalanceTypeIdGetAsync(payModel.AccountId, serviceBalanceTypeId);
            if ((decimal)balance.TotalAvailableBalance < totalAmount && (decimal)balance.TotalAvailableBalance != 0)
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
            var billReferenceNumber = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "ar", "billReferenceNumber");

            var switchRequestDto = new PaymentCashIn
            {
                BillReference = payModel.BillingAccount,
                TransactionId = providerServiceRequestId,
                Amount = payModel.Amount,
                InquiryReference = billReferenceNumber.Select(s => s.Value).FirstOrDefault().ToString(),
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

            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.payment.ToString(), "Basic ");

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);


                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, null, newRequestId);
                paymentResponse.TransactionId = transactionId;

                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                // send add invoice to another data base system
                _transactionService.AddInvoiceCashIn(newRequestId, payModel.Amount, userId, o["providerTransactionId"].ToString(), int.Parse(denominationServiceProviderDetails.ProviderCode),
                    _providerService.GetProviderServiceResponseParams(providerServiceRequestId, "ar", "arabicName").Select(x => x.Value).FirstOrDefault());

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                // add commission
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

            }
            else if (response.Contains("The operation has timed out"))
            {

                var switchRequestDto2 = new CheckPaymentStatusCashIn
                {
                    TransactionId = int.Parse("C" + providerServiceRequestId.ToString()),
                    PaymentTransactionId = providerServiceRequestId.ToString(),
                };

                await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                  providerServiceRequestId,
                  LoggingType.ProviderRequest);

                response = _switchService.Connect(switchRequestDto, switchEndPoint, "check-status", "Basic ");

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);
                if (!string.IsNullOrEmpty(response))
                {

                    if (Validates.CheckJSON(response))
                    {

                        var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, null, newRequestId);
                        paymentResponse.TransactionId = transactionId;

                        // confirm sof
                        await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                            new List<int?> { transactionId });

                        // send add invoice to another data base system
                        //_transactionService.AddInvoice(newRequestId, payModel.Amount, userId, payModel.BillingAccount, fees, extraBillInfo.Values);

                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                        _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                        _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Pending, userId, payModel.Brn);

                        // add commission
                        _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

                    }
                    else
                    {
                        _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);

                    }
                }
                else
                {
                    _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);

                }
            }
            else
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                // GET MESSAGE PROVIDER ID
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);

            }

            paymentResponse.Code = 200;
            paymentResponse.Message = "Success";
            paymentResponse.InvoiceId = 0;
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.AvailableBalance = (decimal)balance.TotalAvailableBalance - totalAmount;
            paymentResponse.Receipt = new List<Root> {
                printedReciept
            };
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }
    }
}
