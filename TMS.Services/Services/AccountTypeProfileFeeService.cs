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
    public class AccountTypeProfileFeeService : IAccountTypeProfileFeeService
    {
        private readonly IBaseRepository<Fee, int> _feeRepository;
        private readonly IBaseRepository<AccountTypeProfileFee, int> _accountTypeProfileFee;
        private readonly IBaseRepository<AccountTypeProfileDenomination, int> _accountTypeProfileDenomination;
        //private readonly IStringLocalizer<LanguageResource> _localizer;
        private readonly IUnitOfWork _unitOfWork;

        public AccountTypeProfileFeeService(
        IBaseRepository<Fee, int> feeRepository,
        IBaseRepository<AccountTypeProfileFee, int> accountTypeProfileFee,
        IBaseRepository<AccountTypeProfileDenomination, int> accountTypeProfileDenomination,
        IUnitOfWork unitOfWork
            )
        {
            _feeRepository = feeRepository;
            _accountTypeProfileFee = accountTypeProfileFee;
            _accountTypeProfileDenomination = accountTypeProfileDenomination;
            _unitOfWork = unitOfWork;
        }

        public AccountTypeProfileFeesDTO Add(AccountTypeProfileFeesDTO model)
        {
            var checkExists = _accountTypeProfileFee.Getwhere(atpf => atpf.FeesID == model.FeesID && atpf.AccountTypeProfileDenominationID == model.AccountTypeProfileDenominationID).Any();
            if (checkExists) throw new TMSException("This is already exists before","-5");

            var addedEntity = _accountTypeProfileFee.Add(new AccountTypeProfileFee
            {
                FeesID = model.FeesID,
                AccountTypeProfileDenominationID = model.AccountTypeProfileDenominationID
            });

            _unitOfWork.SaveChanges();

            return _accountTypeProfileFee.Getwhere(x => x.ID == addedEntity.ID).Select(atpf => new AccountTypeProfileFeesDTO
            {
                Id = atpf.ID,
                FeesValue = atpf.Fee.Value,
                FeesTypeName = atpf.Fee.FeesType.Name,
                PaymentModeName = atpf.Fee.PaymentMode.Name,
                AmountFrom = atpf.Fee.AmountFrom,
                AmountTo = atpf.Fee.AmountTo,
                DenomintionName = atpf.AccountTypeProfileDenomination.Denomination.Name,
                ServiceName = atpf.AccountTypeProfileDenomination.Denomination.Service.Name,
                FeesID = atpf.FeesID,
                AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
            }).FirstOrDefault();
        }

        public void Delete(int id)
        {
            _accountTypeProfileFee.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public PagedResult<AccountTypeProfileFeesDTO> GetAccountTypeProfileFees(int id, int page, int pageSize, string language)
        {
            var accountTypeProfileFee = _accountTypeProfileFee.Getwhere(x => x.AccountTypeProfileDenominationID == id).Select(atpf => new
            {
                atpf.ID,
                FeeID = atpf.FeesID,
                atpf.AccountTypeProfileDenominationID,
                atpf.Fee.Value,
                FeeTypeName = language == "en" ? atpf.Fee.FeesType.Name : atpf.Fee.FeesType.ArName,
                PaymentModeName = language == "en" ? atpf.Fee.PaymentMode.Name : atpf.Fee.PaymentMode.ArName,
                atpf.Fee.AmountFrom,
                atpf.Fee.AmountTo,
                atpf.CreationDate
            });

            var count = accountTypeProfileFee.Count();

            var resultList = accountTypeProfileFee.OrderBy(x => x.AmountFrom)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<AccountTypeProfileFeesDTO>
            {
                Results = resultList.Select(atpf => new AccountTypeProfileFeesDTO
                {
                    Id = atpf.ID,
                    FeesValue = atpf.Value,
                    FeesTypeName = atpf.FeeTypeName,
                    PaymentModeName = atpf.PaymentModeName,
                    AmountFrom = atpf.AmountFrom,
                    AmountTo = atpf.AmountTo,
                    FeesID = atpf.FeeID,
                    AccountTypeProfileDenominationID = atpf.AccountTypeProfileDenominationID
                }).ToList(),
                PageCount = count
            };
        }
    }
}
