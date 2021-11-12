using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Para.LockAutomation.Repository
{
    public abstract class BaseRepository<T> where T : BaseEntity
    {
        protected readonly LockAutomationDbContext _dbContext;
        protected readonly TelemetryClient _telementry;
        public BaseRepository(LockAutomationDbContext dbContext, TelemetryClient telementry)
        {
            _dbContext = dbContext;
            _telementry = telementry;
        }

        protected async Task<List<T>> GetAllEntities()
        {
            return await _dbContext.Entity<T>().ToListAsync();
        }

        protected async Task<T> GetEntityById(int id)
        {
            return await _dbContext.Entity<T>().Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        protected async Task<RepositoryActionResult<T>> CreateEntity(T newEntity)
        {
            _dbContext.Add(newEntity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ?
                new RepositoryActionResult<T>(newEntity, RepositoryActionStatus.Created) :
                new RepositoryActionResult<T>(null, RepositoryActionStatus.NothingModified);
        }

        protected async Task<RepositoryActionResult<int>> DeleteEntity(T delEntity)
        {
            if (_dbContext.Entry(delEntity).State == EntityState.Detached)
            {
                _dbContext.Attach(delEntity);
            }
            _dbContext.Remove(delEntity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ?
                new RepositoryActionResult<int>(delEntity.Id, RepositoryActionStatus.Deleted) :
                new RepositoryActionResult<int>(0, RepositoryActionStatus.NothingModified);
        }

        protected async Task<RepositoryActionResult<T>> UpdateEntity(T updateEntity)
        {
            if (_dbContext.Entry(updateEntity).State == EntityState.Detached)
            {
                _dbContext.Attach(updateEntity);
                _dbContext.Entry(updateEntity).State = EntityState.Modified;
            }
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ?
                new RepositoryActionResult<T>(updateEntity, RepositoryActionStatus.Updated) :
                new RepositoryActionResult<T>(null, RepositoryActionStatus.NothingModified);
        } 

    }

    public abstract class GenericRepository<T> : BaseRepository<T> where T : GenericEntity
    {
        public GenericRepository(LockAutomationDbContext dbContext, TelemetryClient telementry) 
            : base(dbContext, telementry)
        {

        }

        protected async Task<T> GetEntityByObjectId(Guid objectId)
        {
            return await _dbContext.Entity<T>().Where(t => t.ObjectId == objectId).FirstOrDefaultAsync();
        }
    }
}
