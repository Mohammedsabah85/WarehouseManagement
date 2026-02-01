using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: true),
                    Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materials_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnualInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ActualQuantity = table.Column<int>(type: "int", nullable: false),
                    RecordedQuantity = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActualLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InventoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InventoryBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDocumented = table.Column<bool>(type: "bit", nullable: false),
                    FormNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualInventories_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsumedMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    ConsumedQuantity = table.Column<int>(type: "int", nullable: false),
                    OriginalPurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OriginalTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DamagePercentage = table.Column<int>(type: "int", nullable: false),
                    UsageDurationDays = table.Column<int>(type: "int", nullable: true),
                    ConsumptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    ReasonDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    CommitteeRecommendations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EvaluatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResidualValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisposalRecordNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDisposed = table.Column<bool>(type: "bit", nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumedMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumedMaterials_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Form2Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaterialDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CodeNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuantityByInventory = table.Column<int>(type: "int", nullable: false),
                    QuantityByRecords = table.Column<int>(type: "int", nullable: false),
                    StoredDifference = table.Column<int>(type: "int", nullable: false),
                    LocationPlace = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CostInDinar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ownership = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDocumentedInForm5 = table.Column<bool>(type: "bit", nullable: false),
                    Form5Id = table.Column<int>(type: "int", nullable: true),
                    InventoryYear = table.Column<int>(type: "int", nullable: false),
                    InventoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form2Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Form2Items_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PurchasePrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    PurchaseYear = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Method = table.Column<int>(type: "int", nullable: false),
                    TransferSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasePrices_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    FromLocationId = table.Column<int>(type: "int", nullable: false),
                    ToLocationId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransferredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.CheckConstraint("CK_Transfer_DifferentLocations", "[FromLocationId] <> [ToLocationId]");
                    table.ForeignKey(
                        name: "FK_Transfers_Locations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Locations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transfers_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Form5Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    MaterialItems = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    PurchaseYear = table.Column<int>(type: "int", nullable: false),
                    OriginalUnitPriceDinar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OriginalUnitPriceFils = table.Column<int>(type: "int", nullable: false),
                    OriginalTotalPriceDinar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OriginalTotalPriceFils = table.Column<int>(type: "int", nullable: false),
                    DamagePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    UsageDuration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsageDurationDays = table.Column<int>(type: "int", nullable: true),
                    ConsumptionReason = table.Column<int>(type: "int", nullable: false),
                    ConsumptionReasonDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CommitteeDecision = table.Column<int>(type: "int", nullable: false),
                    CommitteeDecisionDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResidualValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDisposed = table.Column<bool>(type: "bit", nullable: false),
                    DisposalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisposalRecordNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Form2ItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form5Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Form5Items_Form2Items_Form2ItemId",
                        column: x => x.Form2ItemId,
                        principalTable: "Form2Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Form2ItemId = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    MaterialName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodeNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PurchaseYear = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StoredTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseBatches_Form2Items_Form2ItemId",
                        column: x => x.Form2ItemId,
                        principalTable: "Form2Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Form5PurchaseDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Form5ItemId = table.Column<int>(type: "int", nullable: false),
                    PurchaseYear = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StoredTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form5PurchaseDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Form5PurchaseDetails_Form5Items_Form5ItemId",
                        column: x => x.Form5ItemId,
                        principalTable: "Form5Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "أجهزة كمبيوتر وملحقاتها", "أجهزة كمبيوتر" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "كراسي ومكاتب وخزائن", "أثاث مكتبي" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "أوراق وأقلام ومستلزمات مكتبية", "قرطاسية" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "أجهزة كهربائية متنوعة", "أجهزة كهربائية" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "مواد ومستلزمات التنظيف", "مواد تنظيف" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "أدوات ومعدات متنوعة", "أدوات ومعدات" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "مستلزمات ومعدات طبية", "مستلزمات طبية" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Building", "Code", "CreatedDate", "Description", "Floor", "MaxCapacity", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "المبنى الرئيسي", "WH-01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, 1000, "المخزن الرئيسي", 1 },
                    { 2, "المبنى الرئيسي", "OF-01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, null, "مكتب الإدارة", 2 },
                    { 3, "المبنى الفرعي", "WS-01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, "ورشة الصيانة", 3 },
                    { 4, "المبنى الرئيسي", "MR-01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, null, "غرفة الاجتماعات", 5 },
                    { 5, "المبنى الفرعي", "LB-01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, null, "المختبر", 4 },
                    { 6, "المبنى الهندسي", "ELEC-01", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, null, "قسم الهندسة الكهروميكانيك", 2 }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CategoryId", "Code", "CreatedDate", "Description", "ExpiryDate", "LastUpdated", "LocationId", "Name", "Notes", "Price", "Quantity", "Unit" },
                values: new object[,]
                {
                    { 1, 1, "1/1/1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "جهاز حاسوب محمول", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "حاسوب لاب توب", null, 750000.00m, 159, "قطعة" },
                    { 2, 1, "1/1/2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "حاسوب مكتبي مدمج", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "حاسوب سيستم (بلتن الكيس)", null, 570000.00m, 73, "قطعة" },
                    { 3, 1, "2/1/1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "صندوق حاسوب مكتبي", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "كيس حاسوب دسك توب", null, 300000.00m, 143, "قطعة" },
                    { 4, 4, "3/1/1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "مصدر طاقة غير منقطع", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "UPS (1000 – 500)", null, 64000.00m, 83, "قطعة" },
                    { 5, 4, "3/1/2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "مصدر طاقة غير منقطع", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "UPS (2500 -1100)", null, 85000.00m, 6, "قطعة" },
                    { 6, 1, "4/1/2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "شاشة حاسوب 17 بوصة", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "شاشة حاسوب 17 in", null, 150000.00m, 4, "قطعة" },
                    { 7, 2, "6/1/1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "منضدة مكتب خشبية مع ملحقات", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "منضدة مكتب مع ملحق خشب", null, 300700.00m, 5, "قطعة" },
                    { 8, 2, "6/3/1", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "منضدة مكتب معدنية", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "منضدة مكتب معدن بدون مجرات", null, 133000.00m, 143, "قطعة" },
                    { 9, 2, "6/2/2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "منضدة مكتب خشبية مع أدراج", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "منضدة مكتب خشب مع مجرات", null, 56500.00m, 160, "قطعة" },
                    { 10, 3, "PAPER-001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ورق طباعة أبيض", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "ورق A4", null, 25000.00m, 100, "علبة" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualInventories_MaterialId_Year",
                table: "AnnualInventories",
                columns: new[] { "MaterialId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsumedMaterials_ConsumptionDate",
                table: "ConsumedMaterials",
                column: "ConsumptionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumedMaterials_MaterialId",
                table: "ConsumedMaterials",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Form2Items_CodeNumber_InventoryYear",
                table: "Form2Items",
                columns: new[] { "CodeNumber", "InventoryYear" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Form2Items_MaterialId",
                table: "Form2Items",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Form5Items_Form2ItemId",
                table: "Form5Items",
                column: "Form2ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Form5Items_ReportDate",
                table: "Form5Items",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_Form5PurchaseDetails_Form5ItemId",
                table: "Form5PurchaseDetails",
                column: "Form5ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code",
                table: "Locations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_CategoryId",
                table: "Materials",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Code",
                table: "Materials",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_LocationId",
                table: "Materials",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBatches_Form2ItemId",
                table: "PurchaseBatches",
                column: "Form2ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBatches_PurchaseYear",
                table: "PurchaseBatches",
                column: "PurchaseYear");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasePrices_MaterialId",
                table: "PurchasePrices",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasePrices_PurchaseYear",
                table: "PurchasePrices",
                column: "PurchaseYear");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_FromLocationId",
                table: "Transfers",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_MaterialId",
                table: "Transfers",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ToLocationId",
                table: "Transfers",
                column: "ToLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualInventories");

            migrationBuilder.DropTable(
                name: "ConsumedMaterials");

            migrationBuilder.DropTable(
                name: "Form5PurchaseDetails");

            migrationBuilder.DropTable(
                name: "PurchaseBatches");

            migrationBuilder.DropTable(
                name: "PurchasePrices");

            migrationBuilder.DropTable(
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "Form5Items");

            migrationBuilder.DropTable(
                name: "Form2Items");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
