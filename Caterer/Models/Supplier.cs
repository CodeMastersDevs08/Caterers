using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
namespace Caterer.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }
        [NotMapped]
        public List<string> WarehouseName { get; set; }  
        public Warehouse Warehouse { get; set; }
        public int WarehouseId { get; set; }  
        public int RestaurantId { get; set; }
        public string SupplierName { get; set; }
         [Required(ErrorMessage = "Email is required")]
         public string Email { get; set; }
        [Required]
        public long PhoneNumber { get; set; }
        public string? Status { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? CreditPeriod { get; set; }
        public decimal? CreditLimit { get; set; }
        public string? ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
    }
}
