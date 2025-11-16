using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;

namespace WarehouseManagement.Database
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(WarehouseContext context)
        {
            // Seed Categories if not exist
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "أجهزة كمبيوتر", Description = "أجهزة كمبيوتر وملحقاتها" },
                    new Category { Name = "أثاث مكتبي", Description = "كراسي ومكاتب وخزائن" },
                    new Category { Name = "قرطاسية", Description = "أوراق وأقلام ومستلزمات مكتبية" },
                    new Category { Name = "أجهزة كهربائية", Description = "أجهزة كهربائية متنوعة" },
                    new Category { Name = "مواد تنظيف", Description = "مواد ومستلزمات التنظيف" },
                    new Category { Name = "أدوات ومعدات", Description = "أدوات ومعدات متنوعة" },
                    new Category { Name = "مستلزمات طبية", Description = "مستلزمات ومعدات طبية" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed Locations if not exist
            if (!await context.Locations.AnyAsync())
            {
                var locations = new List<Location>
                {
                    new Location { Name = "المخزن الرئيسي", Code = "WH-01", Type = LocationType.Warehouse, MaxCapacity = 1000, Floor = 1, Building = "المبنى الرئيسي" },
                    new Location { Name = "مخزن المواد الطبية", Code = "WH-02", Type = LocationType.Warehouse, MaxCapacity = 500, Floor = 1, Building = "المبنى الطبي" },
                    new Location { Name = "مكتب الإدارة", Code = "OF-01", Type = LocationType.Office, Floor = 2, Building = "المبنى الرئيسي" },
                    new Location { Name = "مكتب المحاسبة", Code = "OF-02", Type = LocationType.Office, Floor = 2, Building = "المبنى الرئيسي" },
                    new Location { Name = "ورشة الصيانة", Code = "WS-01", Type = LocationType.Workshop, Floor = 1, Building = "المبنى الفرعي" },
                    new Location { Name = "غرفة الاجتماعات الكبرى", Code = "MR-01", Type = LocationType.MeetingRoom, Floor = 3, Building = "المبنى الرئيسي" },
                    new Location { Name = "غرفة الاجتماعات الصغرى", Code = "MR-02", Type = LocationType.MeetingRoom, Floor = 3, Building = "المبنى الرئيسي" },
                    new Location { Name = "المختبر الرئيسي", Code = "LB-01", Type = LocationType.Laboratory, Floor = 2, Building = "المبنى الفرعي" },
                    new Location { Name = "مختبر الأبحاث", Code = "LB-02", Type = LocationType.Laboratory, Floor = 3, Building = "المبنى الفرعي" }
                };

                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();
            }

            // Seed Sample Materials if not exist
            if (!await context.Materials.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var locations = await context.Locations.ToListAsync();

                var materials = new List<Material>
                {
                    // أجهزة كمبيوتر
                    new Material { Name = "جهاز كمبيوتر ديل", Code = "COMP-001", Description = "جهاز كمبيوتر مكتبي ديل OptiPlex", Quantity = 15, Unit = "قطعة", Price = 2500.00m, CategoryId = categories.First(c => c.Name == "أجهزة كمبيوتر").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    new Material { Name = "شاشة سامسونج 24 بوصة", Code = "MON-001", Description = "شاشة LED 24 بوصة", Quantity = 20, Unit = "قطعة", Price = 800.00m, CategoryId = categories.First(c => c.Name == "أجهزة كمبيوتر").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    new Material { Name = "لوحة مفاتيح لاسلكية", Code = "KEY-001", Description = "لوحة مفاتيح لاسلكية عربي/إنجليزي", Quantity = 8, Unit = "قطعة", Price = 150.00m, CategoryId = categories.First(c => c.Name == "أجهزة كمبيوتر").Id, LocationId = locations.First(l => l.Code == "OF-01").Id },
                    
                    // أثاث مكتبي
                    new Material { Name = "مكتب خشبي", Code = "DESK-001", Description = "مكتب خشبي بأدراج", Quantity = 12, Unit = "قطعة", Price = 1200.00m, CategoryId = categories.First(c => c.Name == "أثاث مكتبي").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    new Material { Name = "كرسي مكتبي دوار", Code = "CHAIR-001", Description = "كرسي مكتبي دوار مريح", Quantity = 25, Unit = "قطعة", Price = 450.00m, CategoryId = categories.First(c => c.Name == "أثاث مكتبي").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    new Material { Name = "خزانة ملفات", Code = "CAB-001", Description = "خزانة ملفات معدنية 4 أدراج", Quantity = 6, Unit = "قطعة", Price = 800.00m, CategoryId = categories.First(c => c.Name == "أثاث مكتبي").Id, LocationId = locations.First(l => l.Code == "OF-02").Id },
                    
                    // قرطاسية
                    new Material { Name = "ورق A4", Code = "PAPER-001", Description = "ورق طباعة A4 أبيض", Quantity = 100, Unit = "علبة", Price = 25.00m, CategoryId = categories.First(c => c.Name == "قرطاسية").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    new Material { Name = "أقلام جافة زرقاء", Code = "PEN-001", Description = "أقلام جافة زرقاء", Quantity = 200, Unit = "قطعة", Price = 2.00m, CategoryId = categories.First(c => c.Name == "قرطاسية").Id, LocationId = locations.First(l => l.Code == "OF-01").Id },
                    new Material { Name = "دباسة مكتبية", Code = "STAP-001", Description = "دباسة مكتبية كبيرة", Quantity = 3, Unit = "قطعة", Price = 45.00m, CategoryId = categories.First(c => c.Name == "قرطاسية").Id, LocationId = locations.First(l => l.Code == "OF-02").Id },
                    
                    // مواد تنظيف - with expiry dates
                    new Material { Name = "مطهر أرضيات", Code = "CLEAN-001", Description = "مطهر أرضيات معطر", Quantity = 5, Unit = "لتر", Price = 15.00m, ExpiryDate = DateTime.Now.AddDays(180), CategoryId = categories.First(c => c.Name == "مواد تنظيف").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    new Material { Name = "مناديل ورقية", Code = "TISSUE-001", Description = "مناديل ورقية ناعمة", Quantity = 2, Unit = "علبة", Price = 8.00m, ExpiryDate = DateTime.Now.AddDays(15), CategoryId = categories.First(c => c.Name == "مواد تنظيف").Id, LocationId = locations.First(l => l.Code == "MR-01").Id },
                    
                    // أجهزة كهربائية
                    new Material { Name = "طابعة ليزر HP", Code = "PRINT-001", Description = "طابعة ليزر أحادية اللون", Quantity = 4, Unit = "قطعة", Price = 1800.00m, CategoryId = categories.First(c => c.Name == "أجهزة كهربائية").Id, LocationId = locations.First(l => l.Code == "OF-01").Id },
                    new Material { Name = "مكيف هواء سبليت", Code = "AC-001", Description = "مكيف هواء سبليت 2 طن", Quantity = 1, Unit = "قطعة", Price = 3500.00m, CategoryId = categories.First(c => c.Name == "أجهزة كهربائية").Id, LocationId = locations.First(l => l.Code == "WH-01").Id },
                    
                    // مستلزمات طبية
                    new Material { Name = "كمامات طبية", Code = "MASK-001", Description = "كمامات طبية يدوية", Quantity = 0, Unit = "علبة", Price = 25.00m, ExpiryDate = DateTime.Now.AddDays(-10), CategoryId = categories.First(c => c.Name == "مستلزمات طبية").Id, LocationId = locations.First(l => l.Code == "WH-02").Id },
                    new Material { Name = "قفازات طبية", Code = "GLOVE-001", Description = "قفازات طبية مطاطية", Quantity = 5, Unit = "علبة", Price = 35.00m, ExpiryDate = DateTime.Now.AddDays(90), CategoryId = categories.First(c => c.Name == "مستلزمات طبية").Id, LocationId = locations.First(l => l.Code == "LB-01").Id }
                };

                await context.Materials.AddRangeAsync(materials);
                await context.SaveChangesAsync();
            }

            // Seed Sample Transfers if not exist
            if (!await context.Transfers.AnyAsync())
            {
                var materials = await context.Materials.ToListAsync();
                var locations = await context.Locations.ToListAsync();

                var transfers = new List<Transfer>
                {
                    new Transfer
                    {
                        MaterialId = materials.First(m => m.Code == "CHAIR-001").Id,
                        FromLocationId = locations.First(l => l.Code == "WH-01").Id,
                        ToLocationId = locations.First(l => l.Code == "OF-01").Id,
                        Quantity = 3,
                        TransferDate = DateTime.Now.AddDays(-5),
                        Reason = "تجهيز مكتب جديد",
                        TransferredBy = "أحمد محمد",
                        IsConfirmed = true,
                        ConfirmedDate = DateTime.Now.AddDays(-5),
                        ConfirmedBy = "سارة أحمد"
                    },
                    new Transfer
                    {
                        MaterialId = materials.First(m => m.Code == "PAPER-001").Id,
                        FromLocationId = locations.First(l => l.Code == "WH-01").Id,
                        ToLocationId = locations.First(l => l.Code == "OF-02").Id,
                        Quantity = 10,
                        TransferDate = DateTime.Now.AddDays(-2),
                        Reason = "تجديد المخزون",
                        TransferredBy = "علي حسن",
                        IsConfirmed = false
                    },
                    new Transfer
                    {
                        MaterialId = materials.First(m => m.Code == "GLOVE-001").Id,
                        FromLocationId = locations.First(l => l.Code == "WH-02").Id,
                        ToLocationId = locations.First(l => l.Code == "LB-02").Id,
                        Quantity = 2,
                        TransferDate = DateTime.Now.AddDays(-1),
                        Reason = "احتياج المختبر",
                        TransferredBy = "فاطمة علي",
                        IsConfirmed = true,
                        ConfirmedDate = DateTime.Now.AddDays(-1),
                        ConfirmedBy = "محمد حسام"
                    }
                };

                await context.Transfers.AddRangeAsync(transfers);
                await context.SaveChangesAsync();
            }
        }
    }
}
