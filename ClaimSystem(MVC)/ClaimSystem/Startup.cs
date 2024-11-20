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
            services.AddControllersWithViews();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 2))
                )
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var supportedCultures = new[] { new CultureInfo("en-US") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedUsers(app.ApplicationServices);
        }

        private void SeedUsers(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Users.Any())
                {
                    var users = new List<User>
                    {
                        new User { Username = "John", Password = "lecturer1pass", Role = "lecturer" },
                        new User { Username = "Rosa", Password = "lecturer2pass", Role = "lecturer" },
                        new User { Username = "Manager", Password = "manpass", Role = "admin" },
                        new User { Username = "Coordinator", Password = "copass", Role = "admin" }
                    };

                    context.Users.AddRange(users);
                    context.SaveChanges();
                    Console.WriteLine("Seed users added successfully.");
                }
                else
                {
                    Console.WriteLine("Users already exist. Skipping seeding.");
                }
            }
        }
    }
}
