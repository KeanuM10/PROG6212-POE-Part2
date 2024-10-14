using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace ClaimSystem
{
    public class Startup
    {
        // Method to configure
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSession(); // Enable session
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
