using ADXAPI.Filter;
using ADXService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ADXAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [RequiresClaimAttribute("ADXAccess", "True")]
    [ApiController]
    public class ADXController : ControllerBase
    {
        private IADXAccess _adxAccces;
        public ADXController(IADXAccess adxAccces) {

            _adxAccces = adxAccces;
        }

        [HttpGet]
        [EndpointSummary("Setup Storm Event Table")]
        [EndpointDescription("Create table and ingest Data")]
        public async Task<IActionResult> SetupStormEventTable()
        {
            try
            {
                if (!_adxAccces.CheckIfTableExist())
                {
                    await _adxAccces.CreateTable();
                    await _adxAccces.IngestionMapping();
                    await _adxAccces.Batching();

                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet]
        [EndpointSummary("Get Storm Data")]
        [EndpointDescription("Requires ADXAccess claim to be set to true")]
        public  IActionResult GetStormData()
        {
            try
            {
                if (_adxAccces.CheckIfTableExist())
                {
                    var data = _adxAccces.StormEventsData();

                    return Ok(data);
                }
                else
                {
                    return BadRequest("Table is not created");
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
