using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;
using RegalEdu.Domain.Entities;
using RegalEdu.Domain.Enumerations;
using RegalEdu.Shared;
using System.Linq.Expressions;

public class SoftDeleteService : ISoftDeleteService
{
    private readonly IRegalEducationDbContext _dbContext;
    private readonly ILogger<SoftDeleteService> _logger;
    private readonly ILocalizationService _localizer;
    private readonly IFileService _fileService;

    public SoftDeleteService(IRegalEducationDbContext dbContext,
        ILogger<SoftDeleteService> logger, ILocalizationService localizer, IFileService fileService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _localizer = localizer;
        _fileService = fileService;
    }
    private async Task RecursiveSoftDeleteInternal(object entity, IRegalEducationDbContext dbContext, ILogger logger, HashSet<object>? visited = null)
    {
        if (entity is not BaseEntity baseEntity) return;

        // Tránh lặp vòng, hoặc lặp entity đã gặp
        visited ??= new HashSet<object> ( );
        if (visited.Contains (entity)) return;
        visited.Add (entity);

        var entityType = entity.GetType ( ).Name;
        var entityId = baseEntity.Id.ToString ( );
        logger.LogInformation ("Soft delete: {EntityType} (ID: {EntityId})", entityType, entityId);

        baseEntity.IsDeleted = true;
        dbContext.Update (entity);
        // Nếu entity là Employee, cũng đánh dấu ApplicationUser là đã xóa mềm
        if (entity is Employee employee)
        {
            // If entity is Employee, also hard delete related ApplicationUser  
            employee.ApplicationUser = await _dbContext.ApplicationUsers.Where (t => t.Id == employee.ApplicationUserId).FirstOrDefaultAsync ( );
            // Đánh dấu IsDeleted = true cho ApplicationUser nếu có
            if (employee.ApplicationUser != null)
            {
                employee.ApplicationUser.IsDeleted = true;
                dbContext.Update (employee.ApplicationUser);  // Đánh dấu ApplicationUser là đã xóa mềm
                _logger.LogInformation ("Soft delete: ApplicationUser (ID: {EntityId})", employee.ApplicationUserId);

            }
        }

        var entry = dbContext.EntryEntity (entity);

        // Chỉ duyệt các navigation là collection (collection con)
        foreach (var nav in entry.Navigations.Where (n => n.Metadata.IsCollection))
        {
            await nav.LoadAsync ( );
            if (nav.CurrentValue is IEnumerable<object> children)
            {
                foreach (var child in children)
                {
                    // Chỉ soft delete nếu là BaseEntity và chưa IsDeleted
                    if (child is BaseEntity be && !be.IsDeleted)
                        await RecursiveSoftDeleteInternal (child, dbContext, logger, visited);
                }
            }
        }
    }

    private bool HasChildren(object entity, IRegalEducationDbContext dbContext)
    {
        var context = (DbContext)dbContext; // Đảm bảo dùng DbContext gốc
        var model = context.Model;
        var entityType = model.FindEntityType (entity.GetType ( ));
        var entityId = ((BaseEntity)entity).Id;

        foreach (var nav in entityType.GetNavigations ( ).Where (n => n.IsCollection))
        {
            var childType = nav.TargetEntityType.ClrType;

            // Lấy DbSet của entity con
            var setMethod = context.GetType ( ).GetMethod ("Set", Type.EmptyTypes);
            if (setMethod == null) continue;
            var dbSet = setMethod.MakeGenericMethod (childType).Invoke (context, null) as IQueryable;
            if (dbSet == null) continue;

            // Lấy foreign key từ navigation
            var fk = nav.ForeignKey;
            var fkProp = fk.Properties.FirstOrDefault ( );
            if (fkProp == null) continue;
            var fkName = fkProp.Name;

            // e => e.FK == entityId
            var param = Expression.Parameter (childType, "e");
            var property = Expression.Property (param, fkName);
            //var idValue = Expression.Constant (entityId, typeof (Guid));

            var idValue = Expression.Constant (entityId, property.Type);

            var equal = Expression.Equal (property, idValue);
            var lambda = Expression.Lambda (equal, param);

            // Where
            var whereMethod = typeof (Queryable).GetMethods ( )
                .First (m => m.Name == "Where" && m.GetParameters ( ).Length == 2)
                .MakeGenericMethod (childType);
            var filteredQuery = whereMethod.Invoke (null, new object[] { dbSet, lambda });

            // IgnoreQueryFilters
            var ignoreFilterMethod = typeof (EntityFrameworkQueryableExtensions)
                .GetMethod ("IgnoreQueryFilters")
                ?.MakeGenericMethod (childType);
            if (ignoreFilterMethod == null) continue;
            var queryWithNoFilter = ignoreFilterMethod.Invoke (null, new object[] { filteredQuery });

            // Any
            var anyMethod = typeof (Queryable).GetMethods ( )
                .First (m => m.Name == "Any" && m.GetParameters ( ).Length == 1)
                .MakeGenericMethod (childType);
            var hasChild = queryWithNoFilter != null && (bool)anyMethod.Invoke (null, new object[] { queryWithNoFilter });

            if (hasChild) return true;
        }
        return false;
    }

