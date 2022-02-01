using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class taxestypeConfiguartion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TaxTypes",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArName",
                table: "TaxTypes",
                type: "nvarchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 144, DateTimeKind.Local).AddTicks(8685));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 146, DateTimeKind.Local).AddTicks(884));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 147, DateTimeKind.Local).AddTicks(6763));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 147, DateTimeKind.Local).AddTicks(6773));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 147, DateTimeKind.Local).AddTicks(6775));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 147, DateTimeKind.Local).AddTicks(6776));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 166, DateTimeKind.Local).AddTicks(1971));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 166, DateTimeKind.Local).AddTicks(2048));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 166, DateTimeKind.Local).AddTicks(2051));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 13, 13, 0, 166, DateTimeKind.Local).AddTicks(2052));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TaxTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArName",
                table: "TaxTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 589, DateTimeKind.Local).AddTicks(5940));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 590, DateTimeKind.Local).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6297));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6310));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6311));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6313));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(510));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(513));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(515));
        }
    }
}
