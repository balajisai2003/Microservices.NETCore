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
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(list);
        }

        public IActionResult CreateCoupon()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateCoupon(CouponDTO couponDTO)
		{
            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync(couponDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
                
            }

            return View(couponDTO);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            ResponseDTO? response = await _couponService.DeleteCouponAsync(id);
			if (response != null && response.IsSuccess)
			{
                TempData["success"] = "Coupon deleted successfully";

                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }


        public async Task<IActionResult> UpdateCoupon(int id)
        {
            CouponDTO couponDTO = new CouponDTO();
			ResponseDTO? response = await _couponService.GetCouponByIdAsync(id);
            couponDTO = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
			return View(couponDTO);
		}

        [HttpPost]
		public async Task<IActionResult> UpdateCoupon(CouponDTO couponDTO)
		{
			if (ModelState.IsValid)
			{
                var response = await _couponService.UpdateCouponAsync(couponDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon updated successfully";

                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
                
			}

            return View(couponDTO);
        }

	}
}
