using System.Reflection;
using System.Security.Claims;
using CompetenceAssessment.Infrastructure;
using CompetenceAssessment.Infrastructure.Database;
using CompetenceAssessment.Infrastructure.UserManagement;
using CompetenceAssessment.Web.Attributes;
using CompetenceAssessment.Web.Handlers;
using CompetenceAssessment.Web.Startup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(c =>
//     c.SwaggerDoc("v1", new OpenApiInfo 
//     { 
//         Title = "Competence assessment", 
//         Version = "v1" 
//     }));
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "client/build";
});

var assemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    Assembly.Load("CompetenceAssessment.Domain"),
    Assembly.Load("CompetenceAssessment.Application"),
    Assembly.Load("CompetenceAssessment.Infrastructure"),
};

builder.Services.RegisterBySuffix( "Provider");
builder.Services.RegisterBySuffix("Repository");
builder.Services.RegisterBySuffix( "Service");
builder.Services.RegisterBySuffix( "Queries");

var connectionString = builder.Configuration.GetConnectionString("CompetenceAssessment");
builder.Services.AddDbContext<UserManagementContext>(options =>
    options.UseLazyLoadingProxies()
           .UseNpgsql(connectionString));
builder.Services.AddDbContext<AssessmentContext>(options =>
    options.UseLazyLoadingProxies()
           .UseNpgsql(connectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, AssessmentAuthorizationHandler>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.LoginPath = "/api/auth/login";
        options.LogoutPath = "/api/auth/logout";
        options.AccessDeniedPath = "/api/auth/access-denied";
        options.Cookie.Name = "AssessmentAuth";
        
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        };
    });

    
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy =>
        policy.Requirements.Add(new AssessmentAuthorizeAttribute()));
    options.AddPolicy("Admin", policy =>
        policy.Requirements.Add(new AssessmentAuthorizeAttribute("Администратор")));
    
    options.AddPolicy("Candidate", policy =>
        policy.Requirements.Add(new AssessmentAuthorizeAttribute("Аттестуемый")));
    
    options.AddPolicy("Inspector", policy =>
        policy.Requirements.Add(new AssessmentAuthorizeAttribute("Проверяющий, Аттестуемый")));
});



builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.Services.RunMigrations();
    app.MapOpenApi();
    // app.UseSwagger();
    // app.UseSwaggerUI(c => 
    // {
    //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Competence assessment V1");
    //     c.RoutePrefix = "swagger";
    // });
}

app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");
    
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "client";
        app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
        {
            appBuilder.UseSpa(spa =>
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
            });
        });
    });
}

app.Run();

public partial class Program { }