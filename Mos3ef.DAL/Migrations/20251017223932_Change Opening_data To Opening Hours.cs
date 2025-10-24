using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mos3ef.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOpening_dataToOpeningHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Opening_Date",
                table: "Hospitals",
                newName: "Opening_hours");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Opening_hours",
                table: "Hospitals",
                newName: "Opening_Date");
        }
    }
}
