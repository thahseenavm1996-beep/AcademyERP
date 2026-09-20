using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherPaymentPayablePeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CycleScheduledDays",
                table: "TeacherPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayableDays",
                table: "TeacherPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PayableFromDate",
                table: "TeacherPayments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PayableToDate",
                table: "TeacherPayments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CycleScheduledDays",
                table: "TeacherPayments");

            migrationBuilder.DropColumn(
                name: "PayableDays",
                table: "TeacherPayments");

            migrationBuilder.DropColumn(
                name: "PayableFromDate",
                table: "TeacherPayments");

            migrationBuilder.DropColumn(
                name: "PayableToDate",
                table: "TeacherPayments");
        }
    }
}
