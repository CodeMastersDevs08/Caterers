using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }
        public int RestaurantId { get; set; }
        [Required]
        public string WarehouseName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string BillingAddress { get; set; }
        [Required]
        public string BankDetails { get; set; }
    }
    public class WarehouseViewModel
    {
        [Key]
        public int WarehouseId { get; set; }
        public int RestaurantId { get; set; }
        [Required]
        public string WarehouseName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone Number is required.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string BillingAddress { get; set; }
        [Required]
        public string BankDetails { get; set; }
    }
    public class WarehousesViewModel
    {
        public List<Warehouse> Warehouses { get; set; }
        public int SelectedWarehouseId { get; set; }
    }
}