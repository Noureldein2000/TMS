using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Services.BusinessLayer;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public class DynamicService : IDynamicService
    {
        private readonly IServiceProvider _sp;
        private readonly IDenominationService _denominationService;
        private readonly ICancelService _cancelService;
        private readonly ITransactionService _transactionService;
        public DynamicService(IServiceProvider sp, IDenominationService denominationService, ICancelService cancelService, ITransactionService transactionService)
        {
            _sp = sp;
            _denominationService = denominationService;
            _cancelService = cancelService;
            _transactionService = transactionService;
        }

        public Task<PaymentResponseDTO> Cancel(int transactionId, int accountId, int userId, int id)
        {

            //var paymentResponse = new Task<PaymentResponseDTO>(;
            if (!string.IsNullOrEmpty(transactionId.ToString()) && !string.IsNullOrEmpty(accountId.ToString()))
            {
                CancelDTO model = new CancelDTO();

                int providerRequestID = _transactionService.GetProviderRequestID(transactionId, accountId, id);
                if (providerRequestID != -1)
                {
                    if (!_transactionService.GetIsReversed(transactionId, accountId))
                    {
                        var denomination = _denominationService.GetDenomination(id);
                        model.MomknPaymentId = _transactionService.GetRequestID(transactionId, accountId).ToString();
                        model.Brn = providerRequestID;
                        model.AccountId = accountId;

                        if (_transactionService.GetPendingPaymentCardStatus(transactionId) == 1 && (denomination.ServiceID == 20 || denomination.ServiceID == 57))
                            return _cancelService.Cancel(model, userId, id, 0, denomination.ServiceProviderId, 0);
                        else if (denomination.ServiceID == 36)
                            return _cancelService.Cancel(model, userId, id, 0, denomination.ServiceProviderId, 0);
                        else
                            throw new TMSException("NoDataFounded", "19");
                    }
                    else
                        throw new TMSException("DupplicatedRefund", "38");
                }
                else
                    throw new TMSException("RequestNotFound", "14");
            }
            else
                throw new TMSException("MissingData", "15");
        }

        public async Task<FeesResponseDTO> Fees(FeesRequestDTO feesModel, int userId, int id)
        {
            var providerClassType = _denominationService.GetServiceClassType(id);
            if (providerClassType == 0)
                throw new TMSException("Invalid service", "");

            using var scope = _sp.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<Provider>();
            var provider = service.CreateServiceProvider(providerClassType);
            return provider.Fees(feesModel, userId, id);
        }

        public async Task<InquiryResponseDTO> Inquiry(InquiryRequestDTO inquiryModel, int userId, int id)
        {
            var providerClassType = _denominationService.GetServiceClassType(id);
            if (providerClassType == 0)
                throw new TMSException("Invalid service", "");

            using var scope = _sp.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<Provider>();
            var provider = service.CreateServiceProvider(providerClassType);
            return await provider.Inquiry(inquiryModel, userId, id);
        }

        public async Task<PaymentResponseDTO> Pay(PaymentRequestDTO payModel, int userId, int id)
        {
            var providerClassType = _denominationService.GetServiceClassType(id);
            if (providerClassType == 0)
                throw new TMSException("Invalid service", "");

            using var scope = _sp.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<Provider>();
            var provider = service.CreateServiceProvider(providerClassType);
            return await provider.Pay(payModel, userId, id);

        }

        //private BaseProvider GetBaseProviderClass(int denominationId)
        //{
        //    var providerClassType = _denominationService.GetServiceClassType(denominationId);
        //    if (providerClassType == 0)
        //        throw new TMSException("Invalid service", "");

        //    using var scope = _sp.CreateScope();
        //    var service = scope.ServiceProvider.GetRequiredService<Provider>();
        //    return service.CreateServiceProvider(providerClassType);
        //}
    }
}
