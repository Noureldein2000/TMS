using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Infrastructure.Utils;
using TMS.Services.Models;
using TMS.Services.Repositories;
using TMS.Services.Services;

namespace TMS.Services.BusinessLayer
{
    public class BillProvider : BaseProvider
    {
        private readonly IDenominationService _denominationService;
        private readonly Provider _provider;
        private readonly IProviderService _providerService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ITransactionService _transactionService;
        private readonly IStringLocalizer<ServiceLanguageResource> _localizer;
        public BillProvider(
            IDenominationService denominationService,
            Provider provider,
            IProviderService providerService,
            IInquiryBillService inquiryBillService,
            ITransactionService transactionService,
            IStringLocalizer<ServiceLanguageResource> localizer
            )
        {
            _denominationService = denominationService;
            _provider = provider;
            _localizer = localizer;
            _providerService = providerService;
            _inquiryBillService = inquiryBillService;
            _transactionService = transactionService;
        }
        public override FeesResponseDTO Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            decimal totalAmount = 0;
            var denomination = _denominationService.GetDenomination(id);
            if (feesModel.Amount <= 0)
                throw new TMSException(_localizer["InvalidData"].Value, "12");

            if (feesModel.Brn == 0 && denomination.Inquirable)
                throw new TMSException(_localizer["MissingData"].Value, "15");

            if (!denomination.Inquirable && feesModel.Brn != 0)
                throw new TMSException(_localizer["RequestNotFound"].Value, "14");

            if (denomination.Inquirable && !_providerService.IsProviderServiceRequestExsist((int)Infrastructure.RequestType.Inquiry, feesModel.Brn, (int)ProviderServiceRequestStatusType.Success, id, userId))
                throw new TMSException(_localizer["RequestNotFound"].Value, "14");

            if (denomination.MinValue > 0 && denomination.MaxValue > 0 && (feesModel.Amount < denomination.MinValue || feesModel.Amount > denomination.MaxValue))
                throw new TMSException(_localizer["InvalidAmount"].Value, "11");
            //{
            //    DtMsg = DB_MessageMapping.GetMessage((int)DB_MessageMapping.MomknMessage.InvalidAmount, 0, _FDTO.Language);
            //    DtMsg.Rows[0][1] = DB_MessageMapping.ReplaceMessage(DtMsg.Rows[0][1].ToString(), D.MinValue, D.MaxValue, D.Interval);
            //}
            if (denomination.Value != 0 && denomination.Value != feesModel.Amount)
                throw new TMSException(_localizer["InvalidAmount"].Value, "11");
            //{
            //    DtMsg = DB_MessageMapping.GetMessage((int)DB_MessageMapping.MomknMessage.InvalidAmount, 0, _FDTO.Language);
            //    DtMsg.Rows[0][1] = DB_MessageMapping.ReplaceMessage(DtMsg.Rows[0][1].ToString(), D.MinValue, D.MaxValue, D.Interval);
            //}
            //if ((feesModel.Data == null || feesModel.Data.Count == 0) && denomination.Inquirable)
            //    throw new TMSException(_localizer["MissingData", "15");

