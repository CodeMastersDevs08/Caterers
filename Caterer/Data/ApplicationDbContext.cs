using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Caterer.Models;

namespace Caterer.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }       
        public DbSet<Caterer.Models.User>? Users{ get; set; }
        public DbSet<Caterer.Models.Restaurant>? Restaurants { get; set; }
        public DbSet<Caterer.Models.Tax> Taxes { get; set; }
        public DbSet<Caterer.Models.Category>? Categories { get; set; }           
        public DbSet<Caterer.Models.Supplier>? Suppliers { get; set; }
        public DbSet<Caterer.Models.Measurement>? Measurements { get; set; }
        public DbSet<Caterer.Models.Product>? Products { get; set; }
        public DbSet<Caterer.Models.PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Caterer.Models.GRN> GRNS { get; set; }
        public DbSet<Caterer.Models.StockAdjustment> StockAdjustments { get; set; }
        public DbSet<Caterer.Models.MenuCategory> MenuCategories { get; set; }
        public DbSet<Caterer.Models.MenuItem> MenuItems { get; set; }
        public DbSet<Caterer.Models.Extra> Extras { get; set; }
        public DbSet<Caterer.Models.CateringCategory> CateringCategories { get; set; }
        public DbSet<Caterer.Models.CateringItem> CateringItems { get; set; }
        public DbSet<Caterer.Models.CateringExtra> CateringExtras { get; set; }
        public DbSet<Caterer.Models.OrderDetailWebsite> OrderDetailWebsites { get; set; }
        public DbSet<Caterer.Models.MenuRecipe> MenuRecipes { get; set; }
        public DbSet<Caterer.Models.Warehouse> Warehouses { get; set; }
        //public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<Caterer.Models.Production> Productions { get; set; }
        public DbSet<Caterer.Models.PreProduction> PreProductions { get; set; }

        public DbSet<Caterer.Models.Tog> Togs { get; set; }
    }

}