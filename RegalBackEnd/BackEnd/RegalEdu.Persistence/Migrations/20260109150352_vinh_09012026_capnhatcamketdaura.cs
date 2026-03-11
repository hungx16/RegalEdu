using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_09012026_capnhatcamketdaura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommitmentOutput",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "IsCommitmentBased",
                table: "Course");

            migrationBuilder.AddColumn<int>(
                name: "CommitmentOutputType",
                table: "Course",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommitmentOutputType",
                table: "Course");

            migrationBuilder.AddColumn<bool>(
                name: "CommitmentOutput",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCommitmentBased",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
