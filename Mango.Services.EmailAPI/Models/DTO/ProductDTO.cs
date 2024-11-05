using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Models.DTO
{
    public class ProductDTO
    {
     
        public int ProductID { get; set; }
        public string? ProductName { get; set; } = "";
        [Range(1, 1000)]
        public double Price { get; set; } = 0;
        public string? Description { get; set; } = "";
        public string? CategoryName { get; set; } = "";
        public string? ImageUrl { get; set; } = "";
        [Range(1, 10)]
        public int Quantity { get; set; } = 1;
    }
}
