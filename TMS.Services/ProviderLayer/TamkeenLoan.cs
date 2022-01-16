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
    public class TamkeenLoan : IBaseProvider
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
        public TamkeenLoan(
           IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITransactionService transactionService,
           IAccountsApi accountsApi
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
            _accountsApi = accountsApi;
        }

        public FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            var feeResponse = new FeesResponseDTO();
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Fees,
                BillingAccount = null,
                Brn = feesModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });
            //var providerResponseParams = _providerService.GetProviderServiceResponseParams(feesModel.Brn, language: "ar", "amountFees");
            //var amountFees = providerResponseParams.Where(s => s.ProviderName == "amountFees").Select(s => s.Value).FirstOrDefault().ToString();

            //var bills = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn);
            //foreach (var item in bills)
            //{
            //    feesModel.Amount = item.Amount;
            //}
            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
            feeResponse.Fees = Math.Round(feesAmount + 0, 3);
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
                               TransactionID = null,
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
                           TransactionID = null,
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
            var denomination = _denominationService.GetDenomination(id);

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

            //var providerResponseParams = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "ar", "BranchNumber",
            //    "DueDate", "AccountNumber", "Balance");
            string ProviderResponse = _loggingService.GetLog(payModel.Brn, LoggingType.ProviderResponse);
            var jsonMessage = JsonConvert.DeserializeObject<ProviderResponseDTO>(ProviderResponse);
            JObject o = JObject.Parse(jsonMessage.Message);

            var branchNumber = o["BranchNumber"].ToString();
            var dueDate = o["DueDate"].ToString();
            var accountNumber = o["AccountNumber"].ToString();
            var balanceParam = o["Balance"].ToString();

            var switchRequestDto = new PaymentLoan
            {
                AccountNumber = accountNumber,
                Amount = payModel.Amount,
                BillingAccount = payModel.BillingAccount,
                UserName = serviceConfiguration.UserName,
                Password = serviceConfiguration.UserPassword,
                RequestID = newRequestId.ToString()
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

            //Note: third paramter maybe puted in url
            var response = _switchService.Connect(switchRequestDto, switchEndPoint, "", "Basic ");
            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                o = JObject.Parse(response.Message);

                if (o["code"].ToString() == "200")
                {
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceTamkeenLoan(payModel.AccountId, payModel.Amount, userId, payModel.BillingAccount, int.Parse(denomination.OldDenominationID),
                    211, totalAmount, fees, accountNumber, o["providerTransactionID"].ToString(), response.Message, branchNumber, dueDate);

                    var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                    paymentResponse.TransactionId = transactionId;
                    // confirm sof
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                        new List<int?> { transactionId });

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                    //_inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                    _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Success);
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
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);
                }
            }
            else if (response.Code == -200)
            {
                paymentResponse.InvoiceId = _transactionService.AddInvoiceTamkeenLoan(payModel.AccountId, payModel.Amount, userId, payModel.BillingAccount, int.Parse(denomination.OldDenominationID),
                   211, totalAmount, fees, accountNumber, "", response.Message, branchNumber, dueDate);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                paymentResponse.TransactionId = transactionId;
                // confirm sof
                //await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                //    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                //_inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Pending);
                _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Pending, userId, payModel.Brn);

                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
            }
            else
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                // GET MESSAGE PROVIDER ID
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: serviceProviderId);
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

            var switchRequestDto = new SwitchPaymentRequestBodyDTO
            {
                BillsCount = 1,
                PaymentCode = inquiryModel.BillingAccount,
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

            var url = switchEndPoint.URL + "?requestID=" + providerServiceRequestId + "&billingAccount=" + inquiryModel.BillingAccount + "&userName=" + switchEndPoint.UserName + "&password=" + switchEndPoint.UserPassword;
            var response = _switchService.Connect(switchEndPoint, url);
            //response = "{\"code\":200,\"message\":\"Successful Inquiry.\",\"requestID\":\"70779\",\"AccountNumber\":\"002-00000461\",\"BranchNumber\":\"\u00d8\u00a7\u00d9\u201e\u00d8\u00a3\u00d9\u201a\u00d8\u00b5\u00d8\u00b1\",\"Balance\":\"EGP15,081.00\",\"DueDate\":\"01-08-2022\",\"totalAmount\":2117.0,\"totalPaidAmount\":\"EGP0.00\",\"penaltyDue\":\"EGP0.00\",\"penaltyPaid\":\"EGP0.00\",\"openAmount\":0.0,\"providerTransactionID\":\"59730\",\"loanNumber\":0}";
            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);
                if (o["code"].ToString() == "200")
                {
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    //totalAmount = decimal.Parse(o["openAmount"].ToString());
                    totalAmount = decimal.Parse(o["totalAmount"].ToString());

                    var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                    {
                        ProviderServiceRequestID = providerServiceRequestId,
                        TotalAmount = totalAmount
                    });

                    _providerService.AddProviderServiceResponseParam(
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "AccountNumber",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["AccountNumber"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "BranchNumber",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["BranchNumber"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "Balance",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["Balance"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "DueDate",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["DueDate"].ToString()
                        },
                         new ProviderServiceResponseParamDTO
                         {
                             ParameterName = "totalAmount",
                             ServiceRequestID = providerServiceResponseId,
                             Value = o["totalAmount"].ToString()
                         },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "openAmount",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["openAmount"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "totalPaidAmount",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["totalPaidAmount"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "penaltyDue",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["penaltyDue"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "penaltyPaid",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["penaltyPaid"].ToString()
                        },
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "loanNumber",
                            ServiceRequestID = providerServiceResponseId,
                            Value = o["loanNumber"].ToString()
                        }
                        );

                    inquiryResponse.Data.AddRange(new List<DataDTO>
                {
                    new DataDTO
                    {
                        Key = "AccountNumber",
                        Value = o["AccountNumber"].ToString()
                    },
                        new DataDTO
                    {
                        Key = "BranchNumber",
                        Value = o["BranchNumber"].ToString()
                    },
                        new DataDTO
                    {
                        Key = "Balance",
                        Value = o["Balance"].ToString()
                    },
                        new DataDTO
                    {
                        Key = "DueDate",
                        Value = o["DueDate"].ToString()
                    }, new DataDTO
                    {
                        Key = "totalAmount",
                        Value = o["totalAmount"].ToString()
                    }
                    ,    new DataDTO
                    {
                        Key = "openAmount",
                        Value = o["openAmount"].ToString()
                    }
                    , new DataDTO
                    {
                        Key = "totalPaidAmount",
                        Value = o["totalPaidAmount"].ToString()
                    }, new DataDTO
                    {
                        Key = "penaltyDue",
                        Value = o["penaltyDue"].ToString()
                    }, new DataDTO
                    {
                        Key = "totalPaidAmount",
                        Value = o["totalPaidAmount"].ToString()
                    }, new DataDTO
                    {
                        Key = "penaltyPaid",
                        Value = o["penaltyPaid"].ToString()
                    },
                        new DataDTO
                    {
                        Key = "loanNumber",
                        Value = o["loanNumber"].ToString()
                    }
                  });

                    //Add InquiryBill
                    _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                    {
                        Amount = decimal.Parse(o["totalAmount"].ToString()),
                        ProviderServiceResponseID = providerServiceResponseId,
                        Sequence = 1
                    });


                    inquiryResponse.Brn = providerServiceRequestId;
                    inquiryResponse.TotalAmount = totalAmount;
                    inquiryResponse.Invoices = new List<InvoiceDTO>
                    {
                    new InvoiceDTO
                        {
                          Amount = decimal.Parse(o["totalAmount"].ToString()),
                          Sequence = 1
                        }
                    };
                }
                else
                {
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);

                }
            }
            else
            {
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            inquiryResponse.Code = 200;
            inquiryResponse.Message = "Success";

            //Logging Client Response
            await _loggingService.Log(JsonConvert.SerializeObject(inquiryResponse), providerServiceRequestId, LoggingType.CustomerResponse);
            return inquiryResponse;
        }
    }
}
