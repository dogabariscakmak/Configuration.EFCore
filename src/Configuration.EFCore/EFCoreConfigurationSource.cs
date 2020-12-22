using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Configuration.EFCore
{
    public class EFCoreConfigurationSource<TDbContext> : IConfigurationSource
        where TDbContext : DbContext
    {
        public readonly Action<DbContextOptionsBuilder> OptionsAction;
        public readonly bool ReloadOnChange;
        public readonly int PollingInterval;
        public readonly string Environment;
        public readonly string ApplicationContext;
        public readonly Action<ConfigurationEFCoreLoadExceptionContext<TDbContext>>? OnLoadException;

        public EFCoreConfigurationSource(Action<DbContextOptionsBuilder> optionsAction, 
                                         bool reloadOnChange = false, 
                                         int pollingInterval = 500, 
                                         string environment = "", 
                                         string applicationContext = "",
                                         Action<ConfigurationEFCoreLoadExceptionContext<TDbContext>> onLoadException = null)
        {
            if (pollingInterval < 500)
            {
                throw new ArgumentException($"{nameof(pollingInterval)} can not less than 500.", nameof(pollingInterval));
            }
            OptionsAction = optionsAction;
            ReloadOnChange = reloadOnChange;
            PollingInterval = pollingInterval;
            Environment = environment;
            ApplicationContext = applicationContext;
            OnLoadException = onLoadException;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EFCoreConfigurationProvider<TDbContext>(this);
        }
    }
}
