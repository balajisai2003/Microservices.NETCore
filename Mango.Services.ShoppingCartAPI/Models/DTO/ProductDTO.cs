using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ShoppingCartAPI.Models.DTO
{
    public class ProductDTO
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        public string? ProductName { get; set; }
        [Range(1, 1000)]
        public double Price { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
