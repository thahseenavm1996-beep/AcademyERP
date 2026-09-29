using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeacherStudentRequestForNewStudents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "TeacherStudentRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "TeacherStudentRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            // Add these temporarily as nullable so existing requests
            // can be populated from the existing Students table.
            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "TeacherStudentRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentPhoneNumber",
                table: "TeacherStudentRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Populate the new fields for all existing requests.
            migrationBuilder.Sql(@"
                UPDATE r
                SET
                    r.StudentName =
                        COALESCE(s.FullName, ''),
                    r.StudentPhoneNumber =
                        COALESCE(s.PhoneNumber, '')
                FROM TeacherStudentRequests r
                LEFT JOIN Students s
                    ON r.StudentId = s.Id;
            ");

            // Existing rows now have values, so make the fields required.
            migrationBuilder.AlterColumn<string>(
                name: "StudentName",
                table: "TeacherStudentRequests",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StudentPhoneNumber",
                table: "TeacherStudentRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "TeacherStudentRequests");

            migrationBuilder.DropColumn(
                name: "StudentPhoneNumber",
                table: "TeacherStudentRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "TeacherStudentRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "TeacherStudentRequests",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
