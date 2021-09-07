using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class RemoveDenoIdAndServiceCategotyId : Migration
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

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "CategoryNameAr",
                table: "ServiceCategories");

            migrationBuilder.AddColumn<string>(
                name: "ArName",
                table: "ServiceCategories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LastNode",
                table: "ServiceCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ServiceCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentID",
                table: "ServiceCategories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceIndex",
                table: "ServiceCategories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceLevel",
                table: "ServiceCategories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ServiceSubCategory",
                table: "ServiceCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ServiceCategories",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceCategoryID",
                table: "Service",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_ParentID",
                table: "ServiceCategories",
                column: "ParentID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCategories_ServiceCategories_ParentID",
                table: "ServiceCategories",
                column: "ParentID",
                principalTable: "ServiceCategories",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCategories_ServiceCategories_ParentID",
                table: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCategories_ParentID",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "ArName",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "LastNode",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "ServiceIndex",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "ServiceLevel",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "ServiceSubCategory",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ServiceCategories");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "ServiceCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryNameAr",
                table: "ServiceCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServiceCategoryID",
                table: "Service",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Service_ServiceCategories_ServiceCategoryID",
                table: "Service",
                column: "ServiceCategoryID",
                principalTable: "ServiceCategories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

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
    }
}
