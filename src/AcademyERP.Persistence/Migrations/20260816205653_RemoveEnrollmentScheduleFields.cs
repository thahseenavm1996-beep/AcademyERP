using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnrollmentScheduleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_ClassDurations_ClassDurationId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_TimeSlots_TimeSlotId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_ClassDurationId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_TimeSlotId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "ClassDurationId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "TimeSlotId",
                table: "Enrollments");

            migrationBuilder.CreateTable(
                name: "EnrollmentSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    TimeSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassDurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrollmentSchedules_ClassDurations_ClassDurationId",
                        column: x => x.ClassDurationId,
                        principalTable: "ClassDurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnrollmentSchedules_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnrollmentSchedules_TimeSlots_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentSchedules_ClassDurationId",
                table: "EnrollmentSchedules",
                column: "ClassDurationId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentSchedules_EnrollmentId_DayOfWeek_TimeSlotId",
                table: "EnrollmentSchedules",
                columns: new[] { "EnrollmentId", "DayOfWeek", "TimeSlotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnrollmentSchedules_TimeSlotId",
                table: "EnrollmentSchedules",
                column: "TimeSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrollmentSchedules");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassDurationId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TimeSlotId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassDurationId",
                table: "Enrollments",
                column: "ClassDurationId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TimeSlotId",
                table: "Enrollments",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_ClassDurations_ClassDurationId",
                table: "Enrollments",
                column: "ClassDurationId",
                principalTable: "ClassDurations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_TimeSlots_TimeSlotId",
                table: "Enrollments",
                column: "TimeSlotId",
                principalTable: "TimeSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
