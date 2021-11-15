using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class AccountTypeProfileCommissionService : IAccountTypeProfileCommissionService
    {
        private readonly IBaseRepository<Commission, int> _commissionRepository;
        private readonly IBaseRepository<AccountTypeProfileCommission, int> _accountTypeProfileCommission;
        private readonly IBaseRepository<AccountTypeProfileDenomination, int> _accountTypeProfileDenomination;
        //private readonly IStringLocalizer<LanguageResource> _localizer;
        private readonly IUnitOfWork _unitOfWork;

        public AccountTypeProfileCommissionService(
        IBaseRepository<Commission, int> commissionRepository,
        IBaseRepository<AccountTypeProfileCommission, int> accountTypeProfileCommission,
        IBaseRepository<AccountTypeProfileDenomination, int> accountTypeProfileDenomination,
        IUnitOfWork unitOfWork
            )
        {
            _commissionRepository = commissionRepository;
            _accountTypeProfileCommission = accountTypeProfileCommission;
            _accountTypeProfileDenomination = accountTypeProfileDenomination;
            _unitOfWork = unitOfWork;
        }

        public void Add(AccountTypeProfileCommissionDTO model)
        {
            var checkExists = _accountTypeProfileCommission.Getwhere(atpf => atpf.CommissionID == model.CommissionID && atpf.AccountTypeProfileDenominationID == model.AccountTypeProfileDenominationID).Any();
            if (checkExists) throw new Exception("This is already exists before");

            _accountTypeProfileCommission.Add(new AccountTypeProfileCommission
            {
                CommissionID = model.CommissionID,
                AccountTypeProfileDenominationID = model.AccountTypeProfileDenominationID
            });

            _unitOfWork.SaveChanges();
        }

        public void Delete(int id)
        {
            _accountTypeProfileCommission.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public PagedResult<AccountTypeProfileCommissionDTO> GetAccountTypeProfileCommissions(int id, int page, int pageSize, string language)
        {
            var accountTypeProfileCommission = _accountTypeProfileCommission.Getwhere(x => x.AccountTypeProfileDenominationID == id).Select(atpf => new
            {
                ID = atpf.ID,
                CommissionID = atpf.CommissionID,
                AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID,
                Value = atpf.Commission.Value,
                CommissionTypeName = language == "en" ? atpf.Commission.CommissionType.Name : atpf.Commission.CommissionType.ArName,
                PaymentModeName = language == "en" ? atpf.Commission.PaymentMode.Name : atpf.Commission.PaymentMode.ArName,
                AmountFrom = atpf.Commission.AmountFrom,
                AmountTo = atpf.Commission.AmountTo,
                DenominationName = atpf.AccountTypeProfileDenomination.Denomination.Name,
                ServiceName = atpf.AccountTypeProfileDenomination.Denomination.Service.Name,
                CreationDate = atpf.CreationDate
            });

            var count = accountTypeProfileCommission.Count();

            var resultList = accountTypeProfileCommission.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<AccountTypeProfileCommissionDTO>
            {
                Results = resultList.Select(atpf => new AccountTypeProfileCommissionDTO
                {
                    Id = atpf.ID,
                    CommissionValue = atpf.Value,
                    CommissionTypeName = atpf.CommissionTypeName,
                    PaymentModeName = atpf.PaymentModeName,
                    AmountFrom = atpf.AmountFrom,
                    AmountTo = atpf.AmountTo,
                    DenomintionName = atpf.DenominationName,
                    ServiceName = atpf.ServiceName,
                    CommissionID = atpf.CommissionID,
                    AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
                }).ToList(),
                PageCount = count
            };
        }
    }
}
