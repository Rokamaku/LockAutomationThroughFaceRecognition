using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Para.LockAutomation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Para.LockAutomation.Repository
{
    public class FaceRepository : BaseRepository<FaceEntity>
    {
        public FaceRepository(LockAutomationDbContext dbContext, TelemetryClient telementry)
            : base(dbContext, telementry)
        {
        }

        public async Task<List<FaceEntity>> GetAllFacesInGroup(int personGroupId)
        {
            return await _dbContext.Entity<FaceEntity>()
                .Where(fc => fc.Person.PersonGroupId == personGroupId).ToListAsync();
        }

        public async Task<List<FaceEntity>> GetPersonFaces(int personGroupId, int personId)
        {
            return await _dbContext.Entity<FaceEntity>()
                .Include(fc => fc.Person).ThenInclude(ps => ps.PersonGroup)
                .Where(fc => fc.Person.PersonGroupId == personGroupId &&
                fc.PersonId == personId).ToListAsync();
        }

        public async Task<FaceEntity> GetFaceById(int personGroupId, int personId, int faceId)
        {
            return await _dbContext.Entity<FaceEntity>()
                .Include(fc => fc.Person).ThenInclude(ps => ps.PersonGroup)
                .Where(fc => fc.Person.PersonGroupId == personGroupId &&
                fc.PersonId == personId && fc.Id == faceId).FirstOrDefaultAsync();
        }

        public async Task<RepositoryActionResult<FaceEntity>> CreateFace(FaceEntity newFace)
        {
            return await CreateEntity(newFace);
        } 

        public async Task<RepositoryActionResult<int>> DeleteFace(int personGroupId, int personId, int faceId)
        {
            var face = await GetFaceById(personGroupId, personId, faceId);
            return await DeleteEntity(face);
        }
    }
}
