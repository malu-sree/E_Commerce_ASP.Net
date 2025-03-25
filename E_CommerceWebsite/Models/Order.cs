using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_CommerceWebsite.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        public int CartId { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PaymentMethod { get; set; }


        [Required]
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }


        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public string UserName { get; set; }

        public Order()
        {
            Status = "Processing"; // Default value for Status
            CreatedAt = DateTime.Now; // Default value for CreatedAt
            OrderItems = new List<OrderItem>();
        }


    }

    
}