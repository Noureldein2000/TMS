using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Data.Entities
{
    public class Invoice : BaseEntity<int>
    {
        public int Invoice_Id_card { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public int? ServId { get; set; }
        public int? UserId { get; set; }
        public bool Status { get; set; }
        public decimal InCome { get; set; }
        public bool InComeStatus { get; set; }
        public int? DeliveryCost { get; set; }
        public int? TransInv { get; set; }
        public int? IncomeBalanceID { get; set; }
        public decimal added_money { get; set; }
        public int MyProperty { get; set; }
        public string cause { get; set; }
        public int? faw_BillerId { get; set; }
        public int? faw_BillTypeCode { get; set; }
        public string faw_FPTN { get; set; }
        public string faw_FCRN { get; set; }
        public decimal faw_paid { get; set; }
        public decimal faw_fees { get; set; }
        public string faw_status_code { get; set; }
        public string faw_status_message { get; set; }
        public bool Is_fawry { get; set; }
        public decimal Basci_val { get; set; }
        public string Provider_Response { get; set; }
        public string account_number { get; set; }
        public int service_id { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public int ProviderTransactionId { get; set; }
        public string data { get; set; }
        public string tel_code { get; set; }
        public string tel_number { get; set; }
        public bool is_retry { get; set; }
        public DateTime Payed_time { get; set; }
        public string ProviderCardTransactionId { get; set; }
        public string ECardmomknPaymentId { get; set; }
        public decimal InCome_cash { get; set; }
        public virtual ICollection<InvoiceDetails> InvoiceDetails { get; set; }
    }
}
