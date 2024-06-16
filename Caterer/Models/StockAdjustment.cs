using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
     public class StockAdjustment
    {
        [Key]
        public int StockAdjustmentId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime StockDate { get; set; }
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public string AdjustmentType { get; set; }
        public int Quantity { get; set; }
        public string AdjustmentReason { get; set; }
        public Product Product { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}
