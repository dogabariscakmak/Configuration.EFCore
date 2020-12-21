using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Configuration.EFCore
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        ///     Adds the EFCore DbContext configuration provider with DbContextOptionsBuilder to builder.
        /// </summary>
        /// <typeparam name="TDbContext">DbContext type that contains setting values.</typeparam>
        /// <param name="builder">The Microsoft.Extensions.Configuration.IConfigurationBuilder to add to.</param>
        /// <param name="optionsAction">DbContextOptionsBuilder used to create related DbContext.</param>
        /// <param name="reloadOnChange">Auto reaload when setting values changed.</param>
        /// <param name="pollingInterval">Polling database interval in milliseconds.</param>
        /// <param name="environment">Environment name.</param>
        /// <param name="application">Application name.</param>
        /// <param name="onLoadException">Error callback for exeption when re/load settings from database.</param>
        /// <returns>The Microsoft.Extensions.Configuration.IConfigurationBuilder.</returns>
        public static IConfigurationBuilder AddEFCoreConfiguration<TDbContext>(this IConfigurationBuilder builder,
                                                                                Action<DbContextOptionsBuilder> optionsAction,
                                                                                bool reloadOnChange = false,
                                                                                int pollingInterval = 5000,
                                                                                string environment = "",
                                                                                string application = "",
                                                                                Action<ConfigurationEFCoreLoadExceptionContext<TDbContext>> onLoadException = null)
            where TDbContext : DbContext
        {
            return builder.Add(new EFCoreConfigurationSource<TDbContext>(optionsAction, reloadOnChange, pollingInterval, environment, application, onLoadException));
        }
    }
}
