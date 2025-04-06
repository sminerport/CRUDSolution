using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.PersonsListResultFilter;

using Entities;

using Microsoft.EntityFrameworkCore;

using Repositories;

using RepositoryContracts;

using ServiceContracts;

using Services;

namespace CRUDExample
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            // Add services to the IoC container
            services.AddTransient<ResponseHeaderActionFilter>();

            services.AddControllersWithViews(options =>
            {
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                //options.Filters.Add<ResponseHeaderActionFilter>();

                options.Filters.Add(new ResponseHeaderActionFilter(services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>()) { Key = "X-From-Global-Filter", Value = "Custom-Global-Value", Order = 2 });
            });

            // add services into IoC container
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddHttpLogging();

            if (!webHostEnvironment.IsEnvironment("Test"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                });
            }

            services.AddTransient<PersonsListResultFilter>();

            return services;
        }
    }
}