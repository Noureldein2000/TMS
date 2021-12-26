﻿using Microsoft.Extensions.Localization;
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
using System.Threading;

namespace TMS.Services.ProviderLayer
{
    public class ElectricityCard : IBaseProvider
    {
        private readonly IDenominationService _denominationService;
        private readonly IProviderService _providerService;
        private readonly ISwitchService _switchService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ILoggingService _loggingService;
        private readonly IDbMessageService _dbMessageService;
        private readonly IFeesService _feesService;
        private readonly ITransactionService _transactionService;
        private readonly ICancelService _cancelService;
        private readonly IAccountsApi _accountsApi;
        public ElectricityCard(
           IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITransactionService transactionService,
           ICancelService cancelService,
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
            _cancelService = cancelService;
            _accountsApi = accountsApi;
        }

        public FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            var feeResponse = new FeesResponseDTO();
            decimal TotalBillAmount = 0;
            decimal TotalDeducts = 0;
            var providerServiceRequestId = 0;
            List<InvoiceDTO> invoiceList = new List<InvoiceDTO>();

            //Get BillingAccount For Inquiry Brn
            var billingAccount = _providerService.GetProviderServiceRequestBillingAccount(feesModel.Brn, userId, id);

            var denomination = _denominationService.GetDenomination(id);

            var DS = _denominationService.GetDenominationServiceProvider(id);
            if (DS.ProviderHasFees)
            {
                var minAmount = decimal.Parse(_providerService.GetProviderServiceResponseParams(feesModel.Brn, "ar", "minAmount").Select(x => x.Value).FirstOrDefault());

                if (feesModel.Amount > minAmount)
                {
                    providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
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

                    var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

                    var switchRequestDto = new FeesElectricityCard
                    {
                        TransactionId = providerServiceRequestId.ToString(),
                        Amount = feesModel.Amount.ToString(),
                        BillRecId = _providerService.GetProviderServiceResponseParams(feesModel.Brn, "ar", "billRecId").Select(x => x.Value).FirstOrDefault()
                    };

                    var switchEndPoint = new SwitchEndPointDTO
                    {
                        URL = serviceConfiguration.URL,
                        TimeOut = serviceConfiguration.TimeOut,
                        UserName = serviceConfiguration.UserName,
                        UserPassword = serviceConfiguration.UserPassword
                    };

                    //Logging Provider Request
                    _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                     providerServiceRequestId,
                     LoggingType.ProviderRequest);

                    //Connect to Switch
                    var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.calculateFees.ToString(), "Basic ");

                    //Logging Provider Response
                    _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

                    if (response.Code == 200)
                    {
                        JObject o = JObject.Parse(response.Message);

                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                        List<FeesAmounts> FeeList = JsonConvert.DeserializeObject<List<FeesAmounts>>(o["feesAmounts"].ToString());
                        List<PaymentAmounts> PayList = JsonConvert.DeserializeObject<List<PaymentAmounts>>(o["paymentAmounts"].ToString());


                        //DataTable FeeDt = JsonConvert.DeserializeObject<DataTable>(o["feesAmounts"].ToString());
                        var ProviderFees = decimal.Parse(FeeList.Sum(item => double.Parse(item.Amount)).ToString());

                        //Add ProviderServiceResponse
                        var providerServiceReponseID = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                        {
                            ProviderServiceRequestID = providerServiceRequestId,
                            TotalAmount = ProviderFees
                        });

                        //Add DataList To Response

                        _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                        {
                            ServiceRequestID = providerServiceReponseID,
                            ParameterName = "billRecId",
                            Value = o["billRecId"].ToString()
                        });

