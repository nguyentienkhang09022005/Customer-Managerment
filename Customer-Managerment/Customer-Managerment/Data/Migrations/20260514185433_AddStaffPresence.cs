using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Managerment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffPresence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_active_at",
                table: "persons",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "persons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "staff_activity_logs",
                columns: table => new
                {
                    id_log = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_staff = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ip_address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("staff_activity_log_pkey", x => x.id_log);
                    table.ForeignKey(
                        name: "fk_staff_activity_log_staff",
                        column: x => x.id_staff,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_staff_activity_logs_id_staff",
                table: "staff_activity_logs",
                column: "id_staff");

            migrationBuilder.CreateIndex(
                name: "ix_staff_activity_logs_timestamp",
                table: "staff_activity_logs",
                column: "timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "staff_activity_logs");

            migrationBuilder.DropColumn(
                name: "last_active_at",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "status",
                table: "persons");
        }
    }
}
