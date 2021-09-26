using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;
using TMS.Infrastructure;

namespace TMS.Services.Services
{
    public interface ITransactionService
    {
        int AddRequest(RequestDTO model);
        //void AddTransactionRecipe(int transactionId);
        Root UpdateRequest(int? transactionId, int requestId, string RRN, RequestStatusCodeType requestStatus, int UserId, int providerServiceRequestId);
        void UpdateRequestStatus(int requestId, RequestStatusCodeType requestStatus);
        int AddInvoiceEducationService(int requestId, decimal basic_value, int userId, string ssn, decimal fees, int subServId, string customerName, string fcrn);
        int AddInvoiceBTech(int requestId, decimal amount, int userId, string billingAccount, decimal fees, string billingInfo);
        int AddInvoiceTedataEgyptCharge(string teleCode, string teleNumber, decimal basicValue, decimal addedMoney, int status, int userId, string msg, string transInv, int fawBillerId, string serviceName);
        int AddInvoiceTedataTest(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string msg, string transInv, string code, string messageTitle, int fawBillerId, string data, int servId);
        int AddInvoiceCashUTopUp(int requestId, decimal amount, int userId, string currency, string holderName);
        int AddInvoiceCashU(int requestId, decimal amount, int userId, string currency, string cardNumber, string serial, DateTime creationDate, DateTime expirationDate, int denominationId);
        int AddInvoiceCashIn(int requestId, decimal amount, int userId, string payment_ref_number, int serviceId, string accountName);
        int AddInvoiceDonation(string providerName, string serviceName, string serviceId, decimal basicValue, decimal addedMoney, int status, decimal userId, string accountName, string mobile);
        int AddInvoiceElectricityBill(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data, string mobile);
        int AddInvoiceUniversityBill(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data);
        int AddInvoiceElectricityCard(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data, string mobile, string cardType, string cardData, int? requestId, string eCardmomknPaymentId);
        int AddInvoiceWaterCard(int serviceId, string accountCode, string accountName, string address, string dueDate, decimal basicValue, decimal addedMoney, int status, decimal userId, string code, string message, int? providerTransactionId, string data, string mobile, string cardType, string cardData, int? requestId, string eCardmomknPaymentId);
        int AddInvoiceEtisalatEgypt(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName);
        int AddInvoiceOrangeMobileEgypt(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName);
        int AddInvoiceEtisalatInternet(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName);
        int AddInvoiceTelecomEgypt(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string msg, int requestId);
        int AddInvoiceLinkDsl(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string mobile);
        int AddInvoiceLinkDslReseller(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string mobile);
        int AddInvoiceVodafoneInternet(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName);
        int AddInvoiceVodafoneMobile(string teleCode, string teleNumber, string billDate, decimal basicValue, decimal addedMoney, int status, int userId, string accountName);
        int AddInvoiceITunes(string requestId, string pin, string serial, decimal basicValue, int userId, string productCode, string currency, decimal amountUAE, string validTo);
        int AddInvoiceSonyMicrosoft(string requestId, string pin, string serial, decimal basicValue, int userId, string productCode, int servId, decimal amountUSD, string currency, string validTo);
        int AddInvoiceValU(string requestId, decimal basicValue, int userId, string mobile, string passportNum, string SSN, string customerId);
        int AddInvoiceOneCard(string providerName, string serviceName, string servId, decimal basicValue, decimal addedMoney, int status, int userId, string accountName, string mobile, string secret);
        int AddInvoicePetrotrade(string requestId, decimal basicValue, int userId, string mobile, string registerNo, string extraBillInfo, string FCRN, decimal fees);
        int AddElectronicChargeLog(string providerCompany, string mobileNumber, float value, float totalAmount, string chargeStatus, int statusTransfer, int userId, int servId);
        int AddElectronicChargeLogVoucher(string providerCompany, string mobileNumber, decimal value, string chargeStatus, int statusTransfer, int userId, int countVoucher);
        int AddInvoiceElectronicCharge(string providerCompany, string mobileNumber, float value, float totalAmount, string chargeStatus, int statusTransfer, int userId, int logId, int servId);
        int AddInvoiceElectronicChargeProcBeeAwIncomeNew(int id, decimal value, int servId, int centerId, int userId, decimal invoicePrice, string mobileNumber);
        int AddInvoiceElectronicChargeNewVoucher(string providerCompany, string mobileNumber, decimal value, string chargeStatus, int statusTransfer, int userId, string masaryTransId, string voucherNumber, string serialNumber, int providerId, int servId, int logId);
        int AddInvoiceElectronicChargeIncomeNewVoucher(string providerCompany, decimal value, int userId, string voucherNumber, int providerId, int servId, int id);
        int AddTransaction(int? accountIdFrom, decimal amount, int denominationId, decimal originalAmount, decimal fees, string originalTrx, int? accountIdTo, int? invoiceId, int? requestId);
        void AddCommission(int transactionId, int? accountId, int denominationId, decimal originalAmount, int? accountProfileId);
        decimal GetTotalCommissions(int denominationId, decimal originalAmount, int? accountId, int? accountProfileId);
        int CheckVoucherValue(int serviceId, string provider, decimal value);
        bool IsIntervalTransationExist(int accountId, int denominationId, string billingAccount, decimal amount);
        bool IsRequestUUIDExist(int accountId, string UUID);
        string GetTransactionReceipt(int transactionId);
        TransactionDTO GetTransactionByBrn(int brn);
        int ReturnInvoice(int invoiceId, int userPayedId, string reason);
        void UpdateTransction(int id);
        int GetProviderRequestID(int transactionId, int accountId, int denominationID);
        int GetRequestID(int transactionId, int accountId);
        bool GetIsReversed(int transactionId, int accountId);
        int GetPendingPaymentCardStatus(int transactionId);
        void AddPendingPaymentCard(string PaymentRefInfo, int TransactionId, string CardType, string HostTransactionId, int? brn);
        void UpdatePendingPaymentCardStatus(int transactionId, int pendingPaymentCardStatusID);
        void TEDataLogUpdate(string chargeStatus, int requestId);
        int AddTEDataProcLog(string providerCompany, string mobileNumber, decimal value, string chargeStatus, int statusTransfer, int userId, int servId, int transId);
    }
}