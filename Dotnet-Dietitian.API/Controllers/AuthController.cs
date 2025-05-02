using Dotnet_Dietitian.Application.Features.Results.AppUserResults;
using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Application.Queries.AppUserQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IMediator mediator, IJwtTokenGenerator jwtTokenGenerator)
        {
            _mediator = mediator;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Mevcut GetCheckAppUserQuery'i kullanarak kullanıcı doğrulama
            var query = new GetCheckAppUserQuery
            {
                Username = loginDto.Username,
                Password = loginDto.Password
            };

            var result = await _mediator.Send(query);

            if (!result.IsExist)
            {
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı" });
            }

            // Kullanıcı doğrulandıysa token üret
            var tokenResponse = _jwtTokenGenerator.GenerateToken(result);
            
            return Ok(new { 
                token = tokenResponse.Token, 
                expiration = tokenResponse.ExpireDate,
                username = result.Username,
                role = result.Role,
                userId = result.Id
            });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}