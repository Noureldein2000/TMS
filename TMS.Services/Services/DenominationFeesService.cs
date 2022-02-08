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
    public class DenominationFeesService : IDenominationFeesService
    {
        private readonly IBaseRepository<Denomination, int> _denominationRepository;
        private readonly IBaseRepository<DenominationFee, int> _denominationFeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DenominationFeesService(
            IBaseRepository<Denomination, int> denominationRepository,
            IBaseRepository<DenominationFee, int> denominationFeeRepository,
        IUnitOfWork unitOfWork
            )
        {
            _denominationRepository = denominationRepository;
            _denominationFeeRepository = denominationFeeRepository;
            _unitOfWork = unitOfWork;
        }
        public void AddDenominationFees(AddDenominationFeesDTO model)
        {
            if (_denominationFeeRepository.Any(x => x.DenominationID == model.DenominationId && x.FeesID == model.FeesId))
            {
                throw new TMSException("Denomination-Fees already exist", "-5");
            }

            _denominationFeeRepository.Add(new DenominationFee
            {
                DenominationID = model.DenominationId,
                FeesID = model.FeesId
            });
            _unitOfWork.SaveChanges();
        }

        public void DeleteDenominationFees(int id)
        {
            _denominationFeeRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<DenominationFeesDTO> GetDeniminationFeesByDenominationId(int denominationId, string language)
        {
            return _denominationFeeRepository.Getwhere(x => x.DenominationID == denominationId).Select(x => new DenominationFeesDTO
            {
                Id = x.ID,
                FeesId = x.FeesID,
                FeesTypeId = x.Fee.FeesTypeID,
                FeesTypeName = language == "en" ? x.Fee.FeesType.Name : x.Fee.FeesType.ArName,
                FeesValue = x.Fee.Value,
                PaymentModeId = x.Fee.PaymentModeID,
                PaymentMode = language == "en" ? x.Fee.PaymentMode.Name : x.Fee.PaymentMode.ArName,
                DenominationId = x.DenominationID,
                DenominationFullName = $"{x.Denomination.Service.Name} - {x.Denomination.Name}",
                Range = $"{x.Fee.AmountFrom} - { x.Fee.AmountTo}",
                CreationDate = x.CreationDate
            }).ToList();
        }
    }
}
