using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utils;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;

        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                Data = couponDTO,
                ApiType = SD.ApiType.POST,
                Url = SD.CouponAPIBase + "/api/coupon/Create",
                AccessToken = "",
            });
        }

        public async Task<ResponseDTO?> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO 
            { 
                ApiType = SD.ApiType.DELETE, 
                Url = SD.CouponAPIBase + "/api/coupon/Delete", 
                AccessToken = "" 
            });
        }

        public async Task<ResponseDTO?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO
            {

                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon",
                AccessToken = "",
            });
        }

        public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode"+couponCode,
                AccessToken = "",
            });
        }

        public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetById/"+id,
                AccessToken = "",
            });
        }

        public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestDTO 
            {
                Data = couponDTO,
                ApiType = SD.ApiType.GET, 
                Url = SD.CouponAPIBase + "/api/coupon/Update", 
                AccessToken = "" 
            });
        }
    }
}
