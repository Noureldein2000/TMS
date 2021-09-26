using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class createPendingPaymentCardEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NameAr = table.Column<string>(nullable: true),
                    Abbreviation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PendingPaymentCardStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NameAr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingPaymentCardStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PendingPaymentCards",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    PaymentRefInfo = table.Column<string>(nullable: true),
                    TransactionID = table.Column<int>(nullable: false),
                    CardTypeID = table.Column<int>(nullable: false),
                    HostTransactionID = table.Column<string>(nullable: true),
                    PengingPaymentCardStatusID = table.Column<int>(nullable: false),
                    Brn = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingPaymentCards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PendingPaymentCards_CardTypes_CardTypeID",
                        column: x => x.CardTypeID,
                        principalTable: "CardTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PendingPaymentCards_PendingPaymentCardStatuses_PengingPaymentCardStatusID",
                        column: x => x.PengingPaymentCardStatusID,
                        principalTable: "PendingPaymentCardStatuses",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_PendingPaymentCards_Transactions_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "Transactions",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "PendingPaymentCardStatuses",
                columns: new[] { "ID", "CreationDate", "Name", "NameAr", "UpdateDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 9, 23, 15, 6, 31, 706, DateTimeKind.Local).AddTicks(3278), "Initiated", "بدأت", null },
                    { 2, new DateTime(2021, 9, 23, 15, 6, 31, 707, DateTimeKind.Local).AddTicks(6296), "Canceled", "ألغيت", null },
                    { 3, new DateTime(2021, 9, 23, 15, 6, 31, 707, DateTimeKind.Local).AddTicks(6326), "Confirmed", "مؤكد", null },
                    { 4, new DateTime(2021, 9, 23, 15, 6, 31, 707, DateTimeKind.Local).AddTicks(6329), "AutoCanceled", "مُلغى تلقائيًا", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingPaymentCards_CardTypeID",
                table: "PendingPaymentCards",
                column: "CardTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PendingPaymentCards_PengingPaymentCardStatusID",
                table: "PendingPaymentCards",
                column: "PengingPaymentCardStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_PendingPaymentCards_TransactionID",
                table: "PendingPaymentCards",
                column: "TransactionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingPaymentCards");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "PendingPaymentCardStatuses");
        }
    }
}
