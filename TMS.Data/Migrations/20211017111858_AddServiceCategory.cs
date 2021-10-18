using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddServiceCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ServiceCategoryID",
                table: "Services",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 18, 58, 135, DateTimeKind.Local).AddTicks(4885));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 18, 58, 136, DateTimeKind.Local).AddTicks(7110));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 18, 58, 136, DateTimeKind.Local).AddTicks(7136));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 18, 58, 136, DateTimeKind.Local).AddTicks(7138));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ServiceCategoryID",
                table: "Services",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 14, 19, 3, DateTimeKind.Local).AddTicks(6844));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 14, 19, 4, DateTimeKind.Local).AddTicks(8157));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 14, 19, 4, DateTimeKind.Local).AddTicks(8187));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 10, 17, 13, 14, 19, 4, DateTimeKind.Local).AddTicks(8189));
        }
    }
}
