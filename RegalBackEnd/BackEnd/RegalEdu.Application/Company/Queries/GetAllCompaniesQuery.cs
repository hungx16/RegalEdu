using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Models;
using RegalEdu.Domain.Models.DTO;

namespace RegalEdu.Application.Company.Queries
{
    public class GetAllCompaniesQuery : IRequest<Result<List<CompanyModel>>>
    {
        public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, Result<List<CompanyModel>>>
        {
            private readonly IRegalEducationDbContext _context;
            private readonly IMapper _mapper;

            public GetAllCompaniesQueryHandler(IRegalEducationDbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException (nameof (context));
                _mapper = mapper ?? throw new ArgumentNullException (nameof (mapper));
            }

            public async Task<Result<List<CompanyModel>>> Handle(GetAllCompaniesQuery request, CancellationToken ct)
            {
                var list = await _context.Companies
                    .AsNoTracking ( )
                    .AsSplitQuery ( ) // Nên dùng vì có nhiều collection con
                    .Select (c => new CompanyModel
                    {
                        Id = c.Id,
                        CompanyName = c.CompanyName,
                        CompanyCode = c.CompanyCode,
                        Status = c.Status,
                        EstablishmentDate = c.EstablishmentDate,
                        CompanyPhone = c.CompanyPhone,
                        ProvinceCode = c.ProvinceCode,
                        CompanyAddress = c.CompanyAddress,
                        CreatedBy = c.CreatedBy,
                        WardCode = c.WardCode,
                        ManagerId = c.ManagerId,
                        IsMultilingual = c.IsMultilingual,
                        IsPublish = c.IsPublish,
                        EnCompanyAddress = c.EnCompanyAddress,
                        EnConvenience = c.EnConvenience,
                        EnLeaningRoadMapTags = c.EnLeaningRoadMapTags,
                        EnCompanyName = c.EnCompanyName,
                        Convenience = c.Convenience,
                        CompanyEmail = c.CompanyEmail,
                        EnDescription = c.EnDescription,
                        IsHeadQuarters = c.IsHeadQuarters,
                        LeaningRoadMapTags = c.LeaningRoadMapTags,
                        VotingRate = c.VotingRate,
                        NumberOfStudents = c.NumberOfStudents,
                        WorkingTime = c.WorkingTime,
                        Description = c.Description,
                        EnWorkingTime = c.EnWorkingTime,
                        Longitude = c.Longitude,
                        Latitude = c.Latitude,
                        Manager = c.Manager == null ? null : new EmployeeDto
                        {
                            Id = c.Manager.Id,
                            ApplicationUser = c.Manager.ApplicationUser == null ? null : new ApplicationUserModel
                            {
                                Id = c.Manager.ApplicationUser.Id,
                                FullName = c.Manager.ApplicationUser.FullName,
                                UserCode = c.Manager.ApplicationUser.UserCode
                            }
                        },

                        // Mapping vùng (active)
                        LogRegionComs = c.LogRegionComs
                            //.Where (m => !m.IsDeleted && m.Status == 0)
                            .OrderByDescending (m => m.CreatedAt)
                            .Select (m => new LogRegionComDto
                            {
                                Id = m.Id,
                                CompanyId = m.CompanyId,
                                RegionId = m.RegionId,
                                Status = m.Status,
                                CreatedAt = m.CreatedAt,
                                Region = m.Region == null ? null : new RegionDto
                                {
                                    Id = m.Region.Id,
                                    RegionName = m.Region.RegionName,
                                    RegionCode = m.Region.RegionCode,
                                }
                            }).ToList ( ),
                        // Mapping vùng (active)
                        CompanyLearningRoadMaps = c.CompanyLearningRoadMaps
                            //.Where (m => !m.IsDeleted && m.Status == 0)
                            .OrderByDescending (m => m.CreatedAt)
                            .Select (m => new CompanyLearningRoadMapModel
                            {
                                Id = m.Id,
                                CompanyId = m.CompanyId,
                                LearningRoadMapId = m.LearningRoadMapId,
                                Status = m.Status,
                                CreatedAt = m.CreatedAt,
                                LearningRoadMap = m.LearningRoadMap == null ? null : new LearningRoadMapDto
                                {
                                    Id = m.LearningRoadMap.Id,
                                    LearningRoadMapName = m.LearningRoadMap.LearningRoadMapName,
                                    LearningRoadMapCode = m.LearningRoadMap.LearningRoadMapCode,
                                }
                            }).ToList ( ),
                        // Ảnh (metadata)
                        CompanyImages = c.CompanyImages
                            .OrderByDescending (i => i.CreatedAt)
                            .Select (i => new ImageDto
                            {
                                Id = i.Id,
                                CompanyId = i.CompanyId,
                                Path = i.Path,
                                Caption = i.Caption,
                                IsCover = i.IsCover,
                                SortOrder = i.SortOrder
                            }).ToList ( ),

                        // === NEW: Employees + ApplicationUser (chỉ lấy field cần thiết) ===
                        Employees = c.Employees
                            .Where (e => !e.IsDeleted)                 // tuỳ bạn có cờ IsDeleted/Status
                            .OrderBy (e => e.CreatedAt)
                            .Select (e => new EmployeeDto
                            {
                                Id = e.Id,
                                CompanyId = e.CompanyId,
                                Status = e.Status,
                                // các mốc thời gian dùng trên FE để tính active:
                                EmployeeStartedDate = e.EmployeeStartedDate,
                                EmployeeEndDate = e.EmployeeEndDate,
                                EmployeeNewEndDate = e.EmployeeNewEndDate,

                                // Position (để biết isSale, isSaleLead…)
                                Position = e.Position == null ? null : new PositionDto
                                {
                                    Id = e.Position.Id,
                                    PositionName = e.Position.PositionName,
                                    PositionCode = e.Position.PositionCode,
                                    IsSale = e.Position.IsSale,
                                    IsSaleLead = e.Position.IsSaleLead
                                },

                                // AppUser (để hiện FullName/UserCode)
                                ApplicationUser = e.ApplicationUser == null ? null : new ApplicationUserModel
                                {
                                    Id = e.ApplicationUser.Id,
                                    FullName = e.ApplicationUser.FullName,
                                    UserCode = e.ApplicationUser.UserCode
                                }
                            }).ToList ( )
                    })
                    .OrderBy (c => c.CompanyName)
                    .ToListAsync (ct);

                return Result<List<CompanyModel>>.Success (list);
            }
        }
    }
}
