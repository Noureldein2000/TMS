using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ElectricityDTO
    {

    }

    public class InquiryElectricity
    {
        public string BillingAcccount { get; set; }
        public string TransactionId { get; set; }
        public string BillerId { get; set; }
    }

    public class InquiryElectricityCard : InquiryElectricity
    {
        public string CardType { get; set; }
        public string CardData { get; set; }
    }

    public class FeesElectricityCard
    {
        public string Amount { get; set; }
        public string BillRecId { get; set; }
        public string TransactionId { get; set; }
    }

    public class FeesElectricity : InquiryElectricity
    {
        public string BillRecId { get; set; }
        public List<PaymentAmounts> PaymentAmounts { get; set; }

    }

   public class PaymentElectricity 
    {
        public string BillingAcccount { get; set; }
        public string TransactionId { get; set; }
        public string BillerId { get; set; }
        public string PaymentRefInfo { get; set; }
        public string BillNumber { get; set; }
        public string BillRecId { get; set; }
        public string CardType { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
        public List<PaymentCardAmounts> PaymentAmounts { get; set; }
    }

    class PaymentElectricityBill : InquiryElectricity
    {
        public string PaymentRefInfo { get; set; }
        public string BillNumber { get; set; }
        public string BillRecId { get; set; }
        public List<FeesAmounts> FeesAmounts { get; set; }
        public List<PaymentAmounts> PaymentAmounts { get; set; }

    }

    public class CustomerInfoElectricityCard
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string MinAmount { get; set; }
        public string TotalDeducts { get; set; }
        public string PrepaidAmount { get; set; }
    }
    public class CustomerInfo
    {
        public string Address { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public string CustomerCode { get; set; }
        public List<Data> GetData()
        {
            Data d = new Data();

            List<Data> DList = new List<Data>();
            d.Key = "address";
            d.Value = Address;
            DList.Add(d);

            d = new Data();
            d.Key = "arabicName";
            d.Value = ArabicName;
            DList.Add(d);

            d = new Data();
            d.Key = "englishName";
            d.Value = EnglishName;
            DList.Add(d);

            d = new Data();
            d.Key = "customerCode";
            d.Value = CustomerCode;
            DList.Add(d);

            return DList;
        }
    }

    public class Data
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public static Data SetData(string _key, string _value)
        {
            Data d = new Data();
            d.Key = _key;
            d.Value = _value;
            return d;
        }
    }

    public class FeesAmounts
    {
        public string Amount { get; set; }
        public string CurrentCode { get; set; }
    }

    public class PaymentAmounts
    {
        public string Amount { get; set; }
        public string CurrentCode { get; set; }
        public string MinAmount { get; set; }
        public string PaymentMode { get; set; }
        public string Sequence { get; set; }
        public string ShortDescAR { get; set; }
        public string ShortDescEN { get; set; }
    }

    public class PaymentCardAmounts
    {
        public string Amount { get; set; }
        public string CurrentCode { get; set; }
        public string Sequence { get; set; }
    }
}
