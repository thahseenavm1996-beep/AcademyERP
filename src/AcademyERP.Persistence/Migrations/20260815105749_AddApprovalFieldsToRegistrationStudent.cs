using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFieldsToRegistrationStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassDurationId",
                table: "RegistrationRequestStudents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "RegistrationRequestStudents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "RegistrationRequestStudents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TimeSlotId",
                table: "RegistrationRequestStudents",
                type: "uniqueidentifier",
                nullable: true);

            
            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestStudents_ClassDurationId",
                table: "RegistrationRequestStudents",
                column: "ClassDurationId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestStudents_CourseId",
                table: "RegistrationRequestStudents",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestStudents_TeacherId",
                table: "RegistrationRequestStudents",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationRequestStudents_TimeSlotId",
                table: "RegistrationRequestStudents",
                column: "TimeSlotId");

           
            

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestStudents_ClassDurations_ClassDurationId",
                table: "RegistrationRequestStudents",
                column: "ClassDurationId",
                principalTable: "ClassDurations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestStudents_Courses_CourseId",
                table: "RegistrationRequestStudents",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestStudents_Teachers_TeacherId",
                table: "RegistrationRequestStudents",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrationRequestStudents_TimeSlots_TimeSlotId",
                table: "RegistrationRequestStudents",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id");

            

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestStudents_ClassDurations_ClassDurationId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestStudents_Courses_CourseId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestStudents_Teachers_TeacherId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrationRequestStudents_TimeSlots_TimeSlotId",
                table: "RegistrationRequestStudents");

            

            

           

            

            migrationBuilder.DropIndex(
                name: "IX_RegistrationRequestStudents_ClassDurationId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationRequestStudents_CourseId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationRequestStudents_TeacherId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropIndex(
                name: "IX_RegistrationRequestStudents_TimeSlotId",
                table: "RegistrationRequestStudents");

            

            migrationBuilder.DropColumn(
                name: "ClassDurationId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "RegistrationRequestStudents");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "RegistrationRequestStudents");
        }
    }
}
