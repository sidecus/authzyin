namespace sample
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Authentication;
    using AuthZyin.Authentication;
    using AuthZyin.Authorization;
    using sample.AuthN;

    /// <summary>
    /// Startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the Startup class
        /// </summary>
        /// <param name="configuration">configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration object
        /// </summary>
        /// <value></value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            // See sample.csproj about how to use custom publish tasks to copy these files over.
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            var authConfig = new AuthConfig();
            Configuration.GetSection(nameof(AuthConfig)).Bind(authConfig);

            // Add jwt bearer token authentication for web apis
            services.AddAadJwtBearer(authConfig.Authority, authConfig.AadAppId);
            
            services.AddAuthZyinAuthorization(options =>
            {
                options.AddPolicy(nameof(Policies.IsCustomer), Policies.IsCustomer);
                options.AddPolicy(nameof(Policies.CanDrinkAlchohol), Policies.CanDrinkAlchohol);
                options.AddPolicy(nameof(Policies.CanEnterBar), Policies.CanEnterBar);
                options.AddPolicy(nameof(Policies.MeetsAgeRangeLimit), Policies.MeetsAgeRangeLimit);
                options.AddPolicy(nameof(Policies.CanBuyDrink), Policies.CanBuyDrink);
            });

            // AuthZyin[sidecus]: Add scoped context, used for authorization on both server and client
            services.AddScoped<IAuthZyinContext, SampleAuthZyinContext>();

            // Add other services
            services.AddSingleton<IClaimsTransformation, ClaimsTransformer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "api/{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
