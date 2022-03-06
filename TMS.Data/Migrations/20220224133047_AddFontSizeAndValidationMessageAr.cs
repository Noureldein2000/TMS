using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddFontSizeAndValidationMessageAr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "FontSize",
                table: "DenominationReceiptParams",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "ValidationMessageAr",
                table: "DenominationParameters",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 499, DateTimeKind.Local).AddTicks(1079));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 500, DateTimeKind.Local).AddTicks(948));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 501, DateTimeKind.Local).AddTicks(3357));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 501, DateTimeKind.Local).AddTicks(3367));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 501, DateTimeKind.Local).AddTicks(3368));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 501, DateTimeKind.Local).AddTicks(3369));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 515, DateTimeKind.Local).AddTicks(9550));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 515, DateTimeKind.Local).AddTicks(9608));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 515, DateTimeKind.Local).AddTicks(9610));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 2, 24, 15, 30, 47, 515, DateTimeKind.Local).AddTicks(9612));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "DenominationReceiptParams");

            migrationBuilder.DropColumn(
                name: "ValidationMessageAr",
                table: "DenominationParameters");

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
    }
}
