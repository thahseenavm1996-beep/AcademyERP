using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteAdmissionsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestStudents_ProgramId",
                table: "RegistrationRequestStudents",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestStudents_Programs_ProgramId",
                table: "RegistrationRequestStudents",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestStudents_Programs_ProgramId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationRequestStudents_ProgramId",
                table: "RegistrationRequestStudents");
        }
    }
}
