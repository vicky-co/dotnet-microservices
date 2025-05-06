using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto? response = await _baseService.SendAsync(new()
            {
                ApiType = ApiType.POST,
                Url = AuthAPIBase+ "/api/auth/AssignRole",
                Data = registrationRequestDto,
            });

            return response;
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.POST,
                Data = loginRequestDto,
                Url = AuthAPIBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}
