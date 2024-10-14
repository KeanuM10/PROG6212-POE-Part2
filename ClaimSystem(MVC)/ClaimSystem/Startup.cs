using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClaimSystem
{
    public class Startup
    {
        // Method to configure
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(); 
        }

        // Method to configure 
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
