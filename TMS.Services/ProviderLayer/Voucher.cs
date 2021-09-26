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
using System.Globalization;

namespace TMS.Services.ProviderLayer
{
    public class Voucher : IBaseProvider
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
        public Voucher(
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
            decimal providerFees = 0;
            int Count = 1;
            bool flag = true;


            var feeResponse = new FeesResponseDTO();
            //Check Count Voucher
            if (feesModel.Data != null)
            {
                var D = _denominationService.GetDenomination(id);
                if (D.ServiceID == 3 || D.ServiceID == 5)
                {
                    foreach (var item in feesModel.Data)
                    {

                        if (item.Key == "Count")
                        {
                            if (string.IsNullOrEmpty(item.Value))
                                Count = 1;
                            else if (!int.TryParse(item.Value, out Count))
                            {
                                throw new TMSException("InvalidData", "12");
                                flag = false;
                            }
                            else
                                Count = int.Parse(item.Value);
                        }
                    }
                }
            }
            if (flag)
            {
                var denomationServiceProvider = _denominationService.GetDenominationServiceProvider(id);
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
                    ParameterName = "Count",
                    ProviderServiceRequestID = providerServiceRequestId,
                    Value = Count.ToString()
                });

                if (denomationServiceProvider.ProviderHasFees)
                    providerFees = 0;

                var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
                feeResponse.Amount = Math.Round(feesModel.Amount, 3);
                feeResponse.Fees = Math.Round(feesAmount + providerFees, 3);
                feeResponse.TotalAmount = feesModel.Amount * Count + feeResponse.Fees;

