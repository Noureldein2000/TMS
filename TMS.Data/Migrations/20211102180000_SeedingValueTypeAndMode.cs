using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class SeedingValueTypeAndMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DenominationParamValueModes",
                columns: new[] { "ID", "CreationDate", "Name", "UpdateDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 11, 2, 20, 0, 0, 47, DateTimeKind.Local).AddTicks(5663), "FIXED", null },
                    { 2, new DateTime(2021, 11, 2, 20, 0, 0, 48, DateTimeKind.Local).AddTicks(8851), "DYNAMIC", null }
                });

            migrationBuilder.InsertData(
                table: "DenominationParamValueTypes",
                columns: new[] { "ID", "CreationDate", "Name", "UpdateDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 11, 2, 20, 0, 0, 50, DateTimeKind.Local).AddTicks(5486), "Number", null },
                    { 2, new DateTime(2021, 11, 2, 20, 0, 0, 50, DateTimeKind.Local).AddTicks(5499), "String", null },
                    { 3, new DateTime(2021, 11, 2, 20, 0, 0, 50, DateTimeKind.Local).AddTicks(5501), "List", null },
                    { 4, new DateTime(2021, 11, 2, 20, 0, 0, 50, DateTimeKind.Local).AddTicks(5502), "Date", null }
                });

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 2, 20, 0, 0, 67, DateTimeKind.Local).AddTicks(7369));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 2, 20, 0, 0, 67, DateTimeKind.Local).AddTicks(7427));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 2, 20, 0, 0, 67, DateTimeKind.Local).AddTicks(7429));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 2, 20, 0, 0, 67, DateTimeKind.Local).AddTicks(7430));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4);

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
        }
    }
}
