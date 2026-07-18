using Microsoft.JSInterop;

namespace AcademyERP.Admin.Services;

public class TokenService
{
    private readonly IJSRuntime _jsRuntime;

    public TokenService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync(
            "localStorage.setItem",
            "jwt",
            token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>(
            "localStorage.getItem",
            "jwt");
    }

    public async Task RemoveTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync(
            "localStorage.removeItem",
            "jwt");
    }
}