using DisasterReadyIndia.Data;
using DisasterReadyIndia.Hubs;
using DisasterReadyIndia.Middleware;
using DisasterReadyIndia.Models.Entities;
using DisasterReadyIndia.Services;
using DisasterReadyIndia.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ── SERILOG BOOTSTRAP LOGGER ─────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting DisasterReady India application...");

    var builder = WebApplication.CreateBuilder(args);

    // ── SERILOG ───────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext()
                     .WriteTo.Console()
                     .WriteTo.File("logs/disasterready-.log", rollingInterval: RollingInterval.Day));

    // ── DATABASE ──────────────────────────────────────────────────────────────
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            options.UseSqlite(connectionString);
        else
            options.UseSqlServer(connectionString);
    });

    // ── IDENTITY ──────────────────────────────────────────────────────────────
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // ── AUTH COOKIE ───────────────────────────────────────────────────────────
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // ── SERVICES ──────────────────────────────────────────────────────────────
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IAuditService, AuditService>();
    builder.Services.AddScoped<IIncidentService, IncidentService>();
    builder.Services.AddScoped<IAlertService, AlertService>();
    builder.Services.AddScoped<IResourceService, ResourceService>();
    builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
    builder.Services.AddScoped<IPreparednessService, PreparednessService>();
    builder.Services.AddScoped<IFamilyPlanService, FamilyPlanService>();
    builder.Services.AddScoped<IDisasterAssistantService, DisasterAssistantService>();

    // ── MVC + SIGNALR ─────────────────────────────────────────────────────────
    builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
    });
    builder.Services.AddSignalR();

    // ── SESSION ───────────────────────────────────────────────────────────────
    builder.Services.AddDistributedMemoryCache();
    // Keep development authentication keys inside the project. This avoids
    // relying on a user-profile key ring that may be unavailable in a demo
    // environment and keeps local sign-in sessions stable between restarts.
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(
            Path.Combine(builder.Environment.ContentRootPath, "data-protection-keys")));
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    var app = builder.Build();

    // ── MIDDLEWARE PIPELINE ───────────────────────────────────────────────────
    if (!app.Environment.IsDevelopment())
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSerilogRequestLogging();

    // ── ROUTES ────────────────────────────────────────────────────────────────
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapHub<DisasterHub>("/hubs/disaster");

    // ── DATABASE SEED ─────────────────────────────────────────────────────────
    await DatabaseSeeder.SeedAsync(app.Services, app.Configuration);

    Log.Information("DisasterReady India started successfully.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
