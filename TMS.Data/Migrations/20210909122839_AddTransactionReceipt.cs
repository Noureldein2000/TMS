using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddTransactionReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TransactionID",
                table: "ReceiptBodyParams",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "TransactionReceipts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    TransactionID = table.Column<int>(nullable: false),
                    Receipt = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionReceipts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TransactionReceipts_Transactions_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "Transactions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_RequestID",
                table: "Transactions",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptBodyParams_TransactionID",
                table: "ReceiptBodyParams",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionReceipts_TransactionID",
                table: "TransactionReceipts",
                column: "TransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptBodyParams_Transactions_TransactionID",
                table: "ReceiptBodyParams",
                column: "TransactionID",
                principalTable: "Transactions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Requests_RequestID",
                table: "Transactions",
                column: "RequestID",
                principalTable: "Requests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptBodyParams_Transactions_TransactionID",
                table: "ReceiptBodyParams");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Requests_RequestID",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionReceipts");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_RequestID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptBodyParams_TransactionID",
                table: "ReceiptBodyParams");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionID",
                table: "ReceiptBodyParams",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
