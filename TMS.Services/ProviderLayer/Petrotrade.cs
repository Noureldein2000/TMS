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
    public class Petrotrade : IBaseProvider
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
        public Petrotrade(
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
                ProviderFees = decimal.Parse(_providerService.GetProviderServiceResponseParams(feesModel.Brn, "ar", "amountFees").Select(x => x.Value).FirstOrDefault());
            }
            var IBList = _inquiryBillService.GetInquiryBillSequence(providerServiceRequestId);
            foreach (var item in IBList)
            {
                feesModel.Amount = item.Amount;
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
            string MobileNo = "";
            bool flag = false;

            var denomation = _denominationService.GetDenominationServiceProvider(id);
            var denomationProvicerConfigList = _denominationService.GetDenominationProviderConfigurationDetails(id);

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
                    if (item.Key == "Mobile")
                    {
                        if (Validates.CheckMobileNumber(item.Value))
                        {
                            flag = true;
                            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                            {
                                ParameterName = "MobileNumber",
                                ProviderServiceRequestID = providerServiceRequestId,
                                Value = item.Value
                            });
                            MobileNo = item.Value;
                        }
                        else
                            throw new TMSException("InvalidMobileNumber", "34");
                        break;
                    }
                }
            }

            if (flag)
            {
                //Logging Client Request
                await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                    providerServiceRequestId,
                    LoggingType.CustomerRequest);

                var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

                var switchBodyRequest = new PetrotradeDTO
                {
                    MobileNo = MobileNo,
                    RegisterNo = inquiryModel.BillingAccount,
                    TransactionId = providerServiceRequestId.ToString()
                };

                var switchEndPoint = new SwitchEndPointDTO
                {
                    URL = serviceConfiguration.URL,
                    TimeOut = serviceConfiguration.TimeOut,
                    UserName = serviceConfiguration.UserName,
                    UserPassword = serviceConfiguration.UserPassword
                };
                //Logging Provider Request
                await _loggingService.Log($"{JsonConvert.SerializeObject(switchBodyRequest)}:{JsonConvert.SerializeObject(switchEndPoint)}",
                   providerServiceRequestId,
                   LoggingType.ProviderRequest);

                var response = _switchService.Connect(switchBodyRequest, switchEndPoint, SwitchEndPointAction.inquiry.ToString(), "Basic ", UrlType.Custom);

                //Logging Provider Response
                await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

                if (Validates.CheckJSON(response))
                {
                    JObject o = JObject.Parse(response);

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    totalAmount = decimal.Parse(o["amount"].ToString()) + decimal.Parse(o["fees"].ToString());

                    var BI = JsonConvert.DeserializeObject<List<BillInfo>>(o["billInfo"].ToString());

                    var Name = BI.FirstOrDefault(t => t.item == "إسم العميل").value;
                    var StartDate = BI.FirstOrDefault(t => t.item == "تاريخ البدايه").value;
                    var EndDate = BI.FirstOrDefault(t => t.item == "تاريخ النهايه").value;
                    var DueBills = BI.FirstOrDefault(t => t.item == "الفواتير المستحقة").value;

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
               }
               ,
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "AsyncRqUID",
                   ServiceRequestID = providerServiceResponseId,
                   Value = o["asyncRqUID"].ToString()
               }
               ,
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
                   Value = Name
               },
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "Start Date",
                   ServiceRequestID = providerServiceResponseId,
                   Value = StartDate
               },
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "End Date",
                   ServiceRequestID = providerServiceResponseId,
                   Value = EndDate
               },
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "Due Bills",
                   ServiceRequestID = providerServiceResponseId,
                   Value = DueBills
               });

                    var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "Start Date", "arabicName", "End Date", "Due Bills");

                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = responseParams.Where(p => p.ProviderName == "arabicName").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = Name
                    });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = responseParams.Where(p => p.ProviderName == "Start Date").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = StartDate
                    });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = responseParams.Where(p => p.ProviderName == "End Date").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = EndDate
                    });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = responseParams.Where(p => p.ProviderName == "Due Bills").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = DueBills
                    });

                    //Add InquiryBill
                    _inquiryBillService.AddInquiryBill(new InquiryBillDTO
                    {
                        Amount = decimal.Parse(o["amount"].ToString()),
                        ProviderServiceResponseID = providerServiceResponseId,
                        Sequence = count
                    });

                    _inquiryBillService.AddReceiptBodyParam(new ReceiptBodyParamDTO
                    {
                        ParameterName = "Provider Service Fees",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = null,
                        Value = o["fees"].ToString()
                    }, new ReceiptBodyParamDTO
                    {
                        ParameterName = "arabicName",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = null,
                        Value = Name
                    }, new ReceiptBodyParamDTO
                    {
                        ParameterName = "Start Date",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = null,
                        Value = StartDate
                    }, new ReceiptBodyParamDTO
                    {
                        ParameterName = "End Date",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = null,
                        Value = EndDate
                    }, new ReceiptBodyParamDTO
                    {
                        ParameterName = "Due Bills",
                        ProviderServiceRequestID = providerServiceRequestId,
                        TransactionID = null,
                        Value = DueBills
                    });

                    inquiryResponse.Invoices = new List<InvoiceDTO>
                {
                    new InvoiceDTO
                    {
                        Amount = decimal.Parse(o["amount"].ToString()),
                        Sequence = 1
                    }
                };

                    totalAmount += inquiryResponse.Invoices.Sum(x => x.Amount);

                    inquiryResponse.Brn = providerServiceRequestId;
                    inquiryResponse.TotalAmount = decimal.Parse(o["amount"].ToString());

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
                throw new TMSException("MissingData", "15");
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

            var D = _denominationService.GetDenomination(id);
            var DPCList = _denominationService.GetDenominationProviderConfigurationDetails(id);
            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);

            var IBList = _inquiryBillService.GetInquiryBillSequence(payModel.Brn);
            foreach (var item in IBList)
            {
                payModel.Amount = item.Amount;
            }

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
                Value = "",
                ProviderServiceRequestID = providerServiceRequestId
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

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var paramters = _providerService.GetProviderServiceResponseParams(payModel.Brn, "en", "billReferenceNumber", "AsyncRqUID", "ExtraBillInfo", "MobileNumber", "amountFees");

            var bilRefNum = paramters.Where(x => x.ProviderName == "billReferenceNumber").Select(x => x.Value).FirstOrDefault();
            var extraBillInfo = paramters.Where(x => x.ProviderName == "ExtraBillInfo").Select(x => x.Value).FirstOrDefault();
            var mobileNo = _providerService.GetProviderServiceRequestParams(payModel.Brn, "en", "MobileNumber")
                .Where(x => x.Key == "Mobile Number").Select(x => x.Value).FirstOrDefault();

            var switchBodyRequest = new PaymentPetrotrade
            {
                RegisterNo = payModel.BillingAccount,
                TransactionId = newRequestId.ToString(),
                Amount = payModel.Amount,
                BillRefNumber = bilRefNum == "" ? "null" : bilRefNum,
                AsyncRqUID = paramters.Where(x => x.ProviderName == "AsyncRqUID").Select(x => x.Value).FirstOrDefault(),
                ExtraBillInfo = extraBillInfo == "" ? "null" : extraBillInfo,
                MobileNo = mobileNo,
                Fees = paramters.Where(x => x.ProviderName == "amountFees").Select(x => x.Value).FirstOrDefault(),
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

                // send add invoice to another data base system
                paymentResponse.InvoiceId = _transactionService.AddInvoicePetrotrade(newRequestId.ToString(), payModel.Amount, userId,
                    mobileNo, payModel.BillingAccount, extraBillInfo, "", fees);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                // add commission
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

            }
            else if (response.Contains("timed out"))
            {
                // send add invoice to another data base system
                paymentResponse.InvoiceId = _transactionService.AddInvoicePetrotrade(newRequestId.ToString(), payModel.Amount, userId,
                  paramters.Where(x => x.ProviderName == "MobileNumber").Select(x => x.Value).FirstOrDefault(), payModel.BillingAccount, paramters.Where(x => x.ProviderName == "ExtraBillInfo").Select(x => x.Value).FirstOrDefault(), "", fees);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Pending, userId, payModel.Brn);

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
