using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Composition;
namespace Caterer.Models
{
    public class PreProduction
    {
        [Key]
        public int PreProductionId { get; set; }
        [DisplayName("Status")]
        public string? PreProductionStatus { get; set; }
        public string? Type { get; set; }
        [DisplayName("Prepared Date")]
        public DateTime? PreProductionDate { get; set; }
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        public int? RestaurantId { get; set; }
        [DisplayName("Warehouse  Name")]
        public int? WarehouseId { get; set; }
        [Required]
        public string MenuItemId { get; set; }
        [DisplayName("Recipe  Name")]
        public string? MenuItemName { get; set; }
        public int? Packs { get; set; }
        [DisplayName("Category")]
        public string? MenuCategory { get; set; }
        public string ProductId { get; set; }
        [DisplayName("Product Code")]
        public string? Productcode { get; set; }
        public string? ProductName { get; set; }
        [DisplayName("Unit")]
        public string? ProductUnit { get; set; }
        [DisplayName("Category")]
        public string? ProductCategory { get; set; }
        [Required]
        [DisplayName("Recipe Qty")]
        public decimal MenuRecipeQty { get; set; }
        [DisplayName("ProductType")]
        public string ProductType { get; set; }
        public decimal TotalQuantity { get; set; }
        [DisplayName("Total Production Packs")]
        public decimal TotalPack { get; set; }
        public decimal ProductTotalQuantity { get; set; }
        public string? Id { get; set; }
    }
}