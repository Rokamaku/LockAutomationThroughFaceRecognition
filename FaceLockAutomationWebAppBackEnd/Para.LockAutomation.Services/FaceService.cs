using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using Para.LockAutomation.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FaceEntity = Para.LockAutomation.Models.FaceEntity;
using TrainingStatus = Para.LockAutomation.Models.TrainingStatus;

namespace Para.LockAutomation.Service
{
    public class FaceService
    {
        private readonly FaceRepository _faceRepo;
        private readonly FaceServiceClient _faceClient;
        private readonly PersonGroupRepository _personGroupRepo;
        private readonly PersonRepository _personRepo;
        private readonly AzureStorageService _storageService;
        private readonly AzureStorageConfig _storageConfig;
        private readonly TelemetryClient _telementry;

        public FaceService(
            FaceRepository faceRepo,
            FaceServiceClient faceClient,
            PersonGroupRepository personGroupRepo,
            PersonRepository personRepo,
            AzureStorageService storageService,
            IOptions<AzureStorageConfig> storageOptions,
            TelemetryClient telementry)
        {
            _faceRepo = faceRepo;
            _faceClient = faceClient;
            _personGroupRepo = personGroupRepo;
            _personRepo = personRepo;
            _storageService = storageService;
            _storageConfig = storageOptions.Value;
            _telementry = telementry;
        }

        public async Task<List<FaceEntity>> GetFaces(int personGroupId, int personId)
        {
            return await _faceRepo.GetPersonFaces(personGroupId, personId);
        }

        public async Task<FaceEntity> GetFace(int personGroupId, int personId, int faceId)
        {
            return await _faceRepo.GetFaceById(personGroupId, personId, faceId);
        }

        private async Task<FaceEntity> AddFace(int personGroupId, int personId, IFormFile file)
        {
            var person = await _personRepo.GetPersonInGroupById(personGroupId, personId);
            AddPersistedFaceResult faceResult = null;
            try
            {
                faceResult = await _faceClient.AddPersonFaceAsync(person.PersonGroup.ObjectId.ToString(), 
                    person.ObjectId, file.OpenReadStream());
            }
            catch (FaceAPIException ex)
            {
                _telementry.TrackException(ex);
                return null;
            }

            try
            {
                string fileDir = $"{personGroupId}/{personId}/{faceResult.PersistedFaceId}";
                var uploadResult = await _storageService.UploadFileToBlobStorage(_storageConfig.FaceDatasetContainer, fileDir, file, false);

                var newFace = new FaceEntity
                {
                    ObjectId = faceResult.PersistedFaceId,
                    PersonId = personId,
                    File = uploadResult,
                    CreatedDate = DateTimeOffset.Now
                };
                var result = await _faceRepo.CreateFace(newFace);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    return newFace;
                }
                else
                {
                    await _faceClient.DeletePersonFaceAsync(person.PersonGroup.ObjectId.ToString(), 
                        person.ObjectId, faceResult.PersistedFaceId);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                await _faceClient.DeletePersonFaceAsync(person.PersonGroup.ObjectId.ToString(),
                    person.ObjectId, faceResult.PersistedFaceId);
                return null;
            }
        }

        public async Task<List<FaceEntity>> AddFaces(int personGroupId, int personId, List<IFormFile> files)
        {
            List<FaceEntity> faces = new List<FaceEntity>();
            foreach (var file in files)
            {
                var result = await AddFace(personGroupId, personId, file);
                if (result != null)
                {
                    faces.Add(result);
                }               
            }
            await ChangeNotTrainStatus(faces, personGroupId);
            return faces;
        }

        private async Task<int> DeleteFace(int personGroupId, int personId, int faceId, string fileDir)
        {
            try
            {
                var face = await _faceRepo.GetFaceById(personGroupId, personId, faceId);
                if (face != null)
                {
                    await _faceClient.DeletePersonFaceAsync(face.Person.PersonGroup.ObjectId.ToString(),
                        face.Person.ObjectId, face.ObjectId);
                }
                if (fileDir != null)
                {
                    await _storageService.DeleteBlobFile(_storageConfig.FaceDatasetContainer, fileDir, false);
                }
                await _faceRepo.DeleteFace(personGroupId, personId, faceId);
                return faceId;
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return -1;
            }
        }

        public async Task<List<int>> DeleteFaces(int personGroupId, int personId, int[] faceIds)
        {
            List<int> delFaces = new List<int>();
            foreach (var faceId in faceIds)
            {
                string fileDir = (await _faceRepo.GetFaceById(personGroupId, personId, faceId))?.File.FilePath;
                delFaces.Add(await DeleteFace(personGroupId, personId, faceId, fileDir));
            }
            await ChangeNotTrainStatus(delFaces, personGroupId);
            return delFaces;
        }

        public async Task DeletePersonGroupFaces(int personGroupId)
        {
            var faces = await _faceRepo.GetAllFacesInGroup(personGroupId);
            foreach (var face in faces)
            {
                string fileDir = face.File.FilePath;
                await _storageService.DeleteBlobFile(_storageConfig.FaceDatasetContainer, fileDir, false);
            }
            await ChangeNotTrainStatus(faces, personGroupId);
        }

        public async Task DeletePersonFaces(int personGroupId, int personId)
        {
            var faces = await _faceRepo.GetPersonFaces(personGroupId, personId);
            foreach (var face in faces)
            {
                string fileDir = face.File.FilePath;
                await _storageService.DeleteBlobFile(_storageConfig.FaceDatasetContainer, fileDir, false);
            }
            await ChangeNotTrainStatus(faces, personGroupId);
        }

        private async Task ChangeNotTrainStatus<T>(List<T> list, int personGroupId)
        {
            if (list.Count() > 0)
            {
                await _personGroupRepo.ChangeTraningStatus(personGroupId, TrainingStatus.NotTrain);
            }
        }
    }
}
