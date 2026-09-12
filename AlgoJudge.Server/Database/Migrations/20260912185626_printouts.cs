using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgoJudge.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class Printouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_FileReferences_OwnerKindMatches",
                table: "FileReferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FileReferences_SingleOwner",
                table: "FileReferences");

            migrationBuilder.AddColumn<Guid>(
                name: "PrintoutId",
                table: "FileReferences",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPrintouts",
                table: "Activities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Printouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestedByUserId = table.Column<string>(type: "text", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FileName = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                    Sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    ResolvedByUserId = table.Column<string>(type: "text", nullable: true),
                    SourceDisposedAt = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Printouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Printouts_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Printouts_ActivityGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ActivityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Printouts_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Printouts_AspNetUsers_ResolvedByUserId",
                        column: x => x.ResolvedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Printouts_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileReferences_PrintoutId",
                table: "FileReferences",
                column: "PrintoutId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FileReferences_OwnerKindMatches",
                table: "FileReferences",
                sql: "(\"OwnerKind\" = 0 AND \"ProblemVersionId\" IS NOT NULL) OR (\"OwnerKind\" = 1 AND \"ActivityId\" IS NOT NULL) OR (\"OwnerKind\" = 2 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 3 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 4 AND \"RunnerId\" IS NOT NULL) OR (\"OwnerKind\" = 5 AND \"SubmissionId\" IS NOT NULL) OR (\"OwnerKind\" = 6 AND \"EvaluationJobId\" IS NOT NULL) OR (\"OwnerKind\" = 7 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 8 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 9 AND \"PrintoutId\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FileReferences_SingleOwner",
                table: "FileReferences",
                sql: "num_nonnulls(\"ProblemVersionId\", \"ActivityId\", \"SubmissionId\", \"EvaluationJobId\", \"RunnerId\", \"InstanceId\", \"PrintoutId\") = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Printouts_ActivityId_RequestedAt",
                table: "Printouts",
                columns: new[] { "ActivityId", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Printouts_GroupId",
                table: "Printouts",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Printouts_RequestedByUserId",
                table: "Printouts",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Printouts_ResolvedByUserId",
                table: "Printouts",
                column: "ResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Printouts_State_RequestedAt",
                table: "Printouts",
                columns: new[] { "State", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Printouts_SubmissionId",
                table: "Printouts",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileReferences_Printouts_PrintoutId",
                table: "FileReferences",
                column: "PrintoutId",
                principalTable: "Printouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileReferences_Printouts_PrintoutId",
                table: "FileReferences");

            migrationBuilder.DropTable(
                name: "Printouts");

            migrationBuilder.DropIndex(
                name: "IX_FileReferences_PrintoutId",
                table: "FileReferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FileReferences_OwnerKindMatches",
                table: "FileReferences");

            migrationBuilder.DropCheckConstraint(
                name: "CK_FileReferences_SingleOwner",
                table: "FileReferences");

            migrationBuilder.DropColumn(
                name: "PrintoutId",
                table: "FileReferences");

            migrationBuilder.DropColumn(
                name: "HasPrintouts",
                table: "Activities");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FileReferences_OwnerKindMatches",
                table: "FileReferences",
                sql: "(\"OwnerKind\" = 0 AND \"ProblemVersionId\" IS NOT NULL) OR (\"OwnerKind\" = 1 AND \"ActivityId\" IS NOT NULL) OR (\"OwnerKind\" = 2 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 3 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 4 AND \"RunnerId\" IS NOT NULL) OR (\"OwnerKind\" = 5 AND \"SubmissionId\" IS NOT NULL) OR (\"OwnerKind\" = 6 AND \"EvaluationJobId\" IS NOT NULL) OR (\"OwnerKind\" = 7 AND \"InstanceId\" IS NOT NULL) OR (\"OwnerKind\" = 8 AND \"InstanceId\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_FileReferences_SingleOwner",
                table: "FileReferences",
                sql: "num_nonnulls(\"ProblemVersionId\", \"ActivityId\", \"SubmissionId\", \"EvaluationJobId\", \"RunnerId\", \"InstanceId\") = 1");
        }
    }
}
