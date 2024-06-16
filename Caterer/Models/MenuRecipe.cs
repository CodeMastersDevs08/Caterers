using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class MenuRecipe
    {
        [Key]
        public int MenuRecipeId { get; set; }
        public int? RestaurantId { get; set; }
        [Required]
        [DisplayName("Recipe  Name")]
        public string MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
        public string? MenuItemName { get; set; }
        public int? Packs { get; set; }
        [DisplayName("Category")]
        public string? MenuCategory { get; set; }
        [DisplayName("Image")]
        public string? MenuRecipeImage { get; set; }
        [DisplayName("Product Name")]
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public string? ProductName { get; set; }
        [DisplayName("Unit")]
        public string? ProductUnit { get; set; }
        [DisplayName("Category")]
        public string? ProductCategory { get; set; }
        [Required]
        [DisplayName("Recipe Qty")]
        public decimal MenuRecipeQty { get; set; }
        [DisplayName("ProductType")]
        public string ProductType { get; set; }
        public int MenuRecipeNo { get; set; }
        public string Productcode { get; set; }
    }
    public class MenuRecipeViewModel
    {
        public int MenuRecipeId { get; set; }
        public int RestaurantId { get; set; }
        [DisplayName("Recipe Name")]
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        [DisplayName("Menu Name")]
        public string MenuCategory { get; set; }
        [DisplayName("Image")]
        public string MenuRecipeImage { get; set; }
        public int? Packs { get; set; }
        [DisplayName("Product Name")]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        [DisplayName("Unit")]
        public string ProductUnit { get; set; }
        [DisplayName("Category")]
        public string ProductCategory { get; set; }
        [DisplayName("Recipe Qty")]
        public decimal MenuRecipeQty { get; set; }
        [DisplayName("ProductType")]
        public string ProductType { get; set; }
        public int MenuRecipeNo { get; set; }
        public string Productcode { get; set; }

    }
    public class EditMenuRecipeViewModel
    {
        public MenuRecipe MenuRecipe { get; set; }
        public List<MenuRecipeItemViewModel> MenuRecipeItems { get; set; }
    }
    public class MenuRecipeItemViewModel
    {
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string ProductUnit { get; set; }
        public string ProductCategory { get; set; }
        public string MenuRecipeQty { get; set; }
    }
}