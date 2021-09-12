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
    public class VoucherProvider : BaseProvider
    {
        private readonly IDenominationService _denominationService;
        private readonly Provider _provider;
        private readonly IProviderService _providerService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ITransactionService _transactionService;
        private readonly IStringLocalizer<ServiceLanguageResource> _localizer;
        public VoucherProvider(
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
            int count = 1;
            var denomination = _denominationService.GetDenomination(id);

            if (feesModel.Data != null)
            {
                foreach (var item in feesModel.Data)
                {
                    if (item.Key == "Count")
                    {
                        if (string.IsNullOrEmpty(item.Value))
                            count = 1;
                        else if (!int.TryParse(item.Value, out count))
                            throw new TMSException(_localizer["InvalidData"].Value, "12");
                        else if (item.Value == "0")
                            throw new TMSException(_localizer["InvalidData"].Value, "12");
                    }


                }
            }

            if (feesModel.Brn != 0)
                throw new TMSException(_localizer["RequestNotFound"].Value, "14");
            else if (count > 10)
                throw new TMSException(_localizer["VoucherExceededLimit"].Value, "35");

            feesModel.Amount = denomination.Value;
            var denoProvider = _provider.CreateDenominationProvider(denomination.ClassType);
            return denoProvider.Fees(feesModel, userId, id);
        }

        public override async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiry, int userId, int id)
        {
            await base.Inquiry(inquiry, userId, id);
            throw new NotImplementedException();
        }

        public override async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id)
        {
            var denomination = _denominationService.GetDenomination(id);

            if (denomination.Status == true)
            {
                if (payModel.Amount <= 0)
                    throw new TMSException(_localizer["InvalidAmount"].Value, "11");
                else if (payModel.Brn == 0 || !_providerService.IsProviderServiceRequestExsist((int)Infrastructure.RequestType.Fees, payModel.Brn, (int)ProviderServiceRequestStatusType.Success, id, userId))
                    throw new TMSException(_localizer["RequestNotFound"].Value, "14");
                else if (_transactionService.IsIntervalTransationExist(userId, id, payModel.BillingAccount, payModel.Amount))
                    throw new TMSException(_localizer["InvalidInterval"].Value, "10");
                else if (payModel.Brn != 0 && _providerService.IsProviderServiceRequestExsist((int)Infrastructure.RequestType.Payment, payModel.Brn, (int)ProviderServiceRequestStatusType.Success, id, userId))
                    throw new TMSException(_localizer["DupplicatedTrx"].Value, "7");
            }
            else
                throw new TMSException(_localizer["ServiceUnavailable"].Value, "8");


            //int BrnFees = _providerService.GetMaxProviderServiceRequest(payModel.Brn, (int)Infrastructure.RequestType.Fees);

            //var inquiryBillList = _inquiryBillService.GetInquiryBillSequence(BrnFees);
            //decimal feesAmount = 0;
            //foreach (var item in inquiryBillList)
            //{
            //    feesAmount += item.Amount;
            //}
            var denoProvider = _provider.CreateDenominationProvider(denomination.ClassType);

            var feesResponse = denoProvider.Fees(new FeesRequestDTO
            {
                AccountId = payModel.AccountId,
                Amount = denomination.Value,
                Brn = payModel.Brn,
                Data = new List<DataDTO>(),
                ServiceListVersion = payModel.ServiceListVersion,
                Version = payModel.Version,
            }, userId, id);

            if (feesResponse.Code != "200")
                throw new TMSException(_localizer["InvalidFees"].Value, "47");

            return await denoProvider.Pay(payModel, userId, id, feesResponse.TotalAmount, feesResponse.Fees, denomination.ServiceProviderId);

        }
    }
}
