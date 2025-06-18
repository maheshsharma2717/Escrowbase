using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_E_SignRecord1588 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "E_SignRecords");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "E_SignRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "E_SignRecords");

            migrationBuilder.AddColumn<long>(
                name: "FileId",
                table: "E_SignRecords",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
