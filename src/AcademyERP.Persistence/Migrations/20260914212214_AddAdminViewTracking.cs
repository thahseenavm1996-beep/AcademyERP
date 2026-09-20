using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminViewTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ViewedByAdminAt",
                table: "RegistrationRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DemoFeeReminderSentAt",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DemoStartedAt",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ViewedByAdminAt",
                table: "AdmissionApplications",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewedByAdminAt",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "DemoFeeReminderSentAt",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "DemoStartedAt",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "ViewedByAdminAt",
                table: "AdmissionApplications");
        }
    }
}
