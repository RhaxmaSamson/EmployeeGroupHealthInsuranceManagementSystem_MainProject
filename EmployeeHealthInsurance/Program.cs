using EmployeeHealthInsurance.Data;
using EmployeeHealthInsurance.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;



var builder = WebApplication.CreateBuilder(args);

// Add this BEFORE any PDF generation happens (right after creating builder is fine)
QuestPDF.Settings.License = LicenseType.Community;
// Optional (prevents font glyph warnings):
QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext to the services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));




builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IPremiumCalculatorService, PremiumCalculatorService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>(); // New registration
builder.Services.AddScoped<IPolicyService, PolicyService>();



builder.Services.AddControllers(); // Use AddControllers() for API controllers
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Configure cookie paths for login and access denied
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Index";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    // Apply any pending EF Core migrations on startup
    await context.Database.MigrateAsync();

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}


// Seed the database with roles and a user (for development only)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages(); // Required for Identity UI pages
app.MapControllers();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Index}/{id?}");



app.Run();