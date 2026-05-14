using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Managerment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskNoteNotificationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id_note = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    content = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    id_staff = table.Column<Guid>(type: "uuid", nullable: false),
                    linked_entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    linked_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_note_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("note_pkey", x => x.id_note);
                    table.ForeignKey(
                        name: "fk_note_parent",
                        column: x => x.parent_note_id,
                        principalTable: "notes",
                        principalColumn: "id_note",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_note_staff",
                        column: x => x.id_staff,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id_notification = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    id_staff = table.Column<Guid>(type: "uuid", nullable: false),
                    related_entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    related_entity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notification_pkey", x => x.id_notification);
                    table.ForeignKey(
                        name: "fk_notification_staff",
                        column: x => x.id_staff,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id_task = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    id_staff_assigned = table.Column<Guid>(type: "uuid", nullable: false),
                    linked_entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    linked_entity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("task_pkey", x => x.id_task);
                    table.ForeignKey(
                        name: "fk_task_staff",
                        column: x => x.id_staff_assigned,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "note_mentions",
                columns: table => new
                {
                    id_mention = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_note = table.Column<Guid>(type: "uuid", nullable: false),
                    id_staff_mentioned = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("note_mention_pkey", x => x.id_mention);
                    table.ForeignKey(
                        name: "fk_note_mention_note",
                        column: x => x.id_note,
                        principalTable: "notes",
                        principalColumn: "id_note",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_note_mention_staff",
                        column: x => x.id_staff_mentioned,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_note_mentions_id_note",
                table: "note_mentions",
                column: "id_note");

            migrationBuilder.CreateIndex(
                name: "ix_note_mentions_id_staff_mentioned",
                table: "note_mentions",
                column: "id_staff_mentioned");

            migrationBuilder.CreateIndex(
                name: "ix_notes_entity",
                table: "notes",
                columns: new[] { "linked_entity_type", "linked_entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_notes_id_staff",
                table: "notes",
                column: "id_staff");

            migrationBuilder.CreateIndex(
                name: "IX_notes_parent_note_id",
                table: "notes",
                column: "parent_note_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_id_staff",
                table: "notifications",
                column: "id_staff");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_id_staff_is_read",
                table: "notifications",
                columns: new[] { "id_staff", "is_read" });

            migrationBuilder.CreateIndex(
                name: "ix_tasks_id_staff_assigned",
                table: "tasks",
                column: "id_staff_assigned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "note_mentions");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "notes");
        }
    }
}
