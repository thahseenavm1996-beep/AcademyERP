using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueFeeInvoicePerEnrollmentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeeInvoices_EnrollmentId",
                table: "FeeInvoices");

            migrationBuilder.CreateIndex(
                name: "IX_FeeInvoices_EnrollmentId_InvoiceDate",
                table: "FeeInvoices",
                columns: new[] { "EnrollmentId", "InvoiceDate" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeeInvoices_EnrollmentId_InvoiceDate",
                table: "FeeInvoices");

            migrationBuilder.CreateIndex(
                name: "IX_FeeInvoices_EnrollmentId",
                table: "FeeInvoices",
                column: "EnrollmentId");
        }
    }
}
