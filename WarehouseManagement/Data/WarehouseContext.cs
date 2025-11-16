using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Models;

namespace WarehouseManagement.Data
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options) : base(options)
        {
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تكوين العلاقات بشكل صريح

            // Material - Category (Many-to-One)
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Materials)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Material - Location (Many-to-One)
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Location)
                .WithMany(l => l.Materials)
                .HasForeignKey(m => m.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer - Material (Many-to-One)
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.Material)
                .WithMany()
                .HasForeignKey(t => t.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer - FromLocation (Many-to-One)
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.FromLocation)
                .WithMany()
                .HasForeignKey(t => t.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transfer - ToLocation (Many-to-One)
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.ToLocation)
                .WithMany()
                .HasForeignKey(t => t.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // تكوين الفهارس
            modelBuilder.Entity<Material>()
                .HasIndex(m => m.Code)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Code)
                .IsUnique();

            // تكوين أنواع البيانات
            modelBuilder.Entity<Material>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");

            // منع حذف المراجع المتقاطعة
            modelBuilder.Entity<Transfer>()
                .HasCheckConstraint("CK_Transfer_DifferentLocations",
                    "[FromLocationId] <> [ToLocationId]");

            // بيانات أولية
            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "أجهزة كمبيوتر", Description = "أجهزة كمبيوتر وملحقاتها", CreatedDate = DateTime.Now },
                new Category { Id = 2, Name = "أثاث مكتبي", Description = "كراسي ومكاتب وخزائن", CreatedDate = DateTime.Now },
                new Category { Id = 3, Name = "قرطاسية", Description = "أوراق وأقلام ومستلزمات مكتبية", CreatedDate = DateTime.Now },
                new Category { Id = 4, Name = "أجهزة كهربائية", Description = "أجهزة كهربائية متنوعة", CreatedDate = DateTime.Now },
                new Category { Id = 5, Name = "مواد تنظيف", Description = "مواد ومستلزمات التنظيف", CreatedDate = DateTime.Now }
            );

            // Seed Locations
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, Name = "المخزن الرئيسي", Code = "WH-01", Type = LocationType.Warehouse, MaxCapacity = 1000, Floor = 1, Building = "المبنى الرئيسي", CreatedDate = DateTime.Now },
                new Location { Id = 2, Name = "مكتب الإدارة", Code = "OF-01", Type = LocationType.Office, Floor = 2, Building = "المبنى الرئيسي", CreatedDate = DateTime.Now },
                new Location { Id = 3, Name = "ورشة الصيانة", Code = "WS-01", Type = LocationType.Workshop, Floor = 1, Building = "المبنى الفرعي", CreatedDate = DateTime.Now },
                new Location { Id = 4, Name = "غرفة الاجتماعات", Code = "MR-01", Type = LocationType.MeetingRoom, Floor = 3, Building = "المبنى الرئيسي", CreatedDate = DateTime.Now },
                new Location { Id = 5, Name = "المختبر", Code = "LB-01", Type = LocationType.Laboratory, Floor = 2, Building = "المبنى الفرعي", CreatedDate = DateTime.Now }
            );

            // Seed Sample Materials
            var now = DateTime.Now;
            modelBuilder.Entity<Material>().HasData(
                new Material { Id = 1, Name = "جهاز كمبيوتر ديل", Code = "COMP-001", Description = "جهاز كمبيوتر مكتبي", Quantity = 10, Unit = "قطعة", Price = 2500.00m, CategoryId = 1, LocationId = 1, CreatedDate = now, LastUpdated = now },
                new Material { Id = 2, Name = "كرسي مكتبي", Code = "CHAIR-001", Description = "كرسي مكتبي دوار", Quantity = 25, Unit = "قطعة", Price = 450.00m, CategoryId = 2, LocationId = 1, CreatedDate = now, LastUpdated = now },
                new Material { Id = 3, Name = "ورق A4", Code = "PAPER-001", Description = "ورق طباعة أبيض", Quantity = 5, Unit = "علبة", Price = 25.00m, CategoryId = 3, LocationId = 1, CreatedDate = now, LastUpdated = now },
                new Material { Id = 4, Name = "طابعة ليزر", Code = "PRINT-001", Description = "طابعة ليزر أحادية", Quantity = 3, Unit = "قطعة", Price = 1800.00m, CategoryId = 4, LocationId = 2, CreatedDate = now, LastUpdated = now },
                new Material { Id = 5, Name = "مطهر أرضيات", Code = "CLEAN-001", Description = "مطهر أرضيات معطر", Quantity = 2, Unit = "لتر", Price = 15.00m, ExpiryDate = now.AddDays(30), CategoryId = 5, LocationId = 1, CreatedDate = now, LastUpdated = now }
            );
        }
    }
}