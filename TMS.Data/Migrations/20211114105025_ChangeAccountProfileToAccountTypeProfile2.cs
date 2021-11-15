using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class ChangeAccountProfileToAccountTypeProfile2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountProfileCommissions_AccountProfileDenominations_AccountProfileDenominationID",
                table: "AccountProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountProfileCommissions_Commissions_CommissionID",
                table: "AccountProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountProfileDenominations_Denominations_DenominationID",
                table: "AccountProfileDenominations");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountProfileFees_AccountProfileDenominations_AccountProfileDenominationID",
                table: "AccountProfileFees");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountProfileFees_Fees_FeesID",
                table: "AccountProfileFees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountProfileFees",
                table: "AccountProfileFees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountProfileDenominations",
                table: "AccountProfileDenominations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountProfileCommissions",
                table: "AccountProfileCommissions");

            migrationBuilder.RenameTable(
                name: "AccountProfileFees",
                newName: "AccountTypeProfileFees");

            migrationBuilder.RenameTable(
                name: "AccountProfileDenominations",
                newName: "AccountTypeProfileDenominations");

            migrationBuilder.RenameTable(
                name: "AccountProfileCommissions",
                newName: "AccountTypeProfileCommissions");

            migrationBuilder.RenameIndex(
                name: "IX_AccountProfileFees_FeesID",
                table: "AccountTypeProfileFees",
                newName: "IX_AccountTypeProfileFees_FeesID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountProfileFees_AccountProfileDenominationID",
                table: "AccountTypeProfileFees",
                newName: "IX_AccountTypeProfileFees_AccountProfileDenominationID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountProfileDenominations_DenominationID",
                table: "AccountTypeProfileDenominations",
                newName: "IX_AccountTypeProfileDenominations_DenominationID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountProfileCommissions_CommissionID",
                table: "AccountTypeProfileCommissions",
                newName: "IX_AccountTypeProfileCommissions_CommissionID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountProfileCommissions_AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions",
                newName: "IX_AccountTypeProfileCommissions_AccountProfileDenominationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTypeProfileFees",
                table: "AccountTypeProfileFees",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTypeProfileDenominations",
                table: "AccountTypeProfileDenominations",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTypeProfileCommissions",
                table: "AccountTypeProfileCommissions",
                column: "ID");

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
                name: "FK_AccountTypeProfileDenominations_Denominations_DenominationID",
                table: "AccountTypeProfileDenominations",
                column: "DenominationID",
                principalTable: "Denominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileFees_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileFees",
                column: "AccountProfileDenominationID",
                principalTable: "AccountTypeProfileDenominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountTypeProfileFees_Fees_FeesID",
                table: "AccountTypeProfileFees",
                column: "FeesID",
                principalTable: "Fees",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileCommissions_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileCommissions_Commissions_CommissionID",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileDenominations_Denominations_DenominationID",
                table: "AccountTypeProfileDenominations");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileFees_AccountTypeProfileDenominations_AccountProfileDenominationID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountTypeProfileFees_Fees_FeesID",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTypeProfileFees",
                table: "AccountTypeProfileFees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTypeProfileDenominations",
                table: "AccountTypeProfileDenominations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTypeProfileCommissions",
                table: "AccountTypeProfileCommissions");

            migrationBuilder.RenameTable(
                name: "AccountTypeProfileFees",
                newName: "AccountProfileFees");

            migrationBuilder.RenameTable(
                name: "AccountTypeProfileDenominations",
                newName: "AccountProfileDenominations");

            migrationBuilder.RenameTable(
                name: "AccountTypeProfileCommissions",
                newName: "AccountProfileCommissions");

            migrationBuilder.RenameIndex(
                name: "IX_AccountTypeProfileFees_FeesID",
                table: "AccountProfileFees",
                newName: "IX_AccountProfileFees_FeesID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountTypeProfileFees_AccountProfileDenominationID",
                table: "AccountProfileFees",
                newName: "IX_AccountProfileFees_AccountProfileDenominationID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountTypeProfileDenominations_DenominationID",
                table: "AccountProfileDenominations",
                newName: "IX_AccountProfileDenominations_DenominationID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountTypeProfileCommissions_CommissionID",
                table: "AccountProfileCommissions",
                newName: "IX_AccountProfileCommissions_CommissionID");

            migrationBuilder.RenameIndex(
                name: "IX_AccountTypeProfileCommissions_AccountProfileDenominationID",
                table: "AccountProfileCommissions",
                newName: "IX_AccountProfileCommissions_AccountProfileDenominationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountProfileFees",
                table: "AccountProfileFees",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountProfileDenominations",
                table: "AccountProfileDenominations",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountProfileCommissions",
                table: "AccountProfileCommissions",
                column: "ID");

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 342, DateTimeKind.Local).AddTicks(1860));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 343, DateTimeKind.Local).AddTicks(6922));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 345, DateTimeKind.Local).AddTicks(6189));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 345, DateTimeKind.Local).AddTicks(6205));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 345, DateTimeKind.Local).AddTicks(6207));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 345, DateTimeKind.Local).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 366, DateTimeKind.Local).AddTicks(3663));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 366, DateTimeKind.Local).AddTicks(3745));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 366, DateTimeKind.Local).AddTicks(3747));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 14, 12, 28, 33, 366, DateTimeKind.Local).AddTicks(3749));

            migrationBuilder.AddForeignKey(
                name: "FK_AccountProfileCommissions_AccountProfileDenominations_AccountProfileDenominationID",
                table: "AccountProfileCommissions",
                column: "AccountProfileDenominationID",
                principalTable: "AccountProfileDenominations",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountProfileCommissions_Commissions_CommissionID",
                table: "AccountProfileCommissions",
                column: "CommissionID",
                principalTable: "Commissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountProfileDenominations_Denominations_DenominationID",
                table: "AccountProfileDenominations",
                column: "DenominationID",
                principalTable: "Denominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountProfileFees_AccountProfileDenominations_AccountProfileDenominationID",
                table: "AccountProfileFees",
                column: "AccountProfileDenominationID",
                principalTable: "AccountProfileDenominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountProfileFees_Fees_FeesID",
                table: "AccountProfileFees",
                column: "FeesID",
                principalTable: "Fees",
                principalColumn: "ID");
        }
    }
}
