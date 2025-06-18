using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class Added_EscrowDirectMessageDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<long>(
            //    name: "SrEscrowFileMasterId",
            //    table: "SrFileMappings",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.AddColumn<long>(
            //    name: "SrEscrowFileMasterId",
            //    table: "SrAssignedFilesDetails",
            //    type: "bigint",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "FileFullPath",
            //    table: "EscrowFileHistories",
            //    type: "longtext",
            //    nullable: true)
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.AddColumn<long>(
            //    name: "SrEscrowFileMasterId",
            //    table: "EscrowFileHistories",
            //    type: "bigint",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "EscrowDirectMessageDetailses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EscrowUserId = table.Column<long>(type: "bigint", nullable: true),
                    SenderUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscrowDirectMessageDetailses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscrowDirectMessageDetailses_AbpUsers_EscrowUserId",
                        column: x => x.EscrowUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EscrowDirectMessageDetailses_AbpUsers_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateTable(
            //    name: "SREscrowFileMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        FileFullName = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        FileShortName = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SREscrowFileMasters", x => x.Id);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            //migrationBuilder.CreateTable(
            //    name: "srEscrowFileReminders",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(type: "bigint", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        ReminderType = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        SentTo = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        ReminderText = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        SentFrom = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        ReminderStatus = table.Column<bool>(type: "tinyint(1)", nullable: false),
            //        SentToUserType = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        SentFromUserType = table.Column<string>(type: "longtext", nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        SREscrowFileMasterId = table.Column<long>(type: "bigint", nullable: true),
            //        CreatedBy = table.Column<long>(type: "bigint", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_srEscrowFileReminders", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_srEscrowFileReminders_AbpUsers_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "AbpUsers",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_srEscrowFileReminders_SREscrowFileMasters_SREscrowFileMaster~",
            //            column: x => x.SREscrowFileMasterId,
            //            principalTable: "SREscrowFileMasters",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EscrowDirectMessageDetailses_EscrowUserId",
                table: "EscrowDirectMessageDetailses",
                column: "EscrowUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EscrowDirectMessageDetailses_SenderUserId",
                table: "EscrowDirectMessageDetailses",
                column: "SenderUserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_srEscrowFileReminders_CreatedBy",
            //    table: "srEscrowFileReminders",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_srEscrowFileReminders_SREscrowFileMasterId",
            //    table: "srEscrowFileReminders",
            //    column: "SREscrowFileMasterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EscrowDirectMessageDetailses");

            //migrationBuilder.DropTable(
            //    name: "srEscrowFileReminders");

            //migrationBuilder.DropTable(
            //    name: "SREscrowFileMasters");

            //migrationBuilder.DropColumn(
            //    name: "SrEscrowFileMasterId",
            //    table: "SrFileMappings");

            //migrationBuilder.DropColumn(
            //    name: "SrEscrowFileMasterId",
            //    table: "SrAssignedFilesDetails");

            //migrationBuilder.DropColumn(
            //    name: "FileFullPath",
            //    table: "EscrowFileHistories");

            //migrationBuilder.DropColumn(
            //    name: "SrEscrowFileMasterId",
            //    table: "EscrowFileHistories");
        }
    }
}
