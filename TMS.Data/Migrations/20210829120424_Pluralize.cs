using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class Pluralize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Denominations_Currency_CurrencyID",
                table: "Denominations");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceEntity_ServiceEntityID",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceType_ServiceTypeID",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceType",
                table: "ServiceType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceEntity",
                table: "ServiceEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.RenameTable(
                name: "ServiceType",
                newName: "ServiceTypes");

            migrationBuilder.RenameTable(
                name: "ServiceEntity",
                newName: "ServiceEntities");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currencies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceTypes",
                table: "ServiceTypes",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceEntities",
                table: "ServiceEntities",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Denominations_Currencies_CurrencyID",
                table: "Denominations",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceEntities_ServiceEntityID",
                table: "Service",
                column: "ServiceEntityID",
                principalTable: "ServiceEntities",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceTypes_ServiceTypeID",
                table: "Service",
                column: "ServiceTypeID",
                principalTable: "ServiceTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Denominations_Currencies_CurrencyID",
                table: "Denominations");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceEntities_ServiceEntityID",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceTypes_ServiceTypeID",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceTypes",
                table: "ServiceTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceEntities",
                table: "ServiceEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.RenameTable(
                name: "ServiceTypes",
                newName: "ServiceType");

            migrationBuilder.RenameTable(
                name: "ServiceEntities",
                newName: "ServiceEntity");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currency");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceType",
                table: "ServiceType",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceEntity",
                table: "ServiceEntity",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Denominations_Currency_CurrencyID",
                table: "Denominations",
                column: "CurrencyID",
                principalTable: "Currency",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceEntity_ServiceEntityID",
                table: "Service",
                column: "ServiceEntityID",
                principalTable: "ServiceEntity",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceType_ServiceTypeID",
                table: "Service",
                column: "ServiceTypeID",
                principalTable: "ServiceType",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
