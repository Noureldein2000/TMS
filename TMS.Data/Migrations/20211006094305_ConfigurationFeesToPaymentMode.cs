using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class ConfigurationFeesToPaymentMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 10, 6, 11, 43, 5, 182, DateTimeKind.Local).AddTicks(3984));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 10, 6, 11, 43, 5, 184, DateTimeKind.Local).AddTicks(7415));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 10, 6, 11, 43, 5, 184, DateTimeKind.Local).AddTicks(7471));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 10, 6, 11, 43, 5, 184, DateTimeKind.Local).AddTicks(7476));

            migrationBuilder.CreateIndex(
                name: "IX_Fees_PaymentModeID",
                table: "Fees",
                column: "PaymentModeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Fees_PaymentModes_PaymentModeID",
                table: "Fees",
                column: "PaymentModeID",
                principalTable: "PaymentModes",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fees_PaymentModes_PaymentModeID",
                table: "Fees");

            migrationBuilder.DropIndex(
                name: "IX_Fees_PaymentModeID",
                table: "Fees");

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 9, 23, 15, 6, 31, 706, DateTimeKind.Local).AddTicks(3278));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 9, 23, 15, 6, 31, 707, DateTimeKind.Local).AddTicks(6296));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 9, 23, 15, 6, 31, 707, DateTimeKind.Local).AddTicks(6326));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 9, 23, 15, 6, 31, 707, DateTimeKind.Local).AddTicks(6329));
        }
    }
}
