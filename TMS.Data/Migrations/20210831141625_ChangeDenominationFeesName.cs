using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class ChangeDenominationFeesName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DenominationFee_Denominations_DenominationID",
                table: "DenominationFee");

            migrationBuilder.DropForeignKey(
                name: "FK_DenominationFee_Fees_FeesID",
                table: "DenominationFee");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderStatusCode_ServiceProviders_ServiceProviderID",
                table: "ProviderStatusCode");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderStatusCode_StatusCode_StatusCodeID",
                table: "ProviderStatusCode");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusCode_MainStatusCode_MainStatusCodeID",
                table: "StatusCode");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusCode_RequestStatus_RequestStatusID",
                table: "StatusCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusCode",
                table: "StatusCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderStatusCode",
                table: "ProviderStatusCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MainStatusCode",
                table: "MainStatusCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DenominationFee",
                table: "DenominationFee");

            migrationBuilder.RenameTable(
                name: "StatusCode",
                newName: "StatusCodes");

            migrationBuilder.RenameTable(
                name: "ProviderStatusCode",
                newName: "ProviderStatusCodes");

            migrationBuilder.RenameTable(
                name: "MainStatusCode",
                newName: "MainStatusCodes");

            migrationBuilder.RenameTable(
                name: "DenominationFee",
                newName: "DenominationFees");

            migrationBuilder.RenameIndex(
                name: "IX_StatusCode_RequestStatusID",
                table: "StatusCodes",
                newName: "IX_StatusCodes_RequestStatusID");

            migrationBuilder.RenameIndex(
                name: "IX_StatusCode_MainStatusCodeID",
                table: "StatusCodes",
                newName: "IX_StatusCodes_MainStatusCodeID");

            migrationBuilder.RenameIndex(
                name: "IX_ProviderStatusCode_StatusCodeID",
                table: "ProviderStatusCodes",
                newName: "IX_ProviderStatusCodes_StatusCodeID");

            migrationBuilder.RenameIndex(
                name: "IX_ProviderStatusCode_ServiceProviderID",
                table: "ProviderStatusCodes",
                newName: "IX_ProviderStatusCodes_ServiceProviderID");

            migrationBuilder.RenameIndex(
                name: "IX_DenominationFee_FeesID",
                table: "DenominationFees",
                newName: "IX_DenominationFees_FeesID");

            migrationBuilder.RenameIndex(
                name: "IX_DenominationFee_DenominationID",
                table: "DenominationFees",
                newName: "IX_DenominationFees_DenominationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusCodes",
                table: "StatusCodes",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderStatusCodes",
                table: "ProviderStatusCodes",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MainStatusCodes",
                table: "MainStatusCodes",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DenominationFees",
                table: "DenominationFees",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DenominationFees_Denominations_DenominationID",
                table: "DenominationFees",
                column: "DenominationID",
                principalTable: "Denominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DenominationFees_Fees_FeesID",
                table: "DenominationFees",
                column: "FeesID",
                principalTable: "Fees",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderStatusCodes_ServiceProviders_ServiceProviderID",
                table: "ProviderStatusCodes",
                column: "ServiceProviderID",
                principalTable: "ServiceProviders",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderStatusCodes_StatusCodes_StatusCodeID",
                table: "ProviderStatusCodes",
                column: "StatusCodeID",
                principalTable: "StatusCodes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusCodes_MainStatusCodes_MainStatusCodeID",
                table: "StatusCodes",
                column: "MainStatusCodeID",
                principalTable: "MainStatusCodes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusCodes_RequestStatus_RequestStatusID",
                table: "StatusCodes",
                column: "RequestStatusID",
                principalTable: "RequestStatus",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DenominationFees_Denominations_DenominationID",
                table: "DenominationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_DenominationFees_Fees_FeesID",
                table: "DenominationFees");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderStatusCodes_ServiceProviders_ServiceProviderID",
                table: "ProviderStatusCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderStatusCodes_StatusCodes_StatusCodeID",
                table: "ProviderStatusCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusCodes_MainStatusCodes_MainStatusCodeID",
                table: "StatusCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_StatusCodes_RequestStatus_RequestStatusID",
                table: "StatusCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatusCodes",
                table: "StatusCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderStatusCodes",
                table: "ProviderStatusCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MainStatusCodes",
                table: "MainStatusCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DenominationFees",
                table: "DenominationFees");

            migrationBuilder.RenameTable(
                name: "StatusCodes",
                newName: "StatusCode");

            migrationBuilder.RenameTable(
                name: "ProviderStatusCodes",
                newName: "ProviderStatusCode");

            migrationBuilder.RenameTable(
                name: "MainStatusCodes",
                newName: "MainStatusCode");

            migrationBuilder.RenameTable(
                name: "DenominationFees",
                newName: "DenominationFee");

            migrationBuilder.RenameIndex(
                name: "IX_StatusCodes_RequestStatusID",
                table: "StatusCode",
                newName: "IX_StatusCode_RequestStatusID");

            migrationBuilder.RenameIndex(
                name: "IX_StatusCodes_MainStatusCodeID",
                table: "StatusCode",
                newName: "IX_StatusCode_MainStatusCodeID");

            migrationBuilder.RenameIndex(
                name: "IX_ProviderStatusCodes_StatusCodeID",
                table: "ProviderStatusCode",
                newName: "IX_ProviderStatusCode_StatusCodeID");

            migrationBuilder.RenameIndex(
                name: "IX_ProviderStatusCodes_ServiceProviderID",
                table: "ProviderStatusCode",
                newName: "IX_ProviderStatusCode_ServiceProviderID");

            migrationBuilder.RenameIndex(
                name: "IX_DenominationFees_FeesID",
                table: "DenominationFee",
                newName: "IX_DenominationFee_FeesID");

            migrationBuilder.RenameIndex(
                name: "IX_DenominationFees_DenominationID",
                table: "DenominationFee",
                newName: "IX_DenominationFee_DenominationID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatusCode",
                table: "StatusCode",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderStatusCode",
                table: "ProviderStatusCode",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MainStatusCode",
                table: "MainStatusCode",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DenominationFee",
                table: "DenominationFee",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DenominationFee_Denominations_DenominationID",
                table: "DenominationFee",
                column: "DenominationID",
                principalTable: "Denominations",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DenominationFee_Fees_FeesID",
                table: "DenominationFee",
                column: "FeesID",
                principalTable: "Fees",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderStatusCode_ServiceProviders_ServiceProviderID",
                table: "ProviderStatusCode",
                column: "ServiceProviderID",
                principalTable: "ServiceProviders",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderStatusCode_StatusCode_StatusCodeID",
                table: "ProviderStatusCode",
                column: "StatusCodeID",
                principalTable: "StatusCode",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusCode_MainStatusCode_MainStatusCodeID",
                table: "StatusCode",
                column: "MainStatusCodeID",
                principalTable: "MainStatusCode",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusCode_RequestStatus_RequestStatusID",
                table: "StatusCode",
                column: "RequestStatusID",
                principalTable: "RequestStatus",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
