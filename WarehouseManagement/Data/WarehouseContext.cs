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
        public DbSet<AnnualInventory> AnnualInventories { get; set; }
        public DbSet<PurchasePrice> PurchasePrices { get; set; }
        public DbSet<ConsumedMaterial> ConsumedMaterials { get; set; }
        public DbSet<Form2Item> Form2Items { get; set; }
        public DbSet<PurchaseBatch> PurchaseBatches { get; set; }
        public DbSet<Form5Item> Form5Items { get; set; }
        public DbSet<Form5PurchaseDetail> Form5PurchaseDetails { get; set; }
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

            // AnnualInventory - Material (Many-to-One)
            modelBuilder.Entity<AnnualInventory>()
                .HasOne(a => a.Material)
                .WithMany()
                .HasForeignKey(a => a.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchasePrice - Material (Many-to-One)
            modelBuilder.Entity<PurchasePrice>()
                .HasOne(p => p.Material)
                .WithMany()
                .HasForeignKey(p => p.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // ConsumedMaterial - Material (Many-to-One)
            modelBuilder.Entity<ConsumedMaterial>()
                .HasOne(c => c.Material)
                .WithMany()
                .HasForeignKey(c => c.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // تكوين الفهارس
            modelBuilder.Entity<Material>()
                .HasIndex(m => m.Code)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasIndex(l => l.Code)
                .IsUnique();

            modelBuilder.Entity<AnnualInventory>()
                .HasIndex(a => new { a.MaterialId, a.Year })
                .IsUnique();

            modelBuilder.Entity<PurchasePrice>()
                .HasIndex(p => p.PurchaseYear);

            modelBuilder.Entity<ConsumedMaterial>()
                .HasIndex(c => c.ConsumptionDate);

            // تكوين أنواع البيانات
            modelBuilder.Entity<Material>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchasePrice>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchasePrice>()
                .Property(p => p.ExchangeRate)
                .HasColumnType("decimal(18,4)");

            modelBuilder.Entity<ConsumedMaterial>()
                .Property(c => c.OriginalUnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ConsumedMaterial>()
                .Property(c => c.OriginalTotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ConsumedMaterial>()
                .Property(c => c.ResidualValue)
                .HasColumnType("decimal(18,2)");
            // Form2Item Configuration
            modelBuilder.Entity<Form2Item>()
                .HasIndex(f => new { f.CodeNumber, f.InventoryYear })
                .IsUnique();

            modelBuilder.Entity<Form2Item>()
                .Property(f => f.CostInDinar)
                .HasColumnType("decimal(18,2)");

            // Form2Item - Material (Optional Many-to-One)
            modelBuilder.Entity<Form2Item>()
                .HasOne(f => f.Material)
                .WithMany()
                .HasForeignKey(f => f.MaterialId)
                .OnDelete(DeleteBehavior.SetNull);

            // PurchaseBatch - Form2Item (Many-to-One)
            modelBuilder.Entity<PurchaseBatch>()
                .HasOne(p => p.Form2Item)
                .WithMany(f => f.PurchaseBatches)
                .HasForeignKey(p => p.Form2ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseBatch>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseBatch>()
                .Property(p => p.StoredTotalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseBatch>()
                .HasIndex(p => p.PurchaseYear);

            // Form5Item Configuration
            modelBuilder.Entity<Form5Item>()
                .Property(f => f.OriginalUnitPriceDinar)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Form5Item>()
                .Property(f => f.OriginalTotalPriceDinar)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Form5Item>()
                .Property(f => f.ResidualValue)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Form5Item>()
                .Property(f => f.DamagePercentage)
                .HasColumnType("decimal(5,2)");

            // Form5Item - Form2Item (Optional Many-to-One)
            modelBuilder.Entity<Form5Item>()
                .HasOne(f => f.Form2Item)
                .WithMany(f2 => f2.Form5Items)
                .HasForeignKey(f => f.Form2ItemId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Form5Item>()
                .HasIndex(f => f.ReportDate);

            // Form5PurchaseDetail - Form5Item (Many-to-One)
            modelBuilder.Entity<Form5PurchaseDetail>()
                .HasOne(p => p.Form5Item)
                .WithMany(f => f.PurchaseDetails)
                .HasForeignKey(p => p.Form5ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Form5PurchaseDetail>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Form5PurchaseDetail>()
                .Property(p => p.StoredTotalPrice)
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
            // تاريخ ثابت لتجنب مشاكل Entity Framework مع DateTime.Now
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "أجهزة كمبيوتر", Description = "أجهزة كمبيوتر وملحقاتها", CreatedDate = seedDate },
                new Category { Id = 2, Name = "أثاث مكتبي", Description = "كراسي ومكاتب وخزائن", CreatedDate = seedDate },
                new Category { Id = 3, Name = "قرطاسية", Description = "أوراق وأقلام ومستلزمات مكتبية", CreatedDate = seedDate },
                new Category { Id = 4, Name = "أجهزة كهربائية", Description = "أجهزة كهربائية متنوعة", CreatedDate = seedDate },
                new Category { Id = 5, Name = "مواد تنظيف", Description = "مواد ومستلزمات التنظيف", CreatedDate = seedDate },
                new Category { Id = 6, Name = "أدوات ومعدات", Description = "أدوات ومعدات متنوعة", CreatedDate = seedDate },
                new Category { Id = 7, Name = "مستلزمات طبية", Description = "مستلزمات ومعدات طبية", CreatedDate = seedDate }
            );

            // Seed Locations
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, Name = "المخزن الرئيسي", Code = "WH-01", Type = LocationType.Warehouse, MaxCapacity = 1000, Floor = 1, Building = "المبنى الرئيسي", CreatedDate = seedDate },
                new Location { Id = 2, Name = "مكتب الإدارة", Code = "OF-01", Type = LocationType.Office, Floor = 2, Building = "المبنى الرئيسي", CreatedDate = seedDate },
                new Location { Id = 3, Name = "ورشة الصيانة", Code = "WS-01", Type = LocationType.Workshop, Floor = 1, Building = "المبنى الفرعي", CreatedDate = seedDate },
                new Location { Id = 4, Name = "غرفة الاجتماعات", Code = "MR-01", Type = LocationType.MeetingRoom, Floor = 3, Building = "المبنى الرئيسي", CreatedDate = seedDate },
                new Location { Id = 5, Name = "المختبر", Code = "LB-01", Type = LocationType.Laboratory, Floor = 2, Building = "المبنى الفرعي", CreatedDate = seedDate },
                new Location { Id = 6, Name = "قسم الهندسة الكهروميكانيك", Code = "ELEC-01", Type = LocationType.Office, Floor = 2, Building = "المبنى الهندسي", CreatedDate = seedDate }
            );

            // Seed Sample Materials
            modelBuilder.Entity<Material>().HasData(
                new Material { Id = 1, Name = "حاسوب لاب توب", Code = "1/1/1", Description = "جهاز حاسوب محمول", Quantity = 159, Unit = "قطعة", Price = 750000.00m, CategoryId = 1, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 2, Name = "حاسوب سيستم (بلتن الكيس)", Code = "1/1/2", Description = "حاسوب مكتبي مدمج", Quantity = 73, Unit = "قطعة", Price = 570000.00m, CategoryId = 1, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 3, Name = "كيس حاسوب دسك توب", Code = "2/1/1", Description = "صندوق حاسوب مكتبي", Quantity = 143, Unit = "قطعة", Price = 300000.00m, CategoryId = 1, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 4, Name = "UPS (1000 – 500)", Code = "3/1/1", Description = "مصدر طاقة غير منقطع", Quantity = 83, Unit = "قطعة", Price = 64000.00m, CategoryId = 4, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 5, Name = "UPS (2500 -1100)", Code = "3/1/2", Description = "مصدر طاقة غير منقطع", Quantity = 6, Unit = "قطعة", Price = 85000.00m, CategoryId = 4, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 6, Name = "شاشة حاسوب 17 in", Code = "4/1/2", Description = "شاشة حاسوب 17 بوصة", Quantity = 4, Unit = "قطعة", Price = 150000.00m, CategoryId = 1, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 7, Name = "منضدة مكتب مع ملحق خشب", Code = "6/1/1", Description = "منضدة مكتب خشبية مع ملحقات", Quantity = 5, Unit = "قطعة", Price = 300700.00m, CategoryId = 2, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 8, Name = "منضدة مكتب معدن بدون مجرات", Code = "6/3/1", Description = "منضدة مكتب معدنية", Quantity = 143, Unit = "قطعة", Price = 133000.00m, CategoryId = 2, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 9, Name = "منضدة مكتب خشب مع مجرات", Code = "6/2/2", Description = "منضدة مكتب خشبية مع أدراج", Quantity = 160, Unit = "قطعة", Price = 56500.00m, CategoryId = 2, LocationId = 6, CreatedDate = seedDate, LastUpdated = seedDate },
                new Material { Id = 10, Name = "ورق A4", Code = "PAPER-001", Description = "ورق طباعة أبيض", Quantity = 100, Unit = "علبة", Price = 25000.00m, CategoryId = 3, LocationId = 1, CreatedDate = seedDate, LastUpdated = seedDate }
            );
        }
    }
}
