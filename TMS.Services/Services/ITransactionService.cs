using System;
using System.Collections.Generic;
using System.Text;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface ITransactionService
    {
        int AddRequest(RequestDTO model);
        void UpdateRequest(int? transactionId, int requestId, string RRN, int requestStatus, int UserId, int providerServiceRequestId);
        void UpdateRequestStatus(int requestId, int requestStatus);
        void AddInvoice(int requestId, decimal amount, int userId, string billingAccount, decimal fees, string billingInfo);
        int AddTransaction(int? accountIdFrom, decimal amount, int denominationId, decimal originalAmount, decimal fees, string originalTrx, int? accountIdTo, int? invoiceId, int? requestId);
        void AddCommission(int transactionId, int? accountId, int denominationId, decimal originalAmount, int? accountProfileId);
        decimal GetTotalCommissions(int denominationId, decimal originalAmount, int? accountId, int? accountProfileId);
        bool IsIntervalTransationExist(int accountId, int denominationId, string billingAccount, decimal amount);
        bool IsRequestUUIDExist(int accountId, string UUID);
    }
}
