using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Caterer.Data;
using Caterer.Models;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using System.Security.Claims;
using System.Text;
namespace Caterer.Controllers.Restaurants
{
    public class RestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        IWebHostEnvironment hostingenvironment;
        public RestaurantsController(ApplicationDbContext context, IWebHostEnvironment hc)
        {
            _context = context;
            hostingenvironment = hc;
        }
        //Super Admin Details
        public IActionResult SuperAdminOrderDetails(int id)
        {
            var orderDetail = _context.OrderDetailWebsites
                .Include(o => o.CateringCategory)
                .Include(o => o.CateringCategory.CateringItems)
                .FirstOrDefault(o => o.OrderDetailsWebsiteId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View("~/Views/SuperAdmin/Restaurants/SuperAdminOrderDetails.cshtml", orderDetail);
        }
        //Restaurant Order List
        public async Task<IActionResult> OrderList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var orderList = await _context.OrderDetailWebsites
             .Where(order => order.RestaurantId == restaurant.RestaurantId && order.SoftDelete == null)
             .ToListAsync();
                return View("~/Views/Restaurant/Restaurants/OrderList.cshtml", orderList);
            }
            return View("Error");
        }
        //Super Admin Order List
        public async Task<IActionResult> SuperAdminOrderList()
        {
            var orderList = await _context.OrderDetailWebsites
      .Where(odw => odw.SoftDelete == null)
      .ToListAsync();
            return View("~/Views/SuperAdmin/Restaurants/SuperAdminOrderList.cshtml", orderList);
        }
        //Restaurant Delete Order List
        public async Task<IActionResult> RestaurantDeletedOrderList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                var orderList = await _context.OrderDetailWebsites
             .Where(order => order.RestaurantId == restaurant.RestaurantId && order.SoftDelete != null)
             .ToListAsync();
                return View("~/Views/Restaurant/Restaurants/RestaurantDeletedOrderList.cshtml", orderList);
            }
            return View("Error");
        }
        //Super Admin Deleted OrderList
        public async Task<IActionResult> SuperAdminDeletedOrderList()
        {
            var orderList = await _context.OrderDetailWebsites
      .Where(odw => odw.SoftDelete != null)
      .ToListAsync();
            return View("~/Views/Restaurant/Restaurants/SuperAdminDeletedOrderList.cshtml", orderList);
        }
        //Restaurant List
        public IActionResult RestaurantList()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var restaurant = _context.Restaurants
                .FirstOrDefault(r => r.OwnerEmail == userEmail);
            if (restaurant != null)
            {
                return View("~/Views/Restaurant/Restaurants/RestaurantList.cshtml", new List<Restaurant> { restaurant });
            }
            return View("Error");
        }
        //  generate a random password
        private string GenerateRandomPassword()
        {
            const string symbols = "!@#$";
            const string alphabets = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            Random random = new Random();
            StringBuilder password = new StringBuilder();
            // Add five numbers
            for (int i = 0; i < 5; i++)
            {
                password.Append(numbers[random.Next(numbers.Length)]);
            }
            // Add two alphabets
            for (int i = 0; i < 2; i++)
            {
                password.Append(alphabets[random.Next(alphabets.Length)]);
            }
            // Add one symbol
            password.Append(symbols[random.Next(symbols.Length)]);
            return password.ToString();
        }
        // GET: Restaurants/Create
        public IActionResult RestaurantCreate()
        {
            string generatedPassword = GenerateRandomPassword();
            return View("~/Views/SuperAdmin/Restaurants/RestaurantCreate.cshtml");
        }
        //Post Restaurant Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestaurantCreate([Bind("ResturantId,ResId,Description,Buffet,Delivery,Role,Address,RestaurantName,Fee,StaticFee,StripeFee,StripeStaticFee,PayNowFee,PayNowStaticFee,IsFeatured,CanAutoReject,CanSelfDelivery,CanAutoAccept,CanPickup,CanDeliver,CanFreeDeliver,CanPreorder,Disableordering,CanDineIn,DisableCOD,DisableCardpayment,DisablePayNow,Logo,Cover,Minimum,TakeawayMin,Minutes,AutoRejectWaitTime,QRPaymentWaitTime,AverageOrderPrepareTimeInMinutes,TimeSlotSeparatedInMinutes,DeliveryPreparationTime,ServiceTax,StripeServiceTax,PayNowServiceTax,ServiceTaxDineIn,FreeDeliveryCost,DefaultLanguage,Currency,DoConversion,Active,UserId,Password,OwnerName,OwnerEmail,OwnerPhone,CanDispalyAllergensOnMenu,CanEnableCoupons,TaxId,CanShowTax,Domain,CanEnableDrivers,CanShowGoogleTranslate,CanShowAllLanguages,LanguageCode,TitleOfImpressum,Impressum,CanEnableKitchenDisplaySystem,ManagerEmail,CanEnableOrderAfterWorkingTimeOfTheStore,CanShowDateTimePickerEvenIfStoreIsOpen,PrintNodeApiKey,MainThermalPrinterId,SecondMainPrinterId,KitchenThermalPrinterId,SecondKitchenThermalPrinterId,StandardPrinterId,PrintA4StandardOrderWhen,PrintOnMainThermalPrinterWhen,PrintOnKitchenThermalPrinterWhen,SelectYoutMenuTemplate,TimeZone,TimeLanguage,TimeFormatForOrders,TimeFormatForClosingTime,WebhookCalledApprovedBySystem,WebhookCalledApprovedByvendor,CanEnableCustomDeliveyCost,CanEnableCostBasedOnRange,FirstRange,SecondRange,ThirdRange,FourthRange,FifthRange,PriceForFirstRange,PriceForSecondRange,PriceForThirdRange,PriceForFourthRange,PriceForFifthRange,Monday,MondayStart,MondayEnd,Tuesday,TuesdayStart,TuesdayEnd,Wednesday,WednesdayStart,WednesdayEnd,Thursday,ThursdayStart,ThursdayEnd,Friday,FridayStart,FridayEnd,Saturday,SaturdayStart,SaturdayEnd,Sunday,SundayStart,SundayEnd,PayNowPayOutDays,CardPayoutdays,RestaurantBankShortCode,RestaurantBankAccountNo,RestaurantBankHolderName,CurrentPlan")] Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Restaurants.AnyAsync(r => r.RestaurantName == restaurant.RestaurantName))
                {
                    ModelState.AddModelError("RestaurantName", "This Restaurant Name is already taken.");
                    return View("~/Views/SuperAdmin/Restaurants/RestaurantCreate.cshtml", restaurant);
                }
                if (await _context.Users.AnyAsync(u => u.UserName == restaurant.OwnerName))
                {
                    ModelState.AddModelError("OwnerName", "This User Name is already taken.");
                    return View("~/Views/SuperAdmin/Restaurants/RestaurantCreate.cshtml", restaurant);
                }
                if (await _context.Users.AnyAsync(u => u.Email == restaurant.OwnerEmail))
                {
                    ModelState.AddModelError("OwnerEmail", "This Email is already registered.");
                    return View("~/Views/SuperAdmin/Restaurants/RestaurantCreate.cshtml", restaurant);
                }
                if (await _context.Users.AnyAsync(u => u.PhoneNumber == restaurant.OwnerPhone))
                {
                    ModelState.AddModelError("OwnerPhone", "This Phone Number is already registered.");
                    return View("~/Views/SuperAdmin/Restaurants/RestaurantCreate.cshtml", restaurant);
                }
                string generatedPassword = GenerateRandomPassword();
                restaurant.Password = generatedPassword;
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                var user = new User
                {
                    UserName = restaurant.OwnerName,
                    Email = restaurant.OwnerEmail,
                    PhoneNumber = restaurant.OwnerPhone,
                    Role = restaurant.Role,
                    RestaurantName = restaurant.RestaurantName,
                    RestaurantId = restaurant.RestaurantId,
                    Password = generatedPassword
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("WebsiteHome", "Websites");
            }
            return View("~/Views/SuperAdmin/Restaurants/RestaurantCreate.cshtml", restaurant);
        }
        public async Task<IActionResult> RestaurantEdit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Restaurants.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            var tax = _context.Taxes
        .GroupBy(t => t.TaxNo)
        .Select(group => group.FirstOrDefault())
        .ToList();
            var restaurant = new Restaurant
            {
                TaxName = tax,
                TaxId = category.TaxId,
                ResId = category.ResId,
                Description = category.Description,
                Address = category.Address,
                RestaurantName = category.RestaurantName,
                Fee = category.Fee,
                StaticFee = category.StaticFee,
                StripeFee = category.StripeFee,
                StripeStaticFee = category.StripeStaticFee,
                PayNowFee = category.PayNowFee,
                PayNowStaticFee = category.PayNowStaticFee,
                IsFeatured = category.IsFeatured,
                CanSelfDelivery = category.CanSelfDelivery,
                CanAutoReject = category.CanAutoReject,
                CanAutoAccept = category.CanAutoAccept,
                CanPickup = category.CanPickup,
                CanDeliver = category.CanDeliver,
                CanFreeDeliver = category.CanFreeDeliver,
                CanPreorder = category.CanPreorder,
                Disableordering = category.Disableordering,
                CanDineIn = category.CanDineIn,
                DisableCOD = category.DisableCOD,
                DisableCardpayment = category.DisableCardpayment,
                DisablePayNow = category.DisablePayNow,
                Minimum = category.Minimum,
                TakeawayMin = category.TakeawayMin,
                Minutes = category.Minutes,
                AutoRejectWaitTime = category.AutoRejectWaitTime,
                QRPaymentWaitTime = category.QRPaymentWaitTime,
                AverageOrderPrepareTimeInMinutes = category.AverageOrderPrepareTimeInMinutes,
                TimeSlotSeparatedInMinutes = category.TimeSlotSeparatedInMinutes,
                DeliveryPreparationTime = category.DeliveryPreparationTime,
                ServiceTax = category.ServiceTax,
                StripeServiceTax = category.StripeServiceTax,
                PayNowServiceTax = category.PayNowServiceTax,
                ServiceTaxDineIn = category.ServiceTaxDineIn,
                FreeDeliveryCost = category.FreeDeliveryCost,
                DefaultLanguage = category.DefaultLanguage,
                Currency = category.Currency,
                DoConversion = category.DoConversion,
                Active = category.Active,
                UserId = category.UserId,
                Password = category.Password,
                OwnerName = category.OwnerName,
                OwnerEmail = category.OwnerEmail,
                OwnerPhone = category.OwnerPhone,
                CanDispalyAllergensOnMenu = category.CanDispalyAllergensOnMenu,
                CanEnableCoupons = category.CanEnableCoupons,
                CanShowTax = category.CanShowTax,
                Domain = category.Domain,
                CanEnableDrivers = category.CanEnableDrivers,
                CanShowGoogleTranslate = category.CanShowGoogleTranslate,
                CanShowAllLanguages = category.CanShowAllLanguages,
                LanguageCode = category.LanguageCode,
                TitleOfImpressum = category.TitleOfImpressum,
                Impressum = category.Impressum,
                CanEnableKitchenDisplaySystem = category.CanEnableKitchenDisplaySystem,
                ManagerEmail = category.ManagerEmail,
                CanEnableOrderAfterWorkingTimeOfTheStore = category.CanEnableOrderAfterWorkingTimeOfTheStore,
                CanShowDateTimePickerEvenIfStoreIsOpen = category.CanShowDateTimePickerEvenIfStoreIsOpen,
                PrintNodeApiKey = category.PrintNodeApiKey,
                MainThermalPrinterId = category.MainThermalPrinterId,
                SecondMainPrinterId = category.SecondMainPrinterId,
                KitchenThermalPrinterId = category.KitchenThermalPrinterId,
                SecondKitchenThermalPrinterId = category.SecondKitchenThermalPrinterId,
                StandardPrinterId = category.StandardPrinterId,
                PrintA4StandardOrderWhen = category.PrintA4StandardOrderWhen,
                PrintOnMainThermalPrinterWhen = category.PrintOnMainThermalPrinterWhen,
                PrintOnKitchenThermalPrinterWhen = category.PrintOnKitchenThermalPrinterWhen,
                SelectYoutMenuTemplate = category.SelectYoutMenuTemplate,
                TimeZone = category.TimeZone,
                TimeLanguage = category.TimeLanguage,
                TimeFormatForOrders = category.TimeFormatForOrders,
                TimeFormatForClosingTime = category.TimeFormatForClosingTime,
                WebhookCalledApprovedBySystem = category.WebhookCalledApprovedBySystem,
                WebhookCalledApprovedByvendor = category.WebhookCalledApprovedByvendor,
                CanEnableCustomDeliveyCost = category.CanEnableCustomDeliveyCost,
                CanEnableCostBasedOnRange = category.CanEnableCostBasedOnRange,
                FirstRange = category.FirstRange,
                SecondRange = category.SecondRange,
                ThirdRange = category.ThirdRange,
                FourthRange = category.FourthRange,
                FifthRange = category.FifthRange,
                PriceForFirstRange = category.PriceForFirstRange,
                PriceForSecondRange = category.PriceForSecondRange,
                PriceForThirdRange = category.PriceForThirdRange,
                PriceForFourthRange = category.PriceForFourthRange,
                PriceForFifthRange = category.PriceForFifthRange,
                Monday = category.Monday,
                MondayStart = category.MondayStart,
                MondayEnd = category.MondayEnd,
                Tuesday = category.Tuesday,
                TuesdayStart = category.TuesdayStart,
                TuesdayEnd = category.TuesdayEnd,
                Wednesday = category.Wednesday,
                WednesdayStart = category.WednesdayStart,
                WednesdayEnd = category.WednesdayEnd,
                Thursday = category.Thursday,
                ThursdayStart = category.ThursdayStart,
                ThursdayEnd = category.ThursdayEnd,
                Friday = category.Friday,
                FridayStart = category.FridayStart,
                FridayEnd = category.FridayEnd,
                Saturday = category.Saturday,
                SaturdayStart = category.SaturdayStart,
                SaturdayEnd = category.SaturdayEnd,
                Sunday = category.Sunday,
                SundayStart = category.SundayStart,
                SundayEnd = category.SundayEnd,
                PayNowPayOutDays = category.PayNowPayOutDays,
                CardPayoutdays = category.CardPayoutdays,
                RestaurantBankShortCode = category.RestaurantBankShortCode,
                RestaurantBankAccountNo = category.RestaurantBankAccountNo,
                RestaurantBankHolderName = category.RestaurantBankHolderName,
                CurrentPlan = category.CurrentPlan,
                Logo = category.Logo,
                Cover = category.Cover,
                Halal = category.Halal,
                FullSetUpBuffet = category.FullSetUpBuffet,
                NonVeg = category.NonVeg,
                International = category.International,
                NonHalal = category.NonHalal,
                Japanese = category.Japanese,
                Malay = category.Malay,
                Vegetarian = category.Vegetarian,
                ChineseLocalAsian = category.ChineseLocalAsian,
                Western = category.Western,
                Thai = category.Thai,
                Indian = category.Indian,
                Delivery = category.Delivery,
                Buffet = category.Buffet,
            };
            return View("~/Views/Restaurant/Restaurants/RestaurantEdit.cshtml", restaurant);
        }
        //Post Restaurant Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RestaurantEdit(int id, [Bind("ResturantId,ResId,Halal,Delivery,Buffet, FullSetUpBuffet,Vegetarian, NonHalal, ChineseLocalAsian, International,Western, Thai, Japanese,Indian, Malay, NonVeg, Role, Description,Address, Role, RestaurantName, Fee,StaticFee,StripeFee,StripeStaticFee,PayNowFee,PayNowStaticFee,IsFeatured,CanSelfDelivery,CanAutoReject,CanAutoAccept,CanPickup,CanDeliver,CanFreeDeliver,CanPreorder,Disableordering,CanDineIn,DisableCOD,DisableCardpayment,DisablePayNow,Logo,Cover,Minimum,TakeawayMin,Minutes,AutoRejectWaitTime,QRPaymentWaitTime,AverageOrderPrepareTimeInMinutes,TimeSlotSeparatedInMinutes,DeliveryPreparationTime,ServiceTax,StripeServiceTax,PayNowServiceTax,ServiceTaxDineIn,FreeDeliveryCost,DefaultLanguage,Currency,DoConversion,Active,UserId,Password,OwnerName,OwnerEmail,OwnerPhone,CanDispalyAllergensOnMenu,CanEnableCoupons,TaxId,CanShowTax,Domain,CanEnableDrivers,CanShowGoogleTranslate,CanShowAllLanguages,LanguageCode,TitleOfImpressum,Impressum,CanEnableKitchenDisplaySystem,ManagerEmail,CanEnableOrderAfterWorkingTimeOfTheStore,CanShowDateTimePickerEvenIfStoreIsOpen,PrintNodeApiKey,MainThermalPrinterId,SecondMainPrinterId,KitchenThermalPrinterId,SecondKitchenThermalPrinterId,StandardPrinterId,PrintA4StandardOrderWhen,PrintOnMainThermalPrinterWhen,PrintOnKitchenThermalPrinterWhen,SelectYoutMenuTemplate,TimeZone,TimeLanguage,TimeFormatForOrders,TimeFormatForClosingTime,WebhookCalledApprovedBySystem,WebhookCalledApprovedByvendor,CanEnableCustomDeliveyCost,CanEnableCostBasedOnRange,FirstRange,SecondRange,ThirdRange,FourthRange,FifthRange,PriceForFirstRange,PriceForSecondRange,PriceForThirdRange,PriceForFourthRange,PriceForFifthRange,Monday,MondayStart,MondayEnd,Tuesday,TuesdayStart,TuesdayEnd,Wednesday,WednesdayStart,WednesdayEnd,Thursday,ThursdayStart,ThursdayEnd,Friday,FridayStart,FridayEnd,Saturday,SaturdayStart,SaturdayEnd,Sunday,SundayStart,SundayEnd,PayNowPayOutDays,CardPayoutdays,RestaurantBankShortCode,RestaurantBankAccountNo,RestaurantBankHolderName,CurrentPlan")] Restaurant category, IFormFile ProductImage, IFormFile coverimg)
        {
            try
            {
                var existingCategory = _context.Restaurants.Find(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == category.OwnerEmail);
                if (existingUser != null)
                {
                    existingUser.UserName = category.OwnerName;
                    existingUser.PhoneNumber = category.OwnerPhone;
                    existingUser.Role = category.Role;
                    existingUser.Password = category.Password;
                    existingUser.RestaurantName = category.RestaurantName;
                    _context.Users.Update(existingUser);
                }

                if (ProductImage != null)
                {
                    if (!string.IsNullOrEmpty(existingCategory.Logo))
                    {
                        string existingFilePath = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantLogo", existingCategory.Logo);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }
                    string filename = Guid.NewGuid().ToString() + "_" + ProductImage.FileName;
                    string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantLogo");
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(fileStream);
                    }
                    existingCategory.Logo = filename;
                }
                if (coverimg != null)
                {
                    if (!string.IsNullOrEmpty(existingCategory.Logo))
                    {
                        string existingFilePath = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantCover", existingCategory.Cover);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }
                    string filename = Guid.NewGuid().ToString() + "_" + coverimg.FileName;
                    string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantCover");
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await coverimg.CopyToAsync(fileStream);
                    }
                    existingCategory.Cover = filename;
                }
                existingCategory.ResId = category.ResId;
                existingCategory.Description = category.Description;
                existingCategory.Address = category.Address;
                existingCategory.RestaurantName = category.RestaurantName;
                existingCategory.Fee = category.Fee;
                existingCategory.StaticFee = category.StaticFee;
                existingCategory.StripeFee = category.StripeFee;
                existingCategory.StripeStaticFee = category.StripeStaticFee;
                existingCategory.PayNowFee = category.PayNowFee;
                existingCategory.PayNowStaticFee = category.PayNowStaticFee;
                existingCategory.IsFeatured = category.IsFeatured;
                existingCategory.CanSelfDelivery = category.CanSelfDelivery;
                existingCategory.CanAutoReject = category.CanAutoReject;
                existingCategory.CanAutoAccept = category.CanAutoAccept;
                existingCategory.CanPickup = category.CanPickup;
                existingCategory.CanDeliver = category.CanDeliver;
                existingCategory.CanFreeDeliver = category.CanFreeDeliver;
                existingCategory.CanPreorder = category.CanPreorder;
                existingCategory.Disableordering = category.Disableordering;
                existingCategory.CanDineIn = category.CanDineIn;
                existingCategory.DisableCOD = category.DisableCOD;
                existingCategory.DisableCardpayment = category.DisableCardpayment;
                existingCategory.DisablePayNow = category.DisablePayNow;
                existingCategory.DisablePayNow = category.DisablePayNow;
                existingCategory.Minimum = category.Minimum;
                existingCategory.TakeawayMin = category.TakeawayMin;
                existingCategory.Minutes = category.Minutes;
                existingCategory.AutoRejectWaitTime = category.AutoRejectWaitTime;
                existingCategory.QRPaymentWaitTime = category.QRPaymentWaitTime;
                existingCategory.AverageOrderPrepareTimeInMinutes = category.AverageOrderPrepareTimeInMinutes;
                existingCategory.TimeSlotSeparatedInMinutes = category.TimeSlotSeparatedInMinutes;
                existingCategory.DeliveryPreparationTime = category.DeliveryPreparationTime;
                existingCategory.ServiceTax = category.ServiceTax;
                existingCategory.StripeServiceTax = category.StripeServiceTax;
                existingCategory.PayNowServiceTax = category.PayNowServiceTax;
                existingCategory.ServiceTaxDineIn = category.ServiceTaxDineIn;
                existingCategory.FreeDeliveryCost = category.FreeDeliveryCost;
                existingCategory.DefaultLanguage = category.DefaultLanguage;
                existingCategory.Currency = category.Currency;
                existingCategory.DoConversion = category.DoConversion;
                existingCategory.Active = category.Active;
                existingCategory.UserId = category.UserId;
                existingCategory.Password = category.Password;
                existingCategory.OwnerName = category.OwnerName;
                existingCategory.OwnerEmail = category.OwnerEmail;
                existingCategory.OwnerPhone = category.OwnerPhone;
                existingCategory.CanDispalyAllergensOnMenu = category.CanDispalyAllergensOnMenu;
                existingCategory.CanEnableCoupons = category.CanEnableCoupons;
                existingCategory.TaxId = category.TaxId;
                existingCategory.CanShowTax = category.CanShowTax;
                existingCategory.Domain = category.Domain;
                existingCategory.CanEnableDrivers = category.CanEnableDrivers;
                existingCategory.CanShowGoogleTranslate = category.CanShowGoogleTranslate;
                existingCategory.CanShowAllLanguages = category.CanShowAllLanguages;
                existingCategory.LanguageCode = category.LanguageCode;
                existingCategory.TitleOfImpressum = category.TitleOfImpressum;
                existingCategory.Impressum = category.Impressum;
                existingCategory.CanEnableKitchenDisplaySystem = category.CanEnableKitchenDisplaySystem;
                existingCategory.ManagerEmail = category.ManagerEmail;
                existingCategory.CanEnableOrderAfterWorkingTimeOfTheStore = category.CanEnableOrderAfterWorkingTimeOfTheStore;
                existingCategory.CanShowDateTimePickerEvenIfStoreIsOpen = category.CanShowDateTimePickerEvenIfStoreIsOpen;
                existingCategory.PrintNodeApiKey = category.PrintNodeApiKey;
                existingCategory.MainThermalPrinterId = category.MainThermalPrinterId;
                existingCategory.SecondMainPrinterId = category.SecondMainPrinterId;
                existingCategory.KitchenThermalPrinterId = category.KitchenThermalPrinterId;
                existingCategory.SecondKitchenThermalPrinterId = category.SecondKitchenThermalPrinterId;
                existingCategory.StandardPrinterId = category.StandardPrinterId;
                existingCategory.PrintA4StandardOrderWhen = category.PrintA4StandardOrderWhen;
                existingCategory.PrintOnMainThermalPrinterWhen = category.PrintOnMainThermalPrinterWhen;
                existingCategory.PrintOnKitchenThermalPrinterWhen = category.PrintOnKitchenThermalPrinterWhen;
                existingCategory.SelectYoutMenuTemplate = category.SelectYoutMenuTemplate;
                existingCategory.TimeZone = category.TimeZone;
                existingCategory.TimeLanguage = category.TimeLanguage;
                existingCategory.TimeFormatForOrders = category.TimeFormatForOrders;
                existingCategory.TimeFormatForClosingTime = category.TimeFormatForClosingTime;
                existingCategory.WebhookCalledApprovedBySystem = category.WebhookCalledApprovedBySystem;
                existingCategory.WebhookCalledApprovedByvendor = category.WebhookCalledApprovedByvendor;
                existingCategory.CanEnableCustomDeliveyCost = category.CanEnableCustomDeliveyCost;
                existingCategory.CanEnableCostBasedOnRange = category.CanEnableCostBasedOnRange;
                existingCategory.FirstRange = category.FirstRange;
                existingCategory.SecondRange = category.SecondRange;
                existingCategory.ThirdRange = category.ThirdRange;
                existingCategory.FourthRange = category.FourthRange;
                existingCategory.FifthRange = category.FifthRange;
                existingCategory.PriceForFirstRange = category.PriceForFirstRange;
                existingCategory.PriceForSecondRange = category.PriceForSecondRange;
                existingCategory.PriceForThirdRange = category.PriceForThirdRange;
                existingCategory.PriceForFourthRange = category.PriceForFourthRange;
                existingCategory.PriceForFifthRange = category.PriceForFifthRange;
                existingCategory.Monday = category.Monday;
                existingCategory.MondayStart = category.MondayStart;
                existingCategory.MondayEnd = category.MondayEnd;
                existingCategory.Tuesday = category.Tuesday;
                existingCategory.TuesdayStart = category.TuesdayStart;
                existingCategory.TuesdayEnd = category.TuesdayEnd;
                existingCategory.Wednesday = category.Wednesday;
                existingCategory.WednesdayStart = category.WednesdayStart;
                existingCategory.WednesdayEnd = category.WednesdayEnd;
                existingCategory.Thursday = category.Thursday;
                existingCategory.ThursdayStart = category.ThursdayStart;
                existingCategory.ThursdayEnd = category.ThursdayEnd;
                existingCategory.Friday = category.Friday;
                existingCategory.FridayStart = category.FridayStart;
                existingCategory.FridayEnd = category.FridayEnd;
                existingCategory.Saturday = category.Saturday;
                existingCategory.SaturdayStart = category.SaturdayStart;
                existingCategory.SaturdayEnd = category.SaturdayEnd;
                existingCategory.Sunday = category.Sunday;
                existingCategory.SundayStart = category.SundayStart;
                existingCategory.SundayEnd = category.SundayEnd;
                existingCategory.PayNowPayOutDays = category.PayNowPayOutDays;
                existingCategory.CardPayoutdays = category.CardPayoutdays;
                existingCategory.RestaurantBankShortCode = category.RestaurantBankShortCode;
                existingCategory.RestaurantBankAccountNo = category.RestaurantBankAccountNo;
                existingCategory.RestaurantBankHolderName = category.RestaurantBankHolderName;
                existingCategory.CurrentPlan = category.CurrentPlan;
                existingCategory.Halal = category.Halal;
                existingCategory.FullSetUpBuffet = category.FullSetUpBuffet;
                existingCategory.Vegetarian = category.Vegetarian;
                existingCategory.NonHalal = category.NonHalal;
                existingCategory.ChineseLocalAsian = category.ChineseLocalAsian;
                existingCategory.International = category.International;
                existingCategory.Western = category.Western;
                existingCategory.Thai = category.Thai;
                existingCategory.Japanese = category.Japanese;
                existingCategory.Indian = category.Indian;
                existingCategory.Malay = category.Malay;
                existingCategory.NonVeg = category.NonVeg;
                existingCategory.Role = category.Role;
                existingCategory.Delivery = category.Delivery;
                existingCategory.Buffet = category.Buffet;
                _context.Update(existingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(RestaurantEdit));
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        //Super Admin Live Orders
        public async Task<IActionResult> SuperAdminLiveOrders()
        {
            var orderList = await _context.OrderDetailWebsites
                .ToListAsync();
            return View("~/Views/SuperAdmin/Restaurants/SuperAdminLiveOrders.cshtml", orderList);
        }
        //Super Admin Client List
        public async Task<IActionResult> SuperAdminClientList()
        {
            var allowedRoles = new List<string> { "Client", "Guest", "Restaurant" };
            var userList = await _context.Users
                .Where(user => allowedRoles.Contains(user.Role))
                .ToListAsync();
            return View("~/Views/SuperAdmin/Restaurants/SuperAdminClientList.cshtml", userList);
        }
        // GET:Super Admin Restaurant List
        public async Task<IActionResult> SuperAdminRestaurantList()
        {
            return _context.Restaurants != null ?
                        View("~/Views/SuperAdmin/Restaurants/SuperAdminRestaurantList.cshtml", await _context.Restaurants.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Restaurants'  is null.");
        }
        //Super Admin Restaurant Edit 
        public async Task<IActionResult> SuperAdminRestaurantEdit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Restaurants.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var tax = _context.Taxes
        .GroupBy(t => t.TaxNo)
        .Select(group => group.FirstOrDefault())
        .ToList();
            var restaurant = new Restaurant
            {
                TaxName = tax,
                TaxId = category.TaxId,
                ResId = category.ResId,
                Description = category.Description,
                Address = category.Address,
                RestaurantName = category.RestaurantName,
                Fee = category.Fee,
                StaticFee = category.StaticFee,
                StripeFee = category.StripeFee,
                StripeStaticFee = category.StripeStaticFee,
                PayNowFee = category.PayNowFee,
                PayNowStaticFee = category.PayNowStaticFee,
                IsFeatured = category.IsFeatured,
                CanSelfDelivery = category.CanSelfDelivery,
                CanAutoReject = category.CanAutoReject,
                CanAutoAccept = category.CanAutoAccept,
                CanPickup = category.CanPickup,
                CanDeliver = category.CanDeliver,
                CanFreeDeliver = category.CanFreeDeliver,
                CanPreorder = category.CanPreorder,
                Disableordering = category.Disableordering,
                CanDineIn = category.CanDineIn,
                DisableCOD = category.DisableCOD,
                DisableCardpayment = category.DisableCardpayment,
                DisablePayNow = category.DisablePayNow,
                Minimum = category.Minimum,
                TakeawayMin = category.TakeawayMin,
                Minutes = category.Minutes,
                AutoRejectWaitTime = category.AutoRejectWaitTime,
                QRPaymentWaitTime = category.QRPaymentWaitTime,
                AverageOrderPrepareTimeInMinutes = category.AverageOrderPrepareTimeInMinutes,
                TimeSlotSeparatedInMinutes = category.TimeSlotSeparatedInMinutes,
                DeliveryPreparationTime = category.DeliveryPreparationTime,
                ServiceTax = category.ServiceTax,
                StripeServiceTax = category.StripeServiceTax,
                PayNowServiceTax = category.PayNowServiceTax,
                ServiceTaxDineIn = category.ServiceTaxDineIn,
                FreeDeliveryCost = category.FreeDeliveryCost,
                DefaultLanguage = category.DefaultLanguage,
                Currency = category.Currency,
                DoConversion = category.DoConversion,
                Active = category.Active,
                UserId = category.UserId,
                Password = category.Password,
                OwnerName = category.OwnerName,
                OwnerEmail = category.OwnerEmail,
                OwnerPhone = category.OwnerPhone,
                CanDispalyAllergensOnMenu = category.CanDispalyAllergensOnMenu,
                CanEnableCoupons = category.CanEnableCoupons,
                CanShowTax = category.CanShowTax,
                Domain = category.Domain,
                CanEnableDrivers = category.CanEnableDrivers,
                CanShowGoogleTranslate = category.CanShowGoogleTranslate,
                CanShowAllLanguages = category.CanShowAllLanguages,
                LanguageCode = category.LanguageCode,
                TitleOfImpressum = category.TitleOfImpressum,
                Impressum = category.Impressum,
                CanEnableKitchenDisplaySystem = category.CanEnableKitchenDisplaySystem,
                ManagerEmail = category.ManagerEmail,
                CanEnableOrderAfterWorkingTimeOfTheStore = category.CanEnableOrderAfterWorkingTimeOfTheStore,
                CanShowDateTimePickerEvenIfStoreIsOpen = category.CanShowDateTimePickerEvenIfStoreIsOpen,
                PrintNodeApiKey = category.PrintNodeApiKey,
                MainThermalPrinterId = category.MainThermalPrinterId,
                SecondMainPrinterId = category.SecondMainPrinterId,
                KitchenThermalPrinterId = category.KitchenThermalPrinterId,
                SecondKitchenThermalPrinterId = category.SecondKitchenThermalPrinterId,
                StandardPrinterId = category.StandardPrinterId,
                PrintA4StandardOrderWhen = category.PrintA4StandardOrderWhen,
                PrintOnMainThermalPrinterWhen = category.PrintOnMainThermalPrinterWhen,
                PrintOnKitchenThermalPrinterWhen = category.PrintOnKitchenThermalPrinterWhen,
                SelectYoutMenuTemplate = category.SelectYoutMenuTemplate,
                TimeZone = category.TimeZone,
                TimeLanguage = category.TimeLanguage,
                TimeFormatForOrders = category.TimeFormatForOrders,
                TimeFormatForClosingTime = category.TimeFormatForClosingTime,
                WebhookCalledApprovedBySystem = category.WebhookCalledApprovedBySystem,
                WebhookCalledApprovedByvendor = category.WebhookCalledApprovedByvendor,
                CanEnableCustomDeliveyCost = category.CanEnableCustomDeliveyCost,
                CanEnableCostBasedOnRange = category.CanEnableCostBasedOnRange,
                FirstRange = category.FirstRange,
                SecondRange = category.SecondRange,
                ThirdRange = category.ThirdRange,
                FourthRange = category.FourthRange,
                FifthRange = category.FifthRange,
                PriceForFirstRange = category.PriceForFirstRange,
                PriceForSecondRange = category.PriceForSecondRange,
                PriceForThirdRange = category.PriceForThirdRange,
                PriceForFourthRange = category.PriceForFourthRange,
                PriceForFifthRange = category.PriceForFifthRange,
                Monday = category.Monday,
                MondayStart = category.MondayStart,
                MondayEnd = category.MondayEnd,
                Tuesday = category.Tuesday,
                TuesdayStart = category.TuesdayStart,
                TuesdayEnd = category.TuesdayEnd,
                Wednesday = category.Wednesday,
                WednesdayStart = category.WednesdayStart,
                WednesdayEnd = category.WednesdayEnd,
                Thursday = category.Thursday,
                ThursdayStart = category.ThursdayStart,
                ThursdayEnd = category.ThursdayEnd,
                Friday = category.Friday,
                FridayStart = category.FridayStart,
                FridayEnd = category.FridayEnd,
                Saturday = category.Saturday,
                SaturdayStart = category.SaturdayStart,
                SaturdayEnd = category.SaturdayEnd,
                Sunday = category.Sunday,
                SundayStart = category.SundayStart,
                SundayEnd = category.SundayEnd,
                PayNowPayOutDays = category.PayNowPayOutDays,
                CardPayoutdays = category.CardPayoutdays,
                RestaurantBankShortCode = category.RestaurantBankShortCode,
                RestaurantBankAccountNo = category.RestaurantBankAccountNo,
                RestaurantBankHolderName = category.RestaurantBankHolderName,
                CurrentPlan = category.CurrentPlan,
                Logo = category.Logo,
                Cover = category.Cover,
                Halal = category.Halal,
                FullSetUpBuffet = category.FullSetUpBuffet,
                NonVeg = category.NonVeg,
                International = category.International,
                NonHalal = category.NonHalal,
                Japanese = category.Japanese,
                Malay = category.Malay,
                Vegetarian = category.Vegetarian,
                ChineseLocalAsian = category.ChineseLocalAsian,
                Western = category.Western,
                Thai = category.Thai,
                Indian = category.Indian,
                Delivery = category.Delivery,
                Buffet = category.Buffet,
            };
            return View("~/Views/SuperAdmin/Restaurants/SuperAdminRestaurantEdit.cshtml", restaurant);
        }
        //Super Admin Restaurant Edit 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SuperAdminRestaurantEdit(int id, [Bind("ResturantId,ResId,Halal,Delivery,Buffet,  FullSetUpBuffet,Vegetarian, NonHalal, ChineseLocalAsian, International,Western, Thai, Japanese,Indian, Malay, NonVeg, Role, Description,Address, Role, RestaurantName, Fee,StaticFee,StripeFee,StripeStaticFee,PayNowFee,PayNowStaticFee,IsFeatured,CanSelfDelivery,CanAutoReject,CanAutoAccept,CanPickup,CanDeliver,CanFreeDeliver,CanPreorder,Disableordering,CanDineIn,DisableCOD,DisableCardpayment,DisablePayNow,Logo,Cover,Minimum,TakeawayMin,Minutes,AutoRejectWaitTime,QRPaymentWaitTime,AverageOrderPrepareTimeInMinutes,TimeSlotSeparatedInMinutes,DeliveryPreparationTime,ServiceTax,StripeServiceTax,PayNowServiceTax,ServiceTaxDineIn,FreeDeliveryCost,DefaultLanguage,Currency,DoConversion,Active,UserId,Password,OwnerName,OwnerEmail,OwnerPhone,CanDispalyAllergensOnMenu,CanEnableCoupons,TaxId,CanShowTax,Domain,CanEnableDrivers,CanShowGoogleTranslate,CanShowAllLanguages,LanguageCode,TitleOfImpressum,Impressum,CanEnableKitchenDisplaySystem,ManagerEmail,CanEnableOrderAfterWorkingTimeOfTheStore,CanShowDateTimePickerEvenIfStoreIsOpen,PrintNodeApiKey,MainThermalPrinterId,SecondMainPrinterId,KitchenThermalPrinterId,SecondKitchenThermalPrinterId,StandardPrinterId,PrintA4StandardOrderWhen,PrintOnMainThermalPrinterWhen,PrintOnKitchenThermalPrinterWhen,SelectYoutMenuTemplate,TimeZone,TimeLanguage,TimeFormatForOrders,TimeFormatForClosingTime,WebhookCalledApprovedBySystem,WebhookCalledApprovedByvendor,CanEnableCustomDeliveyCost,CanEnableCostBasedOnRange,FirstRange,SecondRange,ThirdRange,FourthRange,FifthRange,PriceForFirstRange,PriceForSecondRange,PriceForThirdRange,PriceForFourthRange,PriceForFifthRange,Monday,MondayStart,MondayEnd,Tuesday,TuesdayStart,TuesdayEnd,Wednesday,WednesdayStart,WednesdayEnd,Thursday,ThursdayStart,ThursdayEnd,Friday,FridayStart,FridayEnd,Saturday,SaturdayStart,SaturdayEnd,Sunday,SundayStart,SundayEnd,PayNowPayOutDays,CardPayoutdays,RestaurantBankShortCode,RestaurantBankAccountNo,RestaurantBankHolderName,CurrentPlan")] Restaurant category, IFormFile ProductImage, IFormFile coverimg)
        {
            try
            {
                var existingCategory = _context.Restaurants.Find(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                var existingUser = _context.Users.FirstOrDefault(u => u.Email == category.OwnerEmail);
                if (existingUser != null)
                {
                    existingUser.UserName = category.OwnerName;
                    existingUser.PhoneNumber = category.OwnerPhone;
                    existingUser.Role = category.Role;
                    existingUser.Password = category.Password;
                    existingUser.RestaurantName = category.RestaurantName;
                    _context.Users.Update(existingUser);
                }
                if (ProductImage != null)
                {
                    if (!string.IsNullOrEmpty(existingCategory.Logo))
                    {
                        string existingFilePath = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantLogo", existingCategory.Logo);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }
                    string filename = Guid.NewGuid().ToString() + "_" + ProductImage.FileName;
                    string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantLogo");
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(fileStream);
                    }
                    existingCategory.Logo = filename;
                }
                if (coverimg != null)
                {
                    if (!string.IsNullOrEmpty(existingCategory.Logo))
                    {
                        string existingFilePath = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantCover", existingCategory.Cover);
                        if (System.IO.File.Exists(existingFilePath))
                        {
                            System.IO.File.Delete(existingFilePath);
                        }
                    }
                    string filename = Guid.NewGuid().ToString() + "_" + coverimg.FileName;
                    string uploadFolder = Path.Combine(hostingenvironment.WebRootPath, "Images/RestaurantCover");
                    string filepath = Path.Combine(uploadFolder, filename);
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await coverimg.CopyToAsync(fileStream);
                    }
                    existingCategory.Cover = filename;
                }
                existingCategory.ResId = category.ResId;
                existingCategory.Description = category.Description;
                existingCategory.Address = category.Address;
                existingCategory.RestaurantName = category.RestaurantName;
                existingCategory.Fee = category.Fee;
                existingCategory.StaticFee = category.StaticFee;
                existingCategory.StripeFee = category.StripeFee;
                existingCategory.StripeStaticFee = category.StripeStaticFee;
                existingCategory.PayNowFee = category.PayNowFee;
                existingCategory.PayNowStaticFee = category.PayNowStaticFee;
                existingCategory.IsFeatured = category.IsFeatured;
                existingCategory.CanSelfDelivery = category.CanSelfDelivery;
                existingCategory.CanAutoReject = category.CanAutoReject;
                existingCategory.CanAutoAccept = category.CanAutoAccept;
                existingCategory.CanPickup = category.CanPickup;
                existingCategory.CanDeliver = category.CanDeliver;
                existingCategory.CanFreeDeliver = category.CanFreeDeliver;
                existingCategory.CanPreorder = category.CanPreorder;
                existingCategory.Disableordering = category.Disableordering;
                existingCategory.CanDineIn = category.CanDineIn;
                existingCategory.DisableCOD = category.DisableCOD;
                existingCategory.DisableCardpayment = category.DisableCardpayment;
                existingCategory.DisablePayNow = category.DisablePayNow;
                existingCategory.DisablePayNow = category.DisablePayNow;
                existingCategory.Minimum = category.Minimum;
                existingCategory.TakeawayMin = category.TakeawayMin;
                existingCategory.Minutes = category.Minutes;
                existingCategory.AutoRejectWaitTime = category.AutoRejectWaitTime;
                existingCategory.QRPaymentWaitTime = category.QRPaymentWaitTime;
                existingCategory.AverageOrderPrepareTimeInMinutes = category.AverageOrderPrepareTimeInMinutes;
                existingCategory.TimeSlotSeparatedInMinutes = category.TimeSlotSeparatedInMinutes;
                existingCategory.DeliveryPreparationTime = category.DeliveryPreparationTime;
                existingCategory.ServiceTax = category.ServiceTax;
                existingCategory.StripeServiceTax = category.StripeServiceTax;
                existingCategory.PayNowServiceTax = category.PayNowServiceTax;
                existingCategory.ServiceTaxDineIn = category.ServiceTaxDineIn;
                existingCategory.FreeDeliveryCost = category.FreeDeliveryCost;
                existingCategory.DefaultLanguage = category.DefaultLanguage;
                existingCategory.Currency = category.Currency;
                existingCategory.DoConversion = category.DoConversion;
                existingCategory.Active = category.Active;
                existingCategory.UserId = category.UserId;
                existingCategory.Password = category.Password;
                existingCategory.OwnerName = category.OwnerName;
                existingCategory.OwnerEmail = category.OwnerEmail;
                existingCategory.OwnerPhone = category.OwnerPhone;
                existingCategory.CanDispalyAllergensOnMenu = category.CanDispalyAllergensOnMenu;
                existingCategory.CanEnableCoupons = category.CanEnableCoupons;
                existingCategory.TaxId = category.TaxId;
                existingCategory.CanShowTax = category.CanShowTax;
                existingCategory.Domain = category.Domain;
                existingCategory.CanEnableDrivers = category.CanEnableDrivers;
                existingCategory.CanShowGoogleTranslate = category.CanShowGoogleTranslate;
                existingCategory.CanShowAllLanguages = category.CanShowAllLanguages;
                existingCategory.LanguageCode = category.LanguageCode;
                existingCategory.TitleOfImpressum = category.TitleOfImpressum;
                existingCategory.Impressum = category.Impressum;
                existingCategory.CanEnableKitchenDisplaySystem = category.CanEnableKitchenDisplaySystem;
                existingCategory.ManagerEmail = category.ManagerEmail;
                existingCategory.CanEnableOrderAfterWorkingTimeOfTheStore = category.CanEnableOrderAfterWorkingTimeOfTheStore;
                existingCategory.CanShowDateTimePickerEvenIfStoreIsOpen = category.CanShowDateTimePickerEvenIfStoreIsOpen;
                existingCategory.PrintNodeApiKey = category.PrintNodeApiKey;
                existingCategory.MainThermalPrinterId = category.MainThermalPrinterId;
                existingCategory.SecondMainPrinterId = category.SecondMainPrinterId;
                existingCategory.KitchenThermalPrinterId = category.KitchenThermalPrinterId;
                existingCategory.SecondKitchenThermalPrinterId = category.SecondKitchenThermalPrinterId;
                existingCategory.StandardPrinterId = category.StandardPrinterId;
                existingCategory.PrintA4StandardOrderWhen = category.PrintA4StandardOrderWhen;
                existingCategory.PrintOnMainThermalPrinterWhen = category.PrintOnMainThermalPrinterWhen;
                existingCategory.PrintOnKitchenThermalPrinterWhen = category.PrintOnKitchenThermalPrinterWhen;
                existingCategory.SelectYoutMenuTemplate = category.SelectYoutMenuTemplate;
                existingCategory.TimeZone = category.TimeZone;
                existingCategory.TimeLanguage = category.TimeLanguage;
                existingCategory.TimeFormatForOrders = category.TimeFormatForOrders;
                existingCategory.TimeFormatForClosingTime = category.TimeFormatForClosingTime;
                existingCategory.WebhookCalledApprovedBySystem = category.WebhookCalledApprovedBySystem;
                existingCategory.WebhookCalledApprovedByvendor = category.WebhookCalledApprovedByvendor;
                existingCategory.CanEnableCustomDeliveyCost = category.CanEnableCustomDeliveyCost;
                existingCategory.CanEnableCostBasedOnRange = category.CanEnableCostBasedOnRange;
                existingCategory.FirstRange = category.FirstRange;
                existingCategory.SecondRange = category.SecondRange;
                existingCategory.ThirdRange = category.ThirdRange;
                existingCategory.FourthRange = category.FourthRange;
                existingCategory.FifthRange = category.FifthRange;
                existingCategory.PriceForFirstRange = category.PriceForFirstRange;
                existingCategory.PriceForSecondRange = category.PriceForSecondRange;
                existingCategory.PriceForThirdRange = category.PriceForThirdRange;
                existingCategory.PriceForFourthRange = category.PriceForFourthRange;
                existingCategory.PriceForFifthRange = category.PriceForFifthRange;
                existingCategory.Monday = category.Monday;
                existingCategory.MondayStart = category.MondayStart;
                existingCategory.MondayEnd = category.MondayEnd;
                existingCategory.Tuesday = category.Tuesday;
                existingCategory.TuesdayStart = category.TuesdayStart;
                existingCategory.TuesdayEnd = category.TuesdayEnd;
                existingCategory.Wednesday = category.Wednesday;
                existingCategory.WednesdayStart = category.WednesdayStart;
                existingCategory.WednesdayEnd = category.WednesdayEnd;
                existingCategory.Thursday = category.Thursday;
                existingCategory.ThursdayStart = category.ThursdayStart;
                existingCategory.ThursdayEnd = category.ThursdayEnd;
                existingCategory.Friday = category.Friday;
                existingCategory.FridayStart = category.FridayStart;
                existingCategory.FridayEnd = category.FridayEnd;
                existingCategory.Saturday = category.Saturday;
                existingCategory.SaturdayStart = category.SaturdayStart;
                existingCategory.SaturdayEnd = category.SaturdayEnd;
                existingCategory.Sunday = category.Sunday;
                existingCategory.SundayStart = category.SundayStart;
                existingCategory.SundayEnd = category.SundayEnd;
                existingCategory.PayNowPayOutDays = category.PayNowPayOutDays;
                existingCategory.CardPayoutdays = category.CardPayoutdays;
                existingCategory.RestaurantBankShortCode = category.RestaurantBankShortCode;
                existingCategory.RestaurantBankAccountNo = category.RestaurantBankAccountNo;
                existingCategory.RestaurantBankHolderName = category.RestaurantBankHolderName;
                existingCategory.CurrentPlan = category.CurrentPlan;
                existingCategory.Halal = category.Halal;
                existingCategory.FullSetUpBuffet = category.FullSetUpBuffet;
                existingCategory.Vegetarian = category.Vegetarian;
                existingCategory.NonHalal = category.NonHalal;
                existingCategory.ChineseLocalAsian = category.ChineseLocalAsian;
                existingCategory.International = category.International;
                existingCategory.Western = category.Western;
                existingCategory.Thai = category.Thai;
                existingCategory.Japanese = category.Japanese;
                existingCategory.Indian = category.Indian;
                existingCategory.Malay = category.Malay;
                existingCategory.NonVeg = category.NonVeg;
                existingCategory.Role = category.Role;
                existingCategory.Delivery = category.Delivery;
                existingCategory.Buffet = category.Buffet;
                _context.Update(existingCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SuperAdminRestaurantEdit));
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while processing your request. Please try again later.");
            }
        }
        // GET: Restaurants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View("~/Views/Restaurant/Restaurants/Details.cshtml", restaurant);
        }
        // GET: Restaurants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View("~/Views/Restaurant/Restaurants/Delete.cshtml", restaurant);
        }
        // POST: Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Restaurants == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Restaurants'  is null.");
            }
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.RestaurantId == id)).GetValueOrDefault();
        }
    }
}
