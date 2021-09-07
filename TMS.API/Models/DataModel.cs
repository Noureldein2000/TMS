using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TMS.API.Models
{
    public class DataModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public static DataModel SetData(string _key, string _value)
        {
            var d = new DataModel();
            d.Key = _key;
            d.Value = _value;
            return d;
        }
    }
}
