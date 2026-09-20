using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ScheduledClasses_ScheduledClassId",
                table: "Attendances");

            migrationBuilder.CreateTable(
                name: "TeacherPaymentConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherPaymentConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherPaymentConfigurations_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherPaymentConfigurations_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeacherPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CycleStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CycleEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherPayments_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeacherPayments_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPaymentConfigurations_EnrollmentId",
                table: "TeacherPaymentConfigurations",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPaymentConfigurations_TeacherId_EnrollmentId_EffectiveFrom",
                table: "TeacherPaymentConfigurations",
                columns: new[] { "TeacherId", "EnrollmentId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPayments_EnrollmentId",
                table: "TeacherPayments",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPayments_TeacherId_EnrollmentId_CycleStartDate_CycleEndDate",
                table: "TeacherPayments",
                columns: new[] { "TeacherId", "EnrollmentId", "CycleStartDate", "CycleEndDate" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ScheduledClasses_ScheduledClassId",
                table: "Attendances",
                column: "ScheduledClassId",
                principalTable: "ScheduledClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_ScheduledClasses_ScheduledClassId",
                table: "Attendances");

            migrationBuilder.DropTable(
                name: "TeacherPaymentConfigurations");

            migrationBuilder.DropTable(
                name: "TeacherPayments");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_ScheduledClasses_ScheduledClassId",
                table: "Attendances",
                column: "ScheduledClassId",
                principalTable: "ScheduledClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
