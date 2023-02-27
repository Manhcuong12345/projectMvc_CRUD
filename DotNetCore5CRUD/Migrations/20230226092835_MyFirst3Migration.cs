using Microsoft.EntityFrameworkCore.Migrations;

namespace DotNetCore5CRUD.Migrations
{
    public partial class MyFirst3Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Staff");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Staff",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Staff",
                newName: "Job");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Job",
                table: "Staff",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Staff",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "Staff",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Staff",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
