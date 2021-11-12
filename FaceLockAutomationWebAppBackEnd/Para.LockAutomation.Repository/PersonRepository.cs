using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Para.LockAutomation.Repository
{
    public class PersonRepository : GenericRepository<PersonEntity>
    {        
        public PersonRepository(LockAutomationDbContext dbContext, TelemetryClient telementry)
            : base(dbContext, telementry)
        {
        }
        
        public async Task<List<PersonEntity>> GetAllPersonInGroup(int personGroupId)
        {
            return await _dbContext.Entity<PersonEntity>().ToListAsync();
        }

        public async Task<PersonEntity> GetPersonInGroupById(int personGroupId, int personId)
        {
            return await _dbContext.Entity<PersonEntity>()
                .Include(ps => ps.PersonGroup)
                .Where(ps => ps.PersonGroupId == personGroupId &&
                ps.Id == personId).FirstOrDefaultAsync();
        }

        public async Task<PersonEntity> GetPersonInGroupByObjectId(int personGroupId, Guid personObjectId)
        {
            return await _dbContext.Entity<PersonEntity>()
                .Include(ps => ps.PersonGroup)
                .Where(ps => ps.PersonGroupId == personGroupId &&
                ps.ObjectId == personObjectId).FirstOrDefaultAsync();
        }

        public async Task<RepositoryActionResult<PersonEntity>> CreatePerson(PersonEntity newPerson)
        {
            return await CreateEntity(newPerson);
        }

        public async Task<RepositoryActionResult<List<int>>> DeleteAllPersonsInGroup(int personGroupId)
        {
            try
            {
                var persons = await _dbContext.Entity<PersonEntity>().Where(ps => ps.PersonGroupId == personGroupId).ToListAsync();
                _dbContext.RemoveRange(persons);
                return new RepositoryActionResult<List<int>>(persons.Select(ps => ps.Id).ToList(), RepositoryActionStatus.Deleted);
            } catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return new RepositoryActionResult<List<int>>(null, RepositoryActionStatus.NothingModified);
            }
        } 

        public async Task<RepositoryActionResult<int>> DeletePerson(int personGroupId, int personId)
        {
            var person = await GetPersonInGroupById(personGroupId, personId);
            return await DeleteEntity(person);
        }

        public async Task<RepositoryActionResult<int>> DeletePerson(PersonEntity person)
        {
            return await DeleteEntity(person);
        }

        public async Task<RepositoryActionResult<PersonEntity>> UpdatePerson(PersonEntity updatePerson)
        {
            return await UpdateEntity(updatePerson);
        }
     }
}