            if (feesModel.Data != null && denomination.Inquirable)
            {
                var sequence = feesModel.Data.Where(f => f.Key == "Sequence").Select(x => x.Value).FirstOrDefault();

                if (string.IsNullOrEmpty(sequence))
                    throw new TMSException(_localizer["InvalidData"].Value, "12");

                var sequenceBills = sequence.Split(',').Select(s => int.Parse(s)).ToList();
                if (sequenceBills == null || sequenceBills.Count <= 0)
                    throw new TMSException(_localizer["InvalidData"].Value, "12");

                var inquiryBillList = _inquiryBillService.GetInquiryBillSequence(feesModel.Brn).ToList();

                if (denomination.PaymentModeID == (int)PaymentModeType.Fixed)
                {
                    switch (denomination.BillPaymentModeID)
                    {
                        case (int)BillPaymentModeType.OnlyOne:
                            //Check Only One Sequence & Amount
                            if (sequenceBills[0] != inquiryBillList.FirstOrDefault(u => u.Sequence == sequenceBills[0]).Sequence ||
                                feesModel.Amount != inquiryBillList.FirstOrDefault(u => u.Sequence == sequenceBills[0]).Amount)
                                throw new TMSException(_localizer["InvalidData"].Value, "12");
                            break;

                        case (int)BillPaymentModeType.MustAll:
                            if (sequenceBills.Count != sequenceBills.Distinct().Count() || sequenceBills.Count != inquiryBillList.Count)
                                throw new TMSException(_localizer["AmountNotMatched"].Value, "22");

                            foreach (var item in inquiryBillList)
                            {
                                if (!sequenceBills.Contains(item.Sequence))
                                    throw new TMSException(_localizer["MissingData"].Value, "15");
                                else
                                    totalAmount += item.Amount;
                            }

                            if (feesModel.Amount != totalAmount)
                                throw new TMSException(_localizer["CanNotPayPartial"].Value, "52");

                            break;
                        case (int)BillPaymentModeType.Multiple:
                            if (sequenceBills.Count != sequenceBills.Distinct().Count())
                                throw new TMSException(_localizer["AmountNotMatched"].Value, "22");

                            foreach (var item in inquiryBillList)
                            {
                                if (!sequenceBills.Contains(item.Sequence))
                                {
                                    throw new TMSException(_localizer["MissingData"].Value, "15");
                                }
                                else
                                    totalAmount += item.Amount;
                            }

                            if (feesModel.Amount != totalAmount)
                                throw new TMSException(_localizer["CanNotPayMoreThanOneBill"].Value, "179");
                            break;
                        default:
                            throw new TMSException(_localizer["NotSupportedMode"].Value, "147");
                    }
                }
                else if (denomination.PaymentModeID == (int)PaymentModeType.Partial)
                {
                    switch (denomination.BillPaymentModeID)
                    {
                        case (int)BillPaymentModeType.OnlyOne:
                            if (!sequenceBills.Contains(1) || feesModel.Amount > inquiryBillList.FirstOrDefault(u => u.Sequence == 1).Amount)
                                throw new TMSException(_localizer["CanNotPayPartial"].Value, "52");
                            break;
                        case (int)BillPaymentModeType.MustAll:
                            if (sequenceBills.Count != sequenceBills.Distinct().Count() || inquiryBillList.Count != sequenceBills.Count)
                                throw new TMSException(_localizer["InvalidData"].Value, "12");

                            foreach (var item in inquiryBillList)
                            {
                                if (!sequenceBills.Contains(item.Sequence))
                                    throw new TMSException(_localizer["MissingData"].Value, "15");
                                else
                                    totalAmount += item.Amount;
                            }

                            if (feesModel.Amount > totalAmount)
                                throw new TMSException(_localizer["CanNotPayMoreThanOneBill"].Value, "179");

                            break;
                        case (int)BillPaymentModeType.Multiple:
                            if (sequenceBills.Count != sequenceBills.Distinct().Count())
                                throw new TMSException(_localizer["InvalidData"].Value, "12");

                            foreach (var item in inquiryBillList)
                            {
                                if (!sequenceBills.Contains(item.Sequence))
                                    throw new TMSException(_localizer["MissingData"].Value, "15");
                                else
                                    totalAmount += item.Amount;
                            }

                            if (feesModel.Amount > totalAmount)
                                throw new TMSException(_localizer["CanNotPayMoreThanOneBill"].Value, "179");

                            break;
                        default:
                            throw new TMSException(_localizer["NotSupportedMode"].Value, "147");
                    }
                }
                else if (denomination.PaymentModeID == (int)PaymentModeType.InAdvance)
                {
                    switch (denomination.BillPaymentModeID)
                    {
                        case (int)BillPaymentModeType.OnlyOne:
                            if (sequenceBills.Count != 1 || sequenceBills[0] != inquiryBillList.FirstOrDefault(u => u.Sequence == sequenceBills[0]).Sequence)
                                throw new TMSException(_localizer["InvalidData"].Value, "12");
                            break;
                        case (int)BillPaymentModeType.MustAll:
                            if (sequenceBills.Count != sequenceBills.Distinct().Count() || sequenceBills.Count != inquiryBillList.Count)
                                throw new TMSException(_localizer["InvalidData"].Value, "12");

                            foreach (var item in inquiryBillList)
                            {
                                if (!sequenceBills.Contains(item.Sequence))
                                    throw new TMSException(_localizer["MissingData"].Value, "15");
                            }
                            break;
                        case (int)BillPaymentModeType.Multiple:
                            if (sequenceBills.Count != sequenceBills.Distinct().Count())
                                throw new TMSException(_localizer["InvalidData"].Value, "12");

                            foreach (var item in inquiryBillList)
                            {
                                if (!sequenceBills.Contains(item.Sequence))
                                    throw new TMSException(_localizer["MissingData"].Value, "15");
                            }

                            break;
                        default:
                            throw new TMSException(_localizer["NotSupportedMode"].Value, "147");
                    }
                }
            }

            var denoProvider = _provider.CreateDenominationProvider(denomination.ClassType);
            return denoProvider.Fees(feesModel, userId, id);
        }

