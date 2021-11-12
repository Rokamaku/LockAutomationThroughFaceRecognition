using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Para.LockAutomation.Repository
{
    public class FaceLogRepository : BaseRepository<FaceLogEntity>
    {
        public FaceLogRepository(LockAutomationDbContext dbContext, TelemetryClient telementry) 
            : base(dbContext, telementry)
        {
        }

        public async Task<List<FaceLogEntity>> GetFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            return await _dbContext.ExecProc<FaceLogEntity>($@"sp_GetFaceLogInGroup " +
                $"@PersonGroupId = {personGroupId}," +
                $"@SortType = '{filter.SortType}', " +
                $"@FromDate = {(filter.FromDate.HasValue ? "'" + filter.FromDate.Value.AddDays(-1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@ToDate = {(filter.ToDate.HasValue ? "'" + filter.ToDate.Value.AddDays(1).ToString("MM-dd-yyyy") + "'" : "null" )}, " +
                $"@PageNumber = {filter.PageNumber}, " +
                $"@PageSize = {filter.PageSize}")
                .ToListAsync();
        }

        public async Task<List<FaceLogEntity>> GetPersonFaceLogInGroup(int personGroupId, int personId, FilterModel filter)
        {
            return await _dbContext.ExecProc<FaceLogEntity>($@"sp_GetPersonFaceLog " +
                $"@PersonGroupId = {personGroupId}, " +
                $"@PersonId = {personId}, "+
                $"@SortType = '{filter.SortType}', " +
                $"@FromDate = {(filter.FromDate.HasValue ? "'" + filter.FromDate.Value.AddDays(-1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@ToDate = {(filter.ToDate.HasValue ? "'" + filter.ToDate.Value.AddDays(1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@PageNumber = {filter.PageNumber}, " +
                $"@PageSize = {filter.PageSize}")
                .ToListAsync();
        }

        public async Task<List<FaceLogEntity>> GetKnownFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            return await _dbContext.ExecProc<FaceLogEntity>($@"sp_GetKnownFaceLog " +
                $"@PersonGroupId = {personGroupId}, " +
                $"@SortType = '{filter.SortType}', " +
                $"@FromDate = {(filter.FromDate.HasValue ? "'" + filter.FromDate.Value.AddDays(-1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@ToDate = {(filter.ToDate.HasValue ? "'" + filter.ToDate.Value.AddDays(1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@PageNumber = {filter.PageNumber}, " +
                $"@PageSize = {filter.PageSize}")
                .ToListAsync();
        }

        public async Task<List<FaceLogEntity>> GetUnknowFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            return await _dbContext.ExecProc<FaceLogEntity>($@"sp_GetUnknownFaceLog 
                @PersonGroupId = {personGroupId}, " +
                $"@SortType = '{filter.SortType}', " +
                $"@FromDate = {(filter.FromDate.HasValue ? "'" + filter.FromDate.Value.AddDays(-1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@ToDate = {(filter.ToDate.HasValue ? "'" + filter.ToDate.Value.AddDays(1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@PageNumber = {filter.PageNumber}, " +
                $"@PageSize = {filter.PageSize}")
                .ToListAsync();
        }

        public async Task<List<FaceLogEntity>> GetUndetectedFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            return await _dbContext.ExecProc<FaceLogEntity>($@"sp_GetUndetectedFaceLog
                @PersonGroupId = {personGroupId}, " +
                $"@SortType = '{filter.SortType}', " +
                $"@FromDate = {(filter.FromDate.HasValue ? "'" + filter.FromDate.Value.AddDays(-1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@ToDate = {(filter.ToDate.HasValue ? "'" + filter.ToDate.Value.AddDays(1).ToString("MM-dd-yyyy") + "'" : "null")}, " +
                $"@PageNumber = {filter.PageNumber}, " +
                $"@PageSize = {filter.PageSize}")
                .ToListAsync();
        }

        public async Task<FaceLogEntity> GetFaceLogById(int personGroupId, int faceLogId)
        {
            return await _dbContext.Entity<FaceLogEntity>().Where(fl => fl.PersonGroupId == personGroupId &&
                fl.Id == faceLogId).FirstOrDefaultAsync();
        }

        public async Task<RepositoryActionResult<FaceLogEntity>> CreateFaceLog(FaceLogEntity newFaceLog)
        {
            return await CreateEntity(newFaceLog);
        }

        public async Task<RepositoryActionResult<int>> DeleteFaceLog(int personGroupId, int faceLogId)
        {
            var faceLog = await GetFaceLogById(personGroupId, faceLogId);
            return await DeleteEntity(faceLog);
        }

        public async Task<RepositoryActionResult<FaceLogEntity>> UpdatePerson(FaceLogEntity updateFaceLog)
        {
            return await UpdateEntity(updateFaceLog);
        }

        public int CountTotalFaceLogInGroup(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            fromDate = fromDate.HasValue ? (DateTime?)fromDate.Value.Date.AddDays(-1) : null;
            toDate = toDate.HasValue ? (DateTime?)toDate.Value.Date.AddDays(1) : null;
            return _dbContext.Entity<FaceLogEntity>().Where(p => p.PersonGroupId == personGroupId)
                .Select(p => LockAutomationDbContext.fn_CountTotalFaceLogInGroup(personGroupId, fromDate, toDate)).FirstOrDefault();
        }

        public int CountTotalKnownFaceLog(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            fromDate = fromDate.HasValue ? (DateTime?)fromDate.Value.Date.AddDays(-1) : null;
            toDate = toDate.HasValue ? (DateTime?)toDate.Value.Date.AddDays(1) : null;
            return _dbContext.Entity<FaceLogEntity>().Where(p => p.PersonGroupId == personGroupId)
                .Select(p => LockAutomationDbContext.fn_CountTotalKnownFaceLog(personGroupId, fromDate, toDate)).FirstOrDefault();
        }

        public int CountTotalUnknownFaceLog(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            fromDate = fromDate.HasValue ? (DateTime?)fromDate.Value.Date.AddDays(-1) : null;
            toDate = toDate.HasValue ? (DateTime?)toDate.Value.Date.AddDays(1) : null;
            return _dbContext.Entity<FaceLogEntity>().Where(p => p.PersonGroupId == personGroupId)
                .Select(p => LockAutomationDbContext.fn_CountTotalUnknownFaceLog(personGroupId, fromDate, toDate)).FirstOrDefault();
        }

        public int CountTotalPersonFaceLog(int personGroupId, int personId, DateTime? fromDate, DateTime? toDate)
        {
            fromDate = fromDate.HasValue ? (DateTime?)fromDate.Value.Date.AddDays(-1) : null;
            toDate = toDate.HasValue ? (DateTime?)toDate.Value.Date.AddDays(1) : null;
            return _dbContext.Entity<FaceLogEntity>().Where(p => p.PersonGroupId == personGroupId)
                .Select(p => LockAutomationDbContext.fn_CountTotalPersonFaceLog(personGroupId, personId, fromDate, toDate)).FirstOrDefault();
        }

        public int CountTotalUndetectedFaceLog(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            fromDate = fromDate.HasValue ? (DateTime?)fromDate.Value.Date.AddDays(-1) : null;
            toDate = toDate.HasValue ? (DateTime?)toDate.Value.Date.AddDays(1) : null;
            return _dbContext.Entity<FaceLogEntity>().Where(p => p.PersonGroupId == personGroupId)
                .Select(p => LockAutomationDbContext.fn_CountTotalUndetectedFaceLog(personGroupId, fromDate, toDate)).FirstOrDefault();
        }
    }
}
