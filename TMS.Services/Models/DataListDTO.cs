using System;
using System.Collections.Generic;
using System.Text;

namespace TMS.Services.Models
{
    public class DataListDTO
    {
        public DataListDTO()
        {
            Data = new List<DataDTO>();
        }
        public List<DataDTO> Data { get; set; }
        public string Key = "";
        public string Value = "";
    }


}
