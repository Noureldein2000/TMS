using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class editCommissionColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountCommissions_Commissions_CommessionID",
                table: "AccountCommissions");

            migrationBuilder.DropIndex(
                name: "IX_AccountCommissions_CommessionID",
                table: "AccountCommissions");

            migrationBuilder.DropColumn(
                name: "CommessionID",
                table: "AccountCommissions");

            migrationBuilder.AddColumn<int>(
                name: "CommissionID",
                table: "AccountCommissions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 15, 14, 53, 526, DateTimeKind.Local).AddTicks(6702));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 15, 14, 53, 527, DateTimeKind.Local).AddTicks(7399));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 15, 14, 53, 527, DateTimeKind.Local).AddTicks(7425));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 15, 14, 53, 527, DateTimeKind.Local).AddTicks(7427));

            migrationBuilder.CreateIndex(
                name: "IX_AccountCommissions_CommissionID",
                table: "AccountCommissions",
                column: "CommissionID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCommissions_Commissions_CommissionID",
                table: "AccountCommissions",
                column: "CommissionID",
                principalTable: "Commissions",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountCommissions_Commissions_CommissionID",
                table: "AccountCommissions");

            migrationBuilder.DropIndex(
                name: "IX_AccountCommissions_CommissionID",
                table: "AccountCommissions");

            migrationBuilder.DropColumn(
                name: "CommissionID",
                table: "AccountCommissions");

            migrationBuilder.AddColumn<int>(
                name: "CommessionID",
                table: "AccountCommissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 14, 54, 37, 684, DateTimeKind.Local).AddTicks(6352));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 14, 54, 37, 685, DateTimeKind.Local).AddTicks(7694));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 14, 54, 37, 685, DateTimeKind.Local).AddTicks(7723));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 10, 14, 14, 54, 37, 685, DateTimeKind.Local).AddTicks(7725));

            migrationBuilder.CreateIndex(
                name: "IX_AccountCommissions_CommessionID",
                table: "AccountCommissions",
                column: "CommessionID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountCommissions_Commissions_CommessionID",
                table: "AccountCommissions",
                column: "CommessionID",
                principalTable: "Commissions",
                principalColumn: "ID");
        }
    }
}
