using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class MenuCategory
    {
        [Key]
        public int MenuCategoryId { get; set; }
        public int RestaurantId { get; set; }
        public string MenuCategoryName { get; set; }
        public int SelectedItem { get; set; }
        public bool ItemAvailable { get; set; }
        public ICollection<MenuItem> MenuItems { get; set; }
    }
}
