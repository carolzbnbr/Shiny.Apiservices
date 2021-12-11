﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shiny.Api.Push.Ef.Infrastructure;


namespace Shiny.Api.Push.Ef
{
    public static class Extension
    {
        public static PushConfigurator UseEfRepository<TDbContext>(this PushConfigurator config) where TDbContext : DbContext, IPushDbContext
        {
            config.Services.AddScoped<IPushDbContext>(sp => sp.GetRequiredService<TDbContext>());
            return config.UseRepository<EfRepository>();
        }
    }
}
