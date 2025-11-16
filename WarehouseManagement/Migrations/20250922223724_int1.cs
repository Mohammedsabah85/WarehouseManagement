using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseManagement.Migrations
{
    /// <inheritdoc />
    public partial class int1 : Migration
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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 23, 1, 37, 23, 727, DateTimeKind.Local).AddTicks(6943), "أجهزة كمبيوتر وملحقاتها", "أجهزة كمبيوتر" },
                    { 2, new DateTime(2025, 9, 23, 1, 37, 23, 727, DateTimeKind.Local).AddTicks(7209), "كراسي ومكاتب وخزائن", "أثاث مكتبي" },
                    { 3, new DateTime(2025, 9, 23, 1, 37, 23, 727, DateTimeKind.Local).AddTicks(7211), "أوراق وأقلام ومستلزمات مكتبية", "قرطاسية" },
                    { 4, new DateTime(2025, 9, 23, 1, 37, 23, 727, DateTimeKind.Local).AddTicks(7213), "أجهزة كهربائية متنوعة", "أجهزة كهربائية" },
                    { 5, new DateTime(2025, 9, 23, 1, 37, 23, 727, DateTimeKind.Local).AddTicks(7215), "مواد ومستلزمات التنظيف", "مواد تنظيف" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Building", "Code", "CreatedDate", "Description", "Floor", "MaxCapacity", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "المبنى الرئيسي", "WH-01", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5192), null, 1, 1000, "المخزن الرئيسي", 1 },
                    { 2, "المبنى الرئيسي", "OF-01", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5437), null, 2, null, "مكتب الإدارة", 2 },
                    { 3, "المبنى الفرعي", "WS-01", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5439), null, 1, null, "ورشة الصيانة", 3 },
                    { 4, "المبنى الرئيسي", "MR-01", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5442), null, 3, null, "غرفة الاجتماعات", 5 },
                    { 5, "المبنى الفرعي", "LB-01", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5445), null, 2, null, "المختبر", 4 }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "CategoryId", "Code", "CreatedDate", "Description", "ExpiryDate", "LastUpdated", "LocationId", "Name", "Notes", "Price", "Quantity", "Unit" },
                values: new object[,]
                {
                    { 1, 1, "COMP-001", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), "جهاز كمبيوتر مكتبي", null, new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), 1, "جهاز كمبيوتر ديل", null, 2500.00m, 10, "قطعة" },
                    { 2, 2, "CHAIR-001", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), "كرسي مكتبي دوار", null, new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), 1, "كرسي مكتبي", null, 450.00m, 25, "قطعة" },
                    { 3, 3, "PAPER-001", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), "ورق طباعة أبيض", null, new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), 1, "ورق A4", null, 25.00m, 5, "علبة" },
                    { 4, 4, "PRINT-001", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), "طابعة ليزر أحادية", null, new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), 2, "طابعة ليزر", null, 1800.00m, 3, "قطعة" },
                    { 5, 5, "CLEAN-001", new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), "مطهر أرضيات معطر", new DateTime(2025, 10, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), new DateTime(2025, 9, 23, 1, 37, 23, 728, DateTimeKind.Local).AddTicks(5565), 1, "مطهر أرضيات", null, 15.00m, 2, "لتر" }
                });

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
                name: "Transfers");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
