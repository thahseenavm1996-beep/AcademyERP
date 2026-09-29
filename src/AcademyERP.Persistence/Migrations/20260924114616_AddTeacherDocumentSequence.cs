using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherDocumentSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM [DocumentSequences]
                    WHERE [Code] = 'TEACHER'
                )
                BEGIN
                    INSERT INTO [DocumentSequences]
                        ([Id], [Code], [Prefix], [NextNumber])
                    VALUES
                        ('22222222-2222-2222-2222-222222222222',
                         'TEACHER',
                         'TCH',
                         01);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [DocumentSequences]
                WHERE [Code] = 'TEACHER'
                  AND [Id] = '22222222-2222-2222-2222-222222222222';
                """);
        }
    }
}