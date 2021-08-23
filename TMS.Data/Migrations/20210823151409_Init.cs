using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeesTypes",
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
                    table.PrimaryKey("PK_FeesTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Parameters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ArName = table.Column<string>(nullable: true),
                    ProviderName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceRequestStatus",
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
                    table.PrimaryKey("PK_ProviderServiceRequestStatus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RequestTypes",
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
                    table.PrimaryKey("PK_RequestTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceConfigerations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    URL = table.Column<string>(nullable: true),
                    TimeOut = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    UserPassword = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceConfigerations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Fees",
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
                    UpdatedBy = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    FeesTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Fees_FeesTypes_FeesTypeID",
                        column: x => x.FeesTypeID,
                        principalTable: "FeesTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceRequests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    RequestTypeID = table.Column<int>(nullable: false),
                    ProviderServiceRequestStatusID = table.Column<int>(nullable: false),
                    Brn = table.Column<int>(nullable: false),
                    DenominationID = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    BillingAccount = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderServiceRequests_ProviderServiceRequestStatus_ProviderServiceRequestStatusID",
                        column: x => x.ProviderServiceRequestStatusID,
                        principalTable: "ProviderServiceRequestStatus",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProviderServiceRequests_RequestTypes_RequestTypeID",
                        column: x => x.RequestTypeID,
                        principalTable: "RequestTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceConfigParms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ServiceConfigerationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceConfigParms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceConfigParms_ServiceConfigerations_ServiceConfigerationID",
                        column: x => x.ServiceConfigerationID,
                        principalTable: "ServiceConfigerations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationServiceProviders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    ServiceProviderID = table.Column<int>(nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    ProviderCode = table.Column<string>(nullable: true),
                    ProviderAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    OldServiceID = table.Column<int>(nullable: true),
                    ProviderHasFees = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationServiceProviders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationServiceProviders_ServiceProviders_ServiceProviderID",
                        column: x => x.ServiceProviderID,
                        principalTable: "ServiceProviders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceRequestParams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ProviderServiceRequestID = table.Column<int>(nullable: false),
                    ParameterID = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceRequestParams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderServiceRequestParams_Parameters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "Parameters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProviderServiceRequestParams_ProviderServiceRequests_ProviderServiceRequestID",
                        column: x => x.ProviderServiceRequestID,
                        principalTable: "ProviderServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceResponses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ProviderServiceRequestID = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceResponses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderServiceResponses_ProviderServiceRequests_ProviderServiceRequestID",
                        column: x => x.ProviderServiceRequestID,
                        principalTable: "ProviderServiceRequests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ReceiptBodyParams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ProviderServiceRequestID = table.Column<int>(nullable: false),
                    ParameterID = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    TransactionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptBodyParams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReceiptBodyParams_Parameters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "Parameters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ReceiptBodyParams_ProviderServiceRequests_ProviderServiceRequestID",
                        column: x => x.ProviderServiceRequestID,
                        principalTable: "ProviderServiceRequests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceConfigerations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationServiceProviderID = table.Column<int>(nullable: false),
                    ServiceConfigerationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceConfigerations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderServiceConfigerations_DenominationServiceProviders_DenominationServiceProviderID",
                        column: x => x.DenominationServiceProviderID,
                        principalTable: "DenominationServiceProviders",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProviderServiceConfigerations_ServiceConfigerations_ServiceConfigerationID",
                        column: x => x.ServiceConfigerationID,
                        principalTable: "ServiceConfigerations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "InquiryBills",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    ProviderServiceResponseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryBills", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InquiryBills_ProviderServiceResponses_ProviderServiceResponseID",
                        column: x => x.ProviderServiceResponseID,
                        principalTable: "ProviderServiceResponses",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceResponseParams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    ProviderServiceResponseID = table.Column<int>(nullable: false),
                    ParameterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceResponseParams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProviderServiceResponseParams_Parameters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "Parameters",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ProviderServiceResponseParams_ProviderServiceResponses_ProviderServiceResponseID",
                        column: x => x.ProviderServiceResponseID,
                        principalTable: "ProviderServiceResponses",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "InquiryBillDetails",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    InquiryBillID = table.Column<int>(nullable: false),
                    ParameterID = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryBillDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_InquiryBillDetails_InquiryBills_InquiryBillID",
                        column: x => x.InquiryBillID,
                        principalTable: "InquiryBills",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_InquiryBillDetails_Parameters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "Parameters",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DenominationServiceProviders_ServiceProviderID",
                table: "DenominationServiceProviders",
                column: "ServiceProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_Fees_FeesTypeID",
                table: "Fees",
                column: "FeesTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryBillDetails_InquiryBillID",
                table: "InquiryBillDetails",
                column: "InquiryBillID");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryBillDetails_ParameterID",
                table: "InquiryBillDetails",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryBills_ProviderServiceResponseID",
                table: "InquiryBills",
                column: "ProviderServiceResponseID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceConfigerations_DenominationServiceProviderID",
                table: "ProviderServiceConfigerations",
                column: "DenominationServiceProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceConfigerations_ServiceConfigerationID",
                table: "ProviderServiceConfigerations",
                column: "ServiceConfigerationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceRequestParams_ParameterID",
                table: "ProviderServiceRequestParams",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceRequestParams_ProviderServiceRequestID",
                table: "ProviderServiceRequestParams",
                column: "ProviderServiceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceRequests_ProviderServiceRequestStatusID",
                table: "ProviderServiceRequests",
                column: "ProviderServiceRequestStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceRequests_RequestTypeID",
                table: "ProviderServiceRequests",
                column: "RequestTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceResponseParams_ParameterID",
                table: "ProviderServiceResponseParams",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceResponseParams_ProviderServiceResponseID",
                table: "ProviderServiceResponseParams",
                column: "ProviderServiceResponseID");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceResponses_ProviderServiceRequestID",
                table: "ProviderServiceResponses",
                column: "ProviderServiceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptBodyParams_ParameterID",
                table: "ReceiptBodyParams",
                column: "ParameterID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptBodyParams_ProviderServiceRequestID",
                table: "ReceiptBodyParams",
                column: "ProviderServiceRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceConfigParms_ServiceConfigerationID",
                table: "ServiceConfigParms",
                column: "ServiceConfigerationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropTable(
                name: "InquiryBillDetails");

            migrationBuilder.DropTable(
                name: "ProviderServiceConfigerations");

            migrationBuilder.DropTable(
                name: "ProviderServiceRequestParams");

            migrationBuilder.DropTable(
                name: "ProviderServiceResponseParams");

            migrationBuilder.DropTable(
                name: "ReceiptBodyParams");

            migrationBuilder.DropTable(
                name: "ServiceConfigParms");

            migrationBuilder.DropTable(
                name: "FeesTypes");

            migrationBuilder.DropTable(
                name: "InquiryBills");

            migrationBuilder.DropTable(
                name: "DenominationServiceProviders");

            migrationBuilder.DropTable(
                name: "Parameters");

            migrationBuilder.DropTable(
                name: "ServiceConfigerations");

            migrationBuilder.DropTable(
                name: "ProviderServiceResponses");

            migrationBuilder.DropTable(
                name: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "ProviderServiceRequests");

            migrationBuilder.DropTable(
                name: "ProviderServiceRequestStatus");

            migrationBuilder.DropTable(
                name: "RequestTypes");
        }
    }
}
