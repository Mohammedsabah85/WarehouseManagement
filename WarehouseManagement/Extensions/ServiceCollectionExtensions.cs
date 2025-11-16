using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Services;

namespace WarehouseManagement.Extensions
{
    /// <summary>
    /// Extension Methods لتكوين الخدمات
    /// تسهل إعداد التطبيق
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// يضيف جميع خدمات التطبيق
        /// </summary>
        public static IServiceCollection AddWarehouseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // إضافة Entity Framework
            services.AddDbContext<WarehouseContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // إضافة الخدمات المخصصة
            services.AddScoped<IReportService, ReportService>();

            // إضافة خدمات أخرى حسب الحاجة
            services.AddScoped<IMaterialService, MaterialService>();
            services.AddScoped<ITransferService, TransferService>();

            return services;
        }

        /// <summary>
        /// يضيف خدمات الذاكرة المؤقتة
        /// </summary>
        public static IServiceCollection AddWarehouseCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();

            return services;
        }

        /// <summary>
        /// يضيف خدمات الأمان
        /// </summary>
        public static IServiceCollection AddWarehouseSecurity(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            return services;
        }
    }

    // واجهات الخدمات الإضافية
    public interface IMaterialService
    {
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task<List<Material>> GetLowStockMaterialsAsync(int threshold = 10);
        Task<bool> UpdateQuantityAsync(int materialId, int newQuantity);
    }

    public interface ITransferService
    {
        Task<bool> CanTransferAsync(int materialId, int fromLocationId, int quantity);
        Task<Transfer> CreateTransferAsync(Transfer transfer);
        Task<bool> ConfirmTransferAsync(int transferId, string confirmedBy);
    }

    // تنفيذ أساسي للخدمات
    public class MaterialService : IMaterialService
    {
        private readonly WarehouseContext _context;

        public MaterialService(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            return !await _context.Materials
                .Where(m => m.Code == code && (!excludeId.HasValue || m.Id != excludeId.Value))
                .AnyAsync();
        }

        public async Task<List<Material>> GetLowStockMaterialsAsync(int threshold = 10)
        {
            return await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.Quantity <= threshold)
                .OrderBy(m => m.Quantity)
                .ToListAsync();
        }

        public async Task<bool> UpdateQuantityAsync(int materialId, int newQuantity)
        {
            var material = await _context.Materials.FindAsync(materialId);
            if (material == null) return false;

            material.Quantity = newQuantity;
            material.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class TransferService : ITransferService
    {
        private readonly WarehouseContext _context;

        public TransferService(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<bool> CanTransferAsync(int materialId, int fromLocationId, int quantity)
        {
            var material = await _context.Materials
                .FirstOrDefaultAsync(m => m.Id == materialId && m.LocationId == fromLocationId);

            return material != null && material.Quantity >= quantity;
        }

        public async Task<Transfer> CreateTransferAsync(Transfer transfer)
        {
            _context.Transfers.Add(transfer);
            await _context.SaveChangesAsync();
            return transfer;
        }

        public async Task<bool> ConfirmTransferAsync(int transferId, string confirmedBy)
        {
            var transfer = await _context.Transfers.FindAsync(transferId);
            if (transfer == null || transfer.IsConfirmed) return false;

            transfer.IsConfirmed = true;
            transfer.ConfirmedDate = DateTime.Now;
            transfer.ConfirmedBy = confirmedBy;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
