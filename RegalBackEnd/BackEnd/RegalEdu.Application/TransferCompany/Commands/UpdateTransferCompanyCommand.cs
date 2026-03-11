using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RegalEdu.Application.TransferCompany.Commands
{
    public class UpdateTransferCompanyCommand : IRequest<Result>
    {
        public required TransferCompanyModel TransferCompanyModel { get; set; }
    }

    public class UpdateTransferCompanyCommandHandler
        : IRequestHandler<UpdateTransferCompanyCommand, Result>
    {
        private readonly IRegalEducationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILocalizationService _localizer;

        public UpdateTransferCompanyCommandHandler(
            IRegalEducationDbContext context,
            IMapper mapper,
            ILocalizationService localizer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public async Task<Result> Handle(
            UpdateTransferCompanyCommand request,
            CancellationToken cancellationToken)
        {
            // ======================================================
            // 1. Lấy phiếu chuyển
            // ======================================================
            var transferCompany = await _context.TransferCompanies
                .FirstOrDefaultAsync(
                    x => x.Id == request.TransferCompanyModel.Id && !x.IsDeleted,
                    cancellationToken);

            if (transferCompany == null)
            {
                return Result.Failure(
                    _localizer.Format(
                        LocalizationKey.EntityNotFound,
                        EntityName.TransferCompany));
            }

            // ======================================================
            // 2. KHÔNG cho sửa phiếu đã kết thúc
            // ======================================================
            if (transferCompany.TransferCompanyStatus == TransferCompanyStatus.Completed ||
                transferCompany.TransferCompanyStatus == TransferCompanyStatus.Rejected ||
                transferCompany.TransferCompanyStatus == TransferCompanyStatus.ParentRejected)
            {
                return Result.Failure(
                    _localizer["TransferCompanyCannotBeUpdatedInFinalState"]);
            }

            // ======================================================
            // 3. Map CÓ KIỂM SOÁT (chỉ field cho phép sửa)
            // ======================================================
            transferCompany.Reason = request.TransferCompanyModel.Reason;
            transferCompany.DestinationCompanyId = request.TransferCompanyModel.DestinationCompanyId;
            transferCompany.TransferDate = request.TransferCompanyModel.TransferDate;
            transferCompany.TransferCompanyStatus =
                request.TransferCompanyModel.TransferCompanyStatus;

            // ======================================================
            // 4. Lưu DB
            // ======================================================
            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                return Result.Success(
                    _localizer.Format(
                        LocalizationKey.MSG_UPDATE_SUCCESS,
                        EntityName.TransferCompany));
            }

            return Result.Failure(
                _localizer.Format(
                    LocalizationKey.ERR_SAVE_NO_EFFECT,
                    EntityName.TransferCompany));
        }
    }
}
