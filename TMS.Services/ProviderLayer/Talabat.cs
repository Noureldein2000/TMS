using Newtonsoft.Json;
using TMS.Services.SOFClientAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Infrastructure.Utils;
using TMS.Services.BusinessLayer;
using TMS.Services.Models;
using TMS.Services.Services;


namespace TMS.Services.ProviderLayer
{
    public class Talabat : IBaseProvider
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
        public Talabat(
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

            var taxesList = _taxesService.GetTaxes(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal taxesAmount).ToList();

            var feesList = _feesService.GetFees(id, feesModel.Amount + taxesAmount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();

            feeResponse.Amount = Math.Round(feesModel.Amount, 3);
            feeResponse.Fees = Math.Round(feesAmount + 0, 3);
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
        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id, decimal totalAmount, decimal fees, int serviceProviderId, decimal taxes)
        {
            var paymentResponse = new PaymentResponseDTO();
            Root printedReciept;

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

            try
            {
                paymentResponse.InvoiceId = _transactionService.AddInvoiceTalabat(payModel.Brn.ToString(), payModel.Amount, userId, payModel.AccountId, id, fees, payModel.BillingAccount);

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, taxes, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;
                // confirm sof
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                //_inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Success);
                printedReciept = _transactionService.UpdateRequest(transactionId, newRequestId, "", RequestStatusCodeType.Success, userId, payModel.Brn);

                // add commission
                _transactionService.AddCommission(transactionId, payModel.AccountId, id, payModel.Amount, payModel.AccountProfileId);

            }
            catch (Exception ex)
            {
                await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                // GET MESSAGE PROVIDER ID
                var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(ex.Message), providerId: serviceProviderId);
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

            //Logging Client Request
            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                providerServiceRequestId,
                LoggingType.CustomerRequest);

            var resuturant = _denominationService.GetResturants(inquiryModel.BillingAccount, "en");

            if (resuturant != null)
            {
                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                //Logging Provider Response
                await _loggingService.Log("Success", providerServiceRequestId, LoggingType.ProviderResponse);

                inquiryResponse.Data.AddRange(new List<DataDTO>
                {
                    new DataDTO
                    {
                        Key = "RestaurantCode",
                        Value = inquiryModel.BillingAccount.ToString()
                    },
                        new DataDTO
                    {
                        Key = "RestaurantName",
                        Value = resuturant.RestaurantName.ToString()
                    }
                });

                inquiryResponse.Brn = providerServiceRequestId;
            }
            else
            {

                //Logging Provider Response
                await _loggingService.Log("NotFound", providerServiceRequestId, LoggingType.ProviderResponse);

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                var message = _dbMessageService.GetMainStatusCodeMessage(id: 404, providerId: serviceProviderId);
                throw new TMSException(message.Message, message.Code);

            }

            inquiryResponse.Code = 200;
            inquiryResponse.Message = "Success";

            //Logging Client Response
            await _loggingService.Log(JsonConvert.SerializeObject(inquiryResponse), providerServiceRequestId, LoggingType.CustomerResponse);
            return inquiryResponse;
        }
    }
}
