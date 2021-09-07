using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddDenominationEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountFees",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AccountID = table.Column<int>(nullable: false),
                    FeesID = table.Column<int>(nullable: false),
                    DenominationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountFees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountFees_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AccountFees_Fees_FeesID",
                        column: x => x.FeesID,
                        principalTable: "Fees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AccountProfileDenominations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AccountProfileID = table.Column<int>(nullable: false),
                    DenominationID = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProfileDenominations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountProfileDenominations_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "CommissionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    ArName = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DenominationEntities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    EntityID = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationEntities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationEntities_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationParamValueModes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationParamValueModes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DenominationParamValueTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationParamValueTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DenominationProviderConfigerations",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    Value = table.Column<string>(maxLength: 50, nullable: true),
                    DenominationProviderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationProviderConfigerations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationProviderConfigerations_DenominationServiceProviders_DenominationProviderID",
                        column: x => x.DenominationProviderID,
                        principalTable: "DenominationServiceProviders",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationReceiptData",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 2000, nullable: true),
                    Disclaimer = table.Column<string>(maxLength: 2000, nullable: true),
                    Footer = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationReceiptData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationReceiptData_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationReceiptParams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    ParameterID = table.Column<int>(nullable: false),
                    Bold = table.Column<bool>(nullable: false),
                    Alignment = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationReceiptParams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationReceiptParams_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DenominationReceiptParams_Parameters_ParameterID",
                        column: x => x.ParameterID,
                        principalTable: "Parameters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountProfileFees",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    FeesID = table.Column<int>(nullable: false),
                    AccountProfileDenominationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProfileFees", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountProfileFees_AccountProfileDenominations_AccountProfileDenominationID",
                        column: x => x.AccountProfileDenominationID,
                        principalTable: "AccountProfileDenominations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AccountProfileFees_Fees_FeesID",
                        column: x => x.FeesID,
                        principalTable: "Fees",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    AmountFrom = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    AmountTo = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    PaymentModeID = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18, 3)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdatedBy = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    CommissionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Commissions_CommissionTypes_CommissionTypeID",
                        column: x => x.CommissionTypeID,
                        principalTable: "CommissionTypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Commissions_PaymentModes_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "PaymentModes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationParams",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Label = table.Column<string>(maxLength: 50, nullable: true),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    ValueModeID = table.Column<int>(nullable: false),
                    ValueTypeID = table.Column<int>(nullable: false),
                    ParamKey = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationParams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationParams_DenominationParamValueModes_ValueModeID",
                        column: x => x.ValueModeID,
                        principalTable: "DenominationParamValueModes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DenominationParams_DenominationParamValueTypes_ValueTypeID",
                        column: x => x.ValueTypeID,
                        principalTable: "DenominationParamValueTypes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AccountCommissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    CommessionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCommissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountCommissions_Commissions_CommessionID",
                        column: x => x.CommessionID,
                        principalTable: "Commissions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_AccountCommissions_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AccountProfileCommissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    CommissionID = table.Column<int>(nullable: false),
                    AccountProfileDenominationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProfileCommissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountProfileCommissions_AccountProfileDenominations_AccountProfileDenominationID",
                        column: x => x.AccountProfileDenominationID,
                        principalTable: "AccountProfileDenominations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountProfileCommissions_Commissions_CommissionID",
                        column: x => x.CommissionID,
                        principalTable: "Commissions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DenominationCommissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    CommissionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationCommissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationCommissions_Commissions_CommissionID",
                        column: x => x.CommissionID,
                        principalTable: "Commissions",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DenominationCommissions_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DenominationParameters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DenominationID = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Optional = table.Column<bool>(nullable: false),
                    DenominationParamID = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: true),
                    ValueList = table.Column<string>(maxLength: 1000, nullable: true),
                    ValidationExpression = table.Column<string>(maxLength: 1000, nullable: true),
                    ValidationMessage = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominationParameters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DenominationParameters_Denominations_DenominationID",
                        column: x => x.DenominationID,
                        principalTable: "Denominations",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DenominationParameters_DenominationParams_DenominationParamID",
                        column: x => x.DenominationParamID,
                        principalTable: "DenominationParams",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCommissions_CommessionID",
                table: "AccountCommissions",
                column: "CommessionID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCommissions_DenominationID",
                table: "AccountCommissions",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFees_DenominationID",
                table: "AccountFees",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountFees_FeesID",
                table: "AccountFees",
                column: "FeesID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountProfileCommissions_AccountProfileDenominationID",
                table: "AccountProfileCommissions",
                column: "AccountProfileDenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountProfileCommissions_CommissionID",
                table: "AccountProfileCommissions",
                column: "CommissionID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountProfileDenominations_DenominationID",
                table: "AccountProfileDenominations",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountProfileFees_AccountProfileDenominationID",
                table: "AccountProfileFees",
                column: "AccountProfileDenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountProfileFees_FeesID",
                table: "AccountProfileFees",
                column: "FeesID");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_CommissionTypeID",
                table: "Commissions",
                column: "CommissionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_PaymentModeID",
                table: "Commissions",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationCommissions_CommissionID",
                table: "DenominationCommissions",
                column: "CommissionID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationCommissions_DenominationID",
                table: "DenominationCommissions",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationEntities_DenominationID",
                table: "DenominationEntities",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationParameters_DenominationID",
                table: "DenominationParameters",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationParameters_DenominationParamID",
                table: "DenominationParameters",
                column: "DenominationParamID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationParams_ValueModeID",
                table: "DenominationParams",
                column: "ValueModeID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationParams_ValueTypeID",
                table: "DenominationParams",
                column: "ValueTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationProviderConfigerations_DenominationProviderID",
                table: "DenominationProviderConfigerations",
                column: "DenominationProviderID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationReceiptData_DenominationID",
                table: "DenominationReceiptData",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationReceiptParams_DenominationID",
                table: "DenominationReceiptParams",
                column: "DenominationID");

            migrationBuilder.CreateIndex(
                name: "IX_DenominationReceiptParams_ParameterID",
                table: "DenominationReceiptParams",
                column: "ParameterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCommissions");

            migrationBuilder.DropTable(
                name: "AccountFees");

            migrationBuilder.DropTable(
                name: "AccountProfileCommissions");

            migrationBuilder.DropTable(
                name: "AccountProfileFees");

            migrationBuilder.DropTable(
                name: "DenominationCommissions");

            migrationBuilder.DropTable(
                name: "DenominationEntities");

            migrationBuilder.DropTable(
                name: "DenominationParameters");

            migrationBuilder.DropTable(
                name: "DenominationProviderConfigerations");

            migrationBuilder.DropTable(
                name: "DenominationReceiptData");

            migrationBuilder.DropTable(
                name: "DenominationReceiptParams");

            migrationBuilder.DropTable(
                name: "AccountProfileDenominations");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "DenominationParams");

            migrationBuilder.DropTable(
                name: "CommissionTypes");

            migrationBuilder.DropTable(
                name: "DenominationParamValueModes");

            migrationBuilder.DropTable(
                name: "DenominationParamValueTypes");
        }
    }
}
