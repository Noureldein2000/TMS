using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class ChangeAccountProfileToAccountTypeProfile3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileCommissions_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileCommissions_Commissions_CommissionID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileFees_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropIndex(
                name: "IX_AccountTypeProfileFees_AccountProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropIndex(
                name: "IX_AccountTypeProfileCommissions_AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropColumn(
                name: "AccountProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropColumn(
                name: "AccountProfileID",
                table: "AccountTypeProfileDenominations");

            migrationBuilder.DropColumn(
                name: "AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeProfileDenominationID",
                table: "AccountTypeProfileFees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeProfileID",
                table: "AccountTypeProfileDenominations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountTypeProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 684, DateTimeKind.Local).AddTicks(6280));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 685, DateTimeKind.Local).AddTicks(9033));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 687, DateTimeKind.Local).AddTicks(7847));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 687, DateTimeKind.Local).AddTicks(7870));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 687, DateTimeKind.Local).AddTicks(7872));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 687, DateTimeKind.Local).AddTicks(7873));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 707, DateTimeKind.Local).AddTicks(5898));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 707, DateTimeKind.Local).AddTicks(5972));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 707, DateTimeKind.Local).AddTicks(5974));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 13, 5, 45, 707, DateTimeKind.Local).AddTicks(5976));

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeProfileFees_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileFees",
                column: "AccountTypeProfileDenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeProfileCommissions_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                column: "AccountTypeProfileDenominationID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileCommissions_AccountTypeProfileDenominations_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                column: "AccountTypeProfileDenominationID",
                principalTable: "AccountTypeProfileDenominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileCommissions_Commissions_CommissionID",
                table: "AccountTypeProfileCommissions",
                column: "CommissionID",
                principalTable: "Commissions",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileFees_AccountTypeProfileDenominations_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileFees",
                column: "AccountTypeProfileDenominationID",
                principalTable: "AccountTypeProfileDenominations",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileCommissions_AccountTypeProfileDenominations_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileCommissions_Commissions_CommissionID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileFees_AccountTypeProfileDenominations_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropIndex(
                name: "IX_AccountTypeProfileFees_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropIndex(
                name: "IX_AccountTypeProfileCommissions_AccountTypeProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropColumn(
                name: "AccountTypeProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropColumn(
                name: "AccountTypeProfileID",
                table: "AccountTypeProfileDenominations");

            migrationBuilder.DropColumn(
                name: "AccountTypeProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.AddColumn<int>(
                name: "AccountProfileDenominationID",
                table: "AccountTypeProfileFees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountProfileID",
                table: "AccountTypeProfileDenominations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 25, DateTimeKind.Local).AddTicks(5884));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 27, DateTimeKind.Local).AddTicks(1782));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 29, DateTimeKind.Local).AddTicks(4368));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 29, DateTimeKind.Local).AddTicks(4394));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 29, DateTimeKind.Local).AddTicks(4397));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 29, DateTimeKind.Local).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 52, DateTimeKind.Local).AddTicks(6857));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 52, DateTimeKind.Local).AddTicks(6968));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 52, DateTimeKind.Local).AddTicks(6972));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 50, 25, 52, DateTimeKind.Local).AddTicks(6974));

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeProfileFees_AccountProfileDenominationID",
                table: "AccountTypeProfileFees",
                column: "AccountProfileDenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeProfileCommissions_AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                column: "AccountProfileDenominationID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileCommissions_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                column: "AccountProfileDenominationID",
                principalTable: "AccountTypeProfileDenominations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileCommissions_Commissions_CommissionID",
                table: "AccountTypeProfileCommissions",
                column: "CommissionID",
                principalTable: "Commissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileFees_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileFees",
                column: "AccountProfileDenominationID",
                principalTable: "AccountTypeProfileDenominations",
                principalColumn: "ID");
        }
    }
}
