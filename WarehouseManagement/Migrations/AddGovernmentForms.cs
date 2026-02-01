using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Migrations
{
    /// <summary>
    /// Migration لإضافة جداول النماذج الحكومية (Form2, Form5, PurchaseBatches)
    /// </summary>
    public partial class AddGovernmentForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // جدول نموذج 2 - قائمة الموجودات الرئيسية
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

            // جدول نموذج 5 - قائمة الموجودات المستهلكة
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

            // جدول دفعات الشراء
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

            // جدول تفاصيل شراء نموذج 5
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

            // الفهارس
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
                name: "IX_Form2Items_InventoryYear",
                table: "Form2Items",
                column: "InventoryYear");

            migrationBuilder.CreateIndex(
                name: "IX_Form5Items_Form2ItemId",
                table: "Form5Items",
                column: "Form2ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Form5Items_ReportDate",
                table: "Form5Items",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBatches_Form2ItemId",
                table: "PurchaseBatches",
                column: "Form2ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBatches_PurchaseYear",
                table: "PurchaseBatches",
                column: "PurchaseYear");

            migrationBuilder.CreateIndex(
                name: "IX_Form5PurchaseDetails_Form5ItemId",
                table: "Form5PurchaseDetails",
                column: "Form5ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Form5PurchaseDetails");
            migrationBuilder.DropTable(name: "PurchaseBatches");
            migrationBuilder.DropTable(name: "Form5Items");
            migrationBuilder.DropTable(name: "Form2Items");
        }
    }
}
