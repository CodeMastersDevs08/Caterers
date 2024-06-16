using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class Tax
    {
        [Key]
        public int TaxId { get; set; }
        public int RestaurantId { get; set; }
        [Column(TypeName = "int")]
        public int TaxNo { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string TaxName { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string TaxType { get; set; }
        [Column(TypeName = "decimal")]
        public decimal TaxPercentage { get; set; }
    }
    public class TaxEditViewModel
    {
        public int RestaurantId { get; set; }
        public List<Tax> Taxes { get; set; }
    }
}