using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddDenomination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "ProviderServiceResponseParams",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "ProviderServiceRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Brn",
                table: "ProviderServiceRequests",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "BillPaymentModes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ArName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPaymentModes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentModes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ArName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    CategoryName = table.Column<string>(nullable: true),
                    CategoryNameAr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Denominations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ServiceID = table.Column<int>(nullable: false),
                    OldDenominationID = table.Column<int>(nullable: false),
                    PaymentModeID = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false),
                    APIValue = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    Interval = table.Column<int>(nullable: false),
                    ServiceCategoryID = table.Column<int>(nullable: false),
                    Inquirable = table.Column<bool>(nullable: false),
                    BillPaymentModeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denominations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Denominations_BillPaymentModes_BillPaymentModeID",
                        column: x => x.BillPaymentModeID,
                        principalTable: "BillPaymentModes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Denominations_PaymentModes_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "PaymentModes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Denominations_ServiceCategories_ServiceCategoryID",
                        column: x => x.ServiceCategoryID,
                        principalTable: "ServiceCategories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DenominationServiceProviders_DenominationID",
                table: "DenominationServiceProviders",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_Denominations_BillPaymentModeID",
                table: "Denominations",
                column: "BillPaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Denominations_PaymentModeID",
                table: "Denominations",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Denominations_ServiceCategoryID",
                table: "Denominations",
                column: "ServiceCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_DenominationServiceProviders_Denominations_DenominationID",
                table: "DenominationServiceProviders",
                column: "DenominationID",
                principalTable: "Denominations",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DenominationServiceProviders_Denominations_DenominationID",
                table: "DenominationServiceProviders");

            migrationBuilder.DropTable(
                name: "Denominations");

            migrationBuilder.DropTable(
                name: "BillPaymentModes");

            migrationBuilder.DropTable(
                name: "PaymentModes");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_DenominationServiceProviders_DenominationID",
                table: "DenominationServiceProviders");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ProviderServiceResponseParams");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "ProviderServiceRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Brn",
                table: "ProviderServiceRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
