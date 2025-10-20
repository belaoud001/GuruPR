using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/test")]
public class TestController : ControllerBase
{
    public TestController()
    {
    }

[Authorize]
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API is working!");
    }
}
