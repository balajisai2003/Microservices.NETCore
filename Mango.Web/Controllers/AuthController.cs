using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;

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

        //[HttpPost]
        //public IActionResult Login(LoginRequestDTO loginRequestDTO)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    return View();
        //}

        public IActionResult Register()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Register(LoginRequestDTO loginRequestDTO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    return View();
        //}
    }
}
