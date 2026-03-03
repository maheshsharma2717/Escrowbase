using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_CurrentEscrow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<string>(
                name: "AccessToken",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AccessTokenTime",
                table: "Enterprises",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ESignApiAccountId",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ESignClientId",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ESignClientSecret",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ESignFolderId",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ESignProviderCode",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ESignUserId",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Enterprises",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdminAssigned",
                table: "Enterprises",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Enterprises",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CurrentEscrows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    EscrowNo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubCompanyName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentEscrows", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "E_SignCompany",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompanyName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SystemCode = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_E_SignCompany", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentEscrows_TenantId",
                table: "CurrentEscrows",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentEscrows");

            migrationBuilder.DropTable(
                name: "E_SignCompany");

            migrationBuilder.DropColumn(
                name: "OtherAction",
                table: "SrFileMappings");

            migrationBuilder.DropColumn(
                name: "OtherActionNote",
                table: "SrFileMappings");

            migrationBuilder.DropColumn(
                name: "OtherAction",
                table: "SREscrowFileMasters");

            migrationBuilder.DropColumn(
                name: "OtherActionNote",
                table: "SREscrowFileMasters");

            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "AccessTokenTime",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "ESignApiAccountId",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "ESignClientId",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "ESignClientSecret",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "ESignFolderId",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "ESignProviderCode",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "ESignUserId",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "IsAdminAssigned",
                table: "Enterprises");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Enterprises");
        }
    }
}
