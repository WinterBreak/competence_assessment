using CompetenceAssessment.Infrastructure;
using CompetenceAssessment.Infrastructure.UserManagement;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

// TODO LazyProxies
var connectionString = builder.Configuration.GetConnectionString("CompetenceAssessment");
builder.Services.AddDbContext<UserManagementContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDbContext<AssessmentContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categories}/{action=Index}/{id?}");
    
app.MapControllers();

app.Run();