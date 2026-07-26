using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionConversionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConverted",
                table: "AdmissionApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "AdmissionApplications",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConverted",
                table: "AdmissionApplications");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "AdmissionApplications");
        }
    }
}
