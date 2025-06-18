using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class columnAddedInInviteeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EscrowClients_TenantId",
                table: "EscrowClients");

            migrationBuilder.DropIndex(
                name: "IX_AppBinaryObjects_TenantId",
                table: "AppBinaryObjects");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EscrowClients");

            migrationBuilder.AddColumn<int>(
                name: "EscrowClientId",
                table: "SRInvitees",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserTypeId",
                table: "SRInvitees",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SRInvitees_EscrowClientId",
                table: "SRInvitees",
                column: "EscrowClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SRInvitees_UserTypeId",
                table: "SRInvitees",
                column: "UserTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SRInvitees_EscrowClients_EscrowClientId",
                table: "SRInvitees",
                column: "EscrowClientId",
                principalTable: "EscrowClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SRInvitees_UserTypes_UserTypeId",
                table: "SRInvitees",
                column: "UserTypeId",
                principalTable: "UserTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SRInvitees_EscrowClients_EscrowClientId",
                table: "SRInvitees");

            migrationBuilder.DropForeignKey(
                name: "FK_SRInvitees_UserTypes_UserTypeId",
                table: "SRInvitees");

            migrationBuilder.DropIndex(
                name: "IX_SRInvitees_EscrowClientId",
                table: "SRInvitees");

            migrationBuilder.DropIndex(
                name: "IX_SRInvitees_UserTypeId",
                table: "SRInvitees");

            migrationBuilder.DropColumn(
                name: "EscrowClientId",
                table: "SRInvitees");

            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "SRInvitees");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "EscrowClients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EscrowClients_TenantId",
                table: "EscrowClients",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaryObjects_TenantId",
                table: "AppBinaryObjects",
                column: "TenantId");
        }
    }
}
