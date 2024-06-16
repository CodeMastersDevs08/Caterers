using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Caterer.Models
{
    public class Restaurant
    {
        [Key]
        public int RestaurantId { get; set; }
        public int ResId { get; set; }
        public string Role { get;set; }
        public long UserId { get; set; }
        public string RestaurantName { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerPhone { get; set; }
        public string Password { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; }
        [Required]
        public double Fee { get; set; } = 0.00;
        [Required]
        [Display(Name = "Static Fee")]
        public double StaticFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "Stripe Fee")]
        public double StripeFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "Stripe Static Fee")]
        public double StripeStaticFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "PayNow Fee")]
        public double PayNowFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "PayNow Static Fee")]
        public double PayNowStaticFee { get; set; } = 0.00;
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }
        [Display(Name = "Can Auto Reject")]
        public bool CanAutoReject { get; set; }
        [Display(Name = "Can Auto Accept")]
        public bool CanAutoAccept { get; set; }
        [Display(Name = "Can Pickup")]
        public bool CanPickup { get; set; }
        [Display(Name = "Can Deliver")]
        public bool CanDeliver { get; set; }
        [Display(Name = "Can Deliver")]
        public bool CanFreeDeliver { get; set; }
        [Display(Name = "Can Preorder")]
        public bool CanPreorder { get; set; }
        [Display(Name = "Can Preorder")]
        public bool Disableordering { get; set; }
        [Display(Name = "Can Dine-in")]
        public bool CanDineIn { get; set; }
        [Display(Name = "Disable COD")]
        public bool DisableCOD { get; set; }
        [Display(Name = "Disable Cardpayment")]
        public bool DisableCardpayment { get; set; }
        [Display(Name = "Disable PayNow")]
        public bool DisablePayNow { get; set; }
        public string Logo { get; set; } = string.Empty;
        public string Cover { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Minimum")]
        public int? Minimum { get; set; }  
        [Display(Name = "Takeaway Minimum")]
        public int? TakeawayMin { get; set; }  
        [Display(Name = "Minutes")]
        public int? Minutes { get; set; }
        [Display(Name = "Auto-reject Wait Time")]
        public int? AutoRejectWaitTime { get; set; }
        [Display(Name = "QR Payment Wait Time")]
        public int? QRPaymentWaitTime { get; set; }
        [Display(Name = "Average order prepare time in minutes")]
        public int? AverageOrderPrepareTimeInMinutes { get; set; }
        [Display(Name = "Time slots separated in minutes")]
        public int? TimeSlotSeparatedInMinutes { get; set; }
        [Display(Name = "Delivery Preparation Time")]
        public int? DeliveryPreparationTime { get; set; }
        [Display(Name = "COD Service Tax")]
        public int? ServiceTax { get; set; }  
        [Display(Name = "Stripe Service Tax")]
        public int? StripeServiceTax { get; set; } 
        [Display(Name = "PayNow Service Tax")]
        public int? PayNowServiceTax { get; set; }  
        [Display(Name = "Dine-in Service Tax")]
        public int? ServiceTaxDineIn { get; set; }  
        [Display(Name = "Free Delivery Cost")]
        public int? FreeDeliveryCost { get; set; }
        [Display(Name = "Default Language")]
        public string? DefaultLanguage { get; set; }
        public string Currency { get; set; }
        [Display(Name = "Money conversion")]
        public bool DoConversion { get; set; } 
        public bool Active { get; set; } = true;
     
        //APPS
        [Display(Name = "Display allergens on menu")]
        public bool CanDispalyAllergensOnMenu { get; set; }
        [Display(Name = "Enable Coupons")]
        public bool CanEnableCoupons { get; set; }
        [Display(Name = "Default tax value")]
        public int TaxId { get; set; }
        [NotMapped]
        public List<Tax> TaxName { get; set; }
        public Tax Tax { get; set; }
        [Display(Name = "Hide tax input")]
        public bool CanShowTax { get; set; }
        [Display(Name = "Domain")]
        public string? Domain { get; set; }  
        [Display(Name = "Enable Drivers")]
        public bool CanEnableDrivers { get; set; }
        [Display(Name = " Self Delivery")]
        public bool CanSelfDelivery { get; set; }
        [Display(Name = "Enable Expenses")]
        public bool CanEnableExpenses { get; set; }
        [Display(Name = "Show Google translate")]
        public bool CanShowGoogleTranslate { get; set; }
        [Display(Name = "Show all languages")]
        public bool CanShowAllLanguages { get; set; }
        [Display(Name = "List of language codes to show")]
        public string? LanguageCode { get; set; } 
        [Display(Name = "Title of the Impressum")]
        public string? TitleOfImpressum { get; set; }
        [Display(Name = "Impressum")]
        public string? Impressum { get; set; }
        [Display(Name = "Enable Kitchen Display System")]
        public bool CanEnableKitchenDisplaySystem { get; set; }
        [EmailAddress]
        [Display(Name = "Enter manager email")]
        public string? ManagerEmail { get; set; }
        [Display(Name = "Enable order after working time of the store")]
        public bool CanEnableOrderAfterWorkingTimeOfTheStore { get; set; }
        [Display(Name = "Show date time picker even if store is open")]
        public bool CanShowDateTimePickerEvenIfStoreIsOpen{ get; set; }
        [Display(Name = "Print Node API Key")]
        public string? PrintNodeApiKey { get; set; }
        [Display(Name = "Main thermal printer ID")]
        public string? MainThermalPrinterId { get; set; }
        [Display(Name = "second Main thermal printer ID")]
        public string? SecondMainPrinterId { get; set; }
        [Display(Name = "Kitchen thermal printer ID")]
        public string? KitchenThermalPrinterId { get; set; }
        [Display(Name = "Second Kitchen thermal printer ID")]
        public string? SecondKitchenThermalPrinterId { get; set; }
        [Display(Name = "Standard printer ID")]
        public string? StandardPrinterId { get; set; }
        [Display(Name = "Print A4 Standard order when")]
        public string? PrintA4StandardOrderWhen { get; set; }
        [Display(Name = "Print on main thermal printer when")]
        public string? PrintOnMainThermalPrinterWhen { get; set; }
        [Display(Name = "Print on kitchen thermal printer when")]
        public string? PrintOnKitchenThermalPrinterWhen { get; set; }
        [Display(Name = "Select your menu template")]
        public string? SelectYoutMenuTemplate { get; set; }
        [Display(Name = "Time Zone")]
        public string? TimeZone { get; set; }
        [Display(Name = "Time Language")]
        public string? TimeLanguage { get; set; }
        [Display(Name = "Time Format for orders")]
        public string? TimeFormatForOrders { get; set; }
        [Display(Name = "Time Format for closing time")]
        public string? TimeFormatForClosingTime { get; set; }
        [Display(Name = "Webhook called when order is approved by system")]
        public string? WebhookCalledApprovedBySystem { get; set; }
        [Display(Name = "Webhook called when order is approved by vendor")]
        public string? WebhookCalledApprovedByvendor { get; set; }
        [Display(Name = "Enable Custom Delivery Cost")]
        public bool CanEnableCustomDeliveyCost { get; set; }
        [Display(Name = "Enable cost based on range")]
        public bool CanEnableCostBasedOnRange { get; set; }
        [Display(Name = "First range")]
        public string? FirstRange { get; set; }
        [Display(Name = "Second range")]
        public string? SecondRange { get; set; }
        [Display(Name = "Third range")]
        public string? ThirdRange { get; set; }
        [Display(Name = "Fourth range")]
        public string? FourthRange { get; set; }
        [Display(Name = "Fifth range")]
        public string? FifthRange { get; set; }
        [Display(Name = "Price for First range")]
        public string? PriceForFirstRange { get; set; }
        [Display(Name = "Price for Second range")]
        public string? PriceForSecondRange { get; set; }
        [Display(Name = "Price for Third range")]
        public string? PriceForThirdRange { get; set; }
        [Display(Name = "Price for Fourth range")]
        public string? PriceForFourthRange { get; set; }
        [Display(Name = "Price for Fifth range")]
        public string? PriceForFifthRange { get; set; }
        [Display(Name = "Monday")]
        public bool Monday { get; set; }
        public DateTime? MondayStart { get; set; }
        public DateTime? MondayEnd { get; set; }
        [Display(Name = "Tuesday")]
        public bool Tuesday { get; set; }
        public DateTime? TuesdayStart { get; set; }
        public DateTime? TuesdayEnd { get; set; }
        [Display(Name = "Wednesday")]
        public bool Wednesday { get; set; }
        public DateTime? WednesdayStart { get; set; }
        public DateTime? WednesdayEnd { get; set; }
        [Display(Name = "Thursday")]
        public bool Thursday { get; set; }
        public DateTime? ThursdayStart { get; set; }
        public DateTime? ThursdayEnd { get; set; }
        [Display(Name = "Friday")]
        public bool Friday { get; set; }
        public DateTime? FridayStart { get; set; }
        public DateTime? FridayEnd { get; set; }
        [Display(Name = "Saturday")]
        public bool Saturday { get; set; }
        public DateTime? SaturdayStart { get; set; }
        public DateTime? SaturdayEnd { get; set; }
        [Display(Name = "Sunday")]
        public bool Sunday { get; set; }
        public DateTime? SundayStart { get; set; }
        public DateTime? SundayEnd { get; set; }

        //FINANCE
        [Display(Name = "Paynow Payout Days")]
        public int? PayNowPayOutDays { get; set; }
        [Display(Name = "Card Payout Days")]
        public int? CardPayoutdays { get; set; }
        [Display(Name = "Restaurant bankShortCode")]
        public string? RestaurantBankShortCode { get; set; }
        [Display(Name = "Restaurant bankAccountNo")]
        public string? RestaurantBankAccountNo { get; set; }
        [Display(Name = "Restaurant bankAccountHolderName")]
        public string? RestaurantBankHolderName { get; set; }
        [Display(Name = "Current plan")]
        public string? CurrentPlan { get; set; }
        public decimal? Delivery { get; set; }
        public decimal? Buffet { get; set; }
        [Display(Name = "Halal")]
        public bool Halal { get; set; }
        [Display(Name = "Full Setup Buffet")]
        public bool FullSetUpBuffet  { get; set; }
        [Display(Name = "Vegetarian")]
        public bool Vegetarian { get; set; }
        [Display(Name = "Nov-Vegetarian")]
        public bool NonVeg { get; set; }
        [Display(Name = "Non-Halal")]
        public bool NonHalal { get; set; }
        [Display(Name = "Chinese/ Local/ Asian foodline")]
        public bool ChineseLocalAsian { get; set; }
        [Display(Name = "International")]
        public bool International { get; set; }
        [Display(Name = "Western")]
        public bool Western { get; set; }
        [Display(Name = "Thai")]
        public bool Thai { get; set; }
        [Display(Name = "Japanese")]
        public bool Japanese { get; set; }
        [Display(Name = "Indian")]
        public bool Indian { get; set; }
        [Display(Name = "Malay")]
        public bool Malay { get; set; }
    }

    public class RestaurantViewModel
    {
        [Key]
        public int RestaurantId { get; set; }
        public int ResId { get; set; }
        public long UserId { get; set; }
        public string Role { get; set; }
        public string RestaurantName { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerPhone { get; set; }
        public string Password { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; }
        [Required]
        public double Fee { get; set; } = 0.00;
        [Required]
        [Display(Name = "Static Fee")]
        public double StaticFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "Stripe Fee")]
        public double StripeFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "Stripe Static Fee")]
        public double StripeStaticFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "PayNow Fee")]
        public double PayNowFee { get; set; } = 0.00;
        [Required]
        [Display(Name = "PayNow Static Fee")]
        public double PayNowStaticFee { get; set; } = 0.00;
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }
        [Display(Name = "Can Auto Reject")]
        public bool CanAutoReject { get; set; }
        [Display(Name = "Can Auto Accept")]
        public bool CanAutoAccept { get; set; }
        [Display(Name = "Can Pickup")]
        public bool CanPickup { get; set; }
        [Display(Name = "Can Deliver")]
        public bool CanDeliver { get; set; }
        [Display(Name = "Can Deliver")]
        public bool CanFreeDeliver { get; set; }
        [Display(Name = "Can Preorder")]
        public bool CanPreorder { get; set; }
        [Display(Name = "Can Preorder")]
        public bool Disableordering { get; set; }
        [Display(Name = "Can Dine-in")]
        public bool CanDineIn { get; set; }
        [Display(Name = "Disable COD")]
        public bool DisableCOD { get; set; }
        [Display(Name = "Disable Cardpayment")]
        public bool DisableCardpayment { get; set; }
        [Display(Name = "Disable PayNow")]
        public bool DisablePayNow { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Choose Logo")]
        public IFormFile Logo { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Choose Cover")]
        public IFormFile Cover { get; set; }  
        [Required]
        [Display(Name = "Minimum")]
        public int? Minimum { get; set; }
        [Display(Name = "Takeaway Minimum")]
        public int? TakeawayMin { get; set; }
        [Display(Name = "Minutes")]
        public int? Minutes { get; set; }
        [Display(Name = "Auto-reject Wait Time")]
        public int? AutoRejectWaitTime { get; set; }
        [Display(Name = "QR Payment Wait Time")]
        public int? QRPaymentWaitTime { get; set; }
        [Display(Name = "Average order prepare time in minutes")]
        public int? AverageOrderPrepareTimeInMinutes { get; set; }
        [Display(Name = "Time slots separated in minutes")]
        public int? TimeSlotSeparatedInMinutes { get; set; }
        [Display(Name = "Delivery Preparation Time")]
        public int? DeliveryPreparationTime { get; set; }
        [Display(Name = "COD Service Tax")]
        public int? ServiceTax { get; set; }
        [Display(Name = "Stripe Service Tax")]
        public int? StripeServiceTax { get; set; }
        [Display(Name = "PayNow Service Tax")]
        public int? PayNowServiceTax { get; set; }
        [Display(Name = "Dine-in Service Tax")]
        public int? ServiceTaxDineIn { get; set; }
        [Display(Name = "Free Delivery Cost")]
        public int? FreeDeliveryCost { get; set; }
        [Display(Name = "Default Language")]
        public string? DefaultLanguage { get; set; }
        public string Currency { get; set; }
        [Display(Name = "Money conversion")]
        public bool DoConversion { get; set; }
        public bool Active { get; set; } = true;

        //APPS
        [Display(Name = "Display allergens on menu")]
        public bool CanDispalyAllergensOnMenu { get; set; }

        [Display(Name = "Enable Coupons")]
        public bool CanEnableCoupons { get; set; }

        [Display(Name = "Default tax value")]
        public int TaxId { get; set; }
        [NotMapped]
        public List<string> TaxName { get; set; }  
        public Tax Tax { get; set; }
        [Display(Name = "Hide tax input")]
        public bool CanShowTax { get; set; }
        [Display(Name = "Domain")]
        public string? Domain { get; set; }
        [Display(Name = "Enable Drivers")]
        public bool CanEnableDrivers { get; set; }
        [Display(Name = " Self Delivery")]
        public bool CanSelfDelivery { get; set; }
        [Display(Name = "Enable Expenses")]
        public bool CanEnableExpenses { get; set; }
        [Display(Name = "Show Google translate")]
        public bool CanShowGoogleTranslate { get; set; }
        [Display(Name = "Show all languages")]
        public bool CanShowAllLanguages { get; set; }
        [Display(Name = "List of language codes to show")]
        public string? LanguageCode { get; set; }
        [Display(Name = "Title of the Impressum")]
        public string? TitleOfImpressum { get; set; }
        [Display(Name = "Impressum")]
        public string? Impressum { get; set; }
        [Display(Name = "Enable Kitchen Display System")]
        public bool CanEnableKitchenDisplaySystem { get; set; }
        [EmailAddress]
        [Display(Name = "Enter manager email")]
        public string? ManagerEmail { get; set; }
        [Display(Name = "Enable order after working time of the store")]
        public bool CanEnableOrderAfterWorkingTimeOfTheStore { get; set; }
        [Display(Name = "Show date time picker even if store is open")]
        public bool CanShowDateTimePickerEvenIfStoreIsOpen { get; set; }
        [Display(Name = "Print Node API Key")]
        public string? PrintNodeApiKey { get; set; }
        [Display(Name = "Main thermal printer ID")]
        public string? MainThermalPrinterId { get; set; }
        [Display(Name = "second Main thermal printer ID")]
        public string? SecondMainPrinterId { get; set; }
        [Display(Name = "Kitchen thermal printer ID")]
        public string? KitchenThermalPrinterId { get; set; }
        [Display(Name = "Second Kitchen thermal printer ID")]
        public string? SecondKitchenThermalPrinterId { get; set; }
        [Display(Name = "Standard printer ID")]
        public string? StandardPrinterId { get; set; }
        [Display(Name = "Print A4 Standard order when")]
        public string? PrintA4StandardOrderWhen { get; set; }
        [Display(Name = "Print on main thermal printer when")]
        public string? PrintOnMainThermalPrinterWhen { get; set; }
        [Display(Name = "Print on kitchen thermal printer when")]
        public string? PrintOnKitchenThermalPrinterWhen { get; set; }
        [Display(Name = "Select your menu template")]
        public string? SelectYoutMenuTemplate { get; set; }
        [Display(Name = "Time Zone")]
        public string? TimeZone { get; set; }
        [Display(Name = "Time Language")]
        public string? TimeLanguage { get; set; }
        [Display(Name = "Time Format for orders")]
        public string? TimeFormatForOrders { get; set; }
        [Display(Name = "Time Format for closing time")]
        public string? TimeFormatForClosingTime { get; set; }
        [Display(Name = "Webhook called when order is approved by system")]
        public string? WebhookCalledApprovedBySystem { get; set; }
        [Display(Name = "Webhook called when order is approved by vendor")]
        public string? WebhookCalledApprovedByvendor { get; set; }
        [Display(Name = "Enable Custom Delivery Cost")]
        public bool CanEnableCustomDeliveyCost { get; set; }
        [Display(Name = "Enable cost based on range")]
        public bool CanEnableCostBasedOnRange { get; set; }
        [Display(Name = "First range")]
        public string? FirstRange { get; set; }
        [Display(Name = "Second range")]
        public string? SecondRange { get; set; }
        [Display(Name = "Third range")]
        public string? ThirdRange { get; set; }
        [Display(Name = "Fourth range")]
        public string? FourthRange { get; set; }
        [Display(Name = "Fifth range")]
        public string? FifthRange { get; set; }
        [Display(Name = "Price for First range")]
        public string? PriceForFirstRange { get; set; }
        [Display(Name = "Price for Second range")]
        public string? PriceForSecondRange { get; set; }
        [Display(Name = "Price for Third range")]
        public string? PriceForThirdRange { get; set; }
        [Display(Name = "Price for Fourth range")]
        public string? PriceForFourthRange { get; set; }
        [Display(Name = "Price for Fifth range")]
        public string? PriceForFifthRange { get; set; }

        //WORKINGHOURS
        [Display(Name = "Monday")]
        public bool Monday { get; set; }
        public DateTime? MondayStart { get; set; }
        public DateTime? MondayEnd { get; set; }
        [Display(Name = "Tuesday")]
        public bool Tuesday { get; set; }
        public DateTime? TuesdayStart { get; set; }
        public DateTime? TuesdayEnd { get; set; }
        [Display(Name = "Wednesday")]
        public bool Wednesday { get; set; }
        public DateTime? WednesdayStart { get; set; }
        public DateTime? WednesdayEnd { get; set; }
        [Display(Name = "Thursday")]
        public bool Thursday { get; set; }
        public DateTime? ThursdayStart { get; set; }
        public DateTime? ThursdayEnd { get; set; }
        [Display(Name = "Friday")]
        public bool Friday { get; set; }
        public DateTime? FridayStart { get; set; }
        public DateTime? FridayEnd { get; set; }
        [Display(Name = "Saturday")]
        public bool Saturday { get; set; }
        public DateTime? SaturdayStart { get; set; }
        public DateTime? SaturdayEnd { get; set; }
        [Display(Name = "Sunday")]
        public bool Sunday { get; set; }
        public DateTime? SundayStart { get; set; }
        public DateTime? SundayEnd { get; set; }

        //FINANCE
        [Display(Name = "Paynow Payout Days")]
        public int? PayNowPayOutDays { get; set; }
        [Display(Name = "Card Payout Days")]
        public int? CardPayoutdays { get; set; }
        [Display(Name = "Restaurant bankShortCode")]
        public string? RestaurantBankShortCode { get; set; }
        [Display(Name = "Restaurant bankAccountNo")]
        public string? RestaurantBankAccountNo { get; set; }
        [Display(Name = "Restaurant bankAccountHolderName")]
        public string? RestaurantBankHolderName { get; set; }

        //SUBSCRIBTION PLAN
        [Display(Name = "Current plan")]
        public string? CurrentPlan { get; set; }
        public decimal Delivery { get; set; }
        public decimal Buffet { get; set; }
        [Display(Name = "Halal")]
        public bool Halal { get; set; }
        [Display(Name = "Full Setup Buffet")]
        public bool FullSetUpBuffet { get; set; }
        [Display(Name = "Vegetarian")]
        public bool Vegetarian { get; set; }
        [Display(Name = "Nov-Vegetarian")]
        public bool NonVeg { get; set; }
        [Display(Name = "Non-Halal")]
        public bool NonHalal { get; set; }
        [Display(Name = "Chinese/ Local/ Asian foodline")]
        public bool ChineseLocalAsian { get; set; }
        [Display(Name = "International")]
        public bool International { get; set; }
        [Display(Name = "Western")]
        public bool Western { get; set; }
        [Display(Name = "Thai")]
        public bool Thai { get; set; }
        [Display(Name = "Japanese")]
        public bool Japanese { get; set; }
        [Display(Name = "Indian")]
        public bool Indian { get; set; }
        [Display(Name = "Malay")]
        public bool Malay { get; set; }
    }
}

