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
        }
    }
}
