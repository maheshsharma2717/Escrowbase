using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SR.EscrowBaseWeb.Migrations
{
    public partial class addedColumnInEnterprise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "EnterpriseExt",
               table: "Enterprises",
               nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "EnterpriseExtFlag",
              table: "Enterprises",
              nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "PrimaryContact",
              table: "Enterprises",
              nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "PrimaryContactCellNo",
              table: "Enterprises",
              nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "AlternateEnterpriseName",
              table: "Enterprises",
              nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "BrokerName",
              table: "Enterprises",
              nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "CorporateName",
              table: "Enterprises",
              nullable: true);

            migrationBuilder.AddColumn<string>(
              name: "OfficePhone",
              table: "Enterprises",
              nullable: true);



            migrationBuilder.AddColumn<string>(
              name: "OfficeFax",
              table: "Enterprises",
              nullable: true);



            migrationBuilder.AddColumn<string>(
             name: "SecondaryEnterpriseEmail",
             table: "Enterprises",
             nullable: true);

            migrationBuilder.AddColumn<string>(
             name: "DisclosureVerbage",
             table: "Enterprises",
             nullable: true);

            migrationBuilder.AddColumn<string>(
             name: "LicenseVerbiage",
             table: "Enterprises",
             nullable: true);

            migrationBuilder.AddColumn<string>(
             name: "DefaultRealtor",
             table: "Enterprises",
             nullable: true);


            migrationBuilder.AddColumn<string>(
        name: "DefaultMbroker",
        table: "Enterprises",
        nullable: true);

            migrationBuilder.AddColumn<string>(
        name: "DefaultTitle",
        table: "Enterprises",
        nullable: true);

            migrationBuilder.AddColumn<string>(
        name: "DefaultRefi",
        table: "Enterprises",
        nullable: true);

            migrationBuilder.AddColumn<string>(
        name: "TaxPayerID",
        table: "Enterprises",
        nullable: true);

            migrationBuilder.AddColumn<string>(
        name: "LicenesNo",
        table: "Enterprises",
        nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnterpriseExt",
                table: "Enterprises");

            migrationBuilder.DropColumn(
               name: "EnterpriseExtFlag",
               table: "Enterprises");

            migrationBuilder.DropColumn(
               name: "PrimaryContact",
               table: "Enterprises");


            migrationBuilder.DropColumn(
             name: "PrimaryContactCellNo",
             table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "AlternateEnterpriseName",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "BrokerName",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "CorporateName",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "OfficePhone",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "OfficeFax",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "SecondaryEnterpriseEmail",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "DisclosureVerbage",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "LicenseVerbiage",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "DefaultRealtor",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "DefaultMbroker",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "DefaultTitle",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "DefaultRefi",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "TaxPayerID",
         table: "Enterprises");

            migrationBuilder.DropColumn(
         name: "LicenesNo",
         table: "Enterprises");



        }
    }
}
