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
        public Provider(
               IDenominationService denominationService,
               IProviderService providerService,
               ISwitchService switchService,
               IInquiryBillService inquiryBillService,
               ILoggingService loggingService,
               IDbMessageService dbMessageService,
               IFeesService feesService,
               ITransactionService transactionService,
               ICancelService cancelService
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
            _cancelService = cancelService;
        }
        public BaseProvider CreateServiceProvider(ServiceClassType type)
        {
            switch (type)
            {
                case ServiceClassType.Bill:
                    return new BillProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService);
                case ServiceClassType.Topup:
                    return new TopupProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService);
                case ServiceClassType.Voucher:
                    return new VoucherProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService);
                default:
                    return null;
            }
        }
        public IBaseProvider CreateDenominationProvider(DenominationClassType type)
        {
            switch (type)
            {
                case DenominationClassType.BTech:
                    return new BTech(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.Cancel:
                    return new Cancel(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.CashIn:
                    return new CashIn(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.CashU:
                    return new CashU(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.CashUTopUp:
                    return new CashUTopUp(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.Donation:
                    return new Donation(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.EducationService:
                    return new EducationService(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.ElectricityBill:
                    return new ElectricityBill(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.ElectricityCard:
                    return new ElectricityCard(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService, _cancelService);
                case DenominationClassType.Etisalat:
                    return new Etisalat(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.Itunes:
                    return new ITunes(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.OneCard:
                    return new OneCard(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.OrangeInternet:
                    return new OrangeInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.OrangeMobile:
                    return new OrangeMobile(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.Petrotrade:
                    return new Petrotrade(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.TelecomeEgypt:
                    return new TelecomeEgypt(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.Topup:
                    return new Topup(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.University:
                    return new University(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.ValU:
                    return new ValU(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.VodafoneInternet:
                    return new VodafoneInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.VodafoneMobile:
                    return new VodafoneMobile(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.Voucher:
                    return new Voucher(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.WaterBill:
                    return new WEInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.WEInternet:
                    return new WEInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                case DenominationClassType.WEInternetExtra:
                    return new WEInternetExtra(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _transactionService);
                default:
                    return null;
            }
        }
    }
}
