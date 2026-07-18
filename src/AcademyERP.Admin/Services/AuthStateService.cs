namespace AcademyERP.Admin.Services;

public class AuthStateService
{
    private readonly TokenService _tokenService;

    public AuthStateService(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await _tokenService.GetTokenAsync();

        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task LogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
    }
}