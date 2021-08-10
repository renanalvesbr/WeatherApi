using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherMvc.Services;

namespace WeatherMvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("cookie")
            .AddOpenIdConnect("oidc", options => 
            {
                options.Authority = Configuration["InteractiveServiceSettings:AuthorityUrl"];
                options.ClientId = Configuration["InteractiveServiceSettings:ClientId"];
                options.ClientSecret = Configuration["InteractiveServiceSettings:ClientSecret"];

                options.ResponseType = "code";
                options.UsePkce = true;
                options.ResponseMode = "query";

                options.Scope.Add(Configuration["InteractiveServiceSettings:Scopes:0"]);
                options.SaveTokens = true;
            });

            services.Configure<IdentityServerSettings>(Configuration.GetSection("IdentityServerSettings"));
            services.AddSingleton<ITokenService, TokenService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
