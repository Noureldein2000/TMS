using System;
using System.Collections.Generic;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;

namespace TMS.Services.Services
{
    public interface IDbMessageService
    {
        StatusCodeDTO GetMainStatusCodeMessage(int? id = null, int? providerId = null, string statusCode = "", string language = "en");
    }
}
