using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class UniversityDTO
    {

    }

    class InquiryUniversity
    {
        public string BillingAcccount { get; set; }
        public string TransactionId { get; set; }
        public string BillerId { get; set; }
    }
    class FeesUniversity
    {
        public string TransactionId { get; set; }
        public string BillerId { get; set; }
        public string BillingAccount { get; set; }
        public string BillRecId { get; set; }
        public List<PaymentAmounts> PaymentAmounts { get; set; }
    }

    class PaymentUniversity
    {
        public string BillingAcccount { get; set; }
        public string TransactionId { get; set; }
        public string BillerId { get; set; }
        public string BillRecId { get; set; }
        public string PaymentRefInfo { get; set; }
        public string BillNumber { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
        public List<PaymentAmounts> PaymentAmounts { get; set; }
    }

    public class CustomerInformation
    {
        public string CollageName { get; set; }
        public string ArabicName { get; set; }
        public string EducationYear { get; set; }
        public string EnglishName { get; set; }

        public List<Data> GetData()
        {
            Data d = new Models.Data();

            List<Data> DList = new List<Data>();
            d.Key = "collageName";
            d.Value = CollageName;
            DList.Add(d);

            d = new Data();
            d.Key = "arabicName";
            d.Value = ArabicName;
            DList.Add(d);

            d = new Data();
            d.Key = "educationYear";
            d.Value = EducationYear;
            DList.Add(d);

            d = new Data();
            d.Key = "englishName";
            d.Value = EnglishName;
            DList.Add(d);

            return DList;
        }
    }
}
