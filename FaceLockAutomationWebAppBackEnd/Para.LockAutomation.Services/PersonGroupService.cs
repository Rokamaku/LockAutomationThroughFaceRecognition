using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using Para.LockAutomation.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrainingStatus = Para.LockAutomation.Models.TrainingStatus;

namespace Para.LockAutomation.Service
{
    public class PersonGroupService
    {
        private readonly FaceServiceClient _faceClient;
        private readonly PersonGroupRepository _personGroupRepo;
        private readonly PersonRepository _personRepo;
        private readonly FaceService _faceService;

        public PersonGroupService(FaceServiceClient faceClient, 
            PersonGroupRepository personGroupRepo,
            PersonRepository personRepo,
            FaceService faceService)
        {
            _faceClient = faceClient;
            _personGroupRepo = personGroupRepo;
            _personRepo = personRepo;
            _faceService = faceService;
        }

        public async Task<List<PersonGroupEntity>> GetAllPersonGroup()
        {
            return await _personGroupRepo.GetAllPersonGroup();
        }

        public async Task<PersonGroupEntity> GetPersonGroup(int personGroupId)
        {
            return await _personGroupRepo.GetPersonGroupById(personGroupId);
        }

        public async Task<PersonGroupEntity> GetDefaultPersonGroup()
        {
            return await _personGroupRepo.GetDefaultPersonGroup();
        }

        public async Task<PersonGroupEntity> CreatePersonGroup(string personGroupName, float confidence)
        {
            Guid personGroupId = Guid.NewGuid();
            await _faceClient.CreatePersonGroupAsync(personGroupId.ToString(), personGroupName);

            var personGroup = new PersonGroupEntity
            {
                Name = personGroupName,
                ObjectId = personGroupId,
                TrainingStatus = TrainingStatus.NotTrain,
                CreatedDate = DateTimeOffset.Now,
                IsDefault = false,
                ConfindenceThreshold = Math.Round(confidence, 2)
            };
            var result = await _personGroupRepo.CreatePersonGroup(personGroup);
            if (result.Status == RepositoryActionStatus.Created)
            {
                return personGroup;
            }
            else
            {
                await _faceClient.DeletePersonGroupAsync(personGroupId.ToString());
                return null;
            }
        }

        public async Task<int> DeletePersonGroup(int personGroupId)
        {
            var personGroup = await _personGroupRepo.GetPersonGroupById(personGroupId);
            await _faceClient.DeletePersonGroupAsync(personGroup.ObjectId.ToString());
            await _faceService.DeletePersonGroupFaces(personGroupId);
            await _personRepo.DeleteAllPersonsInGroup(personGroupId);
            await _personGroupRepo.DeletePersonGroup(personGroupId);           
            return personGroupId;
        }

        public async Task<int?> ChangeDefaultPersonGroup(int personGroupId)
        {
            var result = await _personGroupRepo.ChangeDefaultPersonGroup(personGroupId);
            if (result.Status == RepositoryActionStatus.Updated)
            {
                return result.Entity.Id;
            }
            else
            {
                return null;
            }
        }

        public async Task<PersonGroupEntity> ChangeConfidenceThreshold(int personGroupId, float newConfidence)
        {
            var result = await _personGroupRepo.ChangeConfidenceThreshold(personGroupId, newConfidence);
            if (result.Status == RepositoryActionStatus.Updated)
            {
                return result.Entity;
            }
            else
            {
                return null;
            }
        }

    }
}
