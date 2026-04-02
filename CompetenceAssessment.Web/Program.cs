using System.Reflection;
using CompetenceAssessment.Infrastructure;
using CompetenceAssessment.Infrastructure.Database;
using CompetenceAssessment.Infrastructure.UserManagement;
using CompetenceAssessment.Web.Startup;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Competence Assessment", 
        Version = "v1" 
    }));

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

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
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Competence Assessment V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");
    
app.MapControllers();

app.Run();