using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Managerment.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditColumnsFromEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "notes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "notes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "notes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "deals");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "deals");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "deals");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "contacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "tasks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "notes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "deals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "deals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "deals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "contacts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "contacts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "contacts",
                type: "text",
                nullable: true);
        }
    }
}
