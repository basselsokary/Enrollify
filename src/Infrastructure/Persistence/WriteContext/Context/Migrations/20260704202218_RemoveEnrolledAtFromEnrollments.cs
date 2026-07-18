using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.WriteContext.Context.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnrolledAtFromEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentAt",
                schema: "enrollify",
                table: "Enrollments");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "enrollify",
                table: "Courses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "enrollify",
                table: "Courses");

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrollmentAt",
                schema: "enrollify",
                table: "Enrollments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
