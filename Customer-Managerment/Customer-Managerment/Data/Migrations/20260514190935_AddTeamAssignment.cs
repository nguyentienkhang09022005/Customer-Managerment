using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer_Managerment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "team_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    entity_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_staff = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    assigned_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    can_edit = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    can_delete = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("team_member_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_team_member_staff",
                        column: x => x.id_staff,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_team_members_entity",
                table: "team_members",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_team_members_id_staff",
                table: "team_members",
                column: "id_staff");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "team_members");
        }
    }
}
