using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCore_3.Migrations
{
    public partial class AddStudentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var tableName = "Student";

            migrationBuilder.CreateTable(
                name: tableName,
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Grade = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.ID);
                });

            migrationBuilder.InsertData(tableName, new string[]
            {
                "ID", "Name", "Grade"
            }, new object[,]
            {
                { 1, "A", 5.0},
                { 2, "A", 6.0},
                { 3, "A", 7.0},
                { 4, "B", 8.0},
                { 5, "B", 9.0},
                { 6, "B", 10.0},
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
