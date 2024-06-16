using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
namespace Caterer.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderId { get; set; }
        [Column(TypeName = "int")]
        public int PNo { get; set; }
        [Column(TypeName = "int")]
        public int RestaurantId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? PurchaseOrderNo { get; set; }
        public int SupplierId { get; set; }
        [NotMapped]
        public List<string> SupplierName { get; set; }
        public Supplier Supplier { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseOrderDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpectedDate { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Paymentmethod { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? DeliveryInstruction { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string ProductName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Measurement { get; set; }
        [Column(TypeName = "decimal")]
        public decimal InStock { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Incoming { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal")]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Total { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Status { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        [NotMapped]
        public List<PurchaseOrder> RelatedPurchaseOrders { get; set; }
    }
    public class PurchaseViewModel
    {
        [Key]
        public int PurchaseOrderId { get; set; }
        [Column(TypeName = "int")]
        public int PNo { get; set; }
        [Column(TypeName = "int")]
        public int RestaurantId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? PurchaseOrderNo { get; set; }
        public int SupplierId { get; set; }
        [NotMapped]
        public List<Supplier> SupplierName { get; set; }  
        public Supplier Supplier { get; set; }
        public DateTime PurchaseOrderDate { get; set; }
        public DateTime ExpectedDate { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Paymentmethod { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? DeliveryInstruction { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string ProductName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Measurement { get; set; }
        [Column(TypeName = "decimal")]
        public decimal InStock { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Incoming { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal")]
        public decimal UnitCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal TotalCost { get; set; }
        [Column(TypeName = "decimal")]
        public decimal Total { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Status { get; set; }
        public PurchaseOrder MainPurchaseOrder { get; set; }
        public List<PurchaseOrder> RelatedPurchaseOrder { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }
    public class PurchaseOrderEditViewModel
    {
        public List<PurchaseOrder> PurchaseOrder { get; set; }
        [NotMapped]
        public List<Supplier> SupplierName { get; set; }  
        public Supplier Supplier { get; set; }
    }
}