using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class addedColumnInEnterpriseEnterpriseID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             

            migrationBuilder.AddColumn<string>(
                name: "EnterpriseId",
                table: "Enterprises",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "Enterprises");
        }
    }
}
