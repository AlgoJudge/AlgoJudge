using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgoJudge.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class InstanceShowHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // `true`, and the generator's `false` was wrong twice over. The
            // column is NOT NULL on a table that already holds the singleton
            // row, so PostgreSQL needs a value to backfill it with — and the
            // value has to be the one the CLR initializer states, or an
            // installation that upgrades has the introduction switched off by a
            // migration rather than by anybody's decision.
            //
            // This leaves a column default behind, the way `Releases` and
            // `Refunds` do. It is inert: nothing declares `HasDefaultValue`, so
            // EF writes the property on every insert and never omits it.
            migrationBuilder.AddColumn<bool>(
                name: "ShowHero",
                table: "Instance",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowHero",
                table: "Instance");
        }
    }
}
