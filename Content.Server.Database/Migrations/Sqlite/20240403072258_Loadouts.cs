// SPDX-FileCopyrightText: 2024 Firewatch <54725557+musicmanvr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <45323883+Dutch-VanDerLinde@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Mr. 27 <koolthunder019@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class Loadouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "profile_role_loadout",
                columns: table => new
                {
                    profile_role_loadout_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    profile_id = table.Column<int>(type: "INTEGER", nullable: false),
                    role_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_role_loadout", x => x.profile_role_loadout_id);
                    table.ForeignKey(
                        name: "FK_profile_role_loadout_profile_profile_id",
                        column: x => x.profile_id,
                        principalTable: "profile",
                        principalColumn: "profile_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_loadout_group",
                columns: table => new
                {
                    profile_loadout_group_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    profile_role_loadout_id = table.Column<int>(type: "INTEGER", nullable: false),
                    group_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_loadout_group", x => x.profile_loadout_group_id);
                    table.ForeignKey(
                        name: "FK_profile_loadout_group_profile_role_loadout_profile_role_loadout_id",
                        column: x => x.profile_role_loadout_id,
                        principalTable: "profile_role_loadout",
                        principalColumn: "profile_role_loadout_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_loadout",
                columns: table => new
                {
                    profile_loadout_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    profile_loadout_group_id = table.Column<int>(type: "INTEGER", nullable: false),
                    loadout_name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_loadout", x => x.profile_loadout_id);
                    table.ForeignKey(
                        name: "FK_profile_loadout_profile_loadout_group_profile_loadout_group_id",
                        column: x => x.profile_loadout_group_id,
                        principalTable: "profile_loadout_group",
                        principalColumn: "profile_loadout_group_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_profile_loadout_profile_loadout_group_id",
                table: "profile_loadout",
                column: "profile_loadout_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_loadout_group_profile_role_loadout_id",
                table: "profile_loadout_group",
                column: "profile_role_loadout_id");

            migrationBuilder.CreateIndex(
                name: "IX_profile_role_loadout_profile_id",
                table: "profile_role_loadout",
                column: "profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profile_loadout");

            migrationBuilder.DropTable(
                name: "profile_loadout_group");

            migrationBuilder.DropTable(
                name: "profile_role_loadout");
        }
    }
}