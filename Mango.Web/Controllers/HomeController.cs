using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IProductService _productService;

		public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
			_productService = productService;
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
    }
}
