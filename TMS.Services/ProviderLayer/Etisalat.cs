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

namespace TMS.Services.ProviderLayer
{
    public class Etisalat : IBaseProvider
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
        public Etisalat(
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

            if (Ds.ProviderHasFees)
            {
                ProviderFees = 0;
            }
            var taxesList = _taxesService.GetTaxes(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal taxesAmount).ToList();
            var feesList = _feesService.GetFees(id, feesModel.Amount + taxesAmount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
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

            //Logging Client Request
            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                providerServiceRequestId,
                LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };
            //Logging Provider Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(switchEndPoint)}",
               providerServiceRequestId,
               LoggingType.ProviderRequest);


            string url = switchEndPoint.URL + "requestID=" + providerServiceRequestId + "&phone=" + inquiryModel.BillingAccount + "&userName=" + switchEndPoint.UserName + "&password=" + switchEndPoint.UserPassword;

            var response = _switchService.Connect(switchEndPoint, url);

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);
                if (o["Code"].ToString() == "200")
                {

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                    {
                        ProviderServiceRequestID = providerServiceRequestId,
                        TotalAmount = totalAmount
                    });

                    inquiryResponse.Invoices = new List<InvoiceDTO>
                {
                    new InvoiceDTO
                    {
                        Amount = decimal.Parse(o["Total_Amount"].ToString()),
                        Sequence = 1
                    }
                };

                    totalAmount += inquiryResponse.Invoices.Sum(x => x.Amount);

                    inquiryResponse.Brn = providerServiceRequestId;
                    inquiryResponse.TotalAmount = totalAmount;

                    //Add InquiryBill
                    foreach (var item in inquiryResponse.Invoices)
                    {
                        _inquiryBillService.AddInquiryBill(new InquiryBillDTO
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

            //Logging Client Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
              payModel.Brn,
              LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = serviceConfiguration.UserPassword
            };


            await _loggingService.Log($"{JsonConvert.SerializeObject(switchEndPoint)}",
              providerServiceRequestId,
              LoggingType.ProviderRequest);
            //var x = 0;

            string url = switchEndPoint.URL + "requestID=" + providerServiceRequestId + "&phone=" + payModel.BillingAccount + "&userName=" + switchEndPoint.UserName + "&password=" + switchEndPoint.UserPassword + "&amount=" + payModel.Amount;
            switchEndPoint.URL = url;
            var response = _switchService.Connect(new EmptyObject(), switchEndPoint, "", "Basic ");

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                JObject o = JObject.Parse(response.Message);
                if (o["Code"].ToString() != "-2")
                {

                    // send add invoice to another data base system
                    if (id == 63)
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceEtisalatEgypt("", payModel.BillingAccount, DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "");
                    else
                        paymentResponse.InvoiceId = _transactionService.AddInvoiceEtisalatInternet("", payModel.BillingAccount, DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "");


                    var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
                    paymentResponse.TransactionId = transactionId;

                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    // confirm sof
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                        new List<int?> { transactionId });

                    var providerServiceResponeId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                    {
                        ProviderServiceRequestID = providerServiceRequestId,
                        TotalAmount = totalAmount
                    });

                    _providerService.AddProviderServiceResponseParam(new ProviderServiceResponseParamDTO
                    {
                        ParameterName = "ProviderTransactionId",
                        ServiceRequestID = providerServiceResponeId,
                        Value = o["ProviderTransactionId"].ToString()
                    });

                    _inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
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
            }
            else if (response.Code == -200)
            {
                // send add invoice to another data base system
                if (denominationServiceProviderDetails.DenominationId == 63)
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceEtisalatEgypt("", payModel.BillingAccount, DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "");
                else
                    paymentResponse.InvoiceId = _transactionService.AddInvoiceEtisalatInternet("", payModel.BillingAccount, DateTime.Now.ToString(), payModel.Amount, fees, 1, userId, "");

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
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
