﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Infrastructure.Utils;
using TMS.Services.BusinessLayer;
using TMS.Services.Models;
using TMS.Services.Services;
using TMS.Services.SOFClientAPIs;

namespace TMS.Services.ProviderLayer
{
    public class WEInternet : IBaseProvider
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
        public WEInternet(
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
        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
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
                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = item.Key,
                        Value = item.Value,
                        ProviderServiceRequestID = providerServiceRequestId
                    });
                }
            }

            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                          providerServiceRequestId,
                          LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchRequestDto = new InquiryWeDTO
            {
                TelephoneNumber = inquiryModel.BillingAccount,
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
                var count = 1;
                JObject o = JObject.Parse(response);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                //DtMsg = DB_MessageMapping.GetMessage((int)DB_MessageMapping.MomknMessage.Sucess, 0, _IDTO.Language);

                var totalAmount = decimal.Parse(o["invoiceAmount"].ToString());
                inquiryResponse.Invoices.Add(new InvoiceDTO
                {
                    Sequence = count,
                    Amount = decimal.Parse(o["invoiceAmount"].ToString()),
                });
                inquiryResponse.TotalAmount = totalAmount;

                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });

                foreach (var item in inquiryResponse.Invoices)
                {
                    var inquiryId = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                    {
                        Amount = item.Amount,
                        ProviderServiceResponseID = providerServiceResponseId,
                        Sequence = count
                    });
                    count++;
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

        public FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            var feeResponse = new FeesResponseDTO();
            decimal providerFees = 0;
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
                        Value = item.Value,
                        ProviderServiceRequestID = providerServiceRequestId
                    });
                }

            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);
            if (denominationServiceProviderDetails.ProviderHasFees)
            {
                providerFees = 0;
            }
            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
            feeResponse.Fees = Math.Round(feesAmount + providerFees, 3);
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

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            Root printedReciept = null;
            string message = "";
            int code = 0;

            RequestStatusCodeType RequestStatus = RequestStatusCodeType.Success;

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });
            //_providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
            //{
            //    ParameterName = "",
            //    Value = "",
            //    ProviderServiceRequestID = providerServiceRequestId
            //});
            foreach (var item in payModel.Data)
            {
                _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                {
                    ParameterName = item.Key,
                    Value = item.Value,
                    ProviderServiceRequestID = providerServiceRequestId
                });
            }


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

            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);
            var requestID = _transactionService.AddTEDataProcLog("TEDATA", payModel.BillingAccount, totalAmount, "", 0, userId, denominationServiceProviderDetails.OldServiceId, 1);
            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);
            var switchRequestDto = new PaymentWeDTO
            {
                TransactionId = newRequestId,
                ExtraQuota = false,
                TelephoneNumber = payModel.BillingAccount,
                InvoiceAmount = Convert.ToInt32(Math.Truncate(payModel.Amount).ToString()).ToString()
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

            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.submitPayment.ToString(), "Basic ");
            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            _transactionService.TEDataLogUpdate("Payment=" + response, newRequestId);
            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);
                if (!string.IsNullOrEmpty(o["transactionId"].ToString()))
                {
                    Thread.Sleep(7000);

                    //CallBackResponse = DB.ExecuteScalar(System.Configuration.ConfigurationManager.ConnectionStrings["MomknConnection"].ConnectionString, "[ReturnTEData_CallbackResponse]", DepositeId).ToString();
                    if (!string.IsNullOrEmpty(""))
                    {
                        JObject oo = JObject.Parse("CallBackResponse");
                        if (oo["ResultCode"].ToString() == "0" || oo["ResultCode"].ToString() == "170001" || (oo["ResultCode"].ToString() == "179999" && oo["ResultDesc"].ToString().ToLower() != "primary party invalid."))
                        {
                            //DtMsg = DB_MessageMapping.GetMessage((int)DB_MessageMapping.MomknMessage.Sucess, 0, _PDTO.Language);
                            message = "Sucess";
                            code = 1;
                            RequestStatus = RequestStatusCodeType.Success;

                            var receiptNumber = oo["Transaction"].ToString();
                            paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataTest(
                                Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], DateTime.Now.ToString(), totalAmount,
                                     fees, 1, userId, payModel.BillingAccount, response, receiptNumber, "200", response, requestID, "WE ADSL", denominationServiceProviderDetails.OldServiceId);

                            var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                            paymentResponse.TransactionId = transactionId;
                            // confirm sof
                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                                new List<int?> { transactionId });

                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                            var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                            {
                                ProviderServiceRequestID = providerServiceRequestId,
                                TotalAmount = totalAmount
                            });
                            _providerService.AddProviderServiceResponseParam(
                           new ProviderServiceResponseParamDTO
                           {
                               ParameterName = "providerPaymentId",
                               ServiceRequestID = providerServiceResponseId,
                               Value = oo["Transaction"].ToString()
                           });
                            var providerPaymentIdParam = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "providerPaymentId");
                            paymentResponse.DataList.Add(new DataListDTO
                            {
                                Key = providerPaymentIdParam.FirstOrDefault().ParameterName,
                                Value = oo["Transaction"].ToString()
                            });

                            _inquiryBillService.AddReceiptBodyParam(
                              new ReceiptBodyParamDTO
                              {
                                  ParameterName = "providerPaymentId",
                                  ProviderServiceRequestID = payModel.Brn,
                                  TransactionID = transactionId,
                                  Value = oo["Transaction"].ToString()
                              });

                            if (oo["ResultCode"].ToString() == "170001" || oo["ResultCode"].ToString() == "179999")
                            {

                                message = "PendingTrx";
                                code = 3;
                                RequestStatus = RequestStatusCodeType.Pending;
                            }
                            else
                            {
                                message = "Success";
                                code = 1;
                                RequestStatus = RequestStatusCodeType.Success;
                            }

                            _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
                        }
                        else
                        {
                            RequestStatus = RequestStatusCodeType.Fail;
                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                            var msg = _dbMessageService.GetMainStatusCodeMessage(id: int.Parse(oo["ResultCode"].ToString()), providerId: serviceProviderId);
                            throw new TMSException(msg.Message, msg.Code);

                        }
                    }
                    else
                    {
                        var checkTransactionDTO = new CheckTransactionDTO
                        {
                            TransactionId = "ChkTrxn" + newRequestId,
                            PaymentTransactionId = newRequestId.ToString()

                        };

                        await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                          providerServiceRequestId,
                          LoggingType.ProviderRequest);

                        switchEndPoint.URL = "http://10.90.3.158:7001/WEService/rest/services/";
                        var checkResponse = _switchService.Connect(checkTransactionDTO, switchEndPoint, SwitchEndPointAction.queryTransaction.ToString(), "Basic ");

                        await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                        _transactionService.TEDataLogUpdate("-CheckTrxn=" + checkResponse, requestID);
                        if (Validates.CheckJSON(checkResponse))
                        {
                            JObject CheckTrxnO = JObject.Parse(checkResponse);
                            if (CheckTrxnO["transactionStatus"].ToString().ToLower() != "declined")
                            {
                                message = "Sucess";
                                code = 1;
                                RequestStatus = RequestStatusCodeType.Success;

                                var receiptNumber = CheckTrxnO["providerPaymentId"].ToString();

                                paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                                paymentResponse.TransactionId = transactionId;

                                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                                    new List<int?> { transactionId });

                                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                                {
                                    ProviderServiceRequestID = providerServiceRequestId,
                                    TotalAmount = totalAmount
                                });
                                _providerService.AddProviderServiceResponseParam(
                               new ProviderServiceResponseParamDTO
                               {
                                   ParameterName = "providerPaymentId",
                                   ServiceRequestID = providerServiceResponseId,
                                   Value = CheckTrxnO["providerPaymentId"].ToString()
                               });
                                var providerPaymentIdParam = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "providerPaymentId");
                                paymentResponse.DataList.Add(new DataListDTO
                                {
                                    Key = providerPaymentIdParam.FirstOrDefault().ParameterName,
                                    Value = CheckTrxnO["providerPaymentId"].ToString()
                                });

                                _inquiryBillService.AddReceiptBodyParam(
                                  new ReceiptBodyParamDTO
                                  {
                                      ParameterName = "providerPaymentId",
                                      ProviderServiceRequestID = payModel.Brn,
                                      TransactionID = transactionId,
                                      Value = CheckTrxnO["providerPaymentId"].ToString()
                                  });
                                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
                            }
                            else
                            {
                                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                                throw new TMSException("FailedTrx", "2");
                            }
                        }
                        else if (checkResponse.Contains("timed out"))
                        {
                            message = "PendingTrx";
                            code = 3;
                            RequestStatus = RequestStatusCodeType.Pending;

                            var receiptNumber = o["Transaction"].ToString();
                            paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                            var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                            paymentResponse.TransactionId = transactionId;

                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                                new List<int?> { transactionId });

                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                            _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
                            //throw new TMSException("PendingTrx", "3");
                        }
                        else if (checkResponse.Contains("720"))
                        {
                            message = "PendingTrx";
                            code = 3;
                            RequestStatus = RequestStatusCodeType.Pending;

                            var receiptNumber = o["Transaction"].ToString();
                            paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                            var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                            paymentResponse.TransactionId = transactionId;

                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                                new List<int?> { transactionId });

                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                            _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

                            //throw new TMSException("PendingTrx", "3");
                        }
                        else
                        {
                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                            //_transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                            throw new TMSException("GeneralError", "6");
                        }
                    }
                }
                else
                {
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                    //_transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                    var msg = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
                    throw new TMSException(msg.Message, msg.Code);
                }
            }
            else if (response.Contains("timed out"))
            {
                message = "PendingTrx";
                code = 3;
                RequestStatus = RequestStatusCodeType.Pending;

                paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", "", paymentResponse.InvoiceId, "Quota Top-up");

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

                //throw new TMSException("PendingTrx", "3");
            }
            else
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                //_transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                throw new TMSException("GeneralError", "6");
            }
            _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, paymentResponse.TransactionId);


            //if (paymentResponse.TransactionId == 0)
            //    paymentResponse.TransactionId = null;
            printedReciept = _transactionService.UpdateRequest(paymentResponse.TransactionId, newRequestId, "", RequestStatus, userId, payModel.Brn);

            //if (paymentResponse.TransactionId == null)
            //    paymentResponse.TransactionId = 0;
            //PR.DataList.Add(DList);

            paymentResponse.Code = code;
            paymentResponse.Message = message;
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
