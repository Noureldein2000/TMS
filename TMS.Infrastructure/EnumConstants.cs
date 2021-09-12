﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Infrastructure
{
    class EnumConstants
    {

    }

    public enum ServiceClassType : short
    {
        Bill = 1,
        Topup = 2,
        Voucher = 3
    }

    public enum DenominationClassType : short
    {
        BTech = 1,
        Cancel = 2,
        CashIn = 3,
        CashU = 4,
        CashTopUp = 5,
        Donation = 6,
        EducationService = 7,
        ElectricityBill = 8,
        ElectricityCard = 9,
        Etisalat = 10,
        Itunes = 11,
        OneCard = 12,
        OrangeInternet = 13,
        OrangeMobile = 14,
        Petrotrade = 15,
        TelecomeEgypt = 16,
        Topup = 17,
        University = 18,
        ValU = 19,
        VodafoneInternet = 20,
        VodafoneMobile = 21,
        Voucher = 22,
        WaterBill = 23,
        WEInternet = 24,
        WEInternetExtra = 25
    }

    public enum ParameterProviderNames 
    {
        ServiceFees = 1,
        Tax = 2,
        AddedValue = 3,
        ProviderTransactionId = 4,
        ProviderServiceFees = 5,
        Serial = 6,
        Pin = 7,
        BillNumber = 8,
        BillRecId = 9,
        Address = 10,
        ArabicName = 11,
        CustomerCode = 12,
        EnglishName = 13,
        PaymentRefInfo = 14,
        CurrentCode = 15,
        MinAmount = 16,
        PaymentMode = 17,
        ShortDescAR = 18,
        ShortDescEN = 19,
        AmountFees = 20,
        ProviderPaymentId = 21,
        CardType,
        CardData,
        prepaidAmount,
        totalDeducts,
        cardAmount,
        momknPaymentId,
        billReferenceNumber,
        programName,
        collageName,
        educationYear,
        PaymentDueDate,
        PurchaseCode,
        MobileNumber,
        PassportNumber,
        NationalId,
        BillCount,
        AsyncRqUID,
        ExtraBillInfo,
        CountInstalmentPenalty,
        ValueInstalmentPenalty,
        CountRemainInstalment,
        RequestCode,
        School,
        Stage,
        StartDate,
        EndDate,
        DueBills,
        Amount,
        Count
    }

    public enum LoggingType : short
    {
        CustomerRequest = 1,
        CustomerResponse = 2,
        ProviderRequest = 3,
        ProviderResponse = 4,
    }
    public enum SwitchEndPointAction : short
    {
        inquiry = 1,
        fees = 2,
        payment = 3,
    }

    public enum MainStatusCodeType
    {
        success = 1,
        FailedTrx = 2,
        PendingTrx = 3,
        UnderProcessing = 4
    }
    public enum RequestStatusCodeType
    {
        UnderProcessing = 1,
        Success = 2,
        Fail = 3,
        Pending = 4,
        Refunded = 5,
        NeedToCheck = 6
    }

    public enum BillPaymentModeType
    {
        OnlyOne = 1,
        MustAll = 2,
        Multiple = 3,
    }
    public enum PaymentModeType
    {
        Fixed = 1,
        Partial = 2,
        InAdvance = 3,
    }
    public enum RequestType
    {
        Inquiry = 1,
        Fees = 2,
        Payment = 3
    }
    public enum ProviderServiceRequestStatusType : int
    {
        UnderProcess = 1,
        Success = 2,
        Failed = 3,
        Canceled = 4
    }
}
