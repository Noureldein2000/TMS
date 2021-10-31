using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Models.SwaggerModels;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly IBaseRepository<Commission, int> _commission;
        private readonly IBaseRepository<AccountCommission, int> _accountCommission;
        private readonly IBaseRepository<AccountProfileCommission, int> _accountProfileCommission;
        private readonly IBaseRepository<DenominationCommission, int> _denominationCommission;
        private readonly IUnitOfWork _unitOfWork;

        public CommissionService(
            IBaseRepository<Commission, int> commission,
            IBaseRepository<AccountCommission, int> accountCommission,
            IBaseRepository<AccountProfileCommission, int> accountProfileCommission,
            IBaseRepository<DenominationCommission, int> denominationCommission,
            IUnitOfWork unitOfWork)
        {
            _commission = commission;
            _accountCommission = accountCommission;
            _accountProfileCommission = accountProfileCommission;
            _denominationCommission = denominationCommission;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CommissionDTO> GetAccountCommission(int denominationId, decimal originalAmount, int accountId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetAccountProfileCommission(int denominationId, decimal originalAmount, int accountProfileId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetCommission(int denominationId, decimal originalAmount, int accountId, int accountProfileId, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CommissionDTO> GetCommission()
        {
            return _commission.Getwhere(x => true).Include(x => x.CommissionType).Select(com => new CommissionDTO()
            {
                ID = com.ID,
                CommissionTypeID = com.CommissionTypeID,
                CommissionTypeName = com.CommissionType.ArName,
                Value = com.Value,
                CommissionRange = com.Value + " [" + com.AmountFrom.ToString() + " - " + com.AmountTo + "] " + com.PaymentMode.Name,
                PaymentModeID = com.PaymentModeID,
                Status = com.Status,
                AmountFrom = com.AmountFrom,
                AmountTo = com.AmountTo
            }).ToList();
        }

        public IEnumerable<CommissionDTO> GetDenominationCommission(int denominationId, decimal originalAmount, out decimal sum, string language = "ar")
        {
            throw new NotImplementedException();
        }
    }
}
