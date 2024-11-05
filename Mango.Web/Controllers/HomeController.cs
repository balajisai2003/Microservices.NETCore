using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

		public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
			_productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index()
        {
			List<ProductDTO> products = new List<ProductDTO>();
			var response = await _productService.GetAllProductsAsync();
			if (response != null && response.IsSuccess)
			{
				products = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(products);
		}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var response = await _productService.GetProductByIdAsync(productId);
            ProductDTO? product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            if (product != null)
            {
                return View(product);
            }
            TempData["error"] = response.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {
            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = new CartHeaderDTO()
                {
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub).FirstOrDefault()?.Value
                }
            };


            CartDetailsDTO cartDetails = new CartDetailsDTO()
            {
                Quantity = productDTO.Quantity,
                ProductId = productDTO.ProductID,
                product = productDTO

            };
            List<CartDetailsDTO> cartDetailsDTOs = new List<CartDetailsDTO>();
            cartDetailsDTOs.Add(cartDetails);
            cartDTO.CartDetails = cartDetailsDTOs;

            var response = await _shoppingCartService.UpsertCartAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = response.Message;

                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = response.Message;

            return View(productDTO);
        }
    }
}
