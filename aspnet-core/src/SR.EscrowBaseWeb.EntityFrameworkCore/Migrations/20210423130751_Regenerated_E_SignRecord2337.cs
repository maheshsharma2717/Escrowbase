using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_E_SignRecord2337 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_E_SignRecords_TenantId",
                table: "E_SignRecords");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "E_SignRecords");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "E_SignRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_E_SignRecords_TenantId",
                table: "E_SignRecords",
                column: "TenantId");
        }
    }
}
