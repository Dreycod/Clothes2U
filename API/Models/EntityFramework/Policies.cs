using Microsoft.AspNetCore.Authorization;

namespace API.Models.EntityFramework;

public class Policies
{
    public const string Authorized = "Authorized";

    public static AuthorizationPolicy Logged()
    {
        return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Authorized).Build();
    }
}