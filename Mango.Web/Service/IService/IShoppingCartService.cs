using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IShoppingCartService
    {
        Task<ResponseDTO> GetCartByUserIdAsync(string userId);
        Task<ResponseDTO> UpsertCartAsync(CartDTO cartDTO);
        Task<ResponseDTO> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDTO> ApplyCouponAsync(CartHeaderDTO cartHeader);


    }
}
