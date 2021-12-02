using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Data;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly IBaseRepository<Log, int> _logRepository;
        private IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _services;
        public LoggingService(IBaseRepository<Log, int> logRepository, IUnitOfWork unitOfWork,
            IServiceProvider services)
        {
            _logRepository = logRepository;
            _unitOfWork = unitOfWork;
            _services = services;
        }
        public async Task Log(string obj, int providerServiceRequestId, LoggingType loggingType)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Logs.Add(new Log
            {
                LogTypeID = loggingType,
                ProviderServiceRequestID = providerServiceRequestId,
                LogText = obj
            });
            context.SaveChanges();
            //_logRepository.Add(new Data.Entities.Log
            //{
            //    LogTypeID = loggingType,
            //    ProviderServiceRequestID = providerServiceRequestId,
            //    LogText = obj
            //});
            //await _unitOfWork.SaveChangesAsync();
        }

        public string GetLog(int providerServiceRequestId, LoggingType loggingType)
        {
            return _logRepository.Getwhere(x => x.ProviderServiceRequestID == providerServiceRequestId && x.LogTypeID == loggingType)
                .Select(x => x.LogText).FirstOrDefault();
        }
    }
}
