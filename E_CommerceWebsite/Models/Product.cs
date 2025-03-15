using System.ComponentModel.DataAnnotations;

namespace E_CommerceWebsite.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name must be between 5 and 100 characters", MinimumLength = 5)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Product description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Product price is required")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Product quantity is required")]
        public int Quantity { get; set; }

        [Required]
        public string Image { get; set; }


    }
}
