using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledClassToClassReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Enrollments_EnrollmentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Enrollments_EnrollmentId1",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_Enrollments_EnrollmentId",
                table: "ClassReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_Students_StudentId",
                table: "ClassReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_Teachers_TeacherId",
                table: "ClassReports");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_EnrollmentId1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "AttendanceStatus",
                table: "ClassReports");

            migrationBuilder.DropColumn(
                name: "ReportDate",
                table: "ClassReports");

            migrationBuilder.DropColumn(
                name: "AttendanceDate",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "Attendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "ClassReports",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "ClassReports",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "EnrollmentId",
                table: "ClassReports",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

           migrationBuilder.AddColumn<Guid>(
    name: "ScheduledClassId",
    table: "ClassReports",
    type: "uniqueidentifier",
    nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EnrollmentId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

           migrationBuilder.AddColumn<Guid>(
    name: "ScheduledClassId",
    table: "Attendances",
    type: "uniqueidentifier",
    nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassReports_ScheduledClassId",
                table: "ClassReports",
                column: "ScheduledClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ScheduledClassId",
                table: "Attendances",
                column: "ScheduledClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Enrollments_EnrollmentId",
                table: "Attendances",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ScheduledClasses_ScheduledClassId",
                table: "Attendances",
                column: "ScheduledClassId",
                principalTable: "ScheduledClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_Enrollments_EnrollmentId",
                table: "ClassReports",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_ScheduledClasses_ScheduledClassId",
                table: "ClassReports",
                column: "ScheduledClassId",
                principalTable: "ScheduledClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_Students_StudentId",
                table: "ClassReports",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_Teachers_TeacherId",
                table: "ClassReports",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Enrollments_EnrollmentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ScheduledClasses_ScheduledClassId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_Enrollments_EnrollmentId",
                table: "ClassReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_ScheduledClasses_ScheduledClassId",
                table: "ClassReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_Students_StudentId",
                table: "ClassReports");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassReports_Teachers_TeacherId",
                table: "ClassReports");

            migrationBuilder.DropIndex(
                name: "IX_ClassReports_ScheduledClassId",
                table: "ClassReports");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ScheduledClassId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "ScheduledClassId",
                table: "ClassReports");

            migrationBuilder.DropColumn(
                name: "ScheduledClassId",
                table: "Attendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "TeacherId",
                table: "ClassReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "ClassReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EnrollmentId",
                table: "ClassReports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttendanceStatus",
                table: "ClassReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportDate",
                table: "ClassReports",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "EnrollmentId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceDate",
                table: "Attendances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId1",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_EnrollmentId1",
                table: "Attendances",
                column: "EnrollmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Enrollments_EnrollmentId",
                table: "Attendances",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Enrollments_EnrollmentId1",
                table: "Attendances",
                column: "EnrollmentId1",
                principalTable: "Enrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_Enrollments_EnrollmentId",
                table: "ClassReports",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_Students_StudentId",
                table: "ClassReports",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassReports_Teachers_TeacherId",
                table: "ClassReports",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
