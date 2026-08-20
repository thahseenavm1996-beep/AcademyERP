using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClassProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChapterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagesCovered = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLessonCompleted = table.Column<bool>(type: "bit", nullable: false),
                    TeacherNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Homework = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeworkGiven = table.Column<bool>(type: "bit", nullable: false),
                    HomeworkCompleted = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_ClassProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassProgresses_ScheduledClasses_ScheduledClassId",
                        column: x => x.ScheduledClassId,
                        principalTable: "ScheduledClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassProgresses_ScheduledClassId",
                table: "ClassProgresses",
                column: "ScheduledClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassProgresses");
        }
    }
}
