using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Regenerated_SrEscrow4979 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            //migrationBuilder.DropColumn(
            //    name: "Idhhhh",
            //    table: "SrEscrows");

            //migrationBuilder.AlterColumn<int>(
            //    name: "EnterpriseIds",
            //    table: "SrEscrows",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SrEscrows",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<int>(
            //    name: "EnterpriseId",
            //    table: "SrEscrows",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_SrEscrows_EnterpriseId",
            //    table: "SrEscrows",
            //    column: "EnterpriseId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_SrEscrows_Enterprises_EnterpriseId",
            //    table: "SrEscrows",
            //    column: "EnterpriseId",
            //    principalTable: "Enterprises",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
