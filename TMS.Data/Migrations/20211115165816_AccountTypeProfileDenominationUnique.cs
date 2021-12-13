using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AccountTypeProfileDenominationUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountTypeProfileDenominations_DenominationID",
                table: "AccountTypeProfileDenominations");

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 163, DateTimeKind.Local).AddTicks(5603));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 164, DateTimeKind.Local).AddTicks(8007));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 166, DateTimeKind.Local).AddTicks(4328));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 166, DateTimeKind.Local).AddTicks(4341));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 166, DateTimeKind.Local).AddTicks(4343));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 166, DateTimeKind.Local).AddTicks(4345));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 184, DateTimeKind.Local).AddTicks(1578));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 184, DateTimeKind.Local).AddTicks(1650));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 184, DateTimeKind.Local).AddTicks(1653));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 15, 18, 58, 16, 184, DateTimeKind.Local).AddTicks(1654));

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypeProfileDenominations_DenominationID_AccountTypeProfileID",
                table: "AccountTypeProfileDenominations",
                columns: new[] { "DenominationID", "AccountTypeProfileID" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountTypeProfileDenominations_DenominationID_AccountTypeProfileID",
                table: "AccountTypeProfileDenominations");

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
                name: "IX_AccountTypeProfileDenominations_DenominationID",
                table: "AccountTypeProfileDenominations",
                column: "DenominationID");
        }
    }
}
