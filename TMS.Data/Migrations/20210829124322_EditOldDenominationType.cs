using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class EditOldDenominationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OldDenominationID",
                table: "Denominations",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Denominations_ServiceID",
                table: "Denominations",
                column: "ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Denominations_Services_ServiceID",
                table: "Denominations",
                column: "ServiceID",
                principalTable: "Services",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Denominations_Services_ServiceID",
                table: "Denominations");

            migrationBuilder.DropIndex(
                name: "IX_Denominations_ServiceID",
                table: "Denominations");

            migrationBuilder.AlterColumn<int>(
                name: "OldDenominationID",
                table: "Denominations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
