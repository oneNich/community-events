using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CommunityEvents.Data;
using CommunityEvents.Services;

var builder = WebApplication.CreateBuilder(args);

var dbPath = builder.Environment.IsProduction()
    ? Path.Combine("/home/data", "community-events.db")
    : Path.Combine(builder.Environment.ContentRootPath, "app.db");

if (builder.Environment.IsProduction())
{
    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TaskDistributionService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ActivityLogService>();

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("OK"));

app.MapGet("/", () => Results.Ok("Community Events API running"));

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        System.Console.WriteLine($"Migration failed: {ex.Message}");
    }
}

app.Run();
