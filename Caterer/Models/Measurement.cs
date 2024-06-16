using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class Measurement
    {
        [Key]
        public int MeasurementId { get; set; }
        public int RestaurantId { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string MeasurementName { get; set;}
    }
}
