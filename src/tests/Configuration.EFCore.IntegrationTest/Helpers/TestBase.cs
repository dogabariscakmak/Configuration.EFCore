using Configuration.EFCore.TestCommon;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;

namespace Configuration.EFCore.IntegrationTest.Helpers
{

    public class TestBase : IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _isDbCreated = false;
        private bool _disposed;

        public DbConnection Connection { get; }

        public List<PersonEntity> PersonTestData { get; private set; }
        public List<SettingEntity> SettingTestData { get; private set; }
        public List<SettingEntity> SettingDevelopmentTestData { get; private set; }
        public List<SettingEntity> SettingMobileTestData { get; private set; }
        public List<SettingEntity> SettingMobileDevelopmentTestData { get; private set; }

        public TestBase(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();

            ReloadTestData();
            CreateDatabase();
        }

        public PersonDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new PersonDbContext(new DbContextOptionsBuilder<PersonDbContext>().UseSqlServer(Connection, b => b.MigrationsAssembly("Configuration.EFCore.IntegrationTest")).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Connection.Close();
                    Connection.Dispose();
                }

                _disposed = true;
            }
        }

        private void CreateDatabase()
        {
            lock (_lock)
            {
                if (!_isDbCreated)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.Migrate();

                        context.AddRange(SettingTestData);
                        context.AddRange(SettingDevelopmentTestData);
                        context.AddRange(SettingMobileTestData);
                        context.AddRange(SettingMobileDevelopmentTestData);

                        context.SaveChanges();
                        _isDbCreated = true;
                    }
                }
            }
        }

        private void ReloadTestData()
        {
            if (PersonTestData == default)
            {
                PersonTestData = new List<PersonEntity>();
            }
            if (SettingTestData == default)
            {
                SettingTestData = new List<SettingEntity>();
            }
            if (SettingDevelopmentTestData == default)
            {
                SettingDevelopmentTestData = new List<SettingEntity>();
            }
            if (SettingMobileTestData == default)
            {
                SettingMobileTestData = new List<SettingEntity>();
            }
            if (SettingMobileDevelopmentTestData == default)
            {
                SettingMobileDevelopmentTestData = new List<SettingEntity>();
            }

            PersonTestData.Clear();
            SettingTestData.Clear();
            SettingDevelopmentTestData.Clear();
            SettingMobileTestData.Clear();
            SettingMobileDevelopmentTestData.Clear();

            using (StreamReader file = File.OpenText("persontestdata.json"))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                PersonTestData.AddRange((List<PersonEntity>)serializer.Deserialize(file, typeof(List<PersonEntity>)));
            }
            using (StreamReader file = File.OpenText("settingtestdata.json"))
            {
                JObject fileAsJson = JObject.Parse(file.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in fileAsJson)
                {
                    SettingTestData.Add(new SettingEntity()
                    {
                        Key = entry.Key,
                        Value = entry.Value.ToString(),
                        Application = "",
                        Environment = ""
                    });
                }
            }
            using (StreamReader file = File.OpenText("settingtestdata.Development.json"))
            {
                JObject fileAsJson = JObject.Parse(file.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in fileAsJson)
                {
                    SettingDevelopmentTestData.Add(new SettingEntity()
                    {
                        Key = entry.Key,
                        Value = entry.Value.ToString(),
                        Application = "",
                        Environment = "Development"
                    });
                }
            }
            using (StreamReader file = File.OpenText("settingtestdata.Mobile.json"))
            {
                JObject fileAsJson = JObject.Parse(file.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in fileAsJson)
                {
                    SettingMobileTestData.Add(new SettingEntity()
                    {
                        Key = entry.Key,
                        Value = entry.Value.ToString(),
                        Application = "Mobile",
                        Environment = ""
                    });
                }
            }
            using (StreamReader file = File.OpenText("settingtestdata.Mobile.Development.json"))
            {
                JObject fileAsJson = JObject.Parse(file.ReadToEnd());
                foreach (KeyValuePair<string, JToken> entry in fileAsJson)
                {
                    SettingMobileDevelopmentTestData.Add(new SettingEntity()
                    {
                        Key = entry.Key,
                        Value = entry.Value.ToString(),
                        Application = "Mobile",
                        Environment = "Development"
                    });
                }
            }
        }
    }
}
