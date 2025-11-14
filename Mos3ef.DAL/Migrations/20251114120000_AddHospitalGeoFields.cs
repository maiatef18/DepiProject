using Microsoft.EntityFrameworkCore.Migrations;

namespace Mos3ef.DAL.Migrations
{
    public partial class AddHospitalGeoFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Hospitals",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Hospitals",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Hospitals");
        }
    }
}