using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto logInRequestDto = new();
            return View(logInRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            ResponseDto loginResponse = await _authService.LoginAsync(request);
            if(loginResponse != null && loginResponse.IsSuccess)
            {
                LoginResponseDto login = JsonConvert.DeserializeObject<LoginResponseDto>(loginResponse.Result.ToString());

                await SignInUserAsync(login);
                _tokenProvider.SetToken(login.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = loginResponse.Message;
                return View(request);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text = SD.RoleAdmin , Value = SD.RoleAdmin},
                new SelectListItem{Text = SD.RoleCustomer , Value = SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto request)
        {
            ResponseDto response = await _authService.RegisterAsync(request);
            ResponseDto assignRole;

            if(response != null && response.IsSuccess)
            {
                if (string.IsNullOrEmpty(request.Role))
                {
                    request.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(request);
                if(assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = response.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View(request);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(model.Token);
            var claims = jwtToken.Claims;
            
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            identity.AddClaim(new Claim(ClaimTypes.Name, 
                jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        } 
    }
}
