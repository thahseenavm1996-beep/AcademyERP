using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixFeeInvoiceRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeeInvoices_Enrollments_EnrollmentId1",
                table: "FeeInvoices");

            migrationBuilder.DropIndex(
                name: "IX_FeeInvoices_EnrollmentId1",
                table: "FeeInvoices");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "FeeInvoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId1",
                table: "FeeInvoices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeInvoices_EnrollmentId1",
                table: "FeeInvoices",
                column: "EnrollmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FeeInvoices_Enrollments_EnrollmentId1",
                table: "FeeInvoices",
                column: "EnrollmentId1",
                principalTable: "Enrollments",
                principalColumn: "Id");
        }
    }
}
