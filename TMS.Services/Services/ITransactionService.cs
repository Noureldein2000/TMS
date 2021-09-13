﻿using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;
using TMS.Infrastructure;

namespace TMS.Services.Services
{
    public interface ITransactionService
    {
        int AddRequest(RequestDTO model);
        void AddTransactionRecipe(int transactionId);
        void UpdateRequest(int? transactionId, int requestId, string RRN, RequestStatusCodeType requestStatus, int UserId, int providerServiceRequestId);
        void UpdateRequestStatus(int requestId, RequestStatusCodeType requestStatus);
        int AddInvoiceEducationService(int requestId, decimal basic_value, int userId, string ssn, decimal fees, int subServId, string customerName, string fcrn);
        int AddInvoiceBTech(int requestId, decimal amount, int userId, string billingAccount, decimal fees, string billingInfo);
        int AddInvoiceCashUTopUp(int requestId, decimal amount, int userId, string currency, string holderName);
        int AddInvoiceCashU(int requestId, decimal amount, int userId, string currency,string cardNumber,string serial,DateTime creationDate,DateTime expirationDate, int denominationId);
        int AddInvoiceCashIn(int requestId, decimal amount, int userId, string payment_ref_number, int serviceId, string accountName);
        int AddTransaction(int? accountIdFrom, decimal amount, int denominationId, decimal originalAmount, decimal fees, string originalTrx, int? accountIdTo, int? invoiceId, int? requestId);
        void AddCommission(int transactionId, int? accountId, int denominationId, decimal originalAmount, int? accountProfileId);
        decimal GetTotalCommissions(int denominationId, decimal originalAmount, int? accountId, int? accountProfileId);
        bool IsIntervalTransationExist(int accountId, int denominationId, string billingAccount, decimal amount);
        bool IsRequestUUIDExist(int accountId, string UUID);
        string GetTransactionReceipt(int transactionId);
    }
}
