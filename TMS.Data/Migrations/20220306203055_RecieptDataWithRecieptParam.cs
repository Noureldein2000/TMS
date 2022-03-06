using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class RecieptDataWithRecieptParam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DenominationReceiptDataID",
                table: "DenominationReceiptParams",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 590, DateTimeKind.Local).AddTicks(4321));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 591, DateTimeKind.Local).AddTicks(3285));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 592, DateTimeKind.Local).AddTicks(7249));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 592, DateTimeKind.Local).AddTicks(7261));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 592, DateTimeKind.Local).AddTicks(7262));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 592, DateTimeKind.Local).AddTicks(7264));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 608, DateTimeKind.Local).AddTicks(463));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 608, DateTimeKind.Local).AddTicks(517));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 608, DateTimeKind.Local).AddTicks(518));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 3, 6, 22, 30, 54, 608, DateTimeKind.Local).AddTicks(519));

            migrationBuilder.CreateIndex(
                name: "IX_DenominationReceiptParams_DenominationReceiptDataID",
                table: "DenominationReceiptParams",
                column: "DenominationReceiptDataID");

            migrationBuilder.AddForeignKey(
                name: "FK_DenominationReceiptParams_DenominationReceiptData_DenominationReceiptDataID",
                table: "DenominationReceiptParams",
                column: "DenominationReceiptDataID",
                principalTable: "DenominationReceiptData",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DenominationReceiptParams_DenominationReceiptData_DenominationReceiptDataID",
                table: "DenominationReceiptParams");

            migrationBuilder.DropIndex(
                name: "IX_DenominationReceiptParams_DenominationReceiptDataID",
                table: "DenominationReceiptParams");

            migrationBuilder.DropColumn(
                name: "DenominationReceiptDataID",
                table: "DenominationReceiptParams");

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
    }
}
