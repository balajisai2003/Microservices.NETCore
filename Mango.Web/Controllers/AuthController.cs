using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new LoginRequestDTO();
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
        {
           
            if (ModelState.IsValid)
            {
                ResponseDTO responseDTO = await _authService.LoginAsync(loginRequestDTO);
                if (responseDTO != null && responseDTO.IsSuccess)
                {
                    LoginResponseDTO loginResponseDTO = JsonConvert.DeserializeObject<LoginResponseDTO>(responseDTO.Result.ToString());
                    await SignInUserAsync(loginResponseDTO);
                    _tokenProvider.SetToken(loginResponseDTO.Token);
                    TempData["success"] = "Login Successful";
                    return RedirectToAction("Index", "Home");
                }
                if (responseDTO != null)
                {
                    TempData["error"] = responseDTO.Message;
                }
                else
                {
                    TempData["error"] = "Invalid Credentials";
                }

            }
            return View(loginRequestDTO);
        }


        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var roleList = new List<SelectListItem>
                                        {
                                            new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                                            new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer }
                                        };
            ViewBag.RoleList = roleList;
            if (ModelState.IsValid)
            {
                ResponseDTO result = await _authService.RegisterAsync(registrationRequestDTO);
                if (result != null && result.IsSuccess)
                {
                    if (string.IsNullOrWhiteSpace(registrationRequestDTO.Role))
                    {
                        registrationRequestDTO.Role = SD.RoleCustomer;
                    }
                    var assingRole = await _authService.AssignRoleAsync(registrationRequestDTO);
                    if (assingRole != null && assingRole.IsSuccess)
                    {
                        TempData["success"] = "Registration successful";
                        return RedirectToAction(nameof(Login));
                    }
           
                }
                TempData["error"] = "Registration failed " + result.Message;

                return View();
            }
            TempData["error"] = "Registration failed " ;
            
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(LoginResponseDTO loginResponseDTO)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginResponseDTO.Token);

            var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            Identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            Identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value));
            Identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name).Value));
            Identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email).Value));
            Identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c => c.Type == "role").Value));



            var principle = new ClaimsPrincipal(Identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principle);
        }

    }
}
