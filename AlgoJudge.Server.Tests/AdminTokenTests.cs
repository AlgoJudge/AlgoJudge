using AlgoJudge.Server.Authorization;

namespace AlgoJudge.Server.Tests;

/// <summary>
/// The administrative surface refuses the token this product publishes.
/// <para>
/// <c>admin-token-development-only</c> is in a compose file in a public
/// repository. The start warned about it and served the surface anyway until
/// 2026-09-09; the peer address beside it is not a second factor wherever a
/// proxy on the same host reaches the Server over loopback, so the token was the
/// whole of the control and its value was published.
/// </para>
/// <para>
/// Asserted against the rule rather than over HTTP because
/// <see cref="ServerFixture"/> runs in Development, where the value is allowed
/// on purpose: a development stack that cannot take itself off the air teaches
/// nobody anything.
/// </para>
/// </summary>
public class AdminTokenTests
{
    [Fact]
    public void The_published_development_token_closes_the_surface_outside_development()
    {
        Assert.False(AdminSurface.Usable(AdminSurface.DevelopmentToken, development: false));
    }

    [Fact]
    public void And_is_allowed_in_development_where_it_is_the_point()
    {
        Assert.True(AdminSurface.Usable(AdminSurface.DevelopmentToken, development: true));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Nothing_configured_closes_it_in_either_environment(string? configured)
    {
        Assert.False(AdminSurface.Usable(configured, development: false));
        Assert.False(AdminSurface.Usable(configured, development: true));
    }

    [Fact]
    public void A_secret_of_the_operators_own_opens_it()
    {
        Assert.True(AdminSurface.Usable("a-token-nobody-else-has", development: false));
    }
}
