using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mos3ef.DAL.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_AspNetUsers_UserId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_AspNetUsers_UserId",
                table: "Hospitals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_AspNetUsers_UserId",
                table: "Hospitals");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Patients");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_AspNetUsers_UserId",
                table: "Hospitals",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
