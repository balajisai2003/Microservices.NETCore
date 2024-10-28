using Mango.Services.AuthAPI.Models.DTO;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ResponseDTO _responseDTO;


        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDTO = new ResponseDTO();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            var errorMessage = await _authService.Register(registrationRequestDTO);

            _responseDTO.Message = errorMessage;
            if (!string.IsNullOrEmpty(errorMessage) && errorMessage != "User created successfully")
            {
                _responseDTO.IsSuccess = false;
                return BadRequest(_responseDTO);

            }
            return Ok(_responseDTO);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseDTO>> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            LoginResponseDTO loginResponseDTO = await _authService.Login(loginRequestDTO);
            if (loginResponseDTO.User == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Login Failed";
                return _responseDTO;
            }
            _responseDTO.Result = loginResponseDTO;
            return Ok(_responseDTO);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            bool assignRoleSuccessful = await _authService.AssignRole(registrationRequestDTO.Email, registrationRequestDTO.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Error Encontered";
                return Unauthorized(_responseDTO);
            }
            return Ok(_responseDTO);
        }
    }
}
