using Microsoft.ProjectOxford.Face;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Para.LockAutomation.Service
{
    public class TrainingService
    {
        private readonly FaceServiceClient _faceClient;
        private readonly PersonGroupRepository _personGroupRepo;
        private readonly ParaFuncService _parafuncService;

        public TrainingService(FaceServiceClient faceClient, 
            PersonGroupRepository personGroupRepo, 
            ParaFuncService parafuncService)
        {
            _faceClient = faceClient;
            _personGroupRepo = personGroupRepo;
            _parafuncService = parafuncService;
        }

        public async Task<TrainingStatus> GetTrainingStatus(int personGroupId)
        {
            var personGroup = await _personGroupRepo.GetPersonGroupById(personGroupId);
            var result = await GetTrainingStatus(personGroup.ObjectId.ToString());
            await _personGroupRepo.ChangeTraningStatus(personGroupId, result);
            return result;
        } 

        public async Task<TrainingStatus> TrainingPersonGroup(int personGroupId)
        {
            var personGroup = await _personGroupRepo.GetPersonGroupById(personGroupId);
            await _faceClient.TrainPersonGroupAsync(personGroup.ObjectId.ToString());
            var result = await GetTrainingStatus(personGroup.ObjectId.ToString());
            await _personGroupRepo.ChangeTraningStatus(personGroupId, result);
            _parafuncService.GetTrainingStatus(personGroupId);
            return result;
        }

        private async Task<TrainingStatus> GetTrainingStatus(string personGroupObjectId)
        {
            try
            {
                var trainingResult = await _faceClient.GetPersonGroupTrainingStatusAsync(personGroupObjectId);
                return Enum.Parse<TrainingStatus>(trainingResult.Status.ToString());
            }
            catch (FaceAPIException)
            {
                return TrainingStatus.NotTrain;
            }
        }
    }
}
