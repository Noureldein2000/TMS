using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class ChangeAccountProfileToAccountTypeProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderServiceConfigerations_DenominationServiceProviderID",
                table: "ProviderServiceConfigerations");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceConfigerations_DenominationServiceProviderID",
                table: "ProviderServiceConfigerations",
                column: "DenominationServiceProviderID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderServiceConfigerations_DenominationServiceProviderID",
                table: "ProviderServiceConfigerations");

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 10, DateTimeKind.Local).AddTicks(5055));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 11, DateTimeKind.Local).AddTicks(9823));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 13, DateTimeKind.Local).AddTicks(6238));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 13, DateTimeKind.Local).AddTicks(6251));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 13, DateTimeKind.Local).AddTicks(6253));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 13, DateTimeKind.Local).AddTicks(6254));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 31, DateTimeKind.Local).AddTicks(7551));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 31, DateTimeKind.Local).AddTicks(7626));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 31, DateTimeKind.Local).AddTicks(7628));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 3, 18, 19, 34, 31, DateTimeKind.Local).AddTicks(7629));

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceConfigerations_DenominationServiceProviderID",
                table: "ProviderServiceConfigerations",
                column: "DenominationServiceProviderID");
        }
    }
}
