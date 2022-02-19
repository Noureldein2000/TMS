using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class addColumnTaxesinTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Taxes",
                table: "Transactions",
                type: "decimal(18, 3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 386, DateTimeKind.Local).AddTicks(6116));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 387, DateTimeKind.Local).AddTicks(8592));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 389, DateTimeKind.Local).AddTicks(6126));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 389, DateTimeKind.Local).AddTicks(6145));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 389, DateTimeKind.Local).AddTicks(6146));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 389, DateTimeKind.Local).AddTicks(6148));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 409, DateTimeKind.Local).AddTicks(9961));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 410, DateTimeKind.Local).AddTicks(41));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 410, DateTimeKind.Local).AddTicks(43));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 2, 1, 15, 27, 21, 410, DateTimeKind.Local).AddTicks(45));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Taxes",
                table: "Transactions");

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
    }
}
