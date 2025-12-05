using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mos3ef.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Hospitals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Hospitals",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
