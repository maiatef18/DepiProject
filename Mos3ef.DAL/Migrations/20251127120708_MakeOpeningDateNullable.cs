using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mos3ef.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeOpeningDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Opening_Date",
                table: "Hospitals",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Opening_Date",
                table: "Hospitals",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.Now,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
