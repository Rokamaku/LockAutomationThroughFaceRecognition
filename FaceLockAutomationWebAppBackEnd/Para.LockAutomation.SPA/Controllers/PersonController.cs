using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using Para.LockAutomation.Service;

namespace Para.LockAutomation.SPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly PersonService _personService;
        private readonly TelemetryClient _telementry;

        public PersonController(PersonService personService, TelemetryClient telemetry)
        {
            _personService = personService;
            _telementry = telemetry;
        }


        [HttpGet("{personGroupId}")]
        public async Task<ActionResult<List<PersonEntity>>> Get(int personGroupId)
        {
            try
            {
                return Ok(await _personService.GetAllPersonInGroup(personGroupId));
            }
            catch (FaceAPIException ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.ErrorCode + ' ' + ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{personGroupId}/{personId}")]
        public async Task<ActionResult<PersonEntity>> Get(int personGroupId, int personId)
        {
            try
            {
                return Ok(await _personService.GetPersonById(personGroupId, personId));
            }
            catch (FaceAPIException ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.ErrorCode + ' ' + ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<ActionResult<PersonEntity>> Post([FromBody]PersonDTO person)
        {
            try
            {
                var result = await _personService.CreatePerson(person);
                if (result != null)
                    return Ok(result);
                return BadRequest();
            }
            catch (FaceAPIException ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.ErrorCode + ' ' + ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        public async Task<ActionResult<PersonDTO>> Put([FromBody]PersonDTO personName)
        {
            try
            {
                var result = await _personService.UpdatePerson(personName);
                if (result != null)
                    return Ok(result);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("{personGroupId}")]
        public async Task<ActionResult<List<int>>> Delete(int personGroupId, [FromBody]int[] rmPerson)
        {
            try
            {
                return await _personService.DeletePersons(personGroupId, rmPerson);
            }
            catch (FaceAPIException ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.ErrorCode + ' ' + ex.ErrorMessage);
            }
            catch (Exception ex) {
                _telementry.TrackException(ex);
                return StatusCode(500,ex.Message);
            }

        }
    }
}
