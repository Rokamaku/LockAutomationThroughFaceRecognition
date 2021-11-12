using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Para.LockAutomation.Repository
{
    public class PersonGroupRepository : BaseRepository<PersonGroupEntity>
    {
        public PersonGroupRepository(LockAutomationDbContext dbContext, TelemetryClient telementry)
            : base(dbContext, telementry)
        {
        }

        public async Task<List<PersonGroupEntity>> GetAllPersonGroup()
        {
            return await _dbContext.Entity<PersonGroupEntity>()
                .Include(pg => pg.Persons)
                .ToListAsync();
        }

        public async Task<PersonGroupEntity> GetPersonGroupById(int personGroupId)
        {
            return await _dbContext.Entity<PersonGroupEntity>().Where(pg => pg.Id == personGroupId).FirstOrDefaultAsync();
        }

        public async Task<PersonGroupEntity> GetDefaultPersonGroup()
        {
            return await _dbContext.Entity<PersonGroupEntity>().Where(pg => pg.IsDefault == true).FirstOrDefaultAsync();
        }

        public async Task<RepositoryActionResult<PersonGroupEntity>> CreatePersonGroup(PersonGroupEntity personGroup)
        {
            return await CreateEntity(personGroup);
        }

        public async Task<RepositoryActionResult<int>> DeletePersonGroup(int personGroupId)
        {
            var delPersonGroup = await _dbContext.Entity<PersonGroupEntity>(true).Where(pg => 
                pg.Id == personGroupId).FirstOrDefaultAsync();
            if (delPersonGroup != null)
            {
                _dbContext.Remove(delPersonGroup);
                await _dbContext.SaveChangesAsync();
                return new RepositoryActionResult<int>(delPersonGroup.Id, RepositoryActionStatus.Deleted);
            }
            return new RepositoryActionResult<int>(0, RepositoryActionStatus.NothingModified);
        }

        public async Task<RepositoryActionResult<PersonGroupEntity>> ChangeTraningStatus(int personGroupId, TrainingStatus newStatus)
        {
            var personGroup = await _dbContext.Entity<PersonGroupEntity>(true).Where(pg => 
                pg.Id == personGroupId).FirstOrDefaultAsync();
            personGroup.TrainingStatus = newStatus;
            return await UpdateEntity(personGroup);
        } 

        public async Task<RepositoryActionResult<PersonGroupEntity>> ChangeDefaultPersonGroup(int personGroupId)
        {
            var prevDefaultPersonGroups = await _dbContext.Entity<PersonGroupEntity>(true)
                .Where(pg => pg.IsDefault == true).ToListAsync();
            var newDefaultPersonGroup = await _dbContext.Entity<PersonGroupEntity>(true)
                .Where(pg => pg.Id == personGroupId).FirstOrDefaultAsync();
            foreach (var prevDefaultPersonGroup in prevDefaultPersonGroups)
            {
                prevDefaultPersonGroup.IsDefault = false;
            }
            newDefaultPersonGroup.IsDefault = true;

            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ?
                new RepositoryActionResult<PersonGroupEntity>(newDefaultPersonGroup, RepositoryActionStatus.Updated) :
                new RepositoryActionResult<PersonGroupEntity>(null, RepositoryActionStatus.NothingModified);
        }

        public async Task<RepositoryActionResult<PersonGroupEntity>> ChangeConfidenceThreshold(int personGroupId, float newConfidence)
        {
            var personGroup = await _dbContext.Entity<PersonGroupEntity>(true).Where(pg => pg.Id == personGroupId)
                .FirstOrDefaultAsync();
            if (personGroup != null)
            {
                personGroup.ConfindenceThreshold = Math.Round(newConfidence, 2);
            }
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ?
                new RepositoryActionResult<PersonGroupEntity>(personGroup, RepositoryActionStatus.Updated) :
                new RepositoryActionResult<PersonGroupEntity>(null, RepositoryActionStatus.NothingModified);
        }
    }
}
