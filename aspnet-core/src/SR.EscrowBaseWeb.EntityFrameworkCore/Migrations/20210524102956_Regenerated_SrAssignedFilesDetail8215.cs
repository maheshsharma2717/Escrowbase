using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_SrAssignedFilesDetail8215 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicStatus",
                table: "SrAssignedFilesDetails");

            migrationBuilder.AddColumn<string>(
                name: "InputStatus",
                table: "SrAssignedFilesDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadStatus",
                table: "SrAssignedFilesDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputStatus",
                table: "SrAssignedFilesDetails");

            migrationBuilder.DropColumn(
                name: "ReadStatus",
                table: "SrAssignedFilesDetails");

            migrationBuilder.AddColumn<string>(
                name: "BasicStatus",
                table: "SrAssignedFilesDetails",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
