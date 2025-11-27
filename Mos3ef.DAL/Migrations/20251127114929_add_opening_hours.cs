using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mos3ef.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_opening_hours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Opening_Hours",
                table: "Hospitals",
                type: "datetime2",
                nullable: true,
                defaultValue: DateTime.Now); 
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Opening_Hours",
                table: "Hospitals");
        }
    }
}
