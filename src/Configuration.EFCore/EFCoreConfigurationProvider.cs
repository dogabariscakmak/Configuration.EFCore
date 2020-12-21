using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Configuration.EFCore
{
    public class EFCoreConfigurationProvider<TDbContext> : ConfigurationProvider, IDisposable
        where TDbContext : DbContext
    {
        private readonly EFCoreConfigurationSource<TDbContext> _configurationSource;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private byte[] _lastComputedHash;
        private Task _watchDbTask;
        private bool _disposed;

        public EFCoreConfigurationProvider(EFCoreConfigurationSource<TDbContext> configurationSource)
        {
            _configurationSource = configurationSource;
            _cancellationTokenSource = new CancellationTokenSource();
            _lastComputedHash = new byte[20];
        }

        public override void Load()
        {
            if (_watchDbTask != null)
            {
                return;
            }

            try
            {
                Data = GetData();
                _lastComputedHash = ComputeHash(Data);
            }
            catch (Exception ex)
            {
                var exceptionContext = new ConfigurationEFCoreLoadExceptionContext<TDbContext>(_configurationSource, ex);
                _configurationSource.OnLoadException?.Invoke(exceptionContext);
                if (!exceptionContext.Ignore)
                {
                    throw;
                }
            }

            var cancellationToken = _cancellationTokenSource.Token;
            if (_configurationSource.ReloadOnChange)
            {
                _watchDbTask = Task.Run(() => WatchDatabase(cancellationToken), cancellationToken);
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _disposed = true;
        }

        private async Task WatchDatabase(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_configurationSource.PollingInterval, cancellationToken);
                    IDictionary<string, string> actualData = await GetDataAsync();
                    byte[] computedHash = ComputeHash(actualData);
                    if (!computedHash.SequenceEqual(_lastComputedHash))
                    {
                        Data = actualData;
                        OnReload();
                    }
                    _lastComputedHash = computedHash;
                }
                catch (Exception ex)
                {
                    var exceptionContext = new ConfigurationEFCoreLoadExceptionContext<TDbContext>(_configurationSource, ex);
                    _configurationSource.OnLoadException?.Invoke(exceptionContext);
                    if (!exceptionContext.Ignore)
                    {
                        throw;
                    }
                }
            }
        }

        private IDictionary<string, string> GetData()
        {
            using TDbContext dbContext = CreateDbContext();
            IQueryable<SettingEntity> settings = GetSettings(dbContext);
            IDictionary<string, string> settingsDictionary = settings.Any() ? settings.ToDictionary(c => c.Key, c => c.Value) : new Dictionary<string, string>();
            return BuildSettingsDictionary(settingsDictionary);
        }

        private async Task<IDictionary<string, string>> GetDataAsync()
        {
            using TDbContext dbContext = CreateDbContext();
            IQueryable<SettingEntity> settings = GetSettings(dbContext);
            IDictionary<string, string> settingsDictionary = settings.Any() ? await settings.ToDictionaryAsync(c => c.Key, c => c.Value) : new Dictionary<string, string>();
            return BuildSettingsDictionary(settingsDictionary);
        }

        private byte[] ComputeHash(IDictionary<string, string> dict)
        {
            List<byte> byteDict = new List<byte>();
            foreach (KeyValuePair<string, string> item in dict)
            {
                byteDict.AddRange(Encoding.Unicode.GetBytes($"{item.Key}{item.Value}"));
            }

            return System.Security.Cryptography.SHA1.Create().ComputeHash(byteDict.ToArray());
        }

        private IQueryable<SettingEntity> GetSettings(TDbContext dbContext)
        {
            DbSet<SettingEntity> settings = dbContext.Set<SettingEntity>();

            return settings.Where(i => i.Environment == _configurationSource.Environment && i.Application == _configurationSource.ApplicationContext);
        }

        private TDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<TDbContext> builder = new DbContextOptionsBuilder<TDbContext>();
            _configurationSource.OptionsAction(builder);

            return (TDbContext)Activator.CreateInstance(typeof(TDbContext), new object[] { builder.Options });
        }

        private IDictionary<string, string> BuildSettingsDictionary(IDictionary<string, string> settingsDictionary)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (KeyValuePair<string, string> setting in settingsDictionary)
            {
                if (setting.Value.StartsWith("{") || setting.Value.StartsWith("["))
                    stringBuilder.Append($"\"{setting.Key}\": {setting.Value}, ");
                else
                    stringBuilder.Append($"\"{setting.Key}\": \"{setting.Value}\", ");
            }
            stringBuilder.Append("}");

            return JsonConfigurationParser.Parse(stringBuilder.ToString());
        }
    }
}
