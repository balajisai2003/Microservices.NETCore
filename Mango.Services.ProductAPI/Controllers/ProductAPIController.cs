using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ResponseDTO _responseDTO;
        private readonly IMapper _mapper;


        public ProductAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _responseDTO = new ResponseDTO();
            _mapper = mapper;
        }


        //Get all Products
        [HttpGet]
        [AllowAnonymous]
        public async Task<ResponseDTO> Get()
        {
            try
            {
                List<Product> products = await _appDbContext.Products.ToListAsync();
                if(products != null)
                {
                    _responseDTO.Result = _mapper.Map<IEnumerable<ProductDTO>>(products);

                }
                
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess=false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }


        //Get Product by ID
        [HttpGet]
        [Route("GetById/{Id:int}")]
        public async Task<ResponseDTO> Get(int Id)
        {
            try
            {
                Product product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.ProductID == Id);
                if(product == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "Not Found";
                }
                else
                {
                    _responseDTO.Result = _mapper.Map<ProductDTO>(product);
                }
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess=false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }


        //Get Product by name
        [HttpGet]
        [Route("GetByName/{name}")]
        public async Task<ResponseDTO> Get(string name)
        {
            try
            {
                Product product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.ProductName.ToLower() == name.ToLower());
                if (product == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "Not Found";
                }
                _responseDTO.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }


        //Get Product by category
        [HttpGet]
        [Route("GetByCategory/{category}")]
        public async Task<ResponseDTO> GetByCategory(string category)
        {
            try
            {
                List<Product> products = await _appDbContext.Products.Where(x => x.CategoryName.ToLower() == category.ToLower()).ToListAsync();
                if (products == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "Not Found";
                }
                _responseDTO.Result = _mapper.Map<IEnumerable<ProductDTO>>(products);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }


        //Create Product
        [HttpPost]
        [Authorize(Roles= "ADMIN")]
        public async Task<ResponseDTO> Post([FromBody] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                await _appDbContext.Products.AddAsync(product);
                await _appDbContext.SaveChangesAsync();
                _responseDTO.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }


        //Update Product
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDTO> Put([FromBody] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                _appDbContext.Products.Update(product);
                await _appDbContext.SaveChangesAsync();
                _responseDTO.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }


        //Delete Product
        [HttpDelete]
        [Route("Delete/{Id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ResponseDTO> Delete(int Id)
        {
            try
            {
                Product product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.ProductID == Id);
                if (product == null)
                {
                    _responseDTO.IsSuccess = false;
                    _responseDTO.Message = "Not Found";
                    _responseDTO.Result = null;
                }
                _appDbContext.Products.Remove(product);
                await _appDbContext.SaveChangesAsync();
                _responseDTO.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                _responseDTO.Result = null;
            }
            return _responseDTO;
        }

    }
}