    public async Task<Result> RecursiveSoftDelete(Guid id, Type entityType)
    {
        try
        {
            var entity = await _dbContext.FindAsync (entityType, id);
            if (entity == null)
                return Result.Failure (_localizer.Format ("EntityNotFound", id));

            var entityId = id.ToString ( );
            List<string> pendingDeleteFiles = new List<string> ( );            
            
            if (HasChildren (entity, _dbContext))
            {
                return Result.Failure (_localizer.Format ("MSG_DELETE_FAILED_HAS_CHILDREN", _localizer[entityType.Name]));
                //await RecursiveSoftDeleteInternal (entity, _dbContext, _logger);
            }
            else
            {
                _logger.LogInformation ("Hard delete: {EntityType} (ID: {EntityId})", entityType.Name, entityId);

                if (entity is Employee employee)
                {
                    // If entity is Employee, also hard delete related ApplicationUser  
                    employee.ApplicationUser = await _dbContext.ApplicationUsers.Where (t => t.Id == employee.ApplicationUserId).FirstOrDefaultAsync ( );
                    if (employee.ApplicationUser != null)
                    {
                        var applicationUserId = employee.ApplicationUserId;
                        // Hard delete ApplicationUser  
                        _dbContext.Remove (employee.ApplicationUser); // Cast ApplicationUser to BaseEntity  
                        _logger.LogInformation ("Hard delete: ApplicationUser (ID: {EntityId})", applicationUserId);
                    }

                    // Hard delete Employee  
                    _dbContext.Remove (employee);
                }
                else if (entity is LectureType lectureType)
                {
                    // Ghi nhận file cần xoá, xoá sau khi DB commit thành công
                    pendingDeleteFiles.Add (lectureType.FileUrl);
                    _dbContext.Remove (lectureType);
                }
                else if (entity is SupportingDocument supportingDocument)
                {
                    _dbContext.Remove (supportingDocument);
                }
                else if (entity is AffiliatePartner affiliatePartner)
                {
                    // Ghi nhận file cần xoá, xoá sau khi DB commit thành công
                    _dbContext.Remove (affiliatePartner);
                }
                else
                {
                    // Hard delete other entities (if any)  
                    _dbContext.Remove (entity);
                }
            }
            await _dbContext.SaveChangesAsync ( );
            // Nếu là LectureType hard delete thì xoá file sau commit
            if (pendingDeleteFiles != null && pendingDeleteFiles.Count > 0)
            {
                foreach (var pendingDeleteFile in pendingDeleteFiles)
                {
                    if (string.IsNullOrEmpty (pendingDeleteFile)) continue; // Bỏ qua nếu không có file

                    try
                    {
                        var ok = await _fileService.DeleteFileAsync (pendingDeleteFile);
                        if (!ok)
                            _logger.LogWarning ("File not found or cannot delete: {FileUrl}", pendingDeleteFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError (ex, "Failed to delete file: {FileUrl}", pendingDeleteFile);
                        // Tuỳ business: có thể enqueue job dọn rác
                    }
                }
            }
            return Result.Success (_localizer.Format ("MSG_DELETE_SUCCESS", _localizer[entityType.Name]));
        }
        catch (Exception ex)
        {
            _logger.LogError (ex, "Soft delete failed for {EntityType} - ID: {Id}", entityType.Name, id);
            return Result.Failure (_localizer.Format ("ActionFailed", "Delete", Functions.GetFullExceptionMessage (ex)));
        }
    }
}
