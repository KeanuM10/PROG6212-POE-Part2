using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ClaimSystem.Models;
using Microsoft.EntityFrameworkCore;
using ClaimSystem.Data;


namespace ClaimSystem
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Existing ConfigureServices code
            services.AddControllersWithViews();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add MySQL database context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 2))
                )
            );
        }

        // Method to configure 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Set culture of app - uses 'en-US' - this allows '.' to be the decimal operator
            var supportedCultures = new[] { new CultureInfo("en-US") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),  // Default culture set - "en-US"
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();  // Show errors
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");  // Go to error page in production
                app.UseHsts();  
            }

            app.UseHttpsRedirection();  // Redirect to HTTPS
            app.UseStaticFiles();  

            app.UseRouting();
            app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

            app.UseSession(); // Use session
            app.UseAuthorization();  

            // Routing for application
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");  // Way to Home controllers Index (action)
            });

            SeedUsers(app.ApplicationServices.GetRequiredService<ApplicationDbContext>());

        }

        private void SeedUsers(ApplicationDbContext context)
        {
            if (!context.Users.Any())
            {
                var users = new List<User>
        {
            new User { Username = "lecturer1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("lecturer1pass"), Role = "Lecturer" },
            new User { Username = "lecturer2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("lecturer2pass"), Role = "Lecturer" },
            new User { Username = "lecturer3", PasswordHash = BCrypt.Net.BCrypt.HashPassword("lecturer3pass"), Role = "Lecturer" },
            new User { Username = "manager", PasswordHash = BCrypt.Net.BCrypt.HashPassword("managerpass"), Role = "Manager" },
            new User { Username = "coordinator", PasswordHash = BCrypt.Net.BCrypt.HashPassword("coordinatorpass"), Role = "Coordinator" }
        };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }

    }
}
