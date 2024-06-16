using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class OrderDetailWebsite
    {
        [Key]
        public int OrderDetailsWebsiteId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string ExtrasName { get; set; }
        public decimal ExtraPrice { get; set; }
        public string ExtraId { get; set; }
        public string Subcategory { get; set; }
        [Required]
        public int CateringCategoryId { get; set; }
        [Required]
        public string CateringItemId { get; set; }
        [Required]
        public string ItemName { get; set; }
        [Required]
        public string ItemPrice { get; set; }
        public string Status { get; set; }
        [Required]
        public int NoOfPerson { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public int SelfCollect { get; set; }
        public int Delivery { get; set; }
        public int Buffet { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        public string? UserAddress { get; set; }
        [Required]
        public decimal FoodCost { get; set; }
        [Required]
        public decimal Transport { get; set; }
        [Required]
        public decimal BuffetSetup { get; set; }
        [Required]
        public decimal Tax { get; set; }
        [Required]
        public decimal GrandTotal { get; set; }
        public int Card { get; set; }
        public int Cash { get; set; }
        public string? CardNumber { get; set; }
        public string? CCV { get; set; }
        [NotMapped]
        public string UpdatedStatus { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        [Required]
        [DisplayName("Date")]
        public string SelectedDate { get; set; }
        [NotMapped]
        public List<SelectListItem> DateOptions { get; set; }
        [Required]
        [DisplayName("Time")]
        public string SelectedTime { get; set; }
        [NotMapped]
        public List<SelectListItem> TimeOptions { get; set; }
        [ForeignKey("CateringCategoryId")]
        public CateringCategory CateringCategory { get; set; }
        public string CreatedAt { get; set; }
        public string OrderTime { get; set; }
        public string UpdatedAt { get; set; }
        public string DeletedAt { get; set; }
        public string? SoftDelete { get; set; }
        public string? DeliveryStatus { get; set; }
        public int? PartialPay { get; set; }
        public int? FullPay { get; set; }
        public decimal? PartialAmount { get; set; }
        public decimal? Discount { get; set; }
        //FOR EDIT ORDER
        public string ExtraCateringItemId { get; set; }
        public string ExtraItemName { get; set; }
        public string ExtraItemPrice { get; set; }
        public string ExtraNoOfPax { get; set; }
        public string ExtraSubcategory { get; set; }
        public bool AddExtra { get; set; }
        //FOR EXTRA CATERING CATEGORY
        public string ExtraCateringCategoryId { get; set; }
        public string ExtraCateringSubCategory { get; set; }
        public string ExtraCateringItemName { get; set; }
        public string ExtraCateringNoOfPax { get; set; }
        public string ExtraCateringItemPrice { get; set; }
        public string ExtrasCateringItemId { get; set; }
        public decimal? MenuAdjustmentTotal { get; set; }
        public decimal? AddExtraTotal { get; set; }
        public string? ExtraItemPriceTotal { get; set; }
        public string? ExtraCateringItemPriceTotal { get; set; }

    }
}