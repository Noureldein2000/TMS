using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class DenominationFeesAndFees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DenominationFee",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    FeesID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationFee", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationFee_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DenominationFee_Fees_FeesID",
                        column: x => x.FeesID,
                        principalTable: "Fees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "MainStatusCode",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Message = table.Column<string>(maxLength: 100, nullable: true),
                    ArMessage = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainStatusCode", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatus",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    ResponseCode = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "StatusCode",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Code = table.Column<string>(maxLength: 10, nullable: true),
                    Message = table.Column<string>(maxLength: 150, nullable: true),
                    ArMessage = table.Column<string>(maxLength: 150, nullable: true),
                    RequestStatusID = table.Column<int>(nullable: false),
                    MainStatusCodeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusCode", x => x.ID);
                    table.ForeignKey(
                        name: "FK_StatusCode_MainStatusCode_MainStatusCodeID",
                        column: x => x.MainStatusCodeID,
                        principalTable: "MainStatusCode",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_StatusCode_RequestStatus_RequestStatusID",
                        column: x => x.RequestStatusID,
                        principalTable: "RequestStatus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderStatusCode",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ServiceProviderID = table.Column<int>(nullable: false),
                    StatusCode = table.Column<string>(maxLength: 50, nullable: true),
                    ProviderMessage = table.Column<string>(maxLength: 100, nullable: true),
                    StatusCodeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderStatusCode", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderStatusCode_ServiceProviders_ServiceProviderID",
                        column: x => x.ServiceProviderID,
                        principalTable: "ServiceProviders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProviderStatusCode_StatusCode_StatusCodeID",
                        column: x => x.StatusCodeID,
                        principalTable: "StatusCode",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DenominationFee_DenominationID",
                table: "DenominationFee",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationFee_FeesID",
                table: "DenominationFee",
                column: "FeesID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderStatusCode_ServiceProviderID",
                table: "ProviderStatusCode",
                column: "ServiceProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderStatusCode_StatusCodeID",
                table: "ProviderStatusCode",
                column: "StatusCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_StatusCode_MainStatusCodeID",
                table: "StatusCode",
                column: "MainStatusCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_StatusCode_RequestStatusID",
                table: "StatusCode",
                column: "RequestStatusID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DenominationFee");

            migrationBuilder.DropTable(
                name: "ProviderStatusCode");

            migrationBuilder.DropTable(
                name: "StatusCode");

            migrationBuilder.DropTable(
                name: "MainStatusCode");

            migrationBuilder.DropTable(
                name: "RequestStatus");
        }
    }
}
