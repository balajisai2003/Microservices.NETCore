using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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

        public IActionResult Logout()
        {
            return View();
        }

    }
}
