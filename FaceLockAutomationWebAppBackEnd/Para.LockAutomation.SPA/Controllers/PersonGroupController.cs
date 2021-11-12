using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using Para.LockAutomation.Models;
using Para.LockAutomation.Service;

namespace Para.LockAutomation.SPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonGroupController : ControllerBase
    {
        private readonly PersonGroupService _personGroupService;
        private readonly TelemetryClient _telementry;

        public PersonGroupController(PersonGroupService personGroupService, TelemetryClient telemetry)
        {
            _personGroupService = personGroupService;
            _telementry = telemetry;
        }


        [HttpGet]
        public async Task<ActionResult<List<PersonGroupEntity>>> GetAll()
        {
            try
            {
                return Ok(await _personGroupService.GetAllPersonGroup());
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

        [HttpGet("{personGroupId}")]
        public async Task<ActionResult<PersonGroupEntity>> Get(int personGroupId)
        {
            try
            {
                return Ok(await _personGroupService.GetPersonGroup(personGroupId));
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


        [HttpPost("{personGroupName}/{confidence}")]
        public async Task<ActionResult<PersonGroupEntity>> Post(string personGroupName, float confidence)
        {
            try
            {
                var result = await _personGroupService.CreatePersonGroup(personGroupName, confidence);
                return Ok(result);
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


        [HttpDelete("{personGroupId}")]
        public async Task<ActionResult<int>> Delete(int personGroupId)
        {
            try
            {
                var result = await _personGroupService.DeletePersonGroup(personGroupId);
                return Ok(result);
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

        [HttpPut("ChangeDefaultGroup/{personGroupId}")]
        public async Task<ActionResult<int>> ChangeDefaultGroup(int personGroupId)
        {
            try
            {
                var result = await _personGroupService.ChangeDefaultPersonGroup(personGroupId);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("ChangeConfidence/{personGroupId}/{newConfidence}")]
        public async Task<ActionResult<PersonGroupEntity>> ChangeConfidenceThreshold(int personGroupId, float newConfidence)
        {
            try
            {
                if (newConfidence > 1)
                {
                    return BadRequest();
                }
                var result = await _personGroupService.ChangeConfidenceThreshold(personGroupId, newConfidence);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
