using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class WaterBillDTO
    {
    }

    public class InquiryWaterDTO
    {
        public string BillingAcccount { get; set; }
        public string TransactionId { get; set; }
        public string BillType = "0";
        public string BillerId { get; set; }
        public string MeterReading { get; set; }

    }

    class FeesWaterDTO
    {
        public string TransactionId { get; set; }

        public string BillerId { get; set; }
        public string BillingAccount { get; set; }
        public string BillRecId { get; set; }
        public List<PaymentAmounts> PaymentAmounts { get; set; }

    }

    class PaymentWater : FeesWaterDTO
    {
        public string PaymentRefInfo { get; set; }
        public string BillNumber { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
    }


    public class FeesAmountDTO
    {
        public string Amount { get; set; }
        public string CurrentCode { get; set; }
    }

    public class PaymentAmountDTO
    {
        public string Amount { get; set; }
        public string CurrentCode { get; set; }
        public string MinAmount { get; set; }
        public string PaymentMode { get; set; }
        public string Sequence { get; set; }

        public string shortDescAR { get; set; }
        public string shortDescEN { get; set; }
    }
    public class CustomerInfoSubscriptionChannelsDTO
    {
        public string Address { get; set; }
        public string CovergeEndDate { get; set; }
        public string CustmerName { get; set; }
        public string ProgramName { get; set; }
        public string RemainingLateDuesAfterPay { get; set; }

        public List<DataDTO> GetData()
        {
            DataDTO d = new Models.DataDTO();

            List<DataDTO> DList = new List<Models.DataDTO>();
            d.Key = "address";
            d.Value = Address;
            DList.Add(d);

            d = new Models.DataDTO();
            d.Key = "covergeEndDate";
            d.Value = CovergeEndDate;
            DList.Add(d);

            d = new Models.DataDTO();
            d.Key = "custmerName";
            d.Value = CustmerName;
            DList.Add(d);

            d = new Models.DataDTO();
            d.Key = "programName";
            d.Value = ProgramName;
            DList.Add(d);

            d = new Models.DataDTO();
            d.Key = "remainingLateDuesAfterPay";
            d.Value = RemainingLateDuesAfterPay;
            DList.Add(d);

            return DList;
        }

    }
}
