using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class CateringCategory
    {
        [Key]
        public int CateringCategoryId { get; set; }
        public int RestaurantId { get; set; }
        public decimal CategoryPrice { get; set; }
        public string CateringCategoryName { get; set; }
        public bool ItemAvailable { get; set; }
        public ICollection<CateringItem> CateringItems { get; set; }
    }
}
