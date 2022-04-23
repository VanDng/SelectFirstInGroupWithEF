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
                { 2, "B", 5.0},
                { 3, "C", 5.0},
                { 4, "D", 6.0},
                { 5, "E", 6.0},
                { 6, "F", 7.0},
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
