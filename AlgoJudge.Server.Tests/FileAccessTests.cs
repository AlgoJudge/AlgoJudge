using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AlgoJudge.Server.Database;
using AlgoJudge.Server.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace AlgoJudge.Server.Tests;

/// <summary>
/// Who may read a stored file, addressed by its id.
/// <para>
/// <b>`/files/{id}` is the one address the screens do not control.</b> Every
/// other read arrives through a route that names an activity, a submission or a
/// problem, and is narrowed by it; this one names bytes. Three holes found by
/// the authorization audit of 2026-09-09 were all of that shape — a rule the
/// screens apply and this address did not.
/// </para>
/// </summary>
[Collection("server-3")]
public class FileAccessTests(ServerFixture server)
{
    private static async Task<HttpClient> AdminAsync(ServerFixture server) =>
        await Sign.InAsync(server, Seeder.DevAdminLogin, Seeder.DevAdminPassword);

    /// <summary>
    /// A manager-scope file on the version an activity is judging.
    /// <para>
    /// Written straight onto the pinned version rather than published through a
    /// second version, because what is under test is the <b>read</b> rule and
    /// pinning would take the new version out of the job's reach.
    /// </para>
    /// </summary>
    private async Task<Guid> ModelSolutionOnAsync(string activitySlug)
    {
        await using var context = server.NewContext();
        var assignment = await context.SeriesProblems
            .Include(sp => sp.Activity)
            .FirstAsync(sp => sp.Activity!.Slug == activitySlug);

        var file = new Database.Models.File
        {
            Id = Guid.NewGuid(),
            Name = "model.cpp",
            MimeType = "text/plain",
            SizeBytes = 12,
            Sha256 = new string('a', 64),
            StorageId = "pg",
        };
        context.Files.Add(file);
        context.FileReferences.Add(new FileReference
        {
            FileId = file.Id,
            OwnerKind = FileOwnerKind.ProblemVersion,
            ProblemVersionId = assignment.PinnedProblemVersionId,
            Scope = FileScope.Manager,
            Name = "model.cpp",
        });
        await context.SaveChangesAsync();
        return file.Id;
    }

    /// <summary>
    /// <b>A Runner judging a problem may not read its model solution.</b>
    /// `FileService.CanReadProblemVersionAsync` says manager scope is "never a
    /// participant, never a Runner"; `RunnerService.MayReadAsync` matched the
    /// version and never the scope, so anything judging a problem could fetch
    /// the answer by id.
    /// </summary>
    [Fact]
    public async Task A_runner_judging_a_problem_may_not_read_its_model_solution()
    {
        var (slug, _) = await Build.ActivityAsync(server);
        var participant = await Build.ParticipantAsync(server, slug);
        var submitted = await Build.SubmitAsync(participant, slug, "print(1)\n");

        var model = await ModelSolutionOnAsync(slug);

        var runner = await Build.RunnerAsync(server);
        await runner.ClaimUntilAsync(submitted.GetProperty("id").GetString()!);

        var read = await runner.Client.GetAsync($"/api/v1/runner/files/{model}");

        Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);
    }

    /// <summary>And it still reads the package, which is what it is there for.</summary>
    [Fact]
    public async Task And_still_reads_the_package_it_was_given_a_job_for()
    {
        var (slug, _) = await Build.ActivityAsync(server);
        var participant = await Build.ParticipantAsync(server, slug);
        var submitted = await Build.SubmitAsync(participant, slug, "print(1)\n");

        var package = await Build.PackageIdOfAsync(server, slug);

        var runner = await Build.RunnerAsync(server);
        await runner.ClaimUntilAsync(submitted.GetProperty("id").GetString()!);

        var read = await runner.Client.GetAsync($"/api/v1/runner/files/{package}");

        Assert.Equal(HttpStatusCode.OK, read.StatusCode);
    }

    /// <summary>
    /// <b>A statement is not readable by file id while its round has not
    /// opened.</b> `ProblemService` treats `ISeriesGate.MayReadProblems` as the
    /// rule for whether a statement may be sent at all; the file address applied
    /// the lockdown and never the gate, so a round that never opened disclosed
    /// what it holds to anybody who could name the file.
    /// </summary>
    [Fact]
    public async Task A_statement_is_not_readable_while_its_round_has_not_opened()
    {
        var (slug, roundId) = await Build.ActivityAsync(server);
        var participant = await Build.ParticipantAsync(server, slug);

        Guid statement;
        await using (var context = server.NewContext())
        {
            var assignment = await context.SeriesProblems
                .Include(sp => sp.Activity)
                .FirstAsync(sp => sp.Activity!.Slug == slug);
            statement = (await context.FileReferences
                .FirstAsync(r => r.ProblemVersionId == assignment.PinnedProblemVersionId
                    && r.Scope == FileScope.Participant)).FileId;

            // Back to never opened, the way a round waiting for the scheduler is.
            var round = await context.Series.FirstAsync(s => s.Id == Guid.Parse(roundId));
            round.IsOpen = false;
            round.StartAnnouncedAt = null;
            await context.SaveChangesAsync();
        }

        var read = await participant.GetAsync($"/api/v1/files/{statement}");

        Assert.Equal(HttpStatusCode.NotFound, read.StatusCode);
    }

    /// <summary>
    /// <b>Attaching a file is a read grant, so the caller must be able to read
    /// it.</b> Existence was the whole test, which turned `problem:update` into
    /// a way to read anything by attaching its id to one's own version and then
    /// fetching it.
    /// </summary>
    [Fact]
    public async Task A_file_the_caller_cannot_read_may_not_be_attached()
    {
        var (slug, _) = await Build.ActivityAsync(server);
        var admin = await AdminAsync(server);

        // Somebody else's upload: unreferenced, so only its uploader may read it.
        var participant = await Build.ParticipantAsync(server, slug);
        var theirs = await Build.UploadAsync(participant, "/api/v1/files", "notes.md", "not yours\n");

        Guid problemId;
        await using (var context = server.NewContext())
        {
            problemId = (await context.SeriesProblems
                .Include(sp => sp.Activity)
                .FirstAsync(sp => sp.Activity!.Slug == slug)).ProblemId;
        }

        // **A version this Server would otherwise publish.** Its own statement,
        // its own config: the only thing wrong with it is the attached file, so
        // a refusal can only be about that. Written with an empty statement list
        // first, which was refused for being empty — the test passed against the
        // unfixed Server, measured 2026-09-09, which is the whole reason this
        // one names the code it expects.
        var mine = await Build.UploadAsync(admin, "/api/v1/files", "content.md", "# Mine\n");
        var published = await admin.PostAsJsonAsync($"/api/v1/problems/{problemId}/versions", new
        {
            statements = new[] { new { fileId = mine } },
            config = new { format = "standard-io", version = 1, limits = new { timeMs = 1000, memoryBytes = 268435456 } },
            files = new[] { new { fileId = theirs, name = "stolen.md", scope = "participant" } },
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, published.StatusCode);
        var refusal = await published.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("file.missing", refusal.GetProperty("code").GetString());
    }
}
