using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeApi.Infrastructure.Migrations
{
    public partial class AddEmailAndPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop any leftover columns from the previously failed migration
            // so we can re-add them with the correct names and sizes cleanly.
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Employees','Email') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN Email;");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Employees','PhoneNumber') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN PhoneNumber;");
            migrationBuilder.Sql("IF COL_LENGTH('dbo.Employees','Phone') IS NOT NULL ALTER TABLE dbo.Employees DROP COLUMN Phone;");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Email", table: "Employees");
            migrationBuilder.DropColumn(name: "Phone", table: "Employees");
        }
    }
}
