using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class userTypedefined : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnterpriseId",
                table: "UserTypes",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "UserTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "UserTypes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserTypes");
        }
    }
}
