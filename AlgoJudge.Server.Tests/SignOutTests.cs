using System.Net;
using System.Net.Http.Json;
using AlgoJudge.Server.Database;
using AlgoJudge.Server.Database.Models;
using AlgoJudge.Server.Realtime;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlgoJudge.Server.Tests;

/// <summary>
/// What signing out ends, which is three things and used to be one.
/// <para>
/// The authentication cookie was cleared and nothing else was: the row the
/// sessions screen reads stayed open, and the <c>aj_session</c> cookie that
/// names it stayed in the browser. Because the row was found by its id alone,
/// the next account signed in on that browser was recorded against the previous
/// account's session — the second person got no row of their own, and the first
/// was shown one on their sessions screen that was somebody else at the
/// keyboard.
/// </para>
/// </summary>
[Collection("server-1")]
public class SignOutTests(ServerFixture server)
{
    [Fact]
    public async Task Signing_out_ends_the_session_row()
    {
        var login = "signout-" + Guid.NewGuid().ToString("N")[..8];
        var client = await Sign.NewAccountAsync(server, login);
        var userId = await UserIdAsync(login);

        // A request of any kind, because the row is opened by the middleware
        // that watches them rather than by the sign-in.
        (await client.GetAsync("/api/v1/account")).EnsureSuccessStatusCode();
        Assert.Single(await OpenAsync(userId));

        (await client.PostAsync("/api/v1/identity/logout", null)).EnsureSuccessStatusCode();

        Assert.Empty(await OpenAsync(userId));
    }

    /// <summary>
    /// And it does not open another on its way out.
    ///
    /// <para>
    /// This middleware does its work <b>after</b> the request has been handled,
    /// so on a sign-out it runs once the row is closed and the cookie gone,
    /// sees a principal that is still on the request, finds no open row for it
    /// and opens one — handing the browser a fresh session cookie in the same
    /// response that was supposed to take one away.
    /// </para>
    ///
    /// <para>
    /// <b>The throttle has to be defeated to see it.</b> A session touched
    /// within the minute makes the middleware return before it decides
    /// anything, so a test that signs in and straight out never reaches the
    /// branch, and stays green with the guard removed. Measured: it did.
    /// </para>
    /// </summary>
    [Fact]
    public async Task Signing_out_does_not_open_another_row_on_its_way_out()
    {
        var login = "signout-" + Guid.NewGuid().ToString("N")[..8];
        var client = await Sign.NewAccountAsync(server, login);
        var userId = await UserIdAsync(login);

        (await client.GetAsync("/api/v1/account")).EnsureSuccessStatusCode();
        var open = await OpenAsync(userId);
        Assert.Single(open);
        SessionTrackingMiddleware.Forget(open[0].Id);

        (await client.PostAsync("/api/v1/identity/logout", null)).EnsureSuccessStatusCode();

        Assert.Empty(await OpenAsync(userId));
    }

    /// <summary>
    /// And the cookie goes with it, or the next sign-in in this browser arrives
    /// carrying the name of a row that has been closed.
    /// </summary>
    [Fact]
    public async Task Signing_out_removes_the_cookie_that_names_the_row()
    {
        var login = "signout-" + Guid.NewGuid().ToString("N")[..8];
        var client = await Sign.NewAccountAsync(server, login);
        (await client.GetAsync("/api/v1/account")).EnsureSuccessStatusCode();

        var response = await client.PostAsync("/api/v1/identity/logout", null);
        response.EnsureSuccessStatusCode();

        var removed = response.Headers.GetValues("Set-Cookie")
            .FirstOrDefault(c => c.StartsWith("aj_session="));
        Assert.NotNull(removed);
        // Emptied and dated to the epoch, which is the whole of what deleting a
        // cookie is.
        Assert.StartsWith("aj_session=;", removed!);
        Assert.Contains("1970", removed, StringComparison.Ordinal);
    }

    /// <summary>
    /// The half that made the other two matter: a session cookie left over from
    /// somebody else buys nothing.
    /// <para>
    /// Held with the cookie sent by hand rather than by signing two accounts in
    /// one after another, because the point is precisely a cookie that should
    /// not have survived — and after the fix above it does not survive, so the
    /// situation has to be built rather than reached.
    /// </para>
    /// </summary>
    [Fact]
    public async Task A_session_cookie_from_another_account_starts_a_row_of_its_own()
    {
        var theirs = await AccountAsync("signout-theirs-" + Guid.NewGuid().ToString("N")[..8]);
        var stale = await OpenRowAsync(theirs.Id);

        // Created but not signed in through the helper: signing in opens a row
        // of its own, and this test counts rows.
        var login = "signout-mine-" + Guid.NewGuid().ToString("N")[..8];
        var mine = (await AccountAsync(login, Sign.Password)).Id;

        // Cookies by hand, so this request can carry one the container would
        // never have kept.
        var client = server.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        var signedIn = await client.PostAsJsonAsync(
            "/api/v1/identity/login?useSessionCookies=true",
            new { email = login, password = Sign.Password });
        signedIn.EnsureSuccessStatusCode();

        var identity = signedIn.Headers.GetValues("Set-Cookie")
            .First(c => c.StartsWith(".AspNetCore.Identity.Application"))
            .Split(';')[0];

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/account");
        request.Headers.Add("Cookie", identity + "; aj_session=" + stale);
        (await client.SendAsync(request)).EnsureSuccessStatusCode();

        using var scope = server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Theirs is untouched: still theirs, still open, still never used.
        var row = await context.UserSessions.AsNoTracking().FirstAsync(s => s.Id == stale);
        Assert.Equal(theirs.Id, row.UserId);
        Assert.Null(row.EndedAt);
        Assert.Null(row.LastRequestPath);

        // And this account has one of its own, which is not that one.
        var opened = await OpenAsync(mine);
        Assert.Single(opened);
        Assert.NotEqual(stale, opened[0].Id);
    }

    // ── Getting there ────────────────────────────────────────────────────────

    private async Task<List<UserSession>> OpenAsync(string userId)
    {
        using var scope = server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.UserSessions.AsNoTracking()
            .Where(s => s.UserId == userId && s.EndedAt == null)
            .ToListAsync();
    }

    private async Task<string> UserIdAsync(string login)
    {
        using var scope = server.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await users.FindByNameAsync(login);
        Assert.NotNull(user);
        return user!.Id;
    }

    private async Task<User> AccountAsync(string login, string? password = null)
    {
        using var scope = server.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = new User
        {
            UserName = login,
            Email = login + "@example.invalid",
            EmailConfirmed = true,
            ApprovedAt = DateTime.UtcNow,
        };
        var created = password is null
            ? await users.CreateAsync(user)
            : await users.CreateAsync(user, password);
        Assert.True(created.Succeeded, string.Join("; ", created.Errors.Select(e => e.Description)));
        return user;
    }

    private async Task<Guid> OpenRowAsync(string userId)
    {
        using var scope = server.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var now = DateTime.UtcNow;
        var session = new UserSession
        {
            UserId = userId,
            StartedAt = now,
            LastRequestAt = now,
            ExpiresAt = now.AddDays(30),
            IpAddress = IPAddress.Parse("10.0.5.17"),
            UserAgent = "Mozilla/5.0 (the browser they left it in)",
        };
        context.UserSessions.Add(session);
        await context.SaveChangesAsync();
        return session.Id;
    }
}
