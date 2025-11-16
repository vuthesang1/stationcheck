using Microsoft.AspNetCore.Mvc;

namespace StationCheck.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashController : ControllerBase
{
    [HttpGet("generate")]
    public IActionResult GenerateHash([FromQuery] string password)
    {
        if (string.IsNullOrEmpty(password))
            return BadRequest("Password required");
            
        var hash = BCrypt.Net.BCrypt.HashPassword(password, 12);
        return Ok(new { password, hash });
    }
    
    [HttpGet("verify")]
    public IActionResult VerifyHash([FromQuery] string password, [FromQuery] string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return BadRequest("Password and hash required");
            
        var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        return Ok(new { password, hash, isValid });
    }
}
