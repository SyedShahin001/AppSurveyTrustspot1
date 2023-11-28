using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppSurveyTrustspot.Migrations
{
    public partial class ab3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUrl",
                table: "Apps",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUrl",
                table: "Apps");
        }
    }
}
