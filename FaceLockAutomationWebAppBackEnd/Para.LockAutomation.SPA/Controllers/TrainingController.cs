using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Para.LockAutomation.Service;

namespace Para.LockAutomation.SPA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly TrainingService _trainingService;
        private readonly TelemetryClient _telementry;

        public TrainingController(TrainingService trainingService, TelemetryClient telementry)
        {
            _trainingService = trainingService;
            _telementry = telementry;
        }

        [HttpGet("{personGroupId}")] 
        public async Task<ActionResult<TrainingStatus>> Get(int personGroupId)
        {
            try
            {
                return Ok(await _trainingService.GetTrainingStatus(personGroupId));
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

        [HttpPost("{personGroupId}")]
        public async Task<ActionResult> Post(int personGroupId)
        {
            try
            {
                await _trainingService.TrainingPersonGroup(personGroupId);
                return Ok();
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
    }
}