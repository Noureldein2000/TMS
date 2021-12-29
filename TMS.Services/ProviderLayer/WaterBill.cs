using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class WaterBill : IBaseProvider
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
        public WaterBill(
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
        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            var switchRequestDto = new InquiryWaterDTO();
            bool flag = true;
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
                    if (item.Key == "CurrentMeterReading")
                        switchRequestDto.MeterReading = item.Value;

                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = item.Key,
                        Value = item.Value,
                        ProviderServiceRequestID = providerServiceRequestId
                    });
                }
            }
            if (id == 450 && string.IsNullOrEmpty(switchRequestDto.MeterReading))
                flag = false;

            if (flag)
            {
                //Logging Client Request
                await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                    providerServiceRequestId,
                    LoggingType.CustomerRequest);


                var serviceConfiguration = _denominationService.GetServiceConfiguration(id);
                var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);
                switchRequestDto.BillingAcccount = inquiryModel.BillingAccount;
                switchRequestDto.BillerId = denominationServiceProviderDetails.ProviderCode;
                switchRequestDto.TransactionId = providerServiceRequestId.ToString();
                switchRequestDto.MeterReading = switchRequestDto.MeterReading ?? "";

                var switchEndPoint = new SwitchEndPointDTO
                {
                    URL = serviceConfiguration.URL,
                    TimeOut = serviceConfiguration.TimeOut,
                    UserName = serviceConfiguration.UserName,
                    UserPassword = serviceConfiguration.UserPassword
                };

                var denominationProviderConfiguration = _denominationService.GetDenominationProviderConfigurationDetails(id);
                if (denominationProviderConfiguration != null && denominationProviderConfiguration.Count() != 0)
                    switchRequestDto.BillType = denominationProviderConfiguration.FirstOrDefault(t => t.Name == "billType").Value;

                await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                     providerServiceRequestId,
                     LoggingType.ProviderRequest);


                var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.inquireBills.ToString(), "Basic ");

                //Logging Provider Response
                await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

                if (response.Code == 200)
                {
                    JObject o = JObject.Parse(response.Message);
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    CustomerInfoSubscriptionChannelsDTO CustList = JsonConvert.DeserializeObject<CustomerInfoSubscriptionChannelsDTO>(o["customerInfo"].ToString());
                    List<FeesAmountDTO> FeeList = JsonConvert.DeserializeObject<List<FeesAmountDTO>>(o["feesAmounts"].ToString());
                    List<PaymentAmountDTO> PayList = JsonConvert.DeserializeObject<List<PaymentAmountDTO>>(o["paymentAmounts"].ToString());
                    var totalAmount = decimal.Parse(PayList.Sum(item => double.Parse(item.Amount)).ToString());

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
                       });
                    var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "billNumber", "billRecId", "paymentRefInfo");
                    foreach (var item in responseParams)
                    {
                        inquiryResponse.Data.Add(new DataDTO
                        {
                            Key = item.Value,
                            Value = o[$"{item.ProviderName}"].ToString()
                        });
                    }

                    foreach (var item in CustList.GetData())
                    {
                        if (!string.IsNullOrEmpty(item.Value))
                        {
                            if (item.Key == "custmerName")
                                item.Key = "arabicName";
                            _providerService.AddProviderServiceResponseParam(
                               new ProviderServiceResponseParamDTO
                               {
                                   ParameterName = item.Key,
                                   ServiceRequestID = providerServiceResponseId,
                                   Value = item.Value
                               });
                            var responseKey = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", item.Key);
                            inquiryResponse.Data.Add(new DataDTO
                            {
                                Key = responseKey.Select(s => s.ParameterName).FirstOrDefault(),
                                Value = item.Value
                            });
                        }
                    }

                    foreach (var item in FeeList)
                    {
                        _providerService.AddProviderServiceResponseParam(
                               new ProviderServiceResponseParamDTO
                               {
                                   ParameterName = "amountFees",
                                   ServiceRequestID = providerServiceResponseId,
                                   Value = item.Amount
                               },
                               new ProviderServiceResponseParamDTO
                               {
                                   ParameterName = "currentCode",
                                   ServiceRequestID = providerServiceResponseId,
                                   Value = item.CurrentCode
                               });

                        var responseKeys = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "amountFees", "currentCode");
                        var amountFees = responseKeys.Where(s => s.ProviderName == "amountFees").Select(s => s.Value).FirstOrDefault().ToString();
                        var currentCode = responseKeys.Where(s => s.ProviderName == "currentCode").Select(s => s.Value).FirstOrDefault().ToString();

                        inquiryResponse.Data.Add(new DataDTO
                        {
                            Key = amountFees,
                            Value = item.Amount
                        });
                        inquiryResponse.Data.Add(new DataDTO
                        {
                            Key = currentCode,
                            Value = item.CurrentCode
                        });
                    }

                    inquiryResponse.Brn = providerServiceRequestId;

                    _inquiryBillService.AddReceiptBodyParam(
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = "arabicName",
                           ProviderServiceRequestID = providerServiceRequestId,
                           TransactionID = null,
                           Value = CustList.CustmerName
                       });

                    foreach (var item in PayList)
                    {
                        var inquiryId = _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                        {
                            Amount = decimal.Parse(item.Amount),
                            ProviderServiceResponseID = providerServiceResponseId,
                            Sequence = int.Parse(item.Sequence)
                        });

                        var billDetailsList = new List<InquiryBillDetailDTO>
                        {
                            new InquiryBillDetailDTO
                        {
                            Value = item.CurrentCode,
                            InquiryBillID = inquiryId,
                            ProviderName = "currentCode"
                        },
                        new InquiryBillDetailDTO
                         {
                            Value = item.MinAmount,
                            InquiryBillID = inquiryId,
                            ProviderName = "minAmount"
                        },
                        new InquiryBillDetailDTO
                        {
                            Value = item.PaymentMode,
                            InquiryBillID = inquiryId,
                            ProviderName = "paymentMode"
                        },
                        new InquiryBillDetailDTO
                        {
                            Value = item.shortDescAR,
                            InquiryBillID = inquiryId,
                            ProviderName = "shortDescAR"
                        },
                        new InquiryBillDetailDTO
                        {
                            Value = item.shortDescEN,
                            InquiryBillID = inquiryId,
                            ProviderName = "shortDescEN"
                        }
                        };

                        _inquiryBillService.AddInquiryBillDetail(billDetailsList.ToArray());

                        var invoice = new InvoiceDTO
                        {
                            Amount = decimal.Parse(item.Amount),
                            Sequence = int.Parse(item.Sequence)
                        };

                        List<InquiryBillDetailDTO> IBDList = _inquiryBillService.GetInquiryBillDetails(providerServiceRequestId, invoice.Sequence);

                        if (billDetailsList != null)
                        {
                            foreach (var IBD in IBDList)
                            {
                                invoice.Data.Add(new DataDTO
                                {
                                    Key = IBD.ParameterName,
                                    Value = IBD.Value
                                });
                            }

                        }
                        inquiryResponse.Invoices.Add(invoice);
                    }
                    inquiryResponse.TotalAmount = totalAmount;
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
                throw new TMSException("MissingData", "15");
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
            bool flag = false;
            var Sequence = "";
            decimal TotalBillAmount = 0;

            Char delimiter = ',';
            String[] SequenceBill;
            List<InvoiceDTO> invoiceList = new List<InvoiceDTO>();
            //Get BillingAccount For Inquiry Brn
            var billingAccount = _providerService.GetProviderServiceRequestBillingAccount(feesModel.Brn, userId, id);

            //Check If Contain Sequence
            if (feesModel.Data != null)
            {
                foreach (var item in feesModel.Data)
                {
                    if (item.Key == "Sequence")
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    var BrnFees = _providerService.GetMaxProviderServiceRequest(feesModel.Brn, Infrastructure.RequestType.Fees);
                    //Get Payment Bills 
                    if (BrnFees != -1)
                    {
                        var IBList = _inquiryBillService.GetInquiryBillSequence(BrnFees);
                        foreach (var item in IBList)
                        {
                            Sequence += item.Sequence + "";
                            Sequence += ",";
                        }

                        if (!string.IsNullOrEmpty(Sequence))
                        {
                            feesModel.Data.Add(new DataDTO { Key = "Sequence", Value = Sequence });
                            flag = true;
                        }
                    }
                }
            }

            if (flag)
            {
                var denomination = _denominationService.GetDenominationServiceProvider(id);
                if (denomination.ProviderHasFees)
                {
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

                    foreach (var item in feesModel.Data)
                    {
                        if (item.Key == "Sequence")
                        {
                            Sequence = (item.Value);
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        List<PaymentAmounts> _PayList = new List<PaymentAmounts>();

                        SequenceBill = Sequence.Split(delimiter);
                        if (SequenceBill.Length == SequenceBill.Distinct().Count())
                        {
                            foreach (var SequenceValue in SequenceBill)
                            {
                                if (!String.IsNullOrEmpty(SequenceValue))
                                {
                                    var IBDList = _inquiryBillService.GetInquiryBillDetails(feesModel.Brn, int.Parse(SequenceValue));
                                    if (IBDList != null)
                                    {
                                        PaymentAmounts PA = new PaymentAmounts();

                                        //PA.amount = IBDList
                                        PA.CurrentCode = IBDList.Where(t => t.ParameterId == 15).Select(x => x.Value).FirstOrDefault();
                                        PA.MinAmount = IBDList.Where(t => t.ParameterId == 16).Select(x => x.Value).FirstOrDefault();
                                        PA.PaymentMode = IBDList.Where(t => t.ParameterId == 17).Select(x => x.Value).FirstOrDefault();
                                        PA.ShortDescAR = IBDList.Where(t => t.ParameterId == 18).Select(x => x.Value).FirstOrDefault();
                                        PA.ShortDescEN = IBDList.Where(t => t.ParameterId == 19).Select(x => x.Value).FirstOrDefault();
                                        PA.Sequence = SequenceValue;

                                        PA.Amount += IBDList.Select(x => x.Amount);
                                        _PayList.Add(PA);
                                    }
                                }
                            }

                            if (_PayList.Count > 0)
                            {
                                var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

                                var switchRequestDto = new FeesWaterDTO
                                {
                                    BillingAccount = billingAccount,
                                    TransactionId = providerServiceRequestId.ToString(),
                                    BillerId = denomination.ProviderCode,
                                    PaymentAmounts = _PayList,
                                    BillRecId = _providerService.GetProviderServiceResponseParams(feesModel.Brn, "ar", "billRecId").Select(x => x.Value).FirstOrDefault()
                                };

                                var switchEndPoint = new SwitchEndPointDTO
                                {
                                    URL = serviceConfiguration.URL,
                                    TimeOut = serviceConfiguration.TimeOut,
                                    UserName = serviceConfiguration.UserName,
                                    UserPassword = serviceConfiguration.UserPassword
                                };

                                _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
                                 providerServiceRequestId,
                                 LoggingType.ProviderRequest);

                                var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.calculateFees.ToString(), "Basic ");

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

                                        List<InquiryBillDetailDTO> IBDList = _inquiryBillService.GetInquiryBillDetails(providerServiceRequestId, int.Parse(item.Sequence));
                                        if (IBDList != null)
                                        {
                                            foreach (var IBD in IBDList)
                                            {
                                                invoice.Data.Add(new DataDTO { Key = IBD.ParameterName, Value = IBD.Value });
                                            }
                                        }

                                        invoiceList.Add(invoice);
                                        TotalBillAmount += invoice.Amount;
                                    }

                                    _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal fees).ToList();

                                    feeResponse.Amount = Math.Round(TotalBillAmount, 3);
                                    feeResponse.Fees = Math.Round(fees + ProviderFees, 3);
                                    feeResponse.TotalAmount = TotalBillAmount + feeResponse.Fees;
                                    //Update ProviderServiceRequestStatus
                                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                                }
                                else
                                {
                                    //Update ProviderServiceRequestStatus
                                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                                }
                            }
                            else
                            {
                                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                                throw new TMSException("MissingData", "15");
                            }
                        }
                        else
                        {
                            _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                            throw new TMSException("InvalidData", "12");
                        }
                    }
                    else
                    {
                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                        throw new TMSException("MissingData", "15");
                    }
                }
                else
                {
                    _providerService.UpdateProviderServiceRequestStatus(0, ProviderServiceRequestStatusType.Failed, userId);
                    //var message = _dbMessageService.GetMainStatusCodeMessage(statusCode: GetData.GetCode(response), providerId: serviceProviderId);
                    //throw new TMSException(message.Message, message.Code);
                }
            }
            else
            {
                throw new TMSException("MissingData", "15");
            }

            feeResponse.Brn = feesModel.Brn;
            feeResponse.Code = 200;
            feeResponse.Message = "Success";
            return feeResponse;
        }
        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            Root printedReciept = null;
            decimal totaAmount;
            List<FeesAmounts> _FeeList = new List<FeesAmounts>();
            List<PaymentAmounts> _PayList = new List<PaymentAmounts>();

            var denomationServiceProvider = _denominationService.GetDenominationServiceProvider(id);
            var denomation = _denominationService.GetDenomination(id);

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

            if (denomation.ServiceID == 56 || denomation.ServiceID == 62)
                BrnFees = _providerService.GetMaxProviderServiceRequest(payModel.Brn, RequestType.Fees);
            else
                BrnFees = payModel.Brn;

            var bills = _inquiryBillService.GetInquiryBillSequence(payModel.Brn);
            foreach (var item in bills)
            {
                var inquiryBillDetails = _inquiryBillService.GetInquiryBillDetails(payModel.Brn, item.Sequence);
                if (inquiryBillDetails != null)
                {
                    PaymentAmounts PA = new PaymentAmounts();

                    //PA.amount = IBDList
                    PA.CurrentCode = inquiryBillDetails.Where(t => t.ParameterId == 15).Select(x => x.Value).FirstOrDefault();
                    PA.MinAmount = inquiryBillDetails.Where(t => t.ParameterId == 16).Select(x => x.Value).FirstOrDefault();
                    PA.PaymentMode = inquiryBillDetails.Where(t => t.ParameterId == 17).Select(x => x.Value).FirstOrDefault();
                    PA.ShortDescAR = inquiryBillDetails.Where(t => t.ParameterId == 18).Select(x => x.Value).FirstOrDefault();
                    PA.ShortDescEN = inquiryBillDetails.Where(t => t.ParameterId == 19).Select(x => x.Value).FirstOrDefault();
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
            FA.Amount = _providerService.GetProviderServiceResponseParams(BrnFees, "ar", "amountFees").Select(x => x.Value).FirstOrDefault();
            FA.CurrentCode = _providerService.GetProviderServiceResponseParams(BrnFees, "ar", "currentCode").Select(x => x.Value).FirstOrDefault();
            _FeeList.Add(FA);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var providerResponseParams = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "ar", "billNumber",
                "billRecId", "paymentRefInfo", "arabicName", "providerPaymentId");

            //var billReferenceNumber = providerResponseParams.Where(s => s.ProviderName == "billReferenceNumber").Select(s => s.Value).FirstOrDefault().ToString();
            ////var billCount = _denominationService.GetProviderServiceRequestParam(payModel.Brn, "BillCount");

            var billNumber = providerResponseParams.Where(s => s.ProviderName == "billNumber").Select(s => s.Value).FirstOrDefault().ToString();
            var billRecId = providerResponseParams.Where(s => s.ProviderName == "billRecId").Select(s => s.Value).FirstOrDefault().ToString();
            var paymentRefInfo = providerResponseParams.Where(s => s.ProviderName == "paymentRefInfo").Select(s => s.Value).FirstOrDefault().ToString();
            var accountName = providerResponseParams.Where(s => s.ProviderName == "arabicName").Select(s => s.Value).FirstOrDefault().ToString();


            var switchRequestDto = new PaymentWater
            {
                BillingAccount = payModel.BillingAccount,
                TransactionId = newRequestId.ToString(),
                BillerId = denomationServiceProvider.ProviderCode,
                BillNumber = billNumber,
                BillRecId = billRecId,
                PaymentRefInfo = paymentRefInfo,
                PaymentAmounts = _PayList,
                FeesAmounts = _FeeList,
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
                if (denomation.ServiceID == 56)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceWaterBill(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, response.Message, response.Message, null, response.Message, "");
                else if (denomation.ServiceID == 49 || denomation.ServiceID == 50 || denomation.ServiceID == 51)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceSubscriptionChannels(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, "200", response.Message, int.Parse(o["providerPaymentId"].ToString()), response.Message, "", newRequestId);
                else if (denomation.ServiceID == 62)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceGas(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, response.Message, response.Message, null, response.Message, "");
                else
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceEfinanceService(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, "200", response.Message, int.Parse(o["providerPaymentId"].ToString()), response.Message, "", newRequestId);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
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
                });

                var providerPaymentId = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "providerPaymentId")
                    .Where(s => s.ProviderName == "providerPaymentId").Select(s => s.ParameterName).FirstOrDefault();

                //Add DataList To Client Response   
                paymentResponse.DataList.Add(new DataListDTO
                {
                    Key = providerPaymentId,
                    Value = o["providerPaymentId"].ToString()
                });

                //Add Value To Receipt
                _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO()
                {
                    ParameterName = "providerPaymentId",
                    ProviderServiceRequestID = providerServiceRequestId,
                    TransactionID = transactionId,
                    Value = o["providerPaymentId"].ToString(),
                });

                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);

                printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                // add commission
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

            }
            else if (response.Code == -200)
            {
                // send add invoice to another data base system
                if (denomation.ServiceID == 56)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceWaterBill(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, response.Message, response.Message, null, response.Message, "");
                else if (denomation.ServiceID == 49 || denomation.ServiceID == 50 || denomation.ServiceID == 51)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceSubscriptionChannels(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, "200", response.Message, null, response.Message, "", newRequestId);
                else if (denomation.ServiceID == 62)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceGas(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, response.Message, response.Message, null, response.Message, "");
                else
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceEfinanceService(int.Parse(denomationServiceProvider.ProviderCode), payModel.BillingAccount, accountName, "", "", payModel.Amount, fees, 1, userId, "200", response.Message, null, response.Message, "", newRequestId);


                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);

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
