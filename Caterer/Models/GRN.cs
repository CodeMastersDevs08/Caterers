using Caterer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class GRN
    {
        [Key]
        public int GrnId { get; set; }
        public int RestaurantId { get; set; }
        [Column(TypeName = "int")]
        public int GNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string GRNNO { get; set; }
        [Required]
        public DateTime GRNDate { get; set; }
        public string GRNType { get; set; }
        public string PurchaseOrderNo { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        [Required]
        public int SupplierId { get; set; }
        [NotMapped]
        public List<string> SupplierName { get; set; }  
        public Supplier Supplier { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string Paymentmethod { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string SupplierInvoiceNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string AdditionalInformation { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string ProductName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Measurement { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime? ExpiryDate { get; set; }
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
        [Column(TypeName = "int")]
        public int addonetime { get; set; }
        [NotMapped]
        public List<string> ProductNames { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
    public class GRNViewModel
    {
        [Key]
        public int GrnId { get; set; }
        public int RestaurantId { get; set; }
        [Column(TypeName = "int")]
        public int GNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string GRNNO { get; set; }
        public DateTime GRNDate { get; set; }
        public string GRNType { get; set; }
        public string PurchaseOrderNo { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public int SupplierId { get; set; }
        [NotMapped]
        public List<Supplier> SupplierName { get; set; }  
        public Supplier Supplier { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Paymentmethod { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string SupplierInvoiceNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string AdditionalInformation { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string ProductName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Measurement { get; set; }
        [Column(TypeName = "datetime2")]
        [Required(ErrorMessage = "Please enter Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
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
        [Column(TypeName = "int")]
        public int addonetime { get; set; }
        public List<GRNViewModel> ProductDetails { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
}
