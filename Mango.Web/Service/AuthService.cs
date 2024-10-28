using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utils;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly BaseService _baseService;

        public AuthService(BaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = registrationRequestDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/Auth/AssignRole",
                AccessToken = "",
            });
        }
        public async Task<ResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = loginRequestDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/Auth/Login",
                AccessToken = "",
            });
        }
        public async Task<ResponseDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = registrationRequestDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/Auth/Register",
                AccessToken = "",
            });
        }
    }
}
