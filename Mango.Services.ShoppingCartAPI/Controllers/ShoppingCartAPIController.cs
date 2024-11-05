using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private readonly ResponseDTO _response;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;


        public ShoppingCartAPIController(IMapper mapper, AppDbContext appDbContext, IProductService productService, ICouponService couponService)
        {
            _mapper = mapper;
            _appDbContext = appDbContext;
            _productService = productService;
            _couponService = couponService;
            _response = new ResponseDTO();
        }


        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartHeader cartHeader = await _appDbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
                IEnumerable<CartDetails> cartDetails = _appDbContext.CartDetails.Where(u => u.CartHeaderId == cartHeader.CartHeaderId);
                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(cartHeader),
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(cartDetails);
                IEnumerable<ProductDTO> productDTOs = await _productService.GetProducts();
                foreach (var detail in cart.CartDetails)
                {
                    foreach(var product in productDTOs)
                    {
                        if (product.ProductID == detail.ProductId)
                        {
                            detail.product = product;
                            break;
                        }
                    }
                    cart.CartHeader.CartTotal += detail.product.Price * detail.Quantity;
                }
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if(coupon != null && cart.CartHeader.CartTotal>coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                    else
                    {
                        _response.Message = "Coupon is valid but cart total is less than minimum amount";
                    }
                }
                _response.Result = cart;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
                _response.Result = null;
            }
            return _response;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartHeaderDTO cartHeader)
        {
            try
            {
                var cartHeaderFromDb = await _appDbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartHeader.UserId);
                cartHeaderFromDb.CouponCode = cartHeader.CouponCode;
                _appDbContext.Update(cartHeaderFromDb);
                await _appDbContext.SaveChangesAsync();
                _response.Result = true;
                _response.Message = "Coupon applied successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _appDbContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);
                cartHeaderFromDb.CouponCode = "";
                _appDbContext.Update(cartHeaderFromDb);
                await _appDbContext.SaveChangesAsync();
                _response.Result = true;
                _response.Message = "Coupon removed successfully";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }


        [HttpPost]
        [Route("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert([FromBody] CartDTO cartDTO)
        {

            try
            {
                var cartHeaderFromDb = await _appDbContext.CartHeaders
                    .FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId);

                if (cartHeaderFromDb == null)
                {
                    //###############Create details##################
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    await _appDbContext.CartHeaders.AddAsync(cartHeader);
                    await _appDbContext.SaveChangesAsync();
                    // we get cartheader id after saving it to db 
                    //then populating it to cartdetails
                    cartDTO.CartHeader.CartHeaderId = cartHeader.CartHeaderId;
                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    //now we are saving cartdetails in cartDTO to db
                    CartDetails cartDetails = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                    await _appDbContext.CartDetails.AddAsync(cartDetails);
                    await _appDbContext.SaveChangesAsync();
                    cartDTO.CartDetails.First().CartDetailsId = cartDetails.CartDetailsId;

                    _response.Message = "Created a new cart header and added a product to it";

                }
                else
                {
                    cartDTO.CartHeader.CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;

                    //################3Update details################3
                    //check if product exists in cart
                    var cartDetailsFromDb = await _appDbContext.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(u=>u.ProductId == cartDTO.CartDetails
                        .First().ProductId && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        // #############create new cart details###################
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId; //getting cartheaderid from db
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                        _appDbContext.CartDetails.Add(cartDetails);
                        await _appDbContext.SaveChangesAsync();
                        _response.Message = "Product added to cart";
                        
                    }
                    else
                    {
                        //####################update Quantity#################
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        cartDTO.CartDetails.First().Quantity += cartDetailsFromDb.Quantity;
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDTO.CartDetails.First());
                        _appDbContext.CartDetails.Update(cartDetails);
                        await _appDbContext.SaveChangesAsync();
                        _response.Message = "Updated Quantity of Product";

                    }
                }
                _response.IsSuccess = true;
                _response.Result = cartDTO;

            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId )
        {
            try
            {
                CartDetails cartDetails = await _appDbContext.CartDetails.FirstOrDefaultAsync(u => u.CartDetailsId == cartDetailsId);
                int totalCountOfCartItems = _appDbContext.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).ToList().Count();
                _appDbContext.CartDetails.Remove(cartDetails);
                _response.Message = "Product removed from cart";
                if (totalCountOfCartItems == 1)
                {
                    //this means this is the last item in the cart
                    var cartHeader = await _appDbContext.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _appDbContext.CartHeaders.Remove(cartHeader);
                    _response.Message = "Product removed from cart and cart is deleted";
                }
                await _appDbContext.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Result = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message.ToString();
            }
            return _response;
        }
    }
}




