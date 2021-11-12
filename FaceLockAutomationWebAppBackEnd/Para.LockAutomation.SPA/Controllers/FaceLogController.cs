using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using Para.LockAutomation.Service;

namespace Para.LockAutomation.SPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceLogController : ControllerBase
    {
        private readonly FaceLogService _faceLogService;
        private readonly TelemetryClient _telementry;

        public FaceLogController(FaceLogService faceLogService, TelemetryClient telementry)
        {
            _faceLogService = faceLogService;
            _telementry = telementry;
        }

        [HttpGet("{personGroupId}")]
        public async Task<ActionResult<FaceLogSearchResultDTO>> GetFaceLogInPersonGroup(int personGroupId, 
            [FromQuery]FilterModel filter )
        {
            if (filter == null || personGroupId <= 0)
            {
                return BadRequest();
            }

            try
            {
                return Ok(await _faceLogService.GetFaceLogInGroup(personGroupId, filter));
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("PersonFaceLog/{personGroupId}/{personId}")]
        public async Task<ActionResult<FaceLogSearchResultDTO>> GetPersonFaceLog(int personGroupId, int personId,
            [FromQuery]FilterModel filter)
        {
            if (filter == null || personGroupId <= 0)
            {
                return BadRequest();
            }
            try
            {
                return await _faceLogService.GetPersonFaceLogInGroup(personGroupId, personId, filter);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("UnknownFaceLog/{personGroupId}")]
        public async Task<ActionResult<FaceLogSearchResultDTO>> GetUnknownFaceLogInGroup(int personGroupId,
            [FromQuery]FilterModel filter)
        {
            if (filter == null || personGroupId <= 0)
            {
                return BadRequest();
            }
            try
            {
                return await _faceLogService.GetUnknownFaceLogInGroup(personGroupId, filter);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("KnownFaceLog/{personGroupId}")]
        public async Task<ActionResult<FaceLogSearchResultDTO>> GetKnownFaceLogInGroup(int personGroupId,
            [FromQuery]FilterModel filter)
        {
            if (filter == null || personGroupId <= 0)
            {
                return BadRequest();
            }
            try
            {
                return await _faceLogService.GetKnownFaceLogInGroup(personGroupId, filter);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("UndetectedFaceLog/{personGroupId}")]
        public async Task<ActionResult<FaceLogSearchResultDTO>> GetUndetectedFaceLogInGroup(int personGroupId,
            [FromQuery]FilterModel filter)
        {
            if (filter == null || personGroupId <= 0)
            {
                return BadRequest();
            }
            try
            {
                return await _faceLogService.GetUndetectedFaceLogInGroup(personGroupId, filter);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{personGroupId}")]
        public async Task<ActionResult<FaceLogEntity>> CreateFaceLog(int personGroupId, IFormFile image)
        {
            try
            {
                return await _faceLogService.CreateFaceLogBeforeUpload(personGroupId, image);
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{personGroupId}")]
        public async Task<ActionResult<int>> DeleteFaceLogs(int personGroupId, [FromBody]int[] faceLogIds)
        {
            try
            {
                return Ok(await _faceLogService.DeleteFaceLogs(personGroupId, faceLogIds));
            }
            catch (Exception ex)
            {
                _telementry.TrackException(ex);
                return StatusCode(500, ex.Message);
            }
        }
    }
}