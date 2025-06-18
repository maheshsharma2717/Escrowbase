using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_E_SignRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "Logo",
            //    table: "Enterprises",
            //    nullable: true);



            migrationBuilder.CreateTable(
                name: "E_SignRecords",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: true),
                    EmailId = table.Column<string>(nullable: true),
                    EmbeddedURL = table.Column<string>(nullable: true),
                    EmbeddedToken = table.Column<string>(nullable: true),
                    FolderId = table.Column<long>(nullable: false),
                    FolderName = table.Column<string>(nullable: true),
                    FolderPassword = table.Column<string>(nullable: true),
                    PartyId = table.Column<int>(nullable: false),
                    ContractId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<long>(nullable: false),
                    DocumentId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_E_SignRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_E_SignRecords_TenantId",
                table: "E_SignRecords",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "E_SignRecords");

            //migrationBuilder.DropColumn(
            //    name: "Logo",
            //    table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "Subcompany",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "AttemptsCount",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "BlockAttemptsTill",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "UserIP",
                table: "AbpUsers");
        }
    }
}
