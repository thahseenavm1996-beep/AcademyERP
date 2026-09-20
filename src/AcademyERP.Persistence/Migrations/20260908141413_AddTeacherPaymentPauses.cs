using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherPaymentPauses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeacherPaymentPauses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherPaymentConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PauseStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResumeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_TeacherPaymentPauses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherPaymentPauses_TeacherPaymentConfigurations_TeacherPaymentConfigurationId",
                        column: x => x.TeacherPaymentConfigurationId,
                        principalTable: "TeacherPaymentConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPaymentPauses_TeacherPaymentConfigurationId_PauseStartDate",
                table: "TeacherPaymentPauses",
                columns: new[] { "TeacherPaymentConfigurationId", "PauseStartDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherPaymentPauses");
        }
    }
}
