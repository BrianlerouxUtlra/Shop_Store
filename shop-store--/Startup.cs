using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Microsoft.EntityFrameworkCore;
//using ShoppingCartAPI.Data;
//using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Shop_store.Data;
using Microsoft.EntityFrameworkCore;
using Shop_store.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Shop_store.Config;

using Shop_store.Middleware;

namespace Shop_store.Controllers
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
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("Allow",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5083")
                                   .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .WithExposedHeaders("Content-Length", "ETag", "Link", "X-RateLimit-Limit", "X-RateLimit-Remaining");
                    });
            });


            services.AddHttpClient();
            services.AddSingleton<IRateLimitService, RateLimitService>();
            services.Configure<RateLimitingOptions>(Configuration.GetSection("RateLimiting"));
            services.AddScoped<ShoppingCartController>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IGoogleAuthenticationService, GoogleAuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddEndpointsApiExplorer();      
            services.AddHttpContextAccessor();       
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
            });



            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/account/login";
            })
            .AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });


            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),
             ServiceLifetime.Scoped);



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
       
            app.UseDeveloperExceptionPage();          
            app.UseHttpsRedirection();
            app.UseMiddleware<RateLimitingMiddleware>();      
            app.UseCors("Allow");
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Cart API V1");
                c.RoutePrefix = "swagger";

            });
            app.UseRouting();


            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(state =>
                {
                    var httpContext = (Microsoft.AspNetCore.Http.HttpContext)state;
                    if (!httpContext.Response.Headers.ContainsKey("Vary"))
                    {
                        httpContext.Response.Headers.Add("Vary", "Origin");
                    }

                    return Task.CompletedTask;
                }, context);

                await next.Invoke();
            });

            app.UseAuthorization();

            app.UseDeveloperExceptionPage();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseRequestLocalization();
        }
    }
}
