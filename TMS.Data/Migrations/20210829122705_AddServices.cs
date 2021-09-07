using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceCategories_ServiceCategoryID",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceEntities_ServiceEntityID",
                table: "Service");

            migrationBuilder.DropForeignKey(
                name: "FK_Service_ServiceTypes_ServiceTypeID",
                table: "Service");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Service",
                table: "Service");

            migrationBuilder.RenameTable(
                name: "Service",
                newName: "Services");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServiceTypeID",
                table: "Services",
                newName: "IX_Services_ServiceTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServiceEntityID",
                table: "Services",
                newName: "IX_Services_ServiceEntityID");

            migrationBuilder.RenameIndex(
                name: "IX_Service_ServiceCategoryID",
                table: "Services",
                newName: "IX_Services_ServiceCategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryID",
                table: "Services",
                column: "ServiceCategoryID",
                principalTable: "ServiceCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceEntities_ServiceEntityID",
                table: "Services",
                column: "ServiceEntityID",
                principalTable: "ServiceEntities",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceTypes_ServiceTypeID",
                table: "Services",
                column: "ServiceTypeID",
                principalTable: "ServiceTypes",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryID",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceEntities_ServiceEntityID",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceTypes_ServiceTypeID",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Service");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceTypeID",
                table: "Service",
                newName: "IX_Service_ServiceTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceEntityID",
                table: "Service",
                newName: "IX_Service_ServiceEntityID");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ServiceCategoryID",
                table: "Service",
                newName: "IX_Service_ServiceCategoryID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Service",
                table: "Service",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceCategories_ServiceCategoryID",
                table: "Service",
                column: "ServiceCategoryID",
                principalTable: "ServiceCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceEntities_ServiceEntityID",
                table: "Service",
                column: "ServiceEntityID",
                principalTable: "ServiceEntities",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceTypes_ServiceTypeID",
                table: "Service",
                column: "ServiceTypeID",
                principalTable: "ServiceTypes",
                principalColumn: "ID");
        }
    }
}
