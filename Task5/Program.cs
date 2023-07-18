
using Azure.Core;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using RestEase;
using RestEase.HttpClientFactory;
using RestEase.Implementation;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using Task5.Models;
using Task5.Data;
using static IdentityModel.OidcConstants;
using Microsoft.Owin;
using Owin;
using Microsoft.Ajax.Utilities;

namespace Task5
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var client = new HttpClient();
            var responseToken = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:7020/connect/token",

                ClientId = "cwm.client",
                ClientSecret = "secret",
                Scope = "myapi"
            }).Result.AccessToken;

            builder.Services.AddRazorPages();

            // Add services to the container.
            builder.Services.AddControllersWithViews();



            var restClient = RestClient.For<IMovieApi>("https://localhost:7242", async (request, cancellationToken) =>
            {
                var auth = request.Headers.Authorization;
                if (auth != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, responseToken);
                }
            });

            builder.Services.AddSingleton(restClient);

            builder.Services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var serviceProvider = builder.Services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Data.ApplicationDbContext>();
                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }

            builder.Services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();
            builder.Services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<Data.ApplicationDbContext>()
               .AddSignInManager<SignInManager<ApplicationUser>>()
               .AddDefaultTokenProviders()
               .AddDefaultUI();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Movies}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "custom",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: "{controller=Movies}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}