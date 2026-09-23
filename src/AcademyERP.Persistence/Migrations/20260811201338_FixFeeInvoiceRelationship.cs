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
    migrationBuilder.Sql("""
        IF OBJECT_ID(N'[FeeInvoices]', N'U') IS NULL
        BEGIN
            CREATE TABLE [FeeInvoices] (
                [Id] uniqueidentifier NOT NULL,
                [EnrollmentId] uniqueidentifier NOT NULL,
                [InvoiceNumber] nvarchar(50) NOT NULL,
                [Amount] decimal(18,2) NOT NULL,
                [DiscountAmount] decimal(18,2) NOT NULL,
                [PaidAmount] decimal(18,2) NOT NULL,
                [BalanceAmount] decimal(18,2) NOT NULL,
                [InvoiceDate] datetime2 NOT NULL,
                [DueDate] datetime2 NOT NULL,
                [PaidDate] datetime2 NULL,
                [Status] int NOT NULL,
                [Remarks] nvarchar(max) NULL,
                [CreatedAt] datetime2 NOT NULL,
                [CreatedBy] uniqueidentifier NULL,
                [UpdatedAt] datetime2 NULL,
                [UpdatedBy] uniqueidentifier NULL,
                [IsDeleted] bit NOT NULL,
                [DeletedAt] datetime2 NULL,
                [DeletedBy] uniqueidentifier NULL,
                CONSTRAINT [PK_FeeInvoices] PRIMARY KEY ([Id]),
                CONSTRAINT [FK_FeeInvoices_Enrollments_EnrollmentId]
                    FOREIGN KEY ([EnrollmentId])
                    REFERENCES [Enrollments] ([Id])
                    ON DELETE NO ACTION
            );
        END
        ELSE
        BEGIN
            IF EXISTS (
                SELECT 1
                FROM sys.foreign_keys
                WHERE name = N'FK_FeeInvoices_Enrollments_EnrollmentId1'
                  AND parent_object_id = OBJECT_ID(N'[FeeInvoices]')
            )
                ALTER TABLE [FeeInvoices]
                DROP CONSTRAINT [FK_FeeInvoices_Enrollments_EnrollmentId1];

            IF EXISTS (
                SELECT 1
                FROM sys.indexes
                WHERE name = N'IX_FeeInvoices_EnrollmentId1'
                  AND object_id = OBJECT_ID(N'[FeeInvoices]')
            )
                DROP INDEX [IX_FeeInvoices_EnrollmentId1]
                ON [FeeInvoices];

            IF COL_LENGTH(N'[FeeInvoices]', N'EnrollmentId1') IS NOT NULL
                ALTER TABLE [FeeInvoices]
                DROP COLUMN [EnrollmentId1];
        END
        """);
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