                var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                {
                    ProviderServiceRequestID = providerServiceRequestId,
                    TotalAmount = feeResponse.TotalAmount
                });

                if (feesList.Count > 0)
                {
                    foreach (var item in feesList)
                    {
                        if (item.Fees != 0)
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

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                feeResponse.Brn = providerServiceRequestId;
            }

            feeResponse.Code = 200;
            feeResponse.Message = "Success";
            return feeResponse;
        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId)
        {
            var paymentResponse = new PaymentResponseDTO();
            List<Root> RecieptList = new List<Root>();
            string printedReciept = "";
            int count;
            var denomination = _denominationService.GetDenomination(id);
            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);
            var denominationServiceConfig = _denominationService.GetDenominationProviderConfigurationDetails(id);

            var newRequestId = _transactionService.AddRequest(new RequestDTO
            {
                AccountId = payModel.AccountId,
                Amount = payModel.Amount,
                BillingAccount = payModel.BillingAccount,
                ChannelID = payModel.ChannelId,
                DenominationId = id,
                HostTransactionId = payModel.HostTransactionID
            });

            count = int.Parse(_providerService.GetProviderServiceRequestParams(payModel.Brn, "ar", "Count").Select(x => x.Value).FirstOrDefault());

            //Add New Request For Old Version
            int queryResult = _transactionService.AddElectronicChargeLogVoucher(GetProviderName(denomination.ServiceEntity), payModel.BillingAccount, payModel.Amount,
                "From Momkn 2 :" + "value is :" + payModel.Amount, 0, userId, count);

            var providerServiceRequestId = _providerService.AddProviderServiceRequest(new ProviderServiceRequestDTO
            {
                ProviderServiceRequestStatusID = ProviderServiceRequestStatusType.UnderProcess,
                RequestTypeID = Infrastructure.RequestType.Payment,
                BillingAccount = payModel.BillingAccount,
                Brn = payModel.Brn,
                CreatedBy = userId,
                DenominationID = id
            });

            _providerService.AddProviderServiceRequestParam(
                new ProviderServiceRequestParamDTO
                {
                    ParameterName = "Total Amount",
                    ProviderServiceRequestID = providerServiceRequestId,
                    Value = totalAmount.ToString()
                });
            _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
            {
                ParameterName = "Fees",
                ProviderServiceRequestID = providerServiceRequestId,
                Value = fees.ToString()
            });

            var serviceBalanceTypeId = _denominationService.GetServiceBalanceType(id);
            var balance = await _accountsApi.ApiAccountsAccountIdBalancesBalanceTypeIdGetAsync(payModel.AccountId, serviceBalanceTypeId);
            if ((decimal)balance.TotalAvailableBalance < totalAmount && (decimal)balance.TotalAvailableBalance != 0)
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

            object switchRequestDto;

            if (denomination.ServiceID == 3 || denomination.ServiceID == 5)
            {
                switchRequestDto = new BulkVoucher
                {
                    TransactionId = newRequestId.ToString(),
                    Amount = payModel.Amount.ToString(),
                    Count = count.ToString(),
                    ServiceId = denomination.OldDenominationID
                };
            }
            else
            {
                switchRequestDto = new VoucherDTO
                {
                    RequestID = newRequestId.ToString(),
                    Amount = payModel.Amount.ToString(),
                    ServiceID = denomination.OldDenominationID,
                    ProviderID = denominationServiceProviderDetails.ServiceProviderId.ToString(),
                    OperatorID = denominationServiceConfig != null ? denominationServiceConfig.Where(t => t.Name == "operatorID").Select(x => x.Value).FirstOrDefault() : denominationServiceProviderDetails.ServiceProviderId.ToString(),
                    TerminalID = denominationServiceConfig != null ? denominationServiceConfig.Where(t => t.Name == "operatorID").Select(x => x.Value).FirstOrDefault() : denominationServiceProviderDetails.ServiceProviderId.ToString()
                };
            }


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

            var response = _switchService.Connect(switchRequestDto, switchEndPoint, "", "Basic ");

            //Logging Provider Response
            await _loggingService.Log(response, providerServiceRequestId, LoggingType.ProviderResponse);

            if (Validates.CheckJSON(response))
            {
                JObject o = JObject.Parse(response);
                int ID_ = ReturnBee_Sev(denominationServiceProviderDetails.ProviderCode, payModel.Amount, GetProviderName(denomination.ServiceEntity));

                if (o["vouchers"] != null)
                {
                    var vouchers = JsonConvert.DeserializeObject<List<VoucherData>>(o["vouchers"].ToString());

                    foreach (var item in vouchers)
                    {
                        // send add invoice to another data base system
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceElectronicChargeNewVoucher(GetProviderName(denomination.ServiceEntity), "", payModel.Amount, response, 1, userId,
                            "", item.pin, item.serial, 0, ID_, queryResult);

                        var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                        paymentResponse.TransactionId = transactionId;

                        // confirm sof
                        await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                            new List<int?> { transactionId });

                        paymentResponse.DataList.AddRange(new List<DataListDTO>
                {
                    new DataListDTO
                    {
                        Key = "Pin",
                        Value = item.pin
                    },
                    new DataListDTO
                    {
                        Key = "Serial",
                        Value = item.serial
                    },
                     new DataListDTO
                    {
                        Key = "TransactionId",
                        Value = transactionId.ToString()
                    }
                });


                        //Add Value To Receipt Body
                        _inquiryBillService.AddReceiptBodyParam(
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = "Pin",
                           ProviderServiceRequestID = providerServiceRequestId,
                           TransactionID = transactionId,
                           Value = item.pin
                       },
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = "Serial",
                           ProviderServiceRequestID = providerServiceRequestId,
                           TransactionID = transactionId,
                           Value = item.serial
                       });

                        printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);
                        RecieptList.Add(JsonConvert.DeserializeObject<Root>(printedReciept));
                        // add commission
                        _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

                        try
                        {
                            _transactionService.AddInvoiceElectronicChargeIncomeNewVoucher(GetProviderName(denomination.ServiceEntity), payModel.Amount, userId, "0", denomination.ServiceProviderId, 0, paymentResponse.InvoiceId);
                        }
                        catch
                        {

                        }
                    }

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                }
                else
                {
                    if (o["code"].ToString() == "200")
                    {

                        // send add invoice to another data base system
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceElectronicChargeNewVoucher(GetProviderName(denomination.ServiceEntity), "", payModel.Amount, response, 1, userId,
                            "", o["pin"].ToString(), o["serialNumber"].ToString(), int.Parse(o["providerID"].ToString()), ID_, queryResult);

                        var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                        paymentResponse.TransactionId = transactionId;

                        // confirm sof
                        await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                            new List<int?> { transactionId });

                        paymentResponse.DataList.AddRange(new List<DataListDTO>
                {
                    new DataListDTO
                    {
                        Key = "Pin",
                        Value =  o["pin"].ToString()
                    },
                    new DataListDTO
                    {
                        Key = "Serial",
                        Value = o["serialNumber"].ToString()
                    },
                     new DataListDTO
                    {
                        Key = "TransactionId",
                        Value = transactionId.ToString()
                    }
                });


                        //Add Value To Receipt Body
                        _inquiryBillService.AddReceiptBodyParam(
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = "Pin",
                           ProviderServiceRequestID = providerServiceRequestId,
                           TransactionID = transactionId,
                           Value = o["pin"].ToString()
                       },
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = "Serial",
                           ProviderServiceRequestID = providerServiceRequestId,
                           TransactionID = transactionId,
                           Value = o["serialNumber"].ToString()
                       });

                        printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);
                        RecieptList.Add(JsonConvert.DeserializeObject<Root>(printedReciept));
                        // add commission
                        _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

                        try
                        {
                            _transactionService.AddInvoiceElectronicChargeIncomeNewVoucher(GetProviderName(denomination.ServiceEntity), payModel.Amount, userId, "0", denomination.ServiceProviderId, 0, paymentResponse.InvoiceId);
                        }
                        catch
                        {

                        }


                        _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
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
                }
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
            //paymentResponse.Receipt = new List<Root> { JsonConvert.DeserializeObject<Root>(printedReciept) };

            paymentResponse.Receipt = RecieptList;
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }

        private string GetProviderName(string serviceEntity)
        {
            switch (serviceEntity)
            {
                case "اورنج":
                    return "موبينيل";
                case "فودافون":
                    return "موبينيل";
                case "إتصالات":
                    return "إتصالات";
                default:
                    return "المصرية WE";
            }
        }

        private int ReturnBee_Sev(string Service_ID, decimal value, string network)
        {
            int serv_id = 0;
            serv_id = _transactionService.CheckVoucherValue(int.Parse(Service_ID), network, value);
            return serv_id;

        }

    }
}
