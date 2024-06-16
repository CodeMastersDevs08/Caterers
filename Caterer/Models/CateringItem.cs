using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
namespace Caterer.Models
{
    public class CateringItem
    {
        public int CateringItemId { get; set; }
        public int RestaurantId { get; set; }
        public MenuCategory MenuCategory { get; set; }
        public int MenuCategoryId { get; set; }
        public string MenuCategoryName { get; set; }
        public int SelectedItem { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public int CateringCategoryId { get; set; }
        public int? CateringExtrasId { get; set; }
        public CateringExtra CateringExtras { get; set; }
        public List<CateringExtra> ExtraList { get; set; }
        public string ItemDescription { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? DineInPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? VATPercentage { get; set; }
        public string ItemImage { get; set; }
        public bool ItemAvailable { get; set; }
        public bool EnableVariants { get; set; }
        public bool EnableAlwaysAvailable { get; set; }
        public bool Monday { get; set; }
        public DateTime? MondayStart { get; set; }
        public DateTime? MondayEnd { get; set; }
        public bool Tuesday { get; set; }
        public DateTime? TuesdayStart { get; set; }
        public DateTime? TuesdayEnd { get; set; }
        public bool Wednesday { get; set; }
        public DateTime? WednesdayStart { get; set; }
        public DateTime? WednesdayEnd { get; set; }
        public bool Thursday { get; set; }
        public DateTime? ThursdayStart { get; set; }
        public DateTime? ThursdayEnd { get; set; }
        public bool Friday { get; set; }
        public DateTime? FridayStart { get; set; }
        public DateTime? FridayEnd { get; set; }
        public bool Saturday { get; set; }
        public DateTime? SaturdayStart { get; set; }
        public DateTime? SaturdayEnd { get; set; }
        public bool Sunday { get; set; }
        public DateTime? SundayStart { get; set; }
        public DateTime? SundayEnd { get; set; }
    }
    public class CateringItemViewModel
    {
        public int CateringItemId { get; set; }
        public int RestaurantId { get; set; }
        public MenuCategory MenuCategory { get; set; }
        public int MenuCategoryId { get; set; }
        public string MenuCategoryName { get; set; }
        public int SelectedItem { get; set; }
        [Required(ErrorMessage = "ItemName is required.")]
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public int CateringCategoryId { get; set; }
        public int? CateringExtrasId { get; set; }
        public CateringExtra CateringExtras { get; set; }
        public List<CateringExtra> ExtraList { get; set; }
        public string ItemDescription { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? DineInPrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? VATPercentage { get; set; }
        [DataType(DataType.Upload)]
        public string ProductImage { get; set; }
        public bool ItemAvailable { get; set; }
        public bool EnableVariants { get; set; }
        public bool EnableAlwaysAvailable { get; set; }
        public bool Monday { get; set; }
        public DateTime? MondayStart { get; set; }
        public DateTime? MondayEnd { get; set; }
        public bool Tuesday { get; set; }
        public DateTime? TuesdayStart { get; set; }
        public DateTime? TuesdayEnd { get; set; }
        public bool Wednesday { get; set; }
        public DateTime? WednesdayStart { get; set; }
        public DateTime? WednesdayEnd { get; set; }
        public bool Thursday { get; set; }
        public DateTime? ThursdayStart { get; set; }
        public DateTime? ThursdayEnd { get; set; }
        public bool Friday { get; set; }
        public DateTime? FridayStart { get; set; }
        public DateTime? FridayEnd { get; set; }
        public bool Saturday { get; set; }
        public DateTime? SaturdayStart { get; set; }
        public DateTime? SaturdayEnd { get; set; }
        public bool Sunday { get; set; }
        public DateTime? SundayStart { get; set; }
        public DateTime? SundayEnd { get; set; }
    }
}
