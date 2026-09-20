using Microsoft.JSInterop;

namespace AcademyERP.Admin.Services;

public class TokenService
{
    private readonly IJSRuntime _jsRuntime;

    private string? _token;

    public TokenService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveTokenAsync(string token)
    {
        // Keep the token in memory immediately.
        _token = token;

        // Persist it for browser refreshes.
        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            "jwt",
            token);
    }

    public async Task<string?> GetTokenAsync()
    {
        // After login, use the in-memory token.
        if (!string.IsNullOrWhiteSpace(_token))
            return _token;

        try
        {
            // On a fresh browser session, restore it from localStorage.
            _token = await _jsRuntime.InvokeAsync<string?>(
                "localStorage.getItem",
                "jwt");

            return _token;
        }
        catch (InvalidOperationException)
        {
            // JavaScript is unavailable during static rendering.
            return null;
        }
    }

    public async Task RemoveTokenAsync()
    {
        _token = null;

        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.removeItem",
                "jwt");
        }
        catch (InvalidOperationException)
        {
            // JavaScript may be unavailable during static rendering.
        }
    }
}