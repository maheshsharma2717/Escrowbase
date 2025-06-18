using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_EscrowFileTags3964 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "EscrowFileTagses");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "EscrowFileTagses",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "EscrowFileTagses");

            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "EscrowFileTagses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
