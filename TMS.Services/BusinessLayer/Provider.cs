﻿using Microsoft.Extensions.Localization;
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
        }
        public BaseProvider CreateServiceProvider(ServiceClassType type)
        {
            switch (type)
            {
                case ServiceClassType.Bill:
                    return new BillProvider(_denominationService, this, _providerService, _inquiryBillService, _transactionService, _localizer);
                case ServiceClassType.Topup:
                    return new TopupProvider();
                case ServiceClassType.Voucher:
                    return new VoucherProvider();
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
                //case DenominationClassType.Cancel:
                //    break;
                //case DenominationClassType.CashIn:
                //    break;
                //case DenominationClassType.CashU:
                //    break;
                //case DenominationClassType.CashTopUp:
                //    break;
                //case DenominationClassType.Donation:
                //    break;
                //case DenominationClassType.EducationService:
                //    break;
                //case DenominationClassType.ElectricityBill:
                //    break;
                //case DenominationClassType.ElectricityCard:
                //    break;
                //case DenominationClassType.Etisalat:
                //    break;
                //case DenominationClassType.Itunes:
                //    break;
                //case DenominationClassType.OneCard:
                //    break;
                //case DenominationClassType.OrangeInternet:
                //    break;
                //case DenominationClassType.OrangeMobile:
                //    break;
                //case DenominationClassType.Petrotrade:
                //    break;
                //case DenominationClassType.TelecomeEgypt:
                //    break;
                //case DenominationClassType.Topup:
                //    break;
                //case DenominationClassType.University:
                //    break;
                //case DenominationClassType.ValU:
                //    break;
                //case DenominationClassType.VodafoneInternet:
                //    break;
                //case DenominationClassType.VodafoneMobile:
                //    break;
                //case DenominationClassType.Voucher:
                //    break;
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
