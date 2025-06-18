using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_EsignRoleMappings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            //migrationBuilder.CreateTable(
            //    name: "EsignRoleMappingses",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        TenantId = table.Column<int>(type: "int", nullable: false),
            //        EsignRole = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        UserRole = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        EsignCompanyCode = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EsignRoleMappingses", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateIndex(
            //    name: "IX_EsignRoleMappingses_TenantId",
            //    table: "EsignRoleMappingses",
            //    column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "EsignRoleMappingses");

        }
    }
}