                        var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "billRecId", "currentCode", "amountFees");

                        //Add DataList To Response
                        feeResponse.Data.Add(new DataDTO
                        {
                            Key = responseParams.Where(p => p.ProviderName == "billRecId").Select(s => s.ParameterName).FirstOrDefault(),
                            Value = o["billRecId"].ToString()
                        });

                        feeResponse.Brn = feesModel.Brn;

                        //Add Fees
                        foreach (var item in FeeList)
                        {
                            _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                            {
                                ServiceRequestID = providerServiceReponseID,
                                ParameterName = "amountFees",
                                Value = item.Amount
                            });

                            _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                            {
                                ServiceRequestID = providerServiceReponseID,
                                ParameterName = "currentCode",
                                Value = item.CurrentCode
                            });

                            //Add DataList To Client Response
                            feeResponse.Data.Add(new DataDTO
                            {
                                Key = responseParams.Where(p => p.ProviderName == "currentCode").Select(s => s.ParameterName).FirstOrDefault(),
                                Value = item.CurrentCode
                            });

                            feeResponse.Data.Add(new DataDTO
                            {
                                Key = responseParams.Where(p => p.ProviderName == "amountFees").Select(s => s.ParameterName).FirstOrDefault(),
                                Value = item.Amount
                            });
                        }

                        //Add Value To Receipt
                        _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                        {
                            ParameterName = "Provider Service Fees",
                            ProviderServiceRequestID = providerServiceRequestId,
                            Value = ProviderFees.ToString(),
                            TransactionID = null
                        });

                        //Add InquiryBill
                        foreach (var item in PayList)
                        {
                            var inquiryId = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                            {
                                Amount = decimal.Parse(item.Amount),
                                ProviderServiceResponseID = providerServiceReponseID,
                                Sequence = int.Parse(item.Sequence)
                            });

                            _inquiryBillService.AddInquiryBillDetail(new InquiryBillDetailDTO
                            {
                                InquiryBillID = inquiryId,
                                ParameterName = "currentCode",
                                Value = item.CurrentCode
                            },
                            new InquiryBillDetailDTO
                            {
                                InquiryBillID = inquiryId,
                                ParameterName = "minAmount",
                                Value = item.MinAmount
                            },
                             new InquiryBillDetailDTO
                             {
                                 InquiryBillID = inquiryId,
                                 ParameterName = "paymentMode",
                                 Value = item.PaymentMode
                             },
                              new InquiryBillDetailDTO
                              {
                                  InquiryBillID = inquiryId,
                                  ParameterName = "shortDescAR",
                                  Value = item.ShortDescAR
                              },
                               new InquiryBillDetailDTO
                               {
                                   InquiryBillID = inquiryId,
                                   ParameterName = "shortDescEN",
                                   Value = item.ShortDescEN
                               }
                            );

                            InvoiceDTO invoice = new InvoiceDTO();
                            invoice.Amount = decimal.Parse(item.Amount);
                            invoice.Sequence = int.Parse(item.Sequence);

                            invoice.Data.Add(new DataDTO { Key = "currentCode", Value = item.CurrentCode });
                            invoice.Data.Add(new DataDTO { Key = "minAmount", Value = item.MinAmount });
                            invoice.Data.Add(new DataDTO { Key = "paymentMode", Value = item.PaymentMode });
                            invoice.Data.Add(new DataDTO { Key = "shortDescAR", Value = item.ShortDescAR });
                            invoice.Data.Add(new DataDTO { Key = "shortDescEN", Value = item.ShortDescEN });

                            invoiceList.Add(invoice);
                            TotalBillAmount += invoice.Amount;
                        }

                        _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal fees).ToList();

                        feeResponse.Amount = Math.Round(TotalBillAmount, 3);
                        feeResponse.Fees = Math.Round(fees + ProviderFees, 3);
                        feeResponse.TotalAmount = TotalBillAmount + feeResponse.Fees;
                        TotalDeducts = decimal.Parse(_providerService.GetProviderServiceResponseParams(feesModel.Brn, "ar", "totalDeducts").Select(x => x.Value).FirstOrDefault());

                        _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "cardAmount",
                            ServiceRequestID = providerServiceReponseID,
                            Value = (feeResponse.TotalAmount - TotalDeducts).ToString()
                        });

                        feeResponse.Data.Add(new DataDTO
                        {
                            Key = "cardAmount",
                            Value = (feeResponse.TotalAmount - TotalDeducts).ToString()
                        });

                        //Update ProviderServiceRequestStatus
                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                    }
                    else
                    {
                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                        var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response.Message), providerId: 0);
                        throw new TMSException(message.Message, message.Code);
                    }
                }
                else
                {
                    throw new TMSException("InvalidAmount", "11");
                }

            }

            feeResponse.Brn = feesModel.Brn;
            feeResponse.Code = 200;
            feeResponse.Message = "Success";

            //Logging Client Response
            _loggingService.Log(JsonConvert.SerializeObject(feeResponse), providerServiceRequestId, LoggingType.CustomerResponse);
            return feeResponse;

        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            decimal totalAmount;
            List<InvoiceDTO> invoiceList = new List<InvoiceDTO>();
            InquiryElectricityCard IE = new InquiryElectricityCard();

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
                    if (item.Key == "CardData")
                        IE.CardData = item.Value;
                    else if (item.Key == "CardType")
                        IE.CardType = item.Value;

                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = item.Key,
                        Value = item.Value,
                        ProviderServiceRequestID = providerServiceRequestId
                    });
                }
            }


            //Logging Client Request
            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                providerServiceRequestId,
                LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);


            IE.BillingAcccount = inquiryModel.BillingAccount;
            IE.BillerId = denomation.ProviderCode;
            IE.TransactionId = providerServiceRequestId.ToString();


            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };
            //Logging Provider Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(IE)} : {JsonConvert.SerializeObject(switchEndPoint)}",
               providerServiceRequestId,
               LoggingType.ProviderRequest);


            var response = _switchService.Connect(IE, switchEndPoint, SwitchEndPointAction.inquireBills.ToString(), "Basic ");

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                var CustList = JsonConvert.DeserializeObject<CustomerInfoElectricityCard>(o["customerInfo"].ToString());

                totalAmount = decimal.Parse(CustList.TotalDeducts);

                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });

                _providerService.AddProviderServiceResponseParam(
                   new ProviderServiceResponseParamDTO
                   {
                       ParameterName = "billNumber",
                       ServiceRequestID = providerServiceResponseId,
                       Value = o["billNumber"].ToString()
                   },
                   new ProviderServiceResponseParamDTO
                   {
                       ParameterName = "billRecId",
                       ServiceRequestID = providerServiceResponseId,
                       Value = o["billRecId"].ToString()
                   },
                   new ProviderServiceResponseParamDTO
                   {
                       ParameterName = "paymentRefInfo",
                       ServiceRequestID = providerServiceResponseId,
                       Value = o["paymentRefInfo"].ToString()
                   },
                    new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "arabicName",
                        ServiceRequestID = providerServiceResponseId,
                        Value = CustList.CustomerName
                    },
                     new ProviderServiceResponseParamDTO
                     {
                         ParameterName = "minAmount",
                         ServiceRequestID = providerServiceResponseId,
                         Value = CustList.MinAmount
                     },
                      new ProviderServiceResponseParamDTO
                      {
                          ParameterName = "prepaidAmount",
                          ServiceRequestID = providerServiceResponseId,
                          Value = CustList.PrepaidAmount
                      },
                      new ProviderServiceResponseParamDTO
                      {
                          ParameterName = "totalDeducts",
                          ServiceRequestID = providerServiceResponseId,
                          Value = CustList.TotalDeducts
                      });

                var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "billNumber", "arabicName", "billRecId", "paymentRefInfo", "minAmount", "prepaidAmount", "totalDeducts");

                inquiryModel.Data.AddRange(new List<DataDTO>
                {
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "billNumber").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = o["billNumber"].ToString()
                    },
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "billRecId").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = o["billRecId"].ToString()
                    },
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "paymentRefInfo").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = o["paymentRefInfo"].ToString()
                    }
                    ,
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "arabicName").Select(s => s.ParameterName).FirstOrDefault(),
                        Value =CustList.CustomerName
                    }
                    ,
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "minAmount").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = CustList.MinAmount
                    }
                    ,
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "prepaidAmount").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = CustList.PrepaidAmount
                    },
                    new DataDTO
                    {
                         Key = responseParams.Where(p => p.ProviderName == "totalDeducts").Select(s => s.ParameterName).FirstOrDefault(),
                        Value =CustList.TotalDeducts
                    }
                });

                _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                {
                    ParameterName = "arabicName",
                    ProviderServiceRequestID = providerServiceRequestId,
                    TransactionID = null,
                    Value = CustList.CustomerName
                });

                //Add InquiryBill

                var inquiryId = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                {
                    Amount = totalAmount,
                    ProviderServiceResponseID = providerServiceResponseId,
                    Sequence = 1
                });

                InvoiceDTO invoice = new InvoiceDTO();
                invoice.Amount = totalAmount;
                invoice.Sequence = 1;

                invoiceList.Add(invoice);

                inquiryResponse.Brn = providerServiceRequestId;
                inquiryResponse.TotalAmount = totalAmount;
                inquiryResponse.Invoices = invoiceList;
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

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            Root printedReciept = null;
            decimal totaAmount;
            bool isCancelled;
            var _FeeList = new List<FeesAmounts>();
            var _PayList = new List<PaymentCardAmounts>();

            var denomation = _denominationService.GetDenomination(id);
            var DS = _denominationService.GetDenominationServiceProvider(id);

            //Get BillingAccount For Inquiry Brn
            payModel.BillingAccount = _providerService.GetProviderServiceRequestBillingAccount(payModel.Brn, userId, id);

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            //Get Max ProviderServiceRequestFees
            var BrnFees = _providerService.GetMaxProviderServiceRequest(payModel.Brn, Infrastructure.RequestType.Fees);

            var bills = _inquiryBillService.GetInquiryBillSequence(payModel.Brn);
            foreach (var item in bills)
            {
                var inquiryBillDetails = _inquiryBillService.GetInquiryBillDetails(payModel.Brn, item.Sequence);
                if (inquiryBillDetails != null)
                {
                    var PA = new PaymentCardAmounts();

                    //PA.amount = IBDList
                    PA.CurrentCode = inquiryBillDetails.Where(t => t.ParameterId == 15).Select(x => x.Value).FirstOrDefault();
                    PA.Sequence = item.Sequence.ToString();
                    PA.Amount = item.Amount.ToString();

                    payModel.Amount += item.Amount;

                    _PayList.Add(PA);
                }
            }

            totaAmount = payModel.Amount + fees;

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

            //Get Fees Amount
            FeesAmounts FA = new FeesAmounts();
            FA.Amount = _providerService.GetProviderServiceResponseParams(BrnFees, "amountFees", "ar").Select(x => x.Value).FirstOrDefault();
            FA.CurrentCode = _providerService.GetProviderServiceResponseParams(BrnFees, "currentCode", "ar").Select(x => x.Value).FirstOrDefault();
            _FeeList.Add(FA);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var providerResponseParams = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "ar", "billNumber",
                "billRecId", "paymentRefInfo", "arabicName");

            var billNumber = providerResponseParams.Where(s => s.ProviderName == "billNumber").Select(s => s.Value).FirstOrDefault().ToString();
            var billRecId = providerResponseParams.Where(s => s.ProviderName == "billRecId").Select(s => s.Value).FirstOrDefault().ToString();
            var paymentRefInfo = providerResponseParams.Where(s => s.ProviderName == "paymentRefInfo").Select(s => s.Value).FirstOrDefault().ToString();
            var accountName = providerResponseParams.Where(s => s.ProviderName == "arabicName").Select(s => s.Value).FirstOrDefault().ToString();
            var cardType = _providerService.GetProviderServiceRequestParams(payModel.Brn, "ar", "CardType").Select(x => x.Value).FirstOrDefault();

            var switchRequestDto = new PaymentElectricity
            {
                BillingAcccount = payModel.BillingAccount,
                TransactionId = newRequestId.ToString(),
                BillerId = DS.ProviderCode,
                BillNumber = billNumber,
                BillRecId = billRecId,
                CardType = cardType,
                PaymentRefInfo = paymentRefInfo,
                PaymentAmounts = _PayList,
                FeesAmounts = _FeeList
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

            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.paymentBills.ToString(), "Basic ");
            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);

                // send add invoice to another data base system
                if (denomation.ServiceID == 20)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceElectricityCard(int.Parse(DS.ProviderCode), payModel.BillingAccount, accountName, "", "", totaAmount, fees, 1, userId,
                    response.Message, response.Message, null, response.Message, "", _providerService.GetProviderServiceRequestParams(payModel.Brn, "ar", "CardType").Select(x => x.Value).FirstOrDefault(), o["cardData"].ToString(), newRequestId, o["momknPaymentId"].ToString());
                else
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceWaterCard(int.Parse(DS.ProviderCode), payModel.BillingAccount, accountName, "", "", totaAmount, fees, 1, userId,
                    response.Message, response.Message, null, response.Message, "", _providerService.GetProviderServiceRequestParams(payModel.Brn, "ar", "CardType").Select(x => x.Value).FirstOrDefault(), o["cardData"].ToString(), newRequestId, o["momknPaymentId"].ToString());

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", 0, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;
                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                //Add ProviderServiceResponse
                var providerServiceReponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = totalAmount
                });
                _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                {
                    ParameterName = "providerPaymentId",
                    ServiceRequestID = providerServiceReponseId,
                    Value = o["providerPaymentId"].ToString()
                },
                new ProviderServiceResponseParamDTO
                {
                    ParameterName = "CardData",
                    ServiceRequestID = providerServiceReponseId,
                    Value = o["cardData"].ToString()
                },
                new ProviderServiceResponseParamDTO
                {
                    ParameterName = "momknPaymentId",
                    ServiceRequestID = providerServiceReponseId,
                    Value = o["momknPaymentId"].ToString()
                });

                var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "providerPaymentId", "CardData", "momknPaymentId");

                //Add DataList To Client Response
                paymentResponse.DataList.Add(new DataListDTO
                {
                    Key = responseParams.Where(p => p.ProviderName == "providerPaymentId").Select(s => s.ParameterName).FirstOrDefault(),
                    Value = o["providerPaymentId"].ToString()
                });
                paymentResponse.DataList.Add(new DataListDTO
                {
                    Key = providerResponseParams.Where(p => p.ProviderName == "CardData").Select(s => s.ParameterName).FirstOrDefault(),
                    Value = o["cardData"].ToString()
                });
                paymentResponse.DataList.Add(new DataListDTO
                {
                    Key = providerResponseParams.Where(p => p.ProviderName == "momknPaymentId").Select(s => s.ParameterName).FirstOrDefault(),
                    Value = o["momknPaymentId"].ToString()
                });

                //Add Value To Receipt
                _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO()
                {
                    ParameterName = "providerPaymentId",
                    ProviderServiceRequestID = providerServiceRequestId,
                    TransactionID = transactionId,
                    Value = o["providerPaymentId"].ToString(),
                },
                new ReceiptBodyParamDTO()
                {
                    ParameterName = "momknPaymentId",
                    ProviderServiceRequestID = providerServiceRequestId,
                    TransactionID = transactionId,
                    Value = o["momknPaymentId"].ToString(),
                });

                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);

                printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                //Add Pending Payment Card  
                _transactionService.AddPendingPaymentCard(paymentRefInfo, transactionId, cardType, payModel.HostTransactionID, payModel.Brn);

                // add commission
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

            }
            else if (response.Code == -200)
            {
                //**Note Noureldein: There are another implment that use Cancel provider

                _cancelService.CallCancellProvider(new CancellProviderDTO
                {
                    ServiceID = denomation.ServiceID,
                    FeesAmounts = _FeeList,
                    PayCardAmounts = _PayList,
                    BillingAccount = payModel.BillingAccount,
                    Brn = payModel.Brn,
                    DenomationId = id,
                    MomknPaymentId = providerServiceRequestId.ToString(),
                    OldDenominationID = denomation.OldDenominationID,
                    ProviderCode = DS.ProviderCode,
                    ProviderServiceRequestId = providerServiceRequestId
                }, out isCancelled);

                if (isCancelled)
                    // Delete sof
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
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

    }
}



