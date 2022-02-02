using Microsoft.EntityFrameworkCore;
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
    public class AccountTypeProfileDenominationService : IAccountTypeProfileDenominationService
    {
        private readonly IBaseRepository<AccountTypeProfileDenomination, int> _accountTypeProfileDenomination;
        private readonly IUnitOfWork _unitOfWork;

        public AccountTypeProfileDenominationService(
            IBaseRepository<AccountTypeProfileDenomination, int> accountTypeProfileDenomination,
            IUnitOfWork unitOfWork
            )
        {
            _accountTypeProfileDenomination = accountTypeProfileDenomination;
            _unitOfWork = unitOfWork;
        }
        public void Add(AccountTypeProfileDenominationDTO model)
        {
            _accountTypeProfileDenomination.Add(new AccountTypeProfileDenomination
            {
                AccountTypeProfileID = model.AccountTypeProfileID,
                DenominationID = model.DenominationID,
            });

            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(int id)
        {
            var current = _accountTypeProfileDenomination.GetById(id);
            current.Status = !current.Status;
            _unitOfWork.SaveChanges();
        }

        public void Delete(int id)
        {
            _accountTypeProfileDenomination.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public PagedResult<AccountTypeProfileDenominationDTO> GetAccountTypeProfileDenominations(int page, int pageSize)
        {
            var accountTypeProfileDenomination = _accountTypeProfileDenomination.Getwhere(x => true).Select(x => new
            {
                Id = x.ID,
                DenominationId = x.DenominationID,
                DenominationName = x.Denomination.Name,
                AccountTypeProfileID = x.AccountTypeProfileID,
                Status = x.Status,
                CreationDate = x.CreationDate
            });

            var count = accountTypeProfileDenomination.Count();

            var resultList = accountTypeProfileDenomination.OrderByDescending(ar => ar.CreationDate)
          .Skip(page - 1).Take(pageSize)
          .ToList();

            return new PagedResult<AccountTypeProfileDenominationDTO>
            {
                Results = resultList.Select(x => new AccountTypeProfileDenominationDTO
                {
                    Id = x.Id,
                    DenominationID = x.DenominationId,
                    DenominationName = x.DenominationName,
                    AccountTypeProfileID = x.AccountTypeProfileID,
                    Status = x.Status
                }).ToList(),
                PageCount = count
            };
        }
    }
}
