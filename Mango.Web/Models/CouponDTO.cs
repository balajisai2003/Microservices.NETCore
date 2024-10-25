using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class CouponDTO
    {
        
        public int CouponId { get; set; }
        [Required(ErrorMessage = "Coupon code is required")]
        public string CouponCode { get; set; }
        [Required(ErrorMessage = "Discount amount is required")]
        public double DiscountAmount { get; set; }
        [Required(ErrorMessage = "Minimum Amount is required")]
        public int MinAmount { get; set; }
    }
}
