using AcademyERP.Admin.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AcademyERP.Admin.Authentication;

public class JwtAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;

    public JwtAuthenticationStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            
            var token = await _tokenService.GetTokenAsync();

Console.WriteLine("TOKEN:");
Console.WriteLine(token);

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(
                    new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(token);
            Console.WriteLine("CLAIMS:");

foreach (var claim in jwt.Claims)
{
    Console.WriteLine($"{claim.Type} = {claim.Value}");
}

           var identity = new ClaimsIdentity(
    jwt.Claims,
    "jwt",
    ClaimTypes.Name,
    ClaimTypes.Role);

            return new AuthenticationState(
                new ClaimsPrincipal(identity));
        }
        catch
        {
            // Happens during prerender because JavaScript is unavailable.
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
    public void NotifyUserAuthentication()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}