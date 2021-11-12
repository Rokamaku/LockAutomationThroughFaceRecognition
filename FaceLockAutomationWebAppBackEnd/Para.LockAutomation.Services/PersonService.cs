using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using Para.LockAutomation.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Para.LockAutomation.Service
{
    public class PersonService
    {
        private readonly FaceServiceClient _faceClient;
        private readonly PersonRepository _personRepo;
        private readonly PersonGroupRepository _personGroupRepo;
        private readonly FaceService _faceService;

        public PersonService(FaceServiceClient faceClient, 
            PersonRepository personRepo,
            PersonGroupRepository personGroupRepo,
            FaceService faceService)
        {
            _faceClient = faceClient;
            _personRepo = personRepo;
            _personGroupRepo = personGroupRepo;
            _faceService = faceService;
        }

        public async Task<List<PersonEntity>> GetAllPersonInGroup(int personGroupId)
        {
            return await _personRepo.GetAllPersonInGroup(personGroupId);
        }

        public async Task<PersonEntity> GetPersonById(int personGroupId, int personId)
        {
            return await _personRepo.GetPersonInGroupById(personGroupId, personId);
        }

        public async Task<PersonEntity> CreatePerson(PersonDTO person)
        {
            var personGroup = await _personGroupRepo.GetPersonGroupById(person.PersonGroupId);
            var personId = (await _faceClient.CreatePersonAsync(personGroup.ObjectId.ToString(), person.InternalName)).PersonId;

            var newPerson = new PersonEntity
            {
                ObjectId = personId,
                FirstName = person.FirstName,
                LastName = person.LastName,
                PersonGroupId = person.PersonGroupId,
                CreatedDate = DateTimeOffset.Now
            };

            var repoResult = await _personRepo.CreatePerson(newPerson);
            if (repoResult.Status == RepositoryActionStatus.Created)
            {
                return repoResult.Entity;
            }
            else
            {
                await _faceClient.DeletePersonAsync(personGroup.ObjectId.ToString(), personId);
                return null;
            }

        }

        public async Task<int> DeletePerson(int personGroupId, int delPersonId)
        {
            var person = await _personRepo.GetPersonInGroupById(personGroupId, delPersonId);
            if (person != null)
            {
                await _faceClient.DeletePersonAsync(person.PersonGroup.ObjectId.ToString(), person.ObjectId);
                await _faceService.DeletePersonFaces(personGroupId, delPersonId);
                await _personRepo.DeletePerson(personGroupId, delPersonId);              
                return delPersonId;
            }
            return ParaConsts.NullInteger;
        }

        public async Task<List<int>> DeletePersons(int personGroupId, int[] delPersonIds)
        {
            List<int> personsIds = new List<int>();
            foreach ( var delPersonId in delPersonIds)
            {
                var result = await DeletePerson(personGroupId, delPersonId);
                if (result != ParaConsts.NullInteger)
                {
                    personsIds.Add(result);
                }
            }
            return personsIds;
        }

        public async Task<PersonDTO> UpdatePerson(PersonDTO updatePerson)
        {
            var person = await _personRepo.GetPersonInGroupById(updatePerson.PersonGroupId,
                updatePerson.Id.GetValueOrDefault());
            await _faceClient.UpdatePersonAsync(person.PersonGroup.ObjectId.ToString(), 
                person.ObjectId, 
                updatePerson.InternalName);


            person.FirstName = updatePerson.FirstName;
            person.LastName = updatePerson.LastName;

            await _personRepo.UpdatePerson(person);
            return updatePerson;
        }
    }
}
