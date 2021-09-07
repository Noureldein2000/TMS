using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
        public DynamicService(IServiceProvider sp, IDenominationService denominationService)
        {
            _sp = sp;
            _denominationService = denominationService;
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
