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
    public class CashU : IBaseProvider
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
        public CashU(
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

            //var providerFees = _denominationService.GetProviderServiceResponseParam(feesModel.Brn, "amountFees").FirstOrDefault();
            var providerFees = 0;


            //Note: maybe is not exist
            var bills = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn);
            foreach (var item in bills)
            {
                feesModel.Amount = item.Amount;
            }
            var currency = _denominationService.GetCurrencyValue(id);
            var taxesList = _taxesService.GetTaxes(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal taxesAmount).ToList();
            var feesList = _feesService.GetFees(id, feesModel.Amount+ taxesAmount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
            feesModel.Amount = Math.Round(feesModel.Amount * currency, 3);
            feeResponse.Fees = Math.Round(feesAmount + providerFees, 3);
            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
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
                            ParameterName = ParameterProviderNames.ServiceFees.ToString(),// "Service Fees",
                            ServiceRequestID = providerServiceResponseId,
                            Value = feesAmount.ToString("0.000")
                        });
                    _inquiryBillService.AddReceiptBodyParam(
                       new ReceiptBodyParamDTO
                       {
                           ParameterName = ParameterProviderNames.ServiceFees.ToString(),// "Service Fees",
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

            if (feesModel.Brn == 0)
                feeResponse.Brn = providerServiceRequestId;
            else
                feeResponse.Brn = feesModel.Brn;

            feeResponse.Code = 200;
            feeResponse.Message = "Success";
            return feeResponse;
        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId, decimal taxes)
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


            //foreach (var item in payModel.Data)
            //{
            //    DB_ProviderServiceRequestParam.AddProviderServiceRequestParam(ProviderServiceRequestID, item.Key, item.Value);
            //}

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

            var switchRequestDto = new PaymentCashU
            {
                TransactionId = providerServiceRequestId,
                CenterId = payModel.AccountId.ToString(),
                DenominationId = id,
                Quantity = 1
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

            var response = _switchService.Connect(switchRequestDto, switchEndPoint, SwitchEndPointAction.generateCoupon.ToString(), "Basic ");

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);

                List<CashUCoupon> coupons = JsonConvert.DeserializeObject<List<CashUCoupon>>(o["coupons"].ToString());

                //Note:Implement  stored  proc CashU_send adding invoices and other table inserted rows

                // send add invoice to another data base system
                paymentResponse.InvoiceId = _transactionService.AddInvoiceCashU(newRequestId, payModel.Amount, userId,
                    "USD",
                    coupons[0].CardNumber,
                    coupons[0].Serial,
                    Convert.ToDateTime(coupons[0].CreationDate, CultureInfo.InvariantCulture.DateTimeFormat),
                    Convert.ToDateTime(coupons[0].ExpirationDate, CultureInfo.InvariantCulture.DateTimeFormat), id);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
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
                  ParameterName = "Pin",
                  ServiceRequestID = providerServiceResponseId,
                  Value = coupons[0].CardNumber
              },
              new ProviderServiceResponseParamDTO
              {
                  ParameterName = "Serial",
                  ServiceRequestID = providerServiceResponseId,
                  Value = coupons[0].Serial
              },
               new ProviderServiceResponseParamDTO
               {
                   ParameterName = "End Date",
                   ServiceRequestID = providerServiceResponseId,
                   Value = coupons[0].ExpirationDate
               });

                var responseParams = _providerService.GetProviderServiceResponseParams(providerServiceRequestId, language: "ar", "Pin", "Serial", "End Date");

                paymentResponse.DataList.AddRange(new List<DataListDTO>
                {
                    new DataListDTO
                    {
                          Key = responseParams.Where(p => p.ProviderName == "Pin").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = coupons[0].CardNumber
                    },
                    new DataListDTO
                    {
                        Key = responseParams.Where(p => p.ProviderName == "Serial").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = coupons[0].Serial
                    },
                     new DataListDTO
                    {
                        Key = responseParams.Where(p => p.ProviderName == "End Date").Select(s => s.ParameterName).FirstOrDefault(),
                        Value = coupons[0].ExpirationDate
                    }
                });

                //Add Value To Receipt Body
                _inquiryBillService.AddReceiptBodyParam(
               new ReceiptBodyParamDTO
               {
                   ParameterName = "Pin",
                   ProviderServiceRequestID = providerServiceRequestId,
                   TransactionID = transactionId,
                   Value = coupons[0].CardNumber
               },
               new ReceiptBodyParamDTO
               {
                   ParameterName = "Serial",
                   ProviderServiceRequestID = providerServiceRequestId,
                   TransactionID = transactionId,
                   Value = coupons[0].Serial
               },
               new ReceiptBodyParamDTO
               {
                   ParameterName = "End Date",
                   ProviderServiceRequestID = providerServiceRequestId,
                   TransactionID = transactionId,
                   Value = coupons[0].ExpirationDate
               });



                _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                //_inquiryBillService.GetReceiptListByTransacationId(paymentResponse.TransactionId);
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

            paymentResponse.Code = 200;
            paymentResponse.Message = "Success";
            paymentResponse.InvoiceId = 0; //note related to CashU_send Stroed Procedure
            paymentResponse.ServerDate = DateTime.Now.ToString();
            paymentResponse.Receipt = new List<Root> { printedReciept };
            await _loggingService.Log(JsonConvert.SerializeObject(paymentResponse), providerServiceRequestId, LoggingType.CustomerResponse);

            return paymentResponse;
        }
    }
}
