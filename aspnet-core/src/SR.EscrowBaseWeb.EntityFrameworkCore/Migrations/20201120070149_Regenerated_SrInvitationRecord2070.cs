using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_SrInvitationRecord2070 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "EscrowOfficerPhoneNumber",
                table: "SrInvitationRecords",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EscrowOfficerPhoneNumber",
                table: "SrInvitationRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
