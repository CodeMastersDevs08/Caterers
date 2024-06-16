using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class Production
    {
        [Key]
        public int ProductionId { get; set; }
        public DateTime? ProductionDate { get; set; }
        [DisplayName("Production Status")]
        public string? ProductionStatus { get; set; }
        public int OrderDetailsWebsiteId { get; set; }
        [DisplayName("Order Status")]
        public string? OrderStatus { get; set; }
        public int RestaurantId { get; set; }
        [DisplayName("Main Category")]
        public int CateringCategoryId { get; set; }
        [DisplayName("Total Number of Packs")]
        public int TotalNoOfPacks { get; set; }
        [DisplayName("Extra Menu Packs")]
        public int ExtraPacks { get; set; }
        public int MenuItemId { get; set; }
        [DisplayName("Menu Category")]
        public string MenuCategory { get; set; }
        [DisplayName("Item Name")]
        public string MenuItemName { get; set; }
        public int ProductId { get; set; }
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        [DisplayName("Product Category")]
        public string ProductCategory { get; set; }
        [DisplayName("Product Type")]
        public string ProductType { get; set; }
        [DisplayName("Unit")]
        public string ProductUnit { get; set; }
        [DisplayName("Pre-Preparing Packs")]
        public int MenuRecipePacks { get; set; }
        [DisplayName("Quantity")]
        public decimal? MenuRecipeQty { get; set; }
        public decimal? UsedQuantity { get; set; } = null;
        public decimal? TotalQuantity { get; set; } = null;
        public int? WarehouseId { get; set; }
        public string ProductCode { get; set; }
    }
}