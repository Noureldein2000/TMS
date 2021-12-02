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
                switchRequestDto.TransactionId = providerServiceRequestId;
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


                var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.inquireBills.ToString(), "Basic ", UrlType.Custom);

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                if (Validates.CheckJSON(response))
                {
                    JObject o = JObject.Parse(response);
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
                           TransactionID = 0,
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
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(response), providerId: serviceProviderId);
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
            throw new NotImplementedException();
        }

        public Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            throw new NotImplementedException();
        }
    }
}
