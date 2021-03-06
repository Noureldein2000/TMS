using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Globalization;
using System.Text.Json.Serialization;
using TMS.Data;
using TMS.Services.BusinessLayer;
using TMS.Services.Repositories;
using TMS.Services.Services;
using TMS.Services.SOFClientAPIs;

namespace TMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(option =>
                option.UseSqlServer(connection)
            );

            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(ApplicationDbContext));
            services.AddScoped(typeof(Provider));
            services.AddScoped<IAccountsApi>(s => new AccountsApi(Configuration.GetValue<string>("Urls:SOF")));

            services.AddScoped<IDynamicService, DynamicService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IDenominationService, DenominationService>();
            services.AddScoped<IInquiryBillService, InquiryBillService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<ISwitchService>(x => new SwitchService(CurrentEnvironment.IsDevelopment()));
            services.AddScoped<IDbMessageService, DbMessageService>();
            services.AddScoped<IFeesService, FeesService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<ICommissionService, CommissionService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICancelService, CancelService>();
            services.AddScoped<IServiceProviderService, ServiceProviderService>();
            services.AddScoped<IServiceConfiguarationService, ServiceConfiguarationService>();
            services.AddScoped<IDenominationFeesService, DenominationFeesService>();
            services.AddScoped<IDenominationTaxService, DenominationTaxService>();
            services.AddScoped<IDenominationCommissionService, DenominationCommissionService>();
            services.AddScoped<IAccountFeesService, AccountFeesService>();
            services.AddScoped<IAccountCommissionService, AccountCommissionService>();
            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<IDenominationParamService, DenominationParamService>();
            services.AddScoped<IAccountTypeProfileDenominationService, AccountTypeProfileDenominationService>();
            services.AddScoped<IAccountTypeProfileFeeService, AccountTypeProfileFeeService>();
            services.AddScoped<IAccountTypeProfileCommissionService, AccountTypeProfileCommissionService>();
            services.AddScoped<ILookupTypeService, LookupTypeService>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedLanguages = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                options.SupportedCultures = supportedLanguages;
                options.SupportedUICultures = supportedLanguages;
            });

            services.AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
                )
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration.GetValue<string>("Urls:Authority");
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false
                    };

                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                //c.DocumentFilter<SwaggerAddEnumDescriptions>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
