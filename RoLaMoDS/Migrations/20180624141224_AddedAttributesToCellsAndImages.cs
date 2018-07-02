using Microsoft.EntityFrameworkCore.Migrations;

namespace RoLaMoDS.Migrations
{
    public partial class AddedAttributesToCellsAndImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cloudy",
                table: "Images",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Humidity",
                table: "Images",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Teperature",
                table: "Images",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WindSpeed",
                table: "Images",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "GroundType",
                table: "Cells",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cloudy",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Teperature",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "WindSpeed",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "GroundType",
                table: "Cells");
        }
    }
}
