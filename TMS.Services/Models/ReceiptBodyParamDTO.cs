using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class ReceiptBodyParamDTO
    {
        public int ProviderServiceRequestID { get; set; }
        public string ParameterName { get; set; }
        public string Value { get; set; }
        public int? TransactionID { get; set; }
    }
    public class Title
    {
        public string serviceName { get; set; }
    }

    public class Datum
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Alignment = "";
        public string Bold = "";
    }

    public class Header
    {
        public List<Datum> data { get; set; }
    }

    public class Body
    {
        public List<Datum> data { get; set; }
    }
    public class Root
    {
        public Title title { get; set; }
        public Header header { get; set; }
        public Body body { get; set; }
        public string disclaimer { get; set; }
        public string footer { get; set; }
    }
}
