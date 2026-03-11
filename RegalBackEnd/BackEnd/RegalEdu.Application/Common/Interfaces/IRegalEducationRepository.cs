
using System;
using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Results;


namespace RegalEdu.Application.Common.Interfaces
{
    public interface IRegalEducationRepository<T> where T : class
    {
        Task<Result> AddAsync(T entity);
        Task<Result> DeleteAsync(Guid id);
        Task<T> GetAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task<Result> Update(Guid id, T model);
        DbSet<T> GetDbSet();
    }
}
