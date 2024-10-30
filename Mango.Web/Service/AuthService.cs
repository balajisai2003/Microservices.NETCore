using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utils;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = registrationRequestDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.AuthAPIBase + "/api/Auth/AssignRole",
                AccessToken = "",
            });
        }
        public async Task<ResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = loginRequestDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.AuthAPIBase + "/api/Auth/Login",
                AccessToken = "",
            },Bearer:false);
        }
        public async Task<ResponseDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = registrationRequestDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.AuthAPIBase + "/api/Auth/Register",
                AccessToken = "",
            }, Bearer: false);
        }
    }
}
