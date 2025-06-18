using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class ColumnAddedInEscrowClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnterprsieId",
                table: "escrowclients",
                nullable: true
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                 name: "EnterprsieId",
                table: "escrowclients"
                );

        }
    }
}
