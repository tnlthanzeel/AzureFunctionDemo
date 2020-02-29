using AutoMapper;
using Bread2Bun.Data;
using Bread2Bun.Service;
using Bread2Bun.Service.Country;
using Bread2Bun.Service.Country.Interface;
using EventHosted;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace EventHosted
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddAutoMapper(new MappingProfile().GetType().Assembly);

            builder.Services.AddDbContext<Bread2BunContext>(cfg =>
            {
                cfg.UseSqlServer(Environment.GetEnvironmentVariable("SQLCONNSTR_ConnectionString"));
            });
        }
    }
}
