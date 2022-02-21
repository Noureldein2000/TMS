using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WEInternetExtra : IBaseProvider
    {
        private readonly IDenominationService _denominationService;
        private readonly IProviderService _providerService;
        private readonly ISwitchService _switchService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ILoggingService _loggingService;
        private readonly IDbMessageService _dbMessageService;
        private readonly IFeesService _feesService;
        private readonly ITaxService _taxesService;
        private readonly ITransactionService _transactionService;
        private readonly IAccountsApi _accountsApi;
        public WEInternetExtra(
                IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITaxService taxesService,
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
            _taxesService = taxesService;
            _transactionService = transactionService;
            _accountsApi = accountsApi;
        }

        public FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            int Sequence = 0;
            bool flag = false;
            decimal providerFees = 0;
            var feeResponse = new FeesResponseDTO();
            if (!_inquiryBillService.CheckBillAmountExist(feesModel.Brn, feesModel.Amount))
            {
                var message = _dbMessageService.GetMainStatusCodeMessage((int)MomknMessage.AmountNotMatched);
                throw new TMSException(message.Message, message.Code);
            }
            else
            {

                if (feesModel.Data != null)
                    foreach (var item in feesModel.Data)
                    {
                        if (item.Key == "Sequence")
                        {
                            Sequence = int.Parse(item.Value);
                            flag = true;
                        }
                    }

                if (!flag)
                {
                    var brnFees = _providerService.GetMaxProviderServiceRequest(feesModel.Brn, RequestType.Fees);

                    List<InquiryBillDTO> IBList = _inquiryBillService.GetInquiryBillSequence(brnFees);
                    flag = true;
                    foreach (var item in IBList)
                    {
                        Sequence = item.Sequence;
                        feesModel.Amount = item.Amount;
                    }
                }
                if (flag)
                {
                    var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);
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
                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = "Sequence",
                        ProviderServiceRequestID = providerServiceRequestId,
                        Value = Sequence.ToString()
                    });
                    if (denominationServiceProviderDetails.ProviderHasFees)
                    {
                        providerFees = 0;
                    }

                    var taxesList = _taxesService.GetTaxes(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal taxesAmount).ToList();

                    var feesList = _feesService.GetFees(id, feesModel.Amount + taxesAmount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();

                    feeResponse.Amount = Math.Round(feesModel.Amount, 3);
                    feeResponse.Fees = Math.Round(feesAmount + providerFees, 3);
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
                                feeResponse.Data.Add(new DataDTO
                                {
                                    Key = item.FeesTypeName,
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
                    //Add taxes into provider service Response param
                    if (taxesList.Count > 0)
                    {
                        foreach (var item in taxesList)
                        {
                            if (item.Taxes.ToString("0.000") != "0.000")
                            {
                                feeResponse.Data.Add(new DataDTO
                                {
                                    Key = item.TaxesTypeName,
                                    Value = item.Taxes.ToString("0.000")
                                });
                                _providerService.AddProviderServiceResponseParam(
                                    new ProviderServiceResponseParamDTO
                                    {
                                        ParameterName = item.TaxesTypeName,
                                        ServiceRequestID = providerServiceResponseId,
                                        Value = item.Taxes.ToString("0.000")
                                    });
                                _inquiryBillService.AddReceiptBodyParam(
                                   new ReceiptBodyParamDTO
                                   {
                                       ParameterName = item.TaxesTypeName,
                                       ProviderServiceRequestID = feesModel.Brn,
                                       TransactionID = null,
                                       Value = item.Taxes.ToString("0.000")
                                   });
                            }
                        }
                    }
                    else
                    {
                        if (taxesAmount.ToString() != "0.000")
                        {
                            _providerService.AddProviderServiceResponseParam(
                                new ProviderServiceResponseParamDTO
                                {
                                    ParameterName = "Tax",// "Service Taxes",
                                    ServiceRequestID = providerServiceResponseId,
                                    Value = taxesAmount.ToString("0.000")
                                });
                            _inquiryBillService.AddReceiptBodyParam(
                               new ReceiptBodyParamDTO
                               {
                                   ParameterName = "Tax",// "Service Taxes",
                                   ProviderServiceRequestID = feesModel.Brn,
                                   TransactionID = null,
                                   Value = taxesAmount.ToString("0.000")
                               });
                        }
                    }

                    feeResponse.Taxes = taxesAmount;

                    _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                    {
                        Amount = feesModel.Amount,
                        ProviderServiceResponseID = providerServiceResponseId,
                        Sequence = 1
                    });
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                }

                else
                {
                    throw new TMSException("MissingData", "15");
                }
            }
            feeResponse.Brn = feesModel.Brn;
            feeResponse.Code = 200;
            feeResponse.Message = "Success";
            return feeResponse;
        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            decimal totalAmount = 0;
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
            var switchRequestDto = new InquiryWeInternetExtraDTO
            {
                TransactionId = providerServiceRequestId,
                TelephoneNumber = inquiryModel.BillingAccount
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


            var response = _switchService.Connect(switchRequestDto, switchEndPoint, "quotaInquiry", "");

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            //response = "{\"transactionId\":\"225457878465\",\"productOffer\": [{\"name\": \"Addon Offer Name\",\"chargeAmount\": 56500,\"chargeAmountWithTaxes\":59800},{\"name\": \"Addon Offer Name 2\",\"chargeAmount\":6900 ,\"chargeAmountWithTaxes\":7800}]}";

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                var count = 0;

                List<ProductOfferDTO> productOffers = JsonConvert.DeserializeObject<List<ProductOfferDTO>>(o["productOffer"]?.ToString() ?? null);
                foreach (var product in productOffers)
                {
                    product.ChargeAmount = int.Parse(product.ChargeAmount) / 100 + "";
                    product.ChargeAmountWithTaxes = int.Parse(product.ChargeAmountWithTaxes) / 100 + "";
                    inquiryResponse.Invoices.Add(new InvoiceDTO
                    {
                        Amount = decimal.Parse(product.ChargeAmountWithTaxes),
                        Sequence = count,
                        Data = new List<DataDTO>
                        {
                            new DataDTO{Key = "name", Value = product.Name}
                        }
                    });
                    totalAmount += decimal.Parse(product.ChargeAmountWithTaxes);
                    count++;
                }
                inquiryResponse.Brn = providerServiceRequestId;
                inquiryResponse.TotalAmount = totalAmount;
                //Add ProviderServiceResponse
                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });

                count = 1;
                //Add InquiryBill
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
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);
            }
            inquiryResponse.Code = 200;
            inquiryResponse.Message = "Success";

            //Logging Client Response
            await _loggingService.Log(JsonConvert.SerializeObject(inquiryResponse), providerServiceRequestId, LoggingType.CustomerResponse);
            return inquiryResponse;
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId, decimal taxes)
        {
            var paymentResponse = new PaymentResponseDTO();
            Root printedReciept;
            string message;
            int code;
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
            var brnFees = _providerService.GetMaxProviderServiceRequest(payModel.Brn, RequestType.Fees);
            foreach (var item in payModel.Data)
            {
                _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                {
                    ParameterName = item.Key,
                    ProviderServiceRequestID = providerServiceRequestId,
                    Value = item.Value
                });
            }
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

            await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
             payModel.Brn,
             LoggingType.CustomerRequest);

            var requestID = _transactionService.AddTEDataProcLog("TEDATA", payModel.BillingAccount, totalAmount, "", 0, userId, denominationServiceProviderDetails.OldServiceId, 1);
            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);
            var switchRequestDto = new PaymentQuotaDTO
            {
                TransactionId = newRequestId,
                ExtraQuota = true,
                InvoiceAmount = Convert.ToInt32(Math.Truncate(payModel.Amount * 100).ToString()).ToString(),
                TelephoneNumber = payModel.BillingAccount,
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
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            //TE_log_update
            _transactionService.TEDataLogUpdate("Payment=" + response, requestID);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);
                if (!string.IsNullOrEmpty(o["transactionId"].ToString()))
                {
                    Thread.Sleep(7000);
                    // CallBackResponse
                    //CallBackResponse = DB.ExecuteScalar(System.Configuration.ConfigurationManager.ConnectionStrings["MomknConnection"].ConnectionString, "[ReturnTEData_CallbackResponse]", DepositeId).ToString();
                    if (!string.IsNullOrEmpty(""))
                    {
                        JObject oo = JObject.Parse("CallBackResponse");
                        if (oo["ResultCode"].ToString() == "0" || oo["ResultCode"].ToString() == "170001" || (oo["ResultCode"].ToString() == "179999" && oo["ResultDesc"].ToString().ToLower() != "primary party invalid."))
                        {
                            message = "Sucess";
                            code = 1;

                            var receiptNumber = oo["Transaction"].ToString();
                            paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                            var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
                            paymentResponse.TransactionId = transactionId;

                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                                new List<int?> { transactionId });

                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                            //Add ProviderServiceResponse
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
                                RequestStatus = RequestStatusCodeType.Pending;
                                message = "PendingTrx";
                                code = 3;
                            }
                            else
                            {
                                RequestStatus = RequestStatusCodeType.Success;
                                message = "Sucess";
                                code = 1;
                            }

                            _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
                        }
                        else
                        {
                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                            _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
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

                        await _loggingService.Log(JsonConvert.SerializeObject(checkResponse), providerServiceRequestId, LoggingType.ProviderResponse);

                        _transactionService.TEDataLogUpdate("-CheckTrxn=" + checkResponse, requestID);

                        //Log Response
                        if (checkResponse.Code == 200)
                        {
                            JObject CheckTrxnO = JObject.Parse(checkResponse.Message);
                            if (CheckTrxnO["transactionStatus"].ToString().ToLower() != "declined")
                            {
                                message = "Sucess";
                                code = 1;
                                RequestStatus = RequestStatusCodeType.Success;

                                var receiptNumber = CheckTrxnO["providerPaymentId"].ToString();

                                paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
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
                        else if (checkResponse.Code == -200)
                        {
                            message = "PendingTrx";
                            code = 3;
                            RequestStatus = RequestStatusCodeType.Pending;

                            var receiptNumber = o["Transaction"].ToString();
                            paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                            var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
                            paymentResponse.TransactionId = transactionId;

                            await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                                new List<int?> { transactionId });

                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                            _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);
                            //throw new TMSException("PendingTrx", "3");
                        }
                        else if (checkResponse.StatusCode == 720)
                        {
                            message = "PendingTrx";
                            code = 3;
                            RequestStatus = RequestStatusCodeType.Pending;

                            var receiptNumber = o["Transaction"].ToString();
                            paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", receiptNumber, paymentResponse.InvoiceId, "Quota Top-up");

                            var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
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
                    var msg = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: serviceProviderId);
                    throw new TMSException(msg.Message, msg.Code);
                }

            }
            else if (response.Code == -200)
            {
                message = "PendingTrx";
                code = 3;
                RequestStatus = RequestStatusCodeType.Pending;

                paymentResponse.InvoiceId = _transactionService.AddInvoiceTedataEgyptCharge(Validates.GetCodeAndTelephone(payModel.BillingAccount)[0], Validates.GetCodeAndTelephone(payModel.BillingAccount)[1], totalAmount, fees, 1, userId, "", "", paymentResponse.InvoiceId, "Quota Top-up");

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
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
