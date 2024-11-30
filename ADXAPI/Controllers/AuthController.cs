using ADXAPI.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ADXAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        List<User> users = new List<User>
        {
            new User
            {
                Email = "Test@gmail.com",
                Password = "Test",
                ADXUser = true
            },
            new User
            {
                Email = "Test2@gmail.com",
                Password = "Test",
                ADXUser = false
            }
        };

        IJWTGenerator jwtGenerator;
        public AuthController(IJWTGenerator jwtGenerator) 
        { 
        
            this.jwtGenerator = jwtGenerator;
        }

  
        [EndpointSummary("Login")]
        [EndpointDescription("returns `jwt`")]
        [HttpGet(Name = "Login")]
        public IActionResult Login(string email, string password)
        {
            try
            {
                if (!users.Any(x => x.Email == email))
                {
                    return NotFound("User not found");

                }
                var user = users.FirstOrDefault(x => x.Email == email);

                if (password != user.Password)
                {
                    return Unauthorized("Wrong email or password");
                }

                return Ok(jwtGenerator.GenerateToken(user.Email, user.ADXUser));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

            }
        }
    }
}
