using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class CateringExtra
    {
        [Key]
        public int CateringExtrasId { get; set; }
        public int CateringItemId { get; set; }
        public string ExtraName { get; set; }
        public decimal ExtraPrice { get; set; }
        public string AddonCategory { get; set; }
    }
}
