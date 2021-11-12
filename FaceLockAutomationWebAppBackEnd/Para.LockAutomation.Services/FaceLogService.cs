using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using Para.LockAutomation.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Para.LockAutomation.Service
{
    public class FaceLogService
    {
        private readonly FaceLogRepository _faceLogRepo;
        private readonly PersonGroupRepository _personGroupRepo;
        private readonly PersonRepository _personRepo;
        private readonly FaceServiceClient _faceClient;
        private readonly AzureStorageService _storageService;
        private readonly AzureStorageConfig _storageConfig;
        private readonly IMapper _mapper;

        public FaceLogService(
            FaceLogRepository faceLogRepo,
            PersonGroupRepository personGroupRepo,
            PersonRepository personRepo,
            FaceServiceClient faceClient,
            AzureStorageService storageService,
            IOptions<AzureStorageConfig> storageConfigOption,
            IMapper mapper)
        {
            _faceLogRepo = faceLogRepo;
            _personGroupRepo = personGroupRepo;
            _personRepo = personRepo;
            _faceClient = faceClient;
            _storageService = storageService;
            _storageConfig = storageConfigOption.Value;
            _mapper = mapper;
        }

        public async Task<FaceLogSearchResultDTO> GetFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            var result = await _faceLogRepo.GetFaceLogInGroup(personGroupId, filter);
            var totalRecs = _faceLogRepo.CountTotalFaceLogInGroup(personGroupId, filter.FromDate, filter.ToDate);
            return CreateSearchDTO(result, filter, totalRecs);
        }

        public async Task<FaceLogSearchResultDTO> GetPersonFaceLogInGroup(int personGroupId, int personId, FilterModel filter)
        {
            var result = await _faceLogRepo.GetPersonFaceLogInGroup(personGroupId, personId, filter);
            var totalRecs = _faceLogRepo.CountTotalPersonFaceLog(personGroupId, personId, filter.FromDate, filter.ToDate);
            return CreateSearchDTO(result, filter, totalRecs);
        }

        public async Task<FaceLogSearchResultDTO> GetUnknownFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            var result = await _faceLogRepo.GetUnknowFaceLogInGroup(personGroupId, filter);
            var totalRecs = _faceLogRepo.CountTotalUnknownFaceLog(personGroupId, filter.FromDate, filter.ToDate);
            return CreateSearchDTO(result, filter, totalRecs);
        }

        public async Task<FaceLogSearchResultDTO> GetKnownFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            var result = await _faceLogRepo.GetKnownFaceLogInGroup(personGroupId, filter);
            var totalRecs = _faceLogRepo.CountTotalKnownFaceLog(personGroupId, filter.FromDate, filter.ToDate);
            return CreateSearchDTO(result, filter, totalRecs);
        }

        public async Task<FaceLogSearchResultDTO> GetUndetectedFaceLogInGroup(int personGroupId, FilterModel filter)
        {
            var result = await _faceLogRepo.GetUndetectedFaceLogInGroup(personGroupId, filter);
            var totalRecs = _faceLogRepo.CountTotalUndetectedFaceLog(personGroupId, filter.FromDate, filter.ToDate);
            return CreateSearchDTO(result, filter, totalRecs);
        }

        private FaceLogSearchResultDTO CreateSearchDTO(List<FaceLogEntity> result, FilterModel filter, int totalRecs)
        {
            var CurrentPage = totalRecs < (filter.PageNumber - 1) * filter.PageSize ? 1 : filter.PageNumber;
            var pagination = new PaginationHeader
            {
                PageNumber = CurrentPage,
                PageSize = filter.PageSize,
                TotalCount = totalRecs,
                TotalPages = (int)Math.Ceiling((double)totalRecs / filter.PageSize) 
            };
            var faceLogDto = new FaceLogSearchResultDTO
            {
                Data = result,
                PaginationHeader = pagination
            };
            return faceLogDto;
        }

        public async Task<FaceLogEntity> CreateFaceLogBeforeUpload(int personGroupId, IFormFile image)
        {
            string fileDir = image.FileName.Split(".").FirstOrDefault() + "_" +
                DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            var file = await _storageService.UploadFileToBlobStorage(_storageConfig.FaceLogContainer, fileDir, image);
            var personGroup = await _personGroupRepo.GetPersonGroupById(personGroupId);
            return await CreateFaceLogAfterUpload(personGroup, file, image.OpenReadStream());
        }

        public async Task<FaceLogEntity> CreateFaceLogAfterUploadForDefaultGroup(AzureFile file, Stream imageStream)
        {
            var personGroup = await _personGroupRepo.GetDefaultPersonGroup();
            return await CreateFaceLogAfterUpload(personGroup, file, imageStream);
        }

        public async Task<FaceLogEntity> CreateFaceLogAfterUpload(PersonGroupEntity personGroup, AzureFile file, Stream imageStream)
        {
            if (personGroup != null)
            {
                var newFaceLog = new FaceLogEntity()
                {
                    PersonGroupId = personGroup.Id,
                    File = file
                };
                await IdentifyPerson(newFaceLog, personGroup.ObjectId.ToString(), imageStream, (float)personGroup.ConfindenceThreshold);
                newFaceLog.CreatedDate = DateTimeOffset.Now;
                newFaceLog.ObjectId = Guid.NewGuid();
                await _faceLogRepo.CreateFaceLog(newFaceLog);
                return newFaceLog;
            }
            else
            {
                return null;
            }
        }

        private async Task IdentifyPerson(FaceLogEntity newFaceLog, string personGroupObjectId, Stream imageStream, float confidence = (float)0.8)
        {
            var faces = await _faceClient.DetectAsync(imageStream);
            var faceIds = faces.Select(fc => fc.FaceId).ToArray();

            if (faceIds.Count() != 0)
            {
                var results = await _faceClient.IdentifyAsync(personGroupObjectId, faceIds, confidenceThreshold: confidence);

                List<FaceRectangle> faceRecs = new List<FaceRectangle>();
                List<Guid> personIds = new List<Guid>();
                List<double> confidences = new List<double>();
                foreach (var face in faces)
                {
                    var candidates = results.Where(ir => ir.FaceId == face.FaceId).FirstOrDefault().Candidates;
                    if (candidates.Count() > 0)
                    {
                        personIds.Add(candidates[0].PersonId);
                        confidences.Add(candidates[0].Confidence);
                    }
                    else
                    {
                        personIds.Add(ParaConsts.UnknownPersonId);
                    }
                    var faceRectangle = new FaceRectangle
                    {
                        Height = face.FaceRectangle.Height,
                        Width = face.FaceRectangle.Width,
                        Top = face.FaceRectangle.Top,
                        Left = face.FaceRectangle.Left,
                    };
                    faceRecs.Add(faceRectangle);
                }
                newFaceLog.FaceRectangles = faceRecs;
                newFaceLog.Persons = personIds;
                newFaceLog.Confidences = confidences;
            }
            else
            {
                newFaceLog.FaceRectangles = new List<FaceRectangle>();
                newFaceLog.Persons = new List<Guid>();
            }
        }

        public async Task<int> DeleteFaceLog(int personGroupId, int faceLogId)
        {
            var result = await _faceLogRepo.DeleteFaceLog(personGroupId, faceLogId);
            return result.Status == RepositoryActionStatus.Deleted ? result.Entity : ParaConsts.NullInteger;
        }

        public async Task<List<int>> DeleteFaceLogs(int personGroupId, int[] faceLogIds)
        {
            List<int> delFaceLogIds = new List<int>();
            foreach (int faceLogId in faceLogIds)
            {
                var result = await DeleteFaceLog(personGroupId, faceLogId);
                if (result != ParaConsts.NullInteger)
                {
                    delFaceLogIds.Add(result);
                }
            }
            return delFaceLogIds;
        }
    }
}