        public override async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiry, int userId, int id)
        {
            await base.Inquiry(inquiry, userId, id);

            var denomination = _denominationService.GetDenomination(id);
            if ((denomination.ServiceID == 19 || denomination.ServiceID == 20 || denomination.ServiceID == 26
                || denomination.ServiceID == 27 || denomination.ServiceID == 31 || denomination.ServiceID == 38
                || denomination.ServiceID == 40 || denomination.ServiceID == 41 || denomination.ServiceID == 56
                || denomination.ServiceID == 62)
                || (Validates.CheckMobileNumber(inquiry.BillingAccount) || Validates.CheckLandLineNumber(inquiry.BillingAccount)))
            {
                if (denomination.Status == false)
                    throw new TMSException(_localizer["ServiceUnavailable"].Value, "");

                if (denomination.ClassType == 0)
                    throw new TMSException(_localizer["UnsupportedService"].Value, "");

                var denoProvider = _provider.CreateDenominationProvider(denomination.ClassType);
                return await denoProvider.Inquiry(inquiry, userId, id, denomination.ServiceProviderId);
            }

            if (Validates.CheckMobileNumber(inquiry.BillingAccount))
                throw new TMSException(_localizer["InvalidMobileNumber"].Value, "");

            else if (Validates.CheckLandLineNumber(inquiry.BillingAccount))
                throw new TMSException(_localizer["InvalidTelephoneNumber"].Value, "");

            else
                throw new TMSException(_localizer["Invalid"].Value, "");
        }

        public override async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id)
        {
            var denomination = _denominationService.GetDenomination(id);
            if (payModel.Brn != 0 && denomination.Inquirable)
                payModel.BillingAccount = _providerService.GetProviderServiceRequestBillingAccount(payModel.Brn, userId, id);

            //Electricty Bill,Electricty Card,Orange Corporate Mobile
            if ((denomination.ServiceID == 19 || denomination.ServiceID == 20 || denomination.ServiceID == 26 || denomination.ServiceID == 27
                || denomination.ServiceID == 31 || denomination.ServiceID == 38 || denomination.ServiceID == 40 || denomination.ServiceID == 41
                || denomination.ServiceID == 56 || denomination.ServiceID == 62) ||
                (Validates.CheckMobileNumber(payModel.BillingAccount) || Validates.CheckLandLineNumber(payModel.BillingAccount)))
            {

                if (denomination.Status != true)
                    throw new TMSException(_localizer["ServiceUnavailable"].Value, "8");

                if (payModel.Amount <= 0)
                    throw new TMSException(_localizer["InvalidData"].Value, "12");

                if (string.IsNullOrEmpty(payModel.BillingAccount))
                    throw new TMSException(_localizer["MissingData"].Value, "15");

                if (denomination.Inquirable && !_providerService.IsProviderServiceRequestExsist((int)Infrastructure.RequestType.Inquiry, payModel.Brn, (int)ProviderServiceRequestStatusType.Success, id, userId))
                    throw new TMSException(_localizer["RequestNotFound"].Value, "14");

                if (!_providerService.IsProviderServiceRequestExsist((int)Infrastructure.RequestType.Fees, payModel.Brn, (int)ProviderServiceRequestStatusType.Success, id, userId))
                    throw new TMSException(_localizer["RequestNotFound"].Value, "14");

                if (denomination.Value != 0 && denomination.Value != payModel.Amount)
                    throw new TMSException(_localizer["InvalidAmount"].Value, "11");

                if (_transactionService.IsIntervalTransationExist(userId, id, payModel.BillingAccount, payModel.Amount))
                {
                    throw new TMSException(_localizer["InvalidInterval"].Value, "10");
                    //DtMsg.Rows[0][1] = DB_MessageMapping.ReplaceMessage(DtMsg.Rows[0][1].ToString(), D.MinValue, D.MaxValue, D.Interval);
                }
                if (payModel.Brn != 0 && _providerService.IsProviderServiceRequestExsist((int)Infrastructure.RequestType.Payment, payModel.Brn, (int)ProviderServiceRequestStatusType.Success, id, userId))
                {
                    throw new TMSException(_localizer["DupplicatedTrx"].Value, "7");
                    //DtMsg = DB_MessageMapping.GetMessage((int)DB_MessageMapping.MomknMessage.DupplicatedTrx, 0, _PDTO.Language);
                }
                if (payModel.HostTransactionID == "" && _transactionService.IsRequestUUIDExist(payModel.AccountId, payModel.HostTransactionID))
                    throw new TMSException(_localizer["DupplicatedTrx"].Value, "7");

                int BrnFees = _providerService.GetMaxProviderServiceRequest(payModel.Brn, (int)Infrastructure.RequestType.Fees);

                var inquiryBillList = _inquiryBillService.GetInquiryBillSequence(BrnFees);
                decimal feesAmount = 0;
                foreach (var item in inquiryBillList)
                {
                    feesAmount += item.Amount;
                }
                var denoProvider = _provider.CreateDenominationProvider(denomination.ClassType);

                var feesResponse = denoProvider.Fees(new FeesRequestDTO
                {
                    AccountId = payModel.AccountId,
                    Amount = feesAmount,
                    Brn = payModel.Brn,
                    Data = new List<DataDTO>(),
                    ServiceListVersion = payModel.ServiceListVersion,
                    Version = payModel.Version,
                }, userId, id);
                if (feesResponse.Code != "200")
                    throw new TMSException(_localizer["InvalidFees"].Value, "47");

                return await denoProvider.Pay(payModel, userId, id, feesResponse.TotalAmount, feesResponse.Fees, denomination.ServiceProviderId);
            }
            else
                throw new TMSException(_localizer["InvalidData"].Value, "12");
        }
    }
}
