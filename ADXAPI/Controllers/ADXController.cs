using ADXAPI.Filter;
using ADXService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace ADXAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ADXController : ControllerBase
    {
        private IADXAccess _adxAccces;
        public ADXController(IADXAccess adxAccces) {

            _adxAccces = adxAccces;
        }

        [HttpGet]
        [RequiresClaimAttribute("ADXAccess", "True")]
        [EndpointSummary("Get Storm Data")]
        [EndpointDescription("Requires ADXAccess claim to be set to true")]
        public IActionResult GetStormData()
        {

            return Ok(_adxAccces.RowCount());
        }
    }
}
