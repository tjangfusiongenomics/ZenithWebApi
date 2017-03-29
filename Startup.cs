using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZenithWebSite.Models;
using ZenithWebSite.Services;
using AspNet.Security.OpenIdConnect.Primitives;

namespace ZenithWebSite
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>{
            options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            options.UseOpenIddict();
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
            options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });


            services.AddOpenIddict(options =>
            {
            // Register the Entity Framework stores.
            options.AddEntityFrameworkCoreStores<ApplicationDbContext>();
            // Register the ASP.NET Core MVC binder used by OpenIddict.
            // Note: if you don't call this method, you won't be able to
            // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
            options.AddMvcBinders();
            // Enable the token endpoint.
            options.EnableTokenEndpoint("/connect/token");
            // Enable the password flow.
            options.AllowPasswordFlow();
            // During development, you can disable the HTTPS requirement.
            options.DisableHttpsRequirement();
            });

            var connection = Configuration["Data:DefaultConnection:ConnectionString"];
            services.AddDbContext<ZenithContext>(options => options.UseSqlite(connection));

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseOAuthValidation();
            app.UseOpenIddict();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvcWithDefaultRoute();
        }
    }
}
