using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class RemoveServiceCategoryFromService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryID",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCategoryID",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceCategoryID",
                table: "Services");

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 292, DateTimeKind.Local).AddTicks(1274));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 293, DateTimeKind.Local).AddTicks(7949));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 295, DateTimeKind.Local).AddTicks(9465));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 295, DateTimeKind.Local).AddTicks(9482));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 295, DateTimeKind.Local).AddTicks(9484));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 295, DateTimeKind.Local).AddTicks(9486));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 324, DateTimeKind.Local).AddTicks(4424));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 324, DateTimeKind.Local).AddTicks(4521));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 324, DateTimeKind.Local).AddTicks(4525));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 17, 12, 19, 28, 324, DateTimeKind.Local).AddTicks(4527));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceCategoryID",
                table: "Services",
                type: "int",
                nullable: true);

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
                name: "IX_Services_ServiceCategoryID",
                table: "Services",
                column: "ServiceCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryID",
                table: "Services",
                column: "ServiceCategoryID",
                principalTable: "ServiceCategories",
                principalColumn: "ID");
        }
    }
}
