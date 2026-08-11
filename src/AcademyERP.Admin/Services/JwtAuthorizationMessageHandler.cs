using System.Net.Http.Headers;

namespace AcademyERP.Admin.Services;

public class JwtAuthorizationMessageHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public JwtAuthorizationMessageHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /*protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }*/
    protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken)
    {
        Console.WriteLine("REQUEST:");
        Console.WriteLine(request.RequestUri);

        var token = await _tokenService.GetTokenAsync();

        Console.WriteLine($"TOKEN EXISTS: {!string.IsNullOrWhiteSpace(token)}");

        if (!string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine(token);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}