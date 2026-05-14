using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Managerment.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_by",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "persons");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "tasks",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "deleted_by",
                table: "tasks",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "tasks",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "persons",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "persons",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "persons",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "persons",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "notes",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "deleted_by",
                table: "notes",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "notes",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "deals",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "deleted_by",
                table: "deals",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "deals",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "contacts",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "deleted_by",
                table: "contacts",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "contacts",
                newName: "CreatedBy");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "persons",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "persons",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "notes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "notes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "notes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "deals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "deals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "deals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "contacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "tasks",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "tasks",
                newName: "deleted_by");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "tasks",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "persons",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "persons",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "persons",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "persons",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "notes",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "notes",
                newName: "deleted_by");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "notes",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "deals",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "deals",
                newName: "deleted_by");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "deals",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "contacts",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "contacts",
                newName: "deleted_by");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "contacts",
                newName: "created_by");

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "deleted_by",
                table: "tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "persons",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "persons",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "notes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "deleted_by",
                table: "notes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "notes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "deals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "deleted_by",
                table: "deals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "deals",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "updated_by",
                table: "contacts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "deleted_by",
                table: "contacts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "contacts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
