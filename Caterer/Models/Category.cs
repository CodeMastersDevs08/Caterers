using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }    
        public string CategoryLogo { get; set; }

        public int RestaurantId { get; set; }


    }
    public class Categoryviewmodel
    {
        [Key]
        public int CategoryId { get; set; }
        public int RestaurantId { get; set; }
        public string CategoryName { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Choose Image")]
        public IFormFile ProductImage { get; set; }
    }
}

