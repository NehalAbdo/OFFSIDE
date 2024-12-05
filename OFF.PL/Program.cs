using Demo_PL.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OFF.BLL.Interfaces;
using OFF.BLL.Repository;
using OFF.DAL.Context;
using OFF.DAL.Model;
using OFF.PL.Utility;
using Stripe;
using System.Reflection;
namespace OFF.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<CompanyDbContext>();
            builder.Services.AddDbContext<CompanyDbContext>
              (options => {
                  options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
              });
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<ISubscriptionService, Utility.SubscriptionService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddHostedService<SubscriptionExpiryService>();



            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddScoped<IMailService, EmailService>();
            StripeConfiguration.ApiKey = builder.Configuration["StripeSetting:SecretKey"];



            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
            })
              .AddEntityFrameworkStores<CompanyDbContext>()
              .AddDefaultTokenProviders();
           builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomUserClaims>();



            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                if (app.Environment.IsDevelopment())
                {
                    // Seed admin user and roles only in the development environment
                    await IdentityContextSeedcs.SeedUserAsync(userManager, roleManager);

                    // Verify user addition
                    var user = await userManager.FindByNameAsync("NehalAbdelrahman");
                    if (user != null)
                    {
                        logger.LogInformation("Admin user 'NehalAbdelrahman' has been successfully added to the database.");
                    }
                    else
                    {
                        logger.LogError("Admin user 'NehalAbdelrahman' was not found in the database.");
                    }

                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }
            }
           
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}