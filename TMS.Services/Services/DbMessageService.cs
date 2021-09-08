using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class DbMessageService : IDbMessageService
    {
        //private readonly IBaseRepository<ProviderStatusCode, int> _providerStatusCodes;
        private readonly IBaseRepository<StatusCode, int> _statusCodes;
        private readonly IStringLocalizer<ServiceLanguageResource> _localizer;
        public DbMessageService(
            //IBaseRepository<ProviderStatusCode, int> providerStatusCodes,
            IStringLocalizer<ServiceLanguageResource> localizer,
            IBaseRepository<StatusCode, int> statusCodes)
        {
            //_providerStatusCodes = providerStatusCodes;
            _statusCodes = statusCodes;
            _localizer = localizer;
        }

        public StatusCodeDTO GetMainStatusCodeMessage(int? id = null, int? providerId = null, string statusCode = "", string language = "ar")
        {
            StatusCodeDTO response;
            if (!providerId.HasValue)
            {
                response = _statusCodes.Getwhere(s => s.ID == id).Select(s => new StatusCodeDTO
                {
                    Id = s.ID,
                    Code = s.Code,
                    Message = language == "ar" ? s.ArMessage : s.Message,
                    MainStatusId = s.MainStatusCodeID
                }).FirstOrDefault();
            }
            else
            {
                response = _statusCodes.Getwhere(s => s.ProviderStatusCodes.Any(p => p.ID == providerId && p.StatusCode == statusCode))
                    .Select(s => new StatusCodeDTO
                    {
                        Id = s.ID,
                        Code = s.Code,
                        Message = language == "ar" ? s.ArMessage : s.Message,
                        MainStatusId = s.MainStatusCodeID
                    }).FirstOrDefault();
            }
            if (response == null)
                throw new TMSException(_localizer["ProviderError"].Value, "-15");
            return response;
        }
    }
}
