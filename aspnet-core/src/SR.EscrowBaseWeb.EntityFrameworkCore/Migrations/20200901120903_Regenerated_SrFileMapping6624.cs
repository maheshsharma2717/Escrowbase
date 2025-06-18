using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_SrFileMapping6624 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "SrFileMappings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EscrowiId",
                table: "SrFileMappings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "SrFileMappings");

            migrationBuilder.DropColumn(
                name: "EscrowiId",
                table: "SrFileMappings");
        }
    }
}
