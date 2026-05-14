using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Managerment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "calendar_events",
                columns: table => new
                {
                    id_event = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    event_type = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_all_day = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    reminder_minutes = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    id_staff = table.Column<Guid>(type: "uuid", nullable: false),
                    related_entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    related_entity_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("calendar_event_pkey", x => x.id_event);
                    table.ForeignKey(
                        name: "fk_calendar_event_staff",
                        column: x => x.id_staff,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_participants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_event = table.Column<Guid>(type: "uuid", nullable: false),
                    id_staff = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", maxLength: 50, nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("event_participant_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_event_participant_event",
                        column: x => x.id_event,
                        principalTable: "calendar_events",
                        principalColumn: "id_event",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_event_participant_staff",
                        column: x => x.id_staff,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_calendar_events_id_staff",
                table: "calendar_events",
                column: "id_staff");

            migrationBuilder.CreateIndex(
                name: "ix_calendar_events_staff_start",
                table: "calendar_events",
                columns: new[] { "id_staff", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ix_calendar_events_start_time",
                table: "calendar_events",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "ix_event_participants_id_event",
                table: "event_participants",
                column: "id_event");

            migrationBuilder.CreateIndex(
                name: "ix_event_participants_id_staff",
                table: "event_participants",
                column: "id_staff");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_participants");

            migrationBuilder.DropTable(
                name: "calendar_events");
        }
    }
}
