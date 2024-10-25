using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? list = new();
            ResponseDTO? response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDTO couponDTO)
		{
            if (!ModelState.IsValid)
            {
                return View(couponDTO);
            }

            await _couponService.CreateCouponAsync(couponDTO);
            return RedirectToAction(nameof(CouponIndex));
        }

        //[HttpPost]

	}
}
