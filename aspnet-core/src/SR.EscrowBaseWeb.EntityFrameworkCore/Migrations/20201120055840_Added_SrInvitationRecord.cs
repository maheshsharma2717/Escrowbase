using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_SrInvitationRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserFileLogs_TenantId",
                table: "UserFileLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserFileLogs");

            migrationBuilder.CreateTable(
                name: "SrInvitationRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    DomainAccessInstance = table.Column<string>(nullable: true),
                    EscrowCompany = table.Column<string>(nullable: true),
                    EscrowOfficer = table.Column<string>(nullable: true),
                    EscrowContactEmail = table.Column<string>(nullable: true),
                    EscrowNumber = table.Column<long>(nullable: false),
                    Usertype = table.Column<string>(nullable: true),
                    EscrowOfficerPhoneNumber = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrInvitationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SrInvitationRecords_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SrInvitationRecords_UserId",
                table: "SrInvitationRecords",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SrInvitationRecords");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserFileLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFileLogs_TenantId",
                table: "UserFileLogs",
                column: "TenantId");
        }
    }
}
