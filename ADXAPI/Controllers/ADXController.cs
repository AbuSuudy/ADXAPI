using ADXService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ADXAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize (Roles= "ADXUser")]
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
                return Problem(ex.Message);
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
                    return Problem("Storm Data table doesn't exsit.");
                }

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
