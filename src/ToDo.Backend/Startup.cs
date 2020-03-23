using System.Linq;
using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ToDo.Backend.Domain;
using ToDo.Backend.DTO;
using ToDo.Backend.DTO.Validators.Account;
using ToDo.Backend.Persistence;
using ToDo.Backend.Swagger;

namespace ToDo.Backend
{
    public sealed class Startup
    {
        private IWebHostEnvironment Environment { get; }
        
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddFluentValidation(c =>
                {
                    c.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>();
                })
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = c =>
                    {
                        var errors = from v in c.ModelState.Values
                            from e in v.Errors
                            select e.ErrorMessage;
                        return new BadRequestObjectResult(new ErrorResponse(errors));
                    };
                });

            services.AddAutoMapper(GetType().Assembly);

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "application/octet-stream"
                });
            });

            services.AddDbContext<AppDbContext>(options => options
                .UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

            services.AddIdentity<User, IdentityRole<long>>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Auth:JwtIssuer"],
                        ValidAudience = Configuration["Auth:JwtAudience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Auth:JwtSecurityKey"]))
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo
                {
                    Title = "ToDo API", 
                    Version = "v1.0"
                });
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. E.g. Authorization: Bearer 'token'"
                });
                
                c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                
                c.OperationFilter<ApiVersionOperationFilter>();
                c.OperationFilter<AuthOperationFilter>();
                c.OperationFilter<ApiJsonResponseOperationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseResponseCompression();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "ToDo API V1.0");
            });
            
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<Frontend.Program>();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToClientSideBlazor<Frontend.Program>("index.html");
            });
        }
    }
}