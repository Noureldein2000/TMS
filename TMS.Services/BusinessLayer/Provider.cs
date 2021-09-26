using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Services.ProviderLayer;
using TMS.Services.Repositories;
using TMS.Services.Services;

namespace TMS.Services.BusinessLayer
{
    public class Provider
    {
        private readonly IDenominationService _denominationService;
        private readonly IProviderService _providerService;
        private readonly ISwitchService _switchService;
        private readonly IInquiryBillService _inquiryBillService;
        private readonly ILoggingService _loggingService;
        private readonly IDbMessageService _dbMessageService;
        private readonly IFeesService _feesService;
        private readonly ITransactionService _transactionService;
        private readonly ICancelService _cancelService;
        private readonly IStringLocalizer<ServiceLanguageResource> _localizer;
        public Provider(
               IDenominationService denominationService,
               IProviderService providerService,
               ISwitchService switchService,
               IInquiryBillService inquiryBillService,
               ILoggingService loggingService,
               IDbMessageService dbMessageService,
               IFeesService feesService,
               ITransactionService transactionService,
               ICancelService cancelService,
               IStringLocalizer<ServiceLanguageResource> localizer
            )
        {
            _denominationService = denominationService;
            _providerService = providerService;
            _switchService = switchService;
            _localizer = localizer;
            _inquiryBillService = inquiryBillService;
            _loggingService = loggingService;
            _dbMessageService = dbMessageService;
            _feesService = feesService;
            _transactionService = transactionService;
            _cancelService = cancelService;
        }
        public BaseProvider CreateServiceProvider(ServiceClassType type)
        {
            switch (type)
            {
                case ServiceClassType.Bill:
                    return new BillProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService, _localizer);
                case ServiceClassType.Topup:
                    return new TopupProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService, _localizer);
                case ServiceClassType.Voucher:
                    return new VoucherProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService, _localizer);
                default:
                    return null;
            }
        }
        public IBaseProvider CreateDenominationProvider(DenominationClassType type)
        {
            switch (type)
            {
                case DenominationClassType.BTech:
                    return new BTech(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.Cancel:
                    return new Cancel(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.CashIn:
                    return new CashIn(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.CashU:
                    return new CashU(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.CashUTopUp:
                    return new CashUTopUp(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.Donation:
                    return new Donation(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.EducationService:
                    return new EducationService(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.ElectricityBill:
                    return new ElectricityBill(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.ElectricityCard:
                    return new ElectricityCard(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _cancelService, _localizer);
                case DenominationClassType.Etisalat:
                    return new Etisalat(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.Itunes:
                    return new ITunes(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.OneCard:
                    return new OneCard(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.OrangeInternet:
                    return new OrangeInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.OrangeMobile:
                    return new OrangeMobile(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.Petrotrade:
                    return new Petrotrade(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.TelecomeEgypt:
                    return new TelecomeEgypt(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.Topup:
                    return new Topup(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.University:
                    return new University(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.ValU:
                    return new ValU(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.VodafoneInternet:
                    return new VodafoneInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.VodafoneMobile:
                    return new VodafoneMobile(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                case DenominationClassType.Voucher:
                    return new Voucher(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _localizer);
                //case DenominationClassType.WaterBill:
                //    break;
                //case DenominationClassType.WEInternet:
                //    break;
                //case DenominationClassType.WEInternetExtra:
                //    break;
                default:
                    return null;
            }
        }
    }
}
