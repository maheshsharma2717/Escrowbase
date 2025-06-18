using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_EscrowClient2781 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EscrowClients_Enterprises_EnterpriseId",
                table: "EscrowClients");

            migrationBuilder.DropIndex(
                name: "IX_EscrowClients_EnterpriseId",
                table: "EscrowClients");

            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "EscrowClients");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnterpriseId",
                table: "EscrowClients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EscrowClients_EnterpriseId",
                table: "EscrowClients",
                column: "EnterpriseId");

            migrationBuilder.AddForeignKey(
                name: "FK_EscrowClients_Enterprises_EnterpriseId",
                table: "EscrowClients",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
