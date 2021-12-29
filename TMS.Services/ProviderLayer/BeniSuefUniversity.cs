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
    public class BeniSuefUniversity : IBaseProvider
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
        public BeniSuefUniversity(
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

            //var providerResponseParams = _providerService.GetProviderServiceResponseParams(feesModel.Brn, language: "ar", "amountFees");
            //var amountFees = providerResponseParams.Where(s => s.ProviderName == "amountFees").Select(s => s.Value).FirstOrDefault().ToString();

            //var bills = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn);
            //foreach (var item in bills)
            //{
            //    feesModel.Amount = item.Amount;
            //}


            var feesList = _feesService.GetFees(id, feesModel.Amount, feesModel.AccountId, feesModel.AccountProfileId, out decimal feesAmount).ToList();
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

            var denominationServiceProviderDetails = _denominationService.GetDenominationServiceProvider(id);
            var denomination = _denominationService.GetDenomination(id);

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

            // i have conflict understand in this point... 
            string providerResponse = _loggingService.GetLog(payModel.Brn, LoggingType.ProviderResponse);
            SwitchInquiryResponseDTO SIRS = JsonConvert.DeserializeObject<SwitchInquiryResponseDTO>(providerResponse);
            payModel.Amount = SIRS.TotalAmount;

            //Logging Client Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(payModel)}- DenominationID:{id} -TotalAmount:{totalAmount} -Fees:{fees}",
              payModel.Brn,
              LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            //var providerResponseParams = _providerService.GetProviderServiceResponseParams(payModel.Brn, language: "ar", "BranchNumber",
            //    "DueDate", "AccountNumber", "Balance");

            //var branchNumber = providerResponseParams.Where(s => s.ProviderName == "BranchNumber").Select(s => s.Value).FirstOrDefault().ToString();
            //var dueDate = providerResponseParams.Where(s => s.ProviderName == "DueDate").Select(s => s.Value).FirstOrDefault().ToString();
            //var accountNumber = providerResponseParams.Where(s => s.ProviderName == "AccountNumber").Select(s => s.Value).FirstOrDefault().ToString();
            //var balanceParam = providerResponseParams.Where(s => s.ProviderName == "Balance").Select(s => s.Value).FirstOrDefault().ToString();

            var switchRequestDto = new SwitchPaymentRequest
            {
                Amount = payModel.Amount,
                Uuid = newRequestId.ToString(),
                Data = SIRS.Invoices.Select(x => new SwitchData { Key = "sequence", Value = x.Sequence.ToString() }).ToList()
            };

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = _switchService.GetToken(serviceConfiguration)
            };


            await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
              providerServiceRequestId,
              LoggingType.ProviderRequest);

            //Connect to switch..with new implement and deserilize object
            var response = _switchService.Connect(switchRequestDto, switchEndPoint, "services/" + 2 + "/payment", "Bearer ");

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            string EducationYear = SIRS.Invoices[0].Data.Where(f => f.Key == "year_descr_ar").Select(x => x.Value).FirstOrDefault();
            string FacultyName = SIRS.Invoices[0].Data.Where(f => f.Key == "fac_name").Select(x => x.Value).FirstOrDefault();
            string StudentName = SIRS.Invoices[0].Data.Where(f => f.Key == "stud_name").Select(x => x.Value).FirstOrDefault();
            string SSN = SIRS.Invoices[0].Data.Where(f => f.Key == "NATIONAL_NUMBER").Select(x => x.Value).FirstOrDefault();
            string StudentCode = SIRS.Invoices[0].Data.Where(f => f.Key == "STUD_CODE").Select(x => x.Value).FirstOrDefault();
            string Section = SIRS.Invoices[0].Data.Where(f => f.Key == "as_node_des").Select(x => x.Value).FirstOrDefault();
            string StudyNature = SIRS.Invoices[0].Data.Where(f => f.Key == "ED_CODE_STUDY_NATURE_DESC").Select(x => x.Value).FirstOrDefault();

            if (response.Code == 200)
            {
                SwitchPaymentResponse SPRS = JsonConvert.DeserializeObject<SwitchPaymentResponse>(response.Message);

                if (SPRS.ResponseCode == "200")
                {

                    paymentResponse.ProviderTransactionId = newRequestId;

                    paymentResponse.InvoiceId = _transactionService.AddInvoiceUniversityBeniSuef(payModel.AccountId, payModel.Amount, userId, payModel.BillingAccount, denominationServiceProviderDetails.OldServiceId,
                     totalAmount, fees, EducationYear, FacultyName, StudentName, SSN, StudentCode, Section, StudyNature, newRequestId.ToString());

                    var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
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
                else
                {
                    await _accountsApi.ApiAccountsAccountIdRequestsRequestIdDeleteAsync(payModel.AccountId, newRequestId);
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                    _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Fail);
                    // GET MESSAGE PROVIDER ID
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(SPRS.ResponseCode), providerId: serviceProviderId);
                    throw new TMSException(message.Message, message.Code);
                }
            }
            else if (response.Code == -200)
            {
                paymentResponse.ProviderTransactionId = newRequestId;

                paymentResponse.InvoiceId = _transactionService.AddInvoiceUniversityBeniSuef(payModel.AccountId, payModel.Amount, userId, payModel.BillingAccount, denominationServiceProviderDetails.OldServiceId,
                 totalAmount, fees, EducationYear, FacultyName, StudentName, SSN, StudentCode, Section, StudyNature, newRequestId.ToString());

                var transactionId = _transactionService.AddTransaction(payModel.AccountId, totalAmount, id, payModel.Amount, fees, "", null, paymentResponse.InvoiceId, newRequestId);
                paymentResponse.TransactionId = transactionId;

                // confirm sof
                //await _accountsApi.ApiAccountsAccountIdRequestsRequestIdPutAsync(payModel.AccountId, newRequestId,
                //    new List<int?> { transactionId });

                _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);
                //_inquiryBillService.UpdateReceiptBodyParam(payModel.Brn, transactionId);
                _transactionService.UpdateRequestStatus(newRequestId, RequestStatusCodeType.Pending);
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
        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id, int serviceProviderId)
        {
            var inquiryResponse = new InquiryResponseDTO();
            decimal totalAmount;
            string BillingAccountType = "";

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
                var billCount = inquiryModel.Data.Where(d => d.Key == "BillingAccountType").FirstOrDefault();
                if (billCount != null)
                {
                    _providerService.AddProviderServiceRequestParam(new ProviderServiceRequestParamDTO
                    {
                        ParameterName = "BillingAccountType",
                        Value = billCount.Value,
                        ProviderServiceRequestID = providerServiceRequestId
                    });

                    BillingAccountType = billCount.Value;
                }
            }

            //Logging Client Request
            await _loggingService.Log($"-DenominationID:{id}-BillingAccount:{inquiryModel.BillingAccount}-{JsonConvert.SerializeObject(inquiryModel.Data)}",
                providerServiceRequestId,
                LoggingType.CustomerRequest);

            var serviceConfiguration = _denominationService.GetServiceConfiguration(id);

            var switchRequestDto = new SwitchInquiryRequest
            {
                BillingAcctNumber = inquiryModel.BillingAccount,
                Data = new List<SwitchData> { new SwitchData { Key = "billingAcctTypeId", Value = BillingAccountType } }
            };

            var userPassword = _switchService.GetToken(serviceConfiguration);

            var switchEndPoint = new SwitchEndPointDTO
            {
                URL = serviceConfiguration.URL,
                TimeOut = serviceConfiguration.TimeOut,
                UserName = serviceConfiguration.UserName,
                UserPassword = userPassword
            };

            //Logging Provider Request
            await _loggingService.Log($"{JsonConvert.SerializeObject(switchRequestDto)} : {JsonConvert.SerializeObject(switchEndPoint)}",
               providerServiceRequestId,
               LoggingType.ProviderRequest);

            //PSC[0] = "http://164.160.104.136:8087/momknswitch/api/v1.0/";
            var response = _switchService.Connect(switchRequestDto, switchEndPoint, "services/" + 2 + "/inquiry", "Bearer ");
            //Response = "{ \"responseCode\": 200, \"responseMessage\": \"SUCCESS\", \"responseDate\": \"07/10/2021 21:12:05\", \"totalAmount\": 300.0, \"invoices\": [ {\"sequence\": 16, \"amount\": 15.0, \"data\": [  {  \"key\": \"sequence\",  \"value\": \"1\" },{  \"key\": \"year_descr_ar\",  \"value\": \"2009\" },{  \"key\": \"fac_name\",  \"value\": \"Computer\" },{  \"key\": \"stud_name\",  \"value\": \"Tony\" },{  \"key\": \"NATIONAL_NUMBER\",  \"value\": \"2551030214456\" },{  \"key\": \"STUD_CODE\",  \"value\": \"123\" },{  \"key\": \"phase_node\",  \"value\": \"1\" },{  \"key\": \"sem_desc\",  \"value\": \"2\" },{  \"key\": \"ED_CODE_STUDY_NATURE_DESC\",  \"value\": \"3\" }  ] } ]}";

            //Logging Provider Response
            await _loggingService.Log(JsonConvert.SerializeObject(response), providerServiceRequestId, LoggingType.ProviderResponse);

            if (response.Code == 200)
            {
                //JObject o = JObject.Parse(response);
                SwitchInquiryResponseDTO SIRS = JsonConvert.DeserializeObject<SwitchInquiryResponseDTO>(response.Message);

                if (SIRS.ResponseCode == "200")
                {
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Success, userId);

                    totalAmount = SIRS.TotalAmount;

                    var providerServiceResponseId = _providerService.AddProviderServiceResponse(new ProviderServiceResponseDTO
                    {
                        ProviderServiceRequestID = providerServiceRequestId,
                        TotalAmount = totalAmount
                    });

                    inquiryResponse.Brn = providerServiceRequestId;
                    inquiryResponse.TotalAmount = totalAmount;
                    inquiryResponse.Invoices = SIRS.Invoices.Select(x => new InvoiceDTO
                    {
                        Amount = x.Amount,
                        Sequence = x.Sequence,
                        Data = x.Data.Select(d => new DataDTO { Key = d.Key, Value = d.Value }).ToList()
                    }).ToList();
                }
                else
                {
                    _providerService.UpdateProviderServiceRequestStatus(providerServiceRequestId, ProviderServiceRequestStatusType.Failed, userId);
                    var message = _dbMessageService.GetMainStatusCodeMessage(id: GetData.GetCode(SIRS.ResponseCode), providerId: serviceProviderId);
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
    }
}
