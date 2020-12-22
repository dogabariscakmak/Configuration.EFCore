using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Configuration.EFCore
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddEFCoreConfiguration<TDbContext>(this IConfigurationBuilder builder,
                                                                                Action<DbContextOptionsBuilder> optionsAction,
                                                                                bool reloadOnChange = false,
                                                                                int pollingInterval = 500,
                                                                                string environment = "",
                                                                                string application = "",
                                                                                Action<ConfigurationEFCoreLoadExceptionContext<TDbContext>> onLoadException = null)
            where TDbContext : DbContext
        {
            return builder.Add(new EFCoreConfigurationSource<TDbContext>(optionsAction, reloadOnChange, pollingInterval, environment, application, onLoadException));
        }
    }
}
