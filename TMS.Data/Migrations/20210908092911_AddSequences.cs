using Microsoft.EntityFrameworkCore.Migrations;

namespace TMS.Data.Migrations
{
    public partial class AddSequences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "Request_seq",
                startValue: 1000L);

            migrationBuilder.CreateSequence<int>(
                name: "Transaction_Seq",
                startValue: 1000L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "Request_seq");

            migrationBuilder.DropSequence(
                name: "Transaction_Seq");
        }
    }
}
