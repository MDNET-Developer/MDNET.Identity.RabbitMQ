using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MDNET.Identity.RabbitMQ.Web.Migrations
{
    /// <inheritdoc />
    public partial class changedatatypebooltointUserFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FileStatus",
                table: "UserFiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "FileStatus",
                table: "UserFiles",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
