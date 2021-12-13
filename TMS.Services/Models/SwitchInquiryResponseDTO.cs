using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class SwitchInquiryResponseDTO
    {
        public SwitchInquiryResponseDTO()
        {
            Invoices = new List<SwitchInvoice>();
        }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SwitchInvoice> Invoices { get; set; }
    }

    public class SwitchInvoice
    {
        public SwitchInvoice()
        {
            Data = new List<SwitchData>();
        }
        public int Sequence { get; set; }
        public decimal Amount { get; set; }
        public List<SwitchData> Data { get; set; }
    }

    public class SwitchInquiryRequest
    {
        public SwitchInquiryRequest()
        {
            Data = new List<SwitchData>();
        }
        public string BillingAcctNumber { get; set; }
        public List<SwitchData> Data { get; set; }
    }
    public class SwitchData
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SwitchPaymentRequest
    {
        public SwitchPaymentRequest()
        {
            Data = new List<SwitchData>();
        }
        public string Uuid { get; set; }
        public decimal Amount { get; set; }
        public List<SwitchData> Data { get; set; }
    }

    class SwitchPaymentResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseDate { get; set; }
        public string TransactionId { get; set; }

        public int? InvoiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
