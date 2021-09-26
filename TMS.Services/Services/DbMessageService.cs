﻿using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Infrastructure;
using TMS.Infrastructure.Helpers;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class DbMessageService : IDbMessageService
    {
        //private readonly IBaseRepository<ProviderStatusCode, int> _providerStatusCodes;
        private readonly IBaseRepository<StatusCode, int> _statusCodes;
        public DbMessageService(
            //IBaseRepository<ProviderStatusCode, int> providerStatusCodes,
            IBaseRepository<StatusCode, int> statusCodes)
        {
            //_providerStatusCodes = providerStatusCodes;
            _statusCodes = statusCodes;
        }

        public StatusCodeDTO GetMainStatusCodeMessage(int? id = null, int? providerId = null, string statusCode = "", string language = "ar")
        {
            StatusCodeDTO response;
            if (!providerId.HasValue)
            {
                response = _statusCodes.Getwhere(s => s.ID == (int)id).Select(s => new StatusCodeDTO
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
                throw new TMSException("ProviderError", "-15");
            return response;
        }
    }
}
