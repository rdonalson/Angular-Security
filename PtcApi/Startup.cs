using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using PtcApi.Model;

namespace PtcApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get JWT Token Settings from JwtSettings.json file
            JwtSettings settings = GetJwtSettings();

            // Create singleton of JwtSettings
            services.AddSingleton(settings);

            // Register Jwt as the Authentication service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            }).AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = settings.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(settings.MinutesToExpiration)
                };
            });
			
			// NOTE: The claim type and value are case-sensitive
			services.AddAuthorization(cfg => {
				cfg.AddPolicy("CanAccessProducts", p => p.RequireClaim("CanAccessProducts", "true"));
			});

            services.AddCors();

            services.AddMvc()
                .AddJsonOptions(options =>
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver()).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                options => options.WithOrigins(
                    "http://localhost:4200").AllowAnyMethod().AllowAnyHeader()
            );

            app.UseAuthentication();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }

        private JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings
            {
                Key = Configuration["JwtSettings:key"],
                Audience = Configuration["JwtSettings:issuer"],
                Issuer = Configuration["JwtSettings:audience"],
                MinutesToExpiration = Convert.ToInt32(Configuration["JwtSettings:minutesToExpiration"])
            };
            return settings;
        }
    }
}