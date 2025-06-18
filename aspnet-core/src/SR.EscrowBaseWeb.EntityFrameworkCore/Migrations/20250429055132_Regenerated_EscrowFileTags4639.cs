using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_EscrowFileTags4639 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscrowFileTagses_AbpUsers_CreatedBy",
                table: "EscrowFileTagses");

            migrationBuilder.DropIndex(
                name: "IX_EscrowFileTagses_CreatedBy",
                table: "EscrowFileTagses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EscrowFileTagses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "EscrowFileTagses",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EscrowFileTagses_CreatedBy",
                table: "EscrowFileTagses",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_EscrowFileTagses_AbpUsers_CreatedBy",
                table: "EscrowFileTagses",
                column: "CreatedBy",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
