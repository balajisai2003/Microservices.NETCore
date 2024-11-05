using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public CartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggedUser());
        }

        private async Task<CartDTO> LoadCartBasedOnLoggedUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _shoppingCartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDTO;
            }
            return new CartDTO();
        }


        
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            var response = await _shoppingCartService.ApplyCouponAsync(cartDTO.CartHeader);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = response.Message;
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["error"] = response.Message;
            return View();
        }

        public async Task<IActionResult> EmailCart()
        {
            CartDTO cartDTO = await LoadCartBasedOnLoggedUser();
            cartDTO.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            var response = await _shoppingCartService.EmialCart(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = response.Message;
                return RedirectToAction(nameof(CartIndex));
            }
            TempData["error"] = response?.Message;
            return View();
        }


        public async Task<IActionResult> RemoveFromCart(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value;
            var response = await _shoppingCartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been removed from the cart";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
    }
}
