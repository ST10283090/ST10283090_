using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using ST10283090_.Areas.Identity.Data;
using System.Globalization;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        var supportedCultures = new[]
        {
            new CultureInfo("en-US"),
            new CultureInfo("fr-FR") 
        };
         
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();
        app.MapRazorPages();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService
                <RoleManager<IdentityRole>>();

            var roles = new[] { "Academic Manager", "Programme Coordinator", "Lecturer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService
                <UserManager<IdentityUser>>();

            string email = "Academic@Manager.com";
            string password = "Academic!23";

            if(await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser();
                user.UserName = email;
                user.Email = email;

                await userManager.CreateAsync (user, password);

                await userManager.AddToRoleAsync(user, "Academic Manager");
            }

            string programmeEmail = "Programme@Coordinator.com";
            string programmePassword = "Programme!23";

            if (await userManager.FindByEmailAsync(programmeEmail) == null)
            {
                var programmeUser = new IdentityUser { UserName = programmeEmail, Email = programmeEmail };
                await userManager.CreateAsync(programmeUser, programmePassword);
                await userManager.AddToRoleAsync(programmeUser, "Programme Coordinator");
            }

            string lecturerEmail = "Lecturer@gmail.com";
            string lecturerPassword = "Lecturer!23";

            if (await userManager.FindByEmailAsync(lecturerEmail) == null)
            {
                var lecturerUser = new IdentityUser { UserName = lecturerEmail, Email = lecturerEmail };
                await userManager.CreateAsync(lecturerUser, lecturerPassword);
                await userManager.AddToRoleAsync(lecturerUser, "Lecturer");
            }
        }

        app.Run();
    }
}