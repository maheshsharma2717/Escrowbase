using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_SrEscrow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SrEscrows",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EscrowNo = table.Column<string>(nullable: false),
                    PropertyAddress = table.Column<string>(nullable: true),
                    EscrowOfficerName = table.Column<string>(nullable: true),
                    EOEmail = table.Column<string>(nullable: true),
                    EOPhone = table.Column<string>(nullable: true),
                    EoPhoneExt = table.Column<string>(nullable: true),
                    EoPhoneCell = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SrEscrows", x => x.Id);
                });


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SrEscrows");
        }
    }
}
