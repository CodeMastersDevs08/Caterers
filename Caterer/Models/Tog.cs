using Caterer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class Tog
    {
        [Key]
        public int TogId { get; set; }
        public int RestaurantId { get; set; }
        [Column(TypeName = "int")]
        public int TNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string TogNO { get; set; }
        [Required]
        public DateTime TogDate { get; set; }
        [Required(ErrorMessage = "Please select a warehouse")]
        public int ToWarehouseId { get; set; }
        public string ToWarehouse { get; set; }
        [NotMapped]
        public List<string> ToWarehouseName { get; set; } 
        public Warehouse Warehouse { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string AdditionalInformation { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string ProductName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Measurement { get; set; }
        [Column(TypeName = "decimal")]
        [Required]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal")]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Total { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Status { get; set; }
        [NotMapped]
        public List<string> ProductNames { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
     public class TogViewModel
    {
        [Key]
        public int TogId { get; set; }
        public int RestaurantId { get; set; }
        [Column(TypeName = "int")]
        public int TNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string TogNO { get; set; }
        public DateTime TogDate { get; set; }
        [Required(ErrorMessage = "Please select a warehouse")]
        public int ToWarehouseId { get; set; }
        public string ToWarehouse { get; set; }
        [NotMapped]
        public List<string> ToWarehouseName { get; set; }  
        public Warehouse Warehouse { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string AdditionalInformation { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string ProductName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Measurement { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal")]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Total { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Status { get; set; }
        public List<TogViewModel> ProductDetails { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}
