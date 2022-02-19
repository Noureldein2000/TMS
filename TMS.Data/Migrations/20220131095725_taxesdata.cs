using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class taxesdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ArName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Taxes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AmountFrom = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    AmountTo = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PaymentModeID = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    TaxTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Taxes_PaymentModes_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "PaymentModes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Taxes_TaxTypes_TaxTypeID",
                        column: x => x.TaxTypeID,
                        principalTable: "TaxTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationTaxes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    TaxID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationTaxes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationTaxes_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DenominationTaxes_Taxes_TaxID",
                        column: x => x.TaxID,
                        principalTable: "Taxes",
                        principalColumn: "ID");
                });

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 589, DateTimeKind.Local).AddTicks(5940));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 590, DateTimeKind.Local).AddTicks(8629));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6297));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6310));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6311));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 592, DateTimeKind.Local).AddTicks(6313));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(441));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(510));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(513));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2022, 1, 31, 11, 57, 24, 611, DateTimeKind.Local).AddTicks(515));

            migrationBuilder.CreateIndex(
                name: "IX_DenominationTaxes_DenominationID",
                table: "DenominationTaxes",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationTaxes_TaxID",
                table: "DenominationTaxes",
                column: "TaxID");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_PaymentModeID",
                table: "Taxes",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_TaxTypeID",
                table: "Taxes",
                column: "TaxTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DenominationTaxes");

            migrationBuilder.DropTable(
                name: "Taxes");

            migrationBuilder.DropTable(
                name: "TaxTypes");

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 500, DateTimeKind.Local).AddTicks(8792));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueModes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 502, DateTimeKind.Local).AddTicks(2466));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 503, DateTimeKind.Local).AddTicks(9825));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 503, DateTimeKind.Local).AddTicks(9842));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 503, DateTimeKind.Local).AddTicks(9844));

            migrationBuilder.UpdateData(
                table: "DenominationParamValueTypes",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 503, DateTimeKind.Local).AddTicks(9845));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 521, DateTimeKind.Local).AddTicks(1405));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 521, DateTimeKind.Local).AddTicks(1473));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 3,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 521, DateTimeKind.Local).AddTicks(1475));

            migrationBuilder.UpdateData(
                table: "PendingPaymentCardStatuses",
                keyColumn: "ID",
                keyValue: 4,
                column: "CreationDate",
                value: new DateTime(2021, 11, 30, 18, 13, 21, 521, DateTimeKind.Local).AddTicks(1476));
        }
    }
}
