using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Caterer.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public int RestaurantId { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [StringLength(maximumLength: 100, ErrorMessage = "The product name shoule be between 2 and 100", MinimumLength = 3)]
        public string ProductName { get; set; } = string.Empty;
        public string ProductType { get; set; }
        [Display(Name = "Measurement")]
        public int? MeasurementId { get; set; }
        [NotMapped]
        public List<string> MeasurementName { get; set; }
        public Measurement Measurement { get; set; }
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }
        [NotMapped]
        public List<string> CategoryName { get; set; }
        public Category Category { get; set; }
        [Required]
        [Column(TypeName = "int")]
        public int productcode { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Barcode { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        [StringLength(maximumLength: 100, ErrorMessage = "The product Description shoule be between 2 and 100")]
        public string? ProductDescription { get; set; }
        [Required]
        [Column(TypeName = "Decimal")]
        public decimal? Quantity { get; set; }
        [Column(TypeName = "int")]
        public int? TaxId { get; set; }
        [NotMapped]
        public List<string> TaxName { get; set; }
        public Tax Tax { get; set; }
        [Column(TypeName = "Decimal")]
        public decimal? UnitPrice { get; set; }
        [Column(TypeName = "Decimal")]
        public decimal? UnitCost { get; set; }
        public int? StockControl { get; set; }
        public int? ExpireDate { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? Instock { get; set; }
        [Column(TypeName = "decimal")]
        public decimal? SafetyStock { get; set; }
        public string? ProductImage { get; set; }
        public decimal? OpeningStock { get; set; }
        public DateTime? OpeningStockDate { get; set; }
        public decimal? Mrp { get; set; }
        public bool? CreatedBy { get; set; }

    }
    public class ProductViewModel
    {
        [Key]
        public int ProductId { get; set; }
        public int RestaurantId { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        [Required]
        [StringLength(maximumLength: 100, ErrorMessage = "The product name should be between 2 and 100", MinimumLength = 3)]
        public string ProductName { get; set; } = string.Empty;
        public string ProductType { get; set; }
        public int? MeasurementId { get; set; }
        [NotMapped]
        public List<Measurement> MeasurementName { get; set; }
        public Measurement Measurement { get; set; }
        public int? CategoryId { get; set; }
        [NotMapped]
        public List<Category> CategoryName { get; set; }
        public Category Category { get; set; }
        public int productcode { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string? Barcode { get; set; }
        [StringLength(maximumLength: 100, ErrorMessage = "The product Description should be between 2 and 100")]
        public string? ProductDescription { get; set; }
        [Column(TypeName = "Decimal")]
        public decimal? Quantity { get; set; }
        public int? TaxId { get; set; }
        [NotMapped]
        public List<Tax> TaxName { get; set; }
        public Tax Tax { get; set; }
        public List<Tax> TaxList { get; set; }
        public IEnumerable<SelectListItem> TaxNameWithPercentage
        {
            get
            {
                if (TaxList == null)
                {
                    TaxList = new List<Tax>();
                }
                return TaxList.Select(tax => new SelectListItem
                {
                    Value = tax.TaxId.ToString(),
                    Text = $"{tax.TaxName} ({tax.TaxPercentage}%)"
                });
            }
        }
        public ProductViewModel()
        {
            TaxList = new List<Tax>();
        }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitCost { get; set; }
        public int? StockControl { get; set; }
        public int? ExpireDate { get; set; }
        public decimal? Instock { get; set; }
        public decimal? SafetyStock { get; set; }
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
        public string ProductImage { get; set; }
        public decimal? OpeningStock { get; set; }
        public DateTime? OpeningStockDate { get; set; }
        public decimal? Mrp { get; set; }
        public bool? CreatedBy { get; set; }
    }
}