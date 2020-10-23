using BuildingBlock.CacheDistribuido.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BuildingBlock.CacheDistribuido.Configurations
{
    public static class Configurations
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration?["REDIS_INSTANCE_NAME"]))
                throw new ArgumentException("O parametro REDIS_INSTANCE_NAME está null ou empty");

            if (string.IsNullOrEmpty(configuration?["REDIS_CONFIGURATION_URL"]))
                throw new ArgumentException("O parametro REDIS_CONFIGURATION_URL está null ou empty");
            
            if (string.IsNullOrEmpty(configuration?["REDIS_TIME_EXPIRED_CACHED"]))
                throw new ArgumentException("O parametro REDIS_TIME_EXPIRED_CACHED está null ou empty");

            string appName = configuration["REDIS_INSTANCE_NAME"];

            services.AddSingleton<IDistributedCaching, DistributedCaching>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["REDIS_CONFIGURATION_URL"];
                options.InstanceName = appName;
            });

            return services;
        }
    }
}
