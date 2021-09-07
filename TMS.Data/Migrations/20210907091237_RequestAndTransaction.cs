using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class RequestAndTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Commissions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    ValExp = table.Column<string>(maxLength: 250, nullable: true),
                    message = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    UUID = table.Column<string>(maxLength: 50, nullable: true),
                    AccountID = table.Column<int>(nullable: false),
                    ServiceDenominationID = table.Column<int>(nullable: false),
                    StatusID = table.Column<int>(nullable: true),
                    TransactionID = table.Column<int>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    RRN = table.Column<string>(maxLength: 150, nullable: true),
                    ResponseDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    BillingAccount = table.Column<string>(maxLength: 200, nullable: true),
                    ChannelID = table.Column<string>(maxLength: 50, nullable: true),
                    ProviderServiceRequestID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Requests_Denominations_ServiceDenominationID",
                        column: x => x.ServiceDenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Requests_RequestStatus_StatusID",
                        column: x => x.StatusID,
                        principalTable: "RequestStatus",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceBalanceTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ServiceID = table.Column<int>(nullable: false),
                    BalanceTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceBalanceTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceBalanceTypes_Services_ServiceID",
                        column: x => x.ServiceID,
                        principalTable: "Services",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AccountIDFrom = table.Column<int>(nullable: true),
                    AccountIDTo = table.Column<int>(nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    IsReversed = table.Column<bool>(nullable: false),
                    OriginalTrx = table.Column<string>(nullable: true),
                    InvoiceID = table.Column<int>(nullable: true),
                    RequestID = table.Column<int>(nullable: true),
                    TransactionID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServicesFields",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ServId = table.Column<int>(nullable: false),
                    FieldName = table.Column<string>(maxLength: 50, nullable: true),
                    FieldTypeID = table.Column<int>(nullable: false),
                    Req = table.Column<bool>(nullable: false),
                    fRank = table.Column<int>(nullable: false),
                    Printed = table.Column<bool>(nullable: false),
                    Display = table.Column<bool>(nullable: false),
                    printed_rank = table.Column<int>(nullable: false),
                    EnglishFieldName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesFields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServicesFields_FieldTypes_FieldTypeID",
                        column: x => x.FieldTypeID,
                        principalTable: "FieldTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AccountTransactionCommissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    TransactionID = table.Column<int>(nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18, 3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTransactionCommissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountTransactionCommissions_Transactions_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "Transactions",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountTransactionCommissions_TransactionID",
                table: "AccountTransactionCommissions",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ServiceDenominationID",
                table: "Requests",
                column: "ServiceDenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_StatusID",
                table: "Requests",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceBalanceTypes_ServiceID",
                table: "ServiceBalanceTypes",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesFields_FieldTypeID",
                table: "ServicesFields",
                column: "FieldTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTransactionCommissions");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "ServiceBalanceTypes");

            migrationBuilder.DropTable(
                name: "ServicesFields");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "FieldTypes");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Commissions",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool));
        }
    }
}
