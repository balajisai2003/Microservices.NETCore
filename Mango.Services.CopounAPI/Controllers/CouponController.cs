using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Azure.Core.HttpHeader;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDTO _response;
        private IMapper _mapper;

        public CouponController(AppDbContext appDbContext, IMapper mapper)
        {
            this._appDbContext = appDbContext;
            _mapper = mapper;
            _response = new ResponseDTO();
        }

        [HttpGet]
        public async Task<ResponseDTO> Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = await _appDbContext.Coupons.ToListAsync();
                if (coupons == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<IEnumerable<CouponDTO>>(coupons);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetById/{id:int}")]
        public object Get(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                }

                _response.Result = _mapper.Map<CouponDTO>(coupon); ;
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public object Get(string code)
        {
            try
            {
                Coupon? coupon = _appDbContext.Coupons.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                if(coupon == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.Result = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Route("Create")]
        public ResponseDTO Post([FromBody] CouponDTO couponDTO)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDTO);
                _appDbContext.Coupons.Add(coupon);
                _appDbContext.SaveChanges();

                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [HttpPost]
        [Route("Update")]
        public ResponseDTO Put([FromBody] CouponDTO couponDTO)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDTO);
                _appDbContext.Coupons.Update(coupon);
                _appDbContext.SaveChanges();

                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [HttpDelete]
        [Route("Delete/{id:int}")]
        public ResponseDTO Delete(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.FirstOrDefault(c => c.CouponId == id);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                }
                else
                {
                    _appDbContext.Coupons.Remove(coupon);
                    _appDbContext.SaveChanges();
                    _response.Result = _mapper.Map<CouponDTO>(coupon);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
