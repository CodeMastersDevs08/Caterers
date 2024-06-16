﻿using System.ComponentModel.DataAnnotations;
namespace Caterer.Models
{
    public class Extra
    {
        [Key]
        public int ExtrasId { get; set; }
        public int MenuItemId { get; set; }
        public string ExtraName { get; set; }
        public decimal ExtraPrice { get; set; }
        public string AddonCategory { get; set; }
    }
}