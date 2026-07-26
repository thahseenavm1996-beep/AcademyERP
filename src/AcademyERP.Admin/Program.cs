using AcademyERP.Admin.Components;
using MudBlazor.Services;
using AcademyERP.Admin.Services;
using Microsoft.AspNetCore.Components.Authorization;
using AcademyERP.Admin.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMudServices();
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<JwtAuthorizationMessageHandler>();

    handler.InnerHandler = new HttpClientHandler();

    return new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5235/")
    };
});
builder.Services.AddScoped<StudentApiService>();
builder.Services.AddScoped<TeacherApiService>();
builder.Services.AddScoped<ParentApiService>();
builder.Services.AddScoped<ProgramApiService>();
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<TeacherDashboardApiService>();
builder.Services.AddScoped<CourseApiService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<DashboardApiService>();
builder.Services.AddScoped<ClassReportApiService>();
builder.Services.AddScoped<EnrollmentApiService>();
builder.Services.AddScoped<LookupApiService>();
builder.Services.AddScoped<AdmissionApplicationApiService>();
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<AuthenticationStateProvider,
    JwtAuthenticationStateProvider>();
/*builder.Services
.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie();*/
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
