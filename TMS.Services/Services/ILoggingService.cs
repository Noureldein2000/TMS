using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMS.Infrastructure;

namespace TMS.Services.Services
{
    public interface ILoggingService
    {
        Task Log(string obj, int providerServiceRequestId, LoggingType loggingType);
        string GetLog(int providerServiceRequestId, LoggingType loggingType);
    }
}
