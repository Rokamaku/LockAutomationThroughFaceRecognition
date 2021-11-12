using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using Para.LockAutomation.DTO;
using Para.LockAutomation.Models;
using Para.LockAutomation.Service;
using Para.LockAutomation.Utils;

namespace LockAutomation_WA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceController : ControllerBase
    {
        private readonly FaceService _faceService;
        private readonly TelemetryClient _telementry;

        public FaceController(FaceService faceService, TelemetryClient telementry)
        {
            _faceService = faceService;
            _telementry = telementry;
        }

        [HttpGet("{personGroupId}/{personId}")]
        public async Task<ActionResult<List<FaceEntity>>> GetAll(int personGroupId, int personId)
        {
            try
            {
                return Ok(await _faceService.GetFaces(personGroupId, personId));
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

        [HttpGet("{personGroupId}/{personId}/{faceId}")]
        public async Task<ActionResult<FaceEntity>> Get(int personGroupId, int personId, int faceId)
        {
            try
            {
                return await _faceService.GetFace(personGroupId, personId, faceId);
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


        [HttpPost("{personGroupId}/{personId}")]
        public async Task<ActionResult<List<FaceEntity>>> Post(int personGroupId, int personId, [FromForm]List<IFormFile> files)
        {
            try
            {
                if (files.Count == 0)
                    return BadRequest();

                return Ok(await _faceService.AddFaces(personGroupId, personId, files));
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

        [HttpDelete("{personGroupId}/{personId}")]
        public async Task<ActionResult<List<int>>> Delete(int personGroupId, int personId, [FromBody] int[] faceIds)
        {
            try
            {
                return Ok(await _faceService.DeleteFaces( personGroupId, personId, faceIds));
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
