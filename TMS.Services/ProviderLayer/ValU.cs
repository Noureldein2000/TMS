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
    public class ValU : IBaseProvider
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
        public ValU(
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
            int count = 1;
            decimal totalAmount = 0;
            InquiryValU IV = new InquiryValU();
            List<InvoiceDTO> InList = new List<InvoiceDTO>();

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

            if (inquiryModel.Data != null)
            {
                foreach (var item in inquiryModel.Data)
                {
                    if (item.Key == "BillingAccountType")
                    {
                        if (item.Value == "4")
                        {
                            IV.CustomerCode = "CustomerCode";
                            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                            {
                                ParameterName = "customerCode",
                                ProviderServiceRequestID = providerServiceRequestId,
                                Value = inquiryModel.BillingAccount
                            });
                        }

                        else if (item.Value == "1")
                        {
                            IV.MobileNumber = "MobileNumber";
                            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                            {
                                ParameterName = "MobileNumber",
                                ProviderServiceRequestID = providerServiceRequestId,
                                Value = inquiryModel.BillingAccount
                            });
                        }

                        else if (item.Value == "3")
                        {
                            IV.PassportNnumber = "PassportNumber";
                            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                            {
                                ParameterName = "PassportNumber",
                                ProviderServiceRequestID = providerServiceRequestId,
                                Value = inquiryModel.BillingAccount
                            });
                        }
                        else if (item.Value == "2")
                        {
                            IV.NationalId = "NationalId";
                            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                            {
                                ParameterName = "NationalId",
                                ProviderServiceRequestID = providerServiceRequestId,
                                Value = inquiryModel.BillingAccount
                            });
                        }
                        break;
                    }
                }
            }

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

            IV.TransactionId = providerServiceRequestId.ToString();

            var response = _switchService.Connect(IV, switchEndPoint, SwitchEndPointAction.inquiry.ToString(), "Basic ", UrlType.Custom);

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);


                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                var PurchaseList = JsonConvert.DeserializeObject<List<Purchase>>(o["purchases"].ToString());

                totalAmount = decimal.Parse(o["totalPayableAmount"].ToString());

                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });

                //Add InquiryBill
                foreach (var item in PurchaseList)
                {
                    var InquiryBill = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                    {
                        Amount = decimal.Parse(item.AmountPayable),
                        ProviderServiceResponseID = providerServiceResponseId,
                        Sequence = count
                    });

                    _inquiryBillService.AddInquiryBillDetail(
                        new InquiryBillDetailDTO
                        {
                            InquiryBillID = InquiryBill,
                            ParameterName = "PaymentDueDate",
                            Value = item.PaymentDueDate
                        },
                         new InquiryBillDetailDTO
                         {
                             InquiryBillID = InquiryBill,
                             ParameterName = "PurchaseCode",
                             Value = item.PurchaseCode
                         },
                          new InquiryBillDetailDTO
                          {
                              InquiryBillID = InquiryBill,
                              ParameterName = "PurchaseId",
                              Value = item.PurchaseId
                          });

                    InvoiceDTO Is = new InvoiceDTO();
                    Is.Amount = decimal.Parse(item.AmountPayable);
                    Is.Sequence = count;

                    var IBDList = _inquiryBillService.GetInquiryBillDetails(providerServiceRequestId, Is.Sequence);
                    if (IBDList != null)
                    {
                        foreach (var IBD in IBDList)
                        {
                            Is.Data.Add(new DataDTO
                            {
                                Key = IBD.ParameterName,
                                Value = IBD.Value
                            });
                        }
                    }

                    InList.Add(Is);
                }

                inquiryResponse.Invoices = InList;
                inquiryResponse.Brn = providerServiceRequestId;
                inquiryResponse.TotalAmount = totalAmount;

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

            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);

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
                ParameterName = "",
                ProviderServiceRequestID = providerServiceRequestId,
                Value = ""
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

            var paramters = _providerService.GetProviderServiceRequestParams(payModel.Brn, "en", "customerCode", "MobileNumber", "NationalId", "PassportNnumber");

            var customerCode = paramters.Where(x => x.Key == "customerCode").Select(x => x.Value).FirstOrDefault();
            var mobileNumber = paramters.Where(x => x.Key == "Mobile Number").Select(x => x.Value).FirstOrDefault();
            var nationalId = paramters.Where(x => x.Key == "NationalId").Select(x => x.Value).FirstOrDefault();
            var passportNnumber = paramters.Where(x => x.Key == "PassportNnumber").Select(x => x.Value).FirstOrDefault();

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchBodyRequest = new PaymentValU
            {
                Amount = payModel.Amount,
                TransactionId = newRequestId.ToString(),
                CustomerCode = customerCode == null ? "" : customerCode,
                MobileNumber = mobileNumber == null ? "" : mobileNumber,
                NationalId = nationalId == null ? "" : nationalId,
                PassportNnumber = passportNnumber == null ? "" : passportNnumber,
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


            var response = _switchService.Connect(switchBodyRequest, switchEndPoint, SwitchEndPointAction.payment.ToString(), "Basic ", UrlType.Custom);

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);


                paymentResponse.InvoiceId = _transactionService.AddInvoiceValU(newRequestId.ToString(), payModel.Amount, userId, mobileNumber, passportNnumber, nationalId, customerCode);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId
});

                var providerServiceResponeId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });
                _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                {
                    ParameterName = "providerPaymentId",
                    ServiceRequestID = providerServiceResponeId,
                    Value = o["providerPaymentId"].ToString()
                });
                var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "providerPaymentId");

                paymentResponse.DataList.Add(new DataListDTO
                {
                    Key = responseParams.Where(p => p.ProviderName == "providerPaymentId").Select(s => s.ParameterName).FirstOrDefault(),
                    Value = o["providerPaymentId"].ToString()
                });

                _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                {
                    ParameterName = "providerPaymentId",
                    ProviderServiceRequestID = providerServiceRequestId,
                    TransactionID = transactionId,
                    Value = o["providerPaymentId"].ToString()
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
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            paymentResponse.Code = 200;
            paymentResponse.Message = "Success";
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
