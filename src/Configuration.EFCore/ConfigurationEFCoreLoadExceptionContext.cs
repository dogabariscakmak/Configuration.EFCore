using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Configuration.EFCore
{
    public sealed class ConfigurationEFCoreLoadExceptionContext<TDbContext>
        where TDbContext : DbContext
    {
        public Exception Exception { get; }
        public bool Ignore { get; set; }
        public EFCoreConfigurationSource<TDbContext> Source { get; }

        internal ConfigurationEFCoreLoadExceptionContext(EFCoreConfigurationSource<TDbContext> source, Exception exception)
        {
            Source = source;
            Exception = exception;
        }
    }
}
