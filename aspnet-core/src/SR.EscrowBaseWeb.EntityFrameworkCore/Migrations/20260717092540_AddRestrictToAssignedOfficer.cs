using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class AddRestrictToAssignedOfficer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscrowAccessHistories_EscrowClients_EscrowId",
                table: "EscrowAccessHistories");

            migrationBuilder.AddColumn<bool>(
                name: "RestrictToAssignedOfficer",
                table: "Enterprises",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_EscrowAccessHistories_SrEscrows_EscrowId",
                table: "EscrowAccessHistories",
                column: "EscrowId",
                principalTable: "SrEscrows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscrowAccessHistories_SrEscrows_EscrowId",
                table: "EscrowAccessHistories");

            migrationBuilder.DropColumn(
                name: "RestrictToAssignedOfficer",
                table: "Enterprises");

            migrationBuilder.AddForeignKey(
                name: "FK_EscrowAccessHistories_EscrowClients_EscrowId",
                table: "EscrowAccessHistories",
                column: "EscrowId",
                principalTable: "EscrowClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
