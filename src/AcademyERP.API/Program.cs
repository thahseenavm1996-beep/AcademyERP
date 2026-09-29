using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using AcademyERP.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AcademyERP.Application.Services;
using AcademyERP.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using AcademyERP.API.Middleware;
using Serilog;
using Serilog.Events;
using AcademyERP.Application.Mappings;
using AcademyERP.Application.Interfaces;
using AcademyERP.Infrastructure.Repositories;
using AcademyERP.Application.Services.Pdf;
using QuestPDF.Infrastructure;
using AcademyERP.Application.Services.Reports;
using AcademyERP.Infrastructure.Services.Reports;
using AcademyERP.Application.Interfaces.TeacherStudentRequests;





var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddCors(options =>
{
    options.AddPolicy("WebsitePolicy", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["WebsiteBaseUrl"]!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 6;
options.Password.RequireDigit = true;
options.Password.RequireUppercase = true;
options.Password.RequireLowercase = true;
options.Password.RequireNonAlphanumeric = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ITeacherRegistrationService, TeacherRegistrationService>();
builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<IAdmissionApplicationService, AdmissionApplicationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ITeacherDashboardService, TeacherDashboardService>();
builder.Services.AddScoped<ITeachingScheduleService, TeachingScheduleService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClassProgressService, ClassProgressService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IClassReportService, ClassReportService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IFeeInvoiceService, FeeInvoiceService>();
builder.Services.AddScoped<ITeacherPaymentService, TeacherPaymentService>();
builder.Services.AddScoped<IAdmissionConversionService, AdmissionConversionService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ITeacherStudentRequestService, TeacherStudentRequestService>();

builder.Services.AddScoped<IStudentReportService, StudentReportService>();
builder.Services.AddScoped<TeacherReportService>();
builder.Services.AddScoped<StudentReportDashboardService>();
builder.Services.AddScoped<IAttendanceReportService, AttendanceReportService>();
builder.Services.AddScoped<IFinanceReportService, FinanceReportService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IParentPortalService, ParentPortalService>();
builder.Services.AddScoped<ITeacherPortalService, TeacherPortalService>();
builder.Services.AddScoped<IReportUnlockRequestService, ReportUnlockRequestService>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<
    IStudentPerformanceService,
    StudentPerformanceService>();
builder.Services.AddScoped<
    IFeePaymentService,
    FeePaymentService>();
    builder.Services.AddScoped<
    IFeePaymentSubmissionService,
    FeePaymentSubmissionService>();
builder.Services.AddScoped<
    IScheduledClassService,
    ScheduledClassService>();
builder.Services.AddScoped<
    IPaymentMethodService,
    PaymentMethodService>();
    builder.Services.AddScoped<
    IRegistrationRequestService,
    RegistrationRequestService>();
    builder.Services.AddScoped<
    ITimeSlotService,
    TimeSlotService>();
    builder.Services.AddScoped<
    IClassReportImageService,
    ClassReportImageService>();
    builder.Services.AddScoped<
    IClassDurationService,
    ClassDurationService>();
builder.Services.AddAutoMapper(typeof(ProgramMappingProfile).Assembly);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT ERROR: {context.Exception.Message}");
            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine("JWT Challenge");
            return Task.CompletedTask;
        }
    };
});
Console.WriteLine("Before Build");
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddScoped<AcademyERP.Application.Services.Reports.IBestPerformingStudentService, AcademyERP.Infrastructure.Services.BestPerformingStudentService>();
builder.Services.AddScoped<AcademyERP.Infrastructure.Services.Reports.BestPerformingStudentsPdfService>();
builder.Services.AddScoped<AcademyERP.Infrastructure.Services.Reports.BestPerformingStudentsImageService>();

builder.Services.AddScoped<AcademyERP.Application.Services.Reports.IBestPerformingFacultyService, AcademyERP.Infrastructure.Services.BestPerformingFacultyService>();
builder.Services.AddScoped<AcademyERP.Infrastructure.Services.Reports.BestPerformingFacultyPdfService>();
builder.Services.AddScoped<AcademyERP.Infrastructure.Services.Reports.BestPerformingFacultyImageService>();

var app = builder.Build();
Console.WriteLine("After Build");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<ApplicationDbContext>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    var roleManager =
        services.GetRequiredService<RoleManager<ApplicationRole>>();

    await DbInitializer.SeedAsync(
        context,
        userManager,
        roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("WebsitePolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    name = "MS International Academy API",
    status = "running"
}));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapControllers();
Console.WriteLine("Before app.Run()");
Console.WriteLine("Environment: " + app.Environment.EnvironmentName);

Console.WriteLine("URLs before Run:");
foreach (var url in app.Urls)
{
    Console.WriteLine(url);
}

app.Run();
