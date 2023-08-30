using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok("ok");
        }



    }

       

       
    
}