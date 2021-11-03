using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class DenominationReceiptDataRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DenominationReceiptData_DenominationID",
                table: "DenominationReceiptData");

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 16, 6, 53, 361, DateTimeKind.Local).AddTicks(2877));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 16, 6, 53, 363, DateTimeKind.Local).AddTicks(1843));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 16, 6, 53, 363, DateTimeKind.Local).AddTicks(1890));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 16, 6, 53, 363, DateTimeKind.Local).AddTicks(1893));

            migrationBuilder.CreateIndex(
                name: "IX_DenominationReceiptData_DenominationID",
                table: "DenominationReceiptData",
                column: "DenominationID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DenominationReceiptData_DenominationID",
                table: "DenominationReceiptData");

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

            migrationBuilder.CreateIndex(
                name: "IX_DenominationReceiptData_DenominationID",
                table: "DenominationReceiptData",
                column: "DenominationID");
        }
    }
}
