using Microsoft.Extensions.Localization;
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
    public class EducationService : IBaseProvider
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
        public EducationService(
                        IDenominationService denominationService,
           IProviderService providerService,
           ISwitchService switchService,
           IInquiryBillService inquiryBillService,
           ILoggingService loggingService,
           IDbMessageService dbMessageService,
           IFeesService feesService,
           ITransactionService transactionService)
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
            var denominationProviderConfiguration = _denominationService.GetDenominationServiceProvider(id);
            if (denominationProviderConfiguration.ProviderHasFees)
            {
                providerFees = decimal.Parse(_providerService.GetProviderServiceResponseParams(feesModel.Brn, language: "ar", "amountFees").FirstOrDefault().Value);
            }
            var bills = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn);
            foreach (var item in bills)
            {
                feesModel.Amount = item.Amount;
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

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            decimal totalAmount;
            string name = "", school = "", stage = "", educationYear = "";
            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Inquiry,
                BillingAccount = inquiryModel.BillingAccount,
                Brn = null,
                CreatedBy = userId,
                DenominationID = id
            });

            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                providerServiceRequestId,
                LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);
            var denominationProviderConfiguration = _denominationService.GetDenominationProviderConfigurationDetails(id);

            var switchRequestDto = new InquiryEducationServiceDTO
            {
                Ssn = inquiryModel.BillingAccount,
                TransactionId = providerServiceRequestId,
                SfId = denominationProviderConfiguration.FirstOrDefault(t => t.Name == "KhadamatyServiceFieldID").Value,
                ServiceId = denominationProviderConfiguration.FirstOrDefault(t => t.Name == "KhadamatyServiceId").Value
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
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                totalAmount = decimal.Parse(o["amount"].ToString()) + decimal.Parse(o["fees"].ToString());
                if (!string.IsNullOrEmpty(o["extraBillInfo"].ToString()))
                {
                    string[] CustomerDetail = o["extraBillInfo"].ToString().Split(';');
                    var SEQ1 = CustomerDetail[0];
                    var SEQ2 = CustomerDetail[1];
                    var SEQ3 = CustomerDetail[2];
                    var SEQ4 = CustomerDetail[3];

                    string[] StudentName = SEQ1.Split(':');
                    string[] SchoolName = SEQ2.Split(':');
                    string[] StageName = SEQ3.Split(':');
                    string[] _EducationYear = SEQ4.Split(':');

                    name = StudentName[1];
                    school = SchoolName[1];
                    stage = StageName[1];
                    educationYear = _EducationYear[1];
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
                        Value = name
                    }
                    );
                if (!string.IsNullOrEmpty(name))
                {
                    _providerService.AddProviderServiceResponseParam(
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "arabicName",
                            ServiceRequestID = providerServiceResponseId,
                            Value = name
                        });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = "arabicName",
                        Value = name
                    });
                }

                if (!string.IsNullOrEmpty(school))
                {
                    _providerService.AddProviderServiceResponseParam(
                       new ProviderServiceResponseParamDTO
                       {
                           ParameterName = "School",
                           ServiceRequestID = providerServiceResponseId,
                           Value = school
                       });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = "School",
                        Value = school
                    });
                }

                if (!string.IsNullOrEmpty(stage))
                {
                    _providerService.AddProviderServiceResponseParam(
                        new ProviderServiceResponseParamDTO
                        {
                            ParameterName = "Stage",
                            ServiceRequestID = providerServiceResponseId,
                            Value = stage
                        });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = "Stage",
                        Value = stage
                    });
                }

                if (!string.IsNullOrEmpty(educationYear))
                {
                    _providerService.AddProviderServiceResponseParam(
                      new ProviderServiceResponseParamDTO
                      {
                          ParameterName = "educationYear",
                          ServiceRequestID = providerServiceResponseId,
                          Value = educationYear
                      });
                    inquiryResponse.Data.Add(new DataDTO
                    {
                        Key = "educationYear",
                        Value = educationYear
                    });
                }

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
                       Value = o["fees"].ToString()
                   });

                if (!string.IsNullOrEmpty(name))
                {
                    _inquiryBillService.AddReceiptBodyParam(
                      new ReceiptBodyParamDTO
                      {
                          ParameterName = "arabicName",
                          ProviderServiceRequestID = providerServiceRequestId,
                          Value = name
                      });
                }
                if (!string.IsNullOrEmpty(stage))
                {
                    _inquiryBillService.AddReceiptBodyParam(
                      new ReceiptBodyParamDTO
                      {
                          ParameterName = "Stage",
                          ProviderServiceRequestID = providerServiceRequestId,
                          Value = stage
                      });
                }
                if (!string.IsNullOrEmpty(school))
                {
                    _inquiryBillService.AddReceiptBodyParam(
                      new ReceiptBodyParamDTO
                      {
                          ParameterName = "School",
                          ProviderServiceRequestID = providerServiceRequestId,
                          Value = school
                      });
                }
                if (!string.IsNullOrEmpty(educationYear))
                {
                    _inquiryBillService.AddReceiptBodyParam(
                      new ReceiptBodyParamDTO
                      {
                          ParameterName = "educationYear",
                          ProviderServiceRequestID = providerServiceRequestId,
                          Value = educationYear
                      });
                }

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

            var denominationProviderConfiguration = _denominationService.GetDenominationProviderConfigurationDetails(id);
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

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);
            var providerResponseParams = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "ar", "billReferenceNumber",
               "AsyncRqUID", "ExtraBillInfo", "amountFees", "providerPaymentId");

            var billReferenceNumber = providerResponseParams.Where(s => s.ProviderName == "billReferenceNumber").Select(s => s.Value).FirstOrDefault().ToString();
            ////var billCount = _denominationService.GetProviderServiceRequestParam(payModel.Brn, "BillCount");
            var asyncRqUID = providerResponseParams.Where(s => s.ProviderName == "AsyncRqUID").Select(s => s.Value).FirstOrDefault().ToString();
            var extraBillInfo = providerResponseParams.Where(s => s.ProviderName == "ExtraBillInfo").Select(s => s.Value).FirstOrDefault().ToString();
            var amountFees = providerResponseParams.Where(s => s.ProviderName == "amountFees").Select(s => s.Value).FirstOrDefault().ToString();
            //var providerPaymentId = providerResponseParams.Where(s => s.ProviderName == "providerPaymentId").Select(s => s.Value).FirstOrDefault();

            //var billReferenceNumber = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "billReferenceNumber");
            ////var billCount = _denominationService.GetProviderServiceRequestParam(payModel.Brn, "BillCount");
            //var asyncRqUID = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "AsyncRqUID");
            //var extraBillInfo = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "ExtraBillInfo");
            //var amountFees = _denominationService.GetProviderServiceResponseParam(payModel.Brn, "amountFees");

            var switchRequestDto = new SwitchPaymentRequestEducationBodyDTO
            {
                TransactionId = newRequestId,
                AsyncRqUID = asyncRqUID,
                Amount = payModel.Amount,
                BillRefNumber = billReferenceNumber,
                ExtraBillInfo = extraBillInfo,
                Fees = amountFees,
                SfId = denominationProviderConfiguration.FirstOrDefault(t => t.Name == "KhadamatyServiceFieldID").Value,
                ServiceId = denominationProviderConfiguration.FirstOrDefault(t => t.Name == "KhadamatyServiceId").Value,
                Ssn = payModel.BillingAccount
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
                               Value = o["providerTransactionId"].ToString()
                           });

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, null, newRequestId);
                paymentResponse.TransactionId = transactionId;
                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                // send add invoice to another data base system
                paymentResponse.InvoiceId = _transactionService.AddInvoiceEducationService(newRequestId, payModel.Amount, userId, payModel.BillingAccount,
                    fees, int.Parse(denominationProviderConfiguration.FirstOrDefault(t => t.Name == "KhadamatyServiceId").Value),
                    extraBillInfo, "");

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                paymentResponse.DataList.Add(new DataListDTO
                {
                    Key = "providerPaymentId",
                    Value = o["providerTransactionId"].ToString()
                });

                _inquiryBillService.AddReceiptBodyParam(
                           new ReceiptBodyParamDTO
                           {
                               ParameterName = "providerPaymentId",
                               ProviderServiceRequestID = payModel.Brn,
                               TransactionID = transactionId,
                               Value = o["providerTransactionId"].ToString()
                           });

                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);


                paymentResponse.InvoiceId = _transactionService.AddInvoiceEducationService(newRequestId, payModel.Amount, userId, payModel.BillingAccount,
                   fees, int.Parse(denominationProviderConfiguration.FirstOrDefault(t => t.Name == "KhadamatyServiceId").Value),
                   extraBillInfo, "");
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

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
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
