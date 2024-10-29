using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmpowerU.Models;
using EmpowerU.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext to the service container
builder.Services.AddDbContext<EmpowerUContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with matching types
builder.Services.AddIdentity<User, IdentityRole<int>>() // Ensure role type is int if User is int
    .AddEntityFrameworkStores<EmpowerUContext>() // Use your DbContext
    .AddDefaultTokenProviders(); // Optional: for token providers

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Create a scope to get services
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Create roles if they don't exist
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = services.GetRequiredService<UserManager<User>>();

    await CreateRoles(roleManager, userManager);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use Authentication and Authorization
app.UseAuthentication(); // Ensure this is before UseAuthorization
app.UseAuthorization();

// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Method to create roles and a default admin user
async Task CreateRoles(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
{
    // Define the roles
    var roles = new[] { "Business", "Consumer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            // Create the role if it doesn't exist
            await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
        }
    }
}
