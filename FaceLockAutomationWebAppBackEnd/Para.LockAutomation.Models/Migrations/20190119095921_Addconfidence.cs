using Microsoft.EntityFrameworkCore.Migrations;

namespace Para.LockAutomation.Models.Migrations
{
    public partial class Addconfidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ConfindenceThreshold",
                table: "personGroup",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Confidences",
                table: "faceLog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfindenceThreshold",
                table: "personGroup");

            migrationBuilder.DropColumn(
                name: "Confidences",
                table: "faceLog");
        }
    }
}
