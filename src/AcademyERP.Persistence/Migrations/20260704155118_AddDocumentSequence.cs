using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyERP.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NextNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSequences", x => x.Id);
                });
            migrationBuilder.InsertData(
table: "DocumentSequences",
columns: new[] { "Id", "Code", "Prefix", "NextNumber" },
values: new object[]
{
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        "STUDENT",
        "SGO",
        100
});

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSequences_Code",
                table: "DocumentSequences",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
    table: "DocumentSequences",
    keyColumn: "Id",
    keyValue: Guid.Parse("11111111-1111-1111-1111-111111111111"));
            migrationBuilder.DropTable(
                name: "DocumentSequences");
        }
    }
}
