using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Services.ProviderLayer;
using TMS.Services.Repositories;
using TMS.Services.Services;
using TMS.Services.SOFClientAPIs;

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
        private readonly ITaxService _taxesService;
        private readonly ITransactionService _transactionService;
        private readonly ICancelService _cancelService;
        private readonly IAccountsApi _accountsApi;
        public Provider(
               IDenominationService denominationService,
               IProviderService providerService,
               ISwitchService switchService,
               IInquiryBillService inquiryBillService,
               ILoggingService loggingService,
               IDbMessageService dbMessageService,
               IFeesService feesService,
               ITaxService taxesService,
               ITransactionService transactionService,
               ICancelService cancelService,
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
            _cancelService = cancelService;
            _accountsApi = accountsApi;
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
                    return new BTech(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Cancel:
                    return new Cancel(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.CashIn:
                    return new CashIn(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.CashU:
                    return new CashU(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.CashUTopUp:
                    return new CashUTopUp(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Donation:
                    return new Donation(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.EducationService:
                    return new EducationService(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.ElectricityBill:
                    return new ElectricityBill(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.ElectricityCard:
                    return new ElectricityCard(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _cancelService, _accountsApi);
                case DenominationClassType.Etisalat:
                    return new Etisalat(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Itunes:
                    return new ITunes(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.OneCard:
                    return new OneCard(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.OrangeInternet:
                    return new OrangeInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.OrangeMobile:
                    return new OrangeMobile(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Petrotrade:
                    return new Petrotrade(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.TelecomeEgypt:
                    return new TelecomeEgypt(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Topup:
                    return new Topup(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.University:
                    return new University(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.ValU:/**/
                    return new ValU(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.VodafoneInternet:
                    return new VodafoneInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.VodafoneMobile:
                    return new VodafoneMobile(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Voucher:
                    return new Voucher(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.WaterBill:
                    return new WaterBill(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.WEInternet:
                    return new WEInternet(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.WEInternetExtra:
                    return new WEInternetExtra(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.SocialInsurance:
                    return new SocialInsurance(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.TamkeenLoan:
                    return new TamkeenLoan(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Talabat:
                    return new Talabat(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.BeniSwefUniversity:
                    return new BeniSuefUniversity(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                case DenominationClassType.Zaha:
                    return new Zaha(_denominationService, _providerService, _switchService, _inquiryBillService, _loggingService, _dbMessageService, _feesService, _taxesService, _transactionService, _accountsApi);
                default:
                    return null;
            }
        }
    }
}
