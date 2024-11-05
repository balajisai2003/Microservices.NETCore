using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utils;

namespace Mango.Web.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;

        public ShoppingCartService(IBaseService baseService)
        {
            _baseService = baseService;
        }


        public async Task<ResponseDTO> ApplyCouponAsync(CartHeaderDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",

            });
        }

        public async Task<ResponseDTO> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Data = "",
                Url = SD.ShoppingCartAPIBase + $"/api/cart/GetCart/{userId}",

            });
        }

        public async Task<ResponseDTO> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart",

            });
        }

        public async Task<ResponseDTO> UpsertCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.ShoppingCartAPIBase + "/api/cart/CartUpsert",

            });
        }
    }
}
