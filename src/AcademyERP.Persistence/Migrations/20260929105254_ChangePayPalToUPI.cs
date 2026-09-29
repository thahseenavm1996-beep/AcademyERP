using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangePayPalToUPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_FeeInvoices_EnrollmentId'
                      AND object_id = OBJECT_ID('dbo.FeeInvoices')
                )
                BEGIN
                    DROP INDEX [IX_FeeInvoices_EnrollmentId] ON [dbo].[FeeInvoices];
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_FeeInvoices_EnrollmentId_InvoiceDate'
                      AND object_id = OBJECT_ID('dbo.FeeInvoices')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_FeeInvoices_EnrollmentId_InvoiceDate]
                    ON [dbo].[FeeInvoices] ([EnrollmentId], [InvoiceDate])
                    WHERE [IsDeleted] = 0;
                END
                """);

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Name",
                value: "UPI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_FeeInvoices_EnrollmentId_InvoiceDate'
                      AND object_id = OBJECT_ID('dbo.FeeInvoices')
                )
                BEGIN
                    DROP INDEX [IX_FeeInvoices_EnrollmentId_InvoiceDate]
                    ON [dbo].[FeeInvoices];
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_FeeInvoices_EnrollmentId'
                      AND object_id = OBJECT_ID('dbo.FeeInvoices')
                )
                BEGIN
                    CREATE INDEX [IX_FeeInvoices_EnrollmentId]
                    ON [dbo].[FeeInvoices] ([EnrollmentId]);
                END
                """);

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "Name",
                value: "PayPal");
        }
    }
}
