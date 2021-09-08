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
    public class BTech : IBaseProvider
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
        public BTech(
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
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = 1,
                RequestTypeID = 2,
                BillingAccount = null,
                Brn = feesModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });
            var providerFees = _denominationService.GetProviderServiceResponseParam(feesModel.Brn, "amountFees").FirstOrDefault();
            var bills = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn);
            foreach (var item in bills)
            {
                feesModel.Amount = item.Amount;
            }
            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
            feeResponse.Fees = Math.Round(feesAmount + decimal.Parse(providerFees.Value), 3);
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
                            ParameterName = ParameterProviderNames.ServiceFees.ToString(),// "Service Fees",
                            ServiceRequestID = providerServiceResponseId,
                            Value = feesAmount.ToString("0.000")
                        });
                    _inquiryBillService.AddReceiptBodyParam(
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = ParameterProviderNames.ServiceFees.ToString(),// "Service Fees",
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
            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, 2, userId);
            feeResponse.Brn = feesModel.Brn;
            feeResponse.Code = 200.ToString();
            feeResponse.Message = _localizer["Success"].Value;
            return feeResponse;
        }
        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = 1,
                RequestTypeID = 3,
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

            ////Conncet Balance System
            //BalancesResponse BR = IntegerationLayer.IL_Balance.Connect("GET", int.Parse(_LoginedUser.AccountID), _DenominationID);
            //BalanceBefore = BR.TotalAvailableBalance;
            //BalanceTypeID = BR.Balances[0].ID;
            // check balance 

            var serviceBalanceTypeId = _denominationService.GetServiceBalanceType(id);
            var balance = await _accountsApi.ApiAccountsAccountIdBalancesBalanceTypeIdGetAsync(payModel.AccountId, serviceBalanceTypeId);
            if ((decimal)balance.TotalAvailableBalance < totalAmount && (decimal)balance.TotalAvailableBalance != 0)
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
            var billReferenceNumber = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "billReferenceNumber");
            var billCount = _denominationService.GetProviderServiceRequestParam(payModel.Brn, "BillCount");
            var asyncRqUID = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "AsyncRqUID");
            var extraBillInfo = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "ExtraBillInfo");
            var amountFees = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "amountFees");

            var switchRequestDto = new SwitchPaymentRequestBodyDTO
            {
                BillsCount = 1,
                PaymentCode = payModel.BillingAccount,
                TransactionId = providerServiceRequestId,
                AsyncRqUID = asyncRqUID.Select(s => s.Value).FirstOrDefault().ToString(),
                Amount = payModel.Amount,
                BillRefNumber = billReferenceNumber.Select(s => s.Value).FirstOrDefault().ToString(),
                ExtraBillInfo = extraBillInfo.Select(s => s.Value).FirstOrDefault().ToString(),
                Fees = amountFees.Select(s => s.Value).FirstOrDefault().ToString(),
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
                //_transactionService.AddInvoice(newRequestId, payModel.Amount, userId, payModel.BillingAccount, fees, extraBillInfo.Values);

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, 2, userId);
                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _transactionService.UpdateRequest(transactionId, newRequestId, "", 2, userId, payModel.Brn);

                // add commission
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

            }
            else if (response.Contains("timed out"))
            {
                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, null, newRequestId);

                paymentResponse.TransactionId = transactionId;
                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, 2, userId);
                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _transactionService.UpdateRequest(transactionId, newRequestId, "", 4, userId, payModel.Brn);
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
            }
            else
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, 3, userId);
                _transactionService.UpdateRequestStatus(newRequestId, 3);
                // GET MESSAGE PROVIDER ID
                var message = _dbMessageService.GetMainStatusCodeMessage(statusCode: GetData.GetCode(response), providerId: serviceProviderId);

            }
            paymentResponse.Code = 200;
            paymentResponse.Message = _localizer["Success"].Value;
            paymentResponse.InvoiceId = 0;
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.AvailableBalance = (decimal)balance.TotalAvailableBalance - totalAmount;
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }
        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            string countRemainInstalment = "",
                requestCode,
                countInstalmentPenalty = "",
                valueInstalmentPenalty = "";
            decimal totalAmount;
            string[] countRemainInstalmentList, requestCodeList;

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = 1,
                RequestTypeID = 1,
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


            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.inquiry.ToString(), "Basic ");

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, 2, userId);

                totalAmount = decimal.Parse(o["amount"].ToString()) + decimal.Parse(o["fees"].ToString());

                string[] customerDetail = o["extraBillInfo"].ToString().Split(';');
                string SEQ1 = customerDetail[0];
                string SEQ2 = customerDetail[1];
                string SEQ3 = customerDetail[2];
                string SEQ4 = customerDetail[3];

                string[] customerName = SEQ1.Split(':');
                string[] instalmentDate = SEQ2.Split(':');

                if (o["serviceId"].ToString() == "309")
                {
                    string[] SEQ3_1 = SEQ3.Split(':');
                    string[] CountInstalmentPenaltyList = SEQ3_1[1].Split(' ');
                    countInstalmentPenalty = CountInstalmentPenaltyList[1];
                    string[] ValueInstalmentPenaltyList = SEQ3_1[2].Split('ج');
                    valueInstalmentPenalty = ValueInstalmentPenaltyList[0].Replace(" ", "");
                    string[] SEQ4_1 = SEQ4.Split(' ');
                    countRemainInstalmentList = SEQ4_1[2].Split(':');
                    requestCodeList = SEQ4_1[4].Split(':');
                    countRemainInstalment = countRemainInstalmentList[1].ToString();
                    requestCode = requestCodeList[1].ToString();
                }
                else
                {
                    requestCodeList = SEQ4.Split(':');
                    requestCode = requestCodeList[1].ToString();
                }
                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });
                _providerService.AddProviderServiceResponseParam(
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "amountFees",
                        ServiceRequestID = providerServiceResponseId,
                        Value = o["fees"].ToString()
                    },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "billReferenceNumber",
                        ServiceRequestID = providerServiceResponseId,
                        Value = o["billRefNumber"].ToString()
                    },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "AsyncRqUID",
                        ServiceRequestID = providerServiceResponseId,
                        Value = o["asyncRqUID"].ToString()
                    },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "ExtraBillInfo",
                        ServiceRequestID = providerServiceResponseId,
                        Value = o["extraBillInfo"].ToString()
                    },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "arabicName",
                        ServiceRequestID = providerServiceResponseId,
                        Value = customerName[1]
                    },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "PaymentDueDate",
                        ServiceRequestID = providerServiceResponseId,
                        Value = instalmentDate[2]
                    },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "RequestCode",
                        ServiceRequestID = providerServiceResponseId,
                        Value = requestCode
                    });

                if (!string.IsNullOrEmpty(countInstalmentPenalty) && countInstalmentPenalty != "0")
                {
                    _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "CountInstalmentPenalty",
                        ServiceRequestID = providerServiceResponseId,
                        Value = countInstalmentPenalty
                    });
                    inquiryModel.Data.Add(new DataDTO
                    {
                        Key = _localizer["CountInstalmentPenalty"].Value,
                        Value = countInstalmentPenalty
                    });
                }

                if (!string.IsNullOrEmpty(valueInstalmentPenalty))
                {
                    _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "ValueInstalmentPenalty",
                        ServiceRequestID = providerServiceResponseId,
                        Value = valueInstalmentPenalty
                    });
                    inquiryModel.Data.Add(new DataDTO
                    {
                        Key = _localizer["ValueInstalmentPenalty"].Value,
                        Value = valueInstalmentPenalty
                    });
                }

                if (string.IsNullOrEmpty(countRemainInstalment))
                {
                    _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "CountRemainInstalment",
                        ServiceRequestID = providerServiceResponseId,
                        Value = countRemainInstalment
                    });
                    inquiryModel.Data.Add(new DataDTO
                    {
                        Key = _localizer["CountRemainInstalment"].Value,
                        Value = countRemainInstalment
                    });
                }

                inquiryModel.Data.AddRange(new List<DataDTO>
                {
                    new DataDTO
                    {
                        Key = _localizer["arabicName"].Value,
                        Value = customerName[1]
                    },
                    new DataDTO
                    {
                        Key = _localizer["PaymentDueDate"].Value,
                        Value = instalmentDate[2]
                    },

                    new DataDTO
                    {
                        Key = _localizer["RequestCode"].Value,
                        Value = requestCode
                    },
                });

                var inquiryId = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                {
                    Amount = totalAmount,
                    ProviderServiceResponseID = providerServiceResponseId,
                    Sequence = 1
                });

                _inquiryBillService.AddReceiptBodyParam(
                    new ReceiptBodyParamDTO
                    {
                        ParameterName = "Provider Service Fees",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = o["fees"].ToString()
                    },
                    new ReceiptBodyParamDTO
                    {
                        ParameterName = "arabicName",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = customerName[1]
                    },
                    new ReceiptBodyParamDTO
                    {
                        ParameterName = "PaymentDueDate",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = instalmentDate[2]
                    },
                    new ReceiptBodyParamDTO
                    {
                        ParameterName = "RequestCode",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = requestCode
                    });

                if (!string.IsNullOrEmpty(countInstalmentPenalty) && countInstalmentPenalty != "0")
                    _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                    {
                        ParameterName = "CountInstalmentPenalty",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = countInstalmentPenalty
                    });
                if (!string.IsNullOrEmpty(valueInstalmentPenalty))
                    _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                    {
                        ParameterName = "ValueInstalmentPenalty",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = valueInstalmentPenalty
                    });
                if (!string.IsNullOrEmpty(countRemainInstalment))
                    _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                    {
                        ParameterName = "CountRemainInstalment",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = 0,
                        Value = countRemainInstalment
                    });

                inquiryResponse.Brn = providerServiceRequestId;
                inquiryResponse.TotalAmount = totalAmount;
                inquiryResponse.Invoices = new List<InvoiceDTO>
                {
                    new InvoiceDTO
                    {
                        Amount = decimal.Parse(o["amount"].ToString()),
                        Sequence = 1
                    }
                };
            }
            else
            {
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, 3, userId);
                var message = _dbMessageService.GetMainStatusCodeMessage(statusCode: GetData.GetCode(response), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }

            inquiryResponse.Code = 200.ToString();
            inquiryResponse.Message = _localizer["Success"].Value;

            //Logging Client Response
            await _loggingService.Log(JsonConvert.SerializeObject(inquiryResponse), providerServiceRequestId, LoggingType.CustomerResponse);
            return inquiryResponse;
        }
    }
}
