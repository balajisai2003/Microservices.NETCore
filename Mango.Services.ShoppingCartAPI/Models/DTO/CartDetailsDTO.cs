using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class CartDetailsDTO
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        //public CartHeaderDTO CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDTO product { get; set; }
        public int Quantity { get; set; }
    }
}
