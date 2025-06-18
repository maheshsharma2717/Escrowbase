using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_EscrowFileHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.AddColumn<string>(
        //        name: "Signin_percentage",
        //        table: "E_SignRecords",
        //        type: "longtext",
        //        nullable: true)
        //        .Annotation("MySql:CharSet", "utf8mb4");

        //    migrationBuilder.CreateTable(
        //        name: "EscrowFileHistories",
        //        columns: table => new
        //        {
        //            Id = table.Column<long>(type: "bigint", nullable: false)
        //                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
        //            Message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
        //                .Annotation("MySql:CharSet", "utf8mb4"),
        //            ActionType = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
        //                .Annotation("MySql:CharSet", "utf8mb4"),
        //            CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
        //            UserId = table.Column<long>(type: "bigint", nullable: true)
        //        },
        //        constraints: table =>
        //        {
        //            table.PrimaryKey("PK_EscrowFileHistories", x => x.Id);
        //            table.ForeignKey(
        //                name: "FK_EscrowFileHistories_AbpUsers_UserId",
        //                column: x => x.UserId,
        //                principalTable: "AbpUsers",
        //                principalColumn: "Id",
        //                onDelete: ReferentialAction.Restrict);
        //        })
        //        .Annotation("MySql:CharSet", "utf8mb4");

        //    migrationBuilder.CreateIndex(
        //        name: "IX_EscrowFileHistories_UserId",
        //        table: "EscrowFileHistories",
        //        column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "EscrowFileHistories");

            //migrationBuilder.DropColumn(
            //    name: "Signin_percentage",
            //    table: "E_SignRecords");
        }
    }
}
