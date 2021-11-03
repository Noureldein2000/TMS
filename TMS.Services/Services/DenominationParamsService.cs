using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using TMS.Data.Entities;
using TMS.Services.Models;
using TMS.Services.Repositories;

namespace TMS.Services.Services
{
    public class DenominationParamService : IDenominationParamService
    {
        private readonly IBaseRepository<DenominationParam, int> _serviceDenominationParam;
        private readonly IUnitOfWork _unitOfWork;

        public DenominationParamService(
            IBaseRepository<DenominationParam, int> serviceDenominationParam,
            IUnitOfWork unitOfWork
            )
        {
            _serviceDenominationParam = serviceDenominationParam;
            _unitOfWork = unitOfWork;
        }

        public void AddParam(DenominationParamDTO model)
        {
            _serviceDenominationParam.Add(new DenominationParam
            {
                ID = model.Id,
                Label = model.Label,
                Title = model.Title,
                ParamKey = model.ParamKey,
                ValueModeID = model.ValueModeID,
                ValueTypeID = model.ValueTypeID
            });

            _unitOfWork.SaveChanges();
        }

        public void DeleteParam(int id)
        {
            _serviceDenominationParam.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void EditParam(DenominationParamDTO model)
        {
            var current = _serviceDenominationParam.GetById(model.Id);

            current.Label = model.Label;
            current.Title = model.Title;
            current.ParamKey = model.ParamKey;
            current.ValueModeID = model.ValueModeID;
            current.ValueTypeID = model.ValueTypeID;

            _unitOfWork.SaveChanges();
        }

        public DenominationParamDTO GetParamById(int id)
        {
            return _serviceDenominationParam.Getwhere(x => x.ID == id).Select(model => new DenominationParamDTO
            {
                Id = model.ID,
                Label = model.Label,
                Title = model.Title,
                ParamKey = model.ParamKey,
                ValueModeID = model.ValueModeID,
                ValueTypeID = model.ValueTypeID
            }).FirstOrDefault();
        }

        public PagedResult<DenominationParamDTO> GetParams(int page, int pageSize, string language)
        {
            var parameters = _serviceDenominationParam.Getwhere(x => true).Select(x => new
            {
                x.ID,
                x.Label,
                x.Title,
                x.ParamKey,
                x.ValueModeID,
                ValueModeName = x.DenominationParamValueMode.Name,
                x.ValueTypeID,
                ValueTypeName = x.DenominationParamValueType.Name,
                x.CreationDate
            });

            var count = parameters.Count();

            var resultList = parameters.OrderByDescending(ar => ar.CreationDate)
                            .Skip(page - 1).Take(pageSize)
                            .ToList();

            return new PagedResult<DenominationParamDTO>
            {
                Results = resultList.Select(x => new DenominationParamDTO
                {
                    Id = x.ID,
                    Label = x.Label,
                    Title = x.Title,
                    ParamKey = x.ParamKey,
                    ValueModeID = x.ValueModeID,
                    ValueModeName = x.ValueModeName,
                    ValueTypeID = x.ValueTypeID,
                    ValueTypeName = x.ValueTypeName,
                }).ToList(),
                PageCount = count
            };
        }
    }
}
