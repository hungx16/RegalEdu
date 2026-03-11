using Microsoft.EntityFrameworkCore;
using RegalEdu.Application.Common.Interfaces;
using RegalEdu.Application.Common.Results;

namespace RegalEdu.Infrastructure.Repositories
{
    public class RegalEducationRepository<T> : IRegalEducationRepository<T> where T : class
    {
        private readonly IRegalEducationDbContext _context;
        private readonly DbSet<T> _dbset;

        public RegalEducationRepository(IRegalEducationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException (nameof (context));
            _dbset = _context.SetEntity<T> ( );
        }

        public async Task<Result> AddAsync(T entity)
        {
            await _dbset.AddAsync (entity);
            return await _context.SaveChangesAsync ( ) > 0
                ? Result.Success ( )
                : Result.Failure ($"Failed to add {typeof (T).Name} entity to database");
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var entity = await _dbset.FindAsync (id);

            if (entity == null)
            {
                throw new ArgumentNullException (nameof (entity));
            }

            _dbset.Remove (entity);
            return await _context.SaveChangesAsync ( ) > 0
                ? Result.Success ( )
                : Result.Failure ($"Failed to delete {typeof (T).Name} entity");
        }

        public async Task<T> GetAsync(Guid id) // Updated to match the interface's return type
        {
            var entity = await _dbset.FindAsync (id);
            if (entity == null)
            {
                throw new InvalidOperationException ($"Entity of type {typeof (T).Name} with ID {id} not found.");
            }
            return entity;
        }

        public async Task<List<T>> GetAllAsync( )
        {
            return await _dbset.AsNoTracking ( ).ToListAsync ( );
        }

        public DbSet<T> GetDbSet( )
        {
            return _dbset;
        }

        public async Task<Result> Update(Guid id, T model)
        {
            var entity = await _dbset.FindAsync (id);

            if (entity == null)
            {
                throw new ArgumentNullException (nameof (entity));
            }

            _context.EntryEntity (entity).CurrentValues.SetValues (model);

            return await _context.SaveChangesAsync ( ) > 0
                ? Result.Success ( )
                : Result.Failure ($"Failed to update {typeof (T).Name} entity");
        }
    }
}

