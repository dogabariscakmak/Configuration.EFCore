using Configuration.EFCore.IntegrationTest.Helpers;
using Configuration.EFCore.TestCommon;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Data.Common;
using System.Threading.Tasks;
using Xunit;

namespace Configuration.EFCore.IntegrationTest
{
    [Collection("DbServer")]
    public class ReloadOnChangeTest : TestBase
    {
        private readonly DbServerFixture _fixture;

        public ReloadOnChangeTest(DbServerFixture fixture) : base(fixture.TestSettings.MssqlConnectionString)
        {
            this._fixture = fixture;
        }

        [Fact]
        public async Task ConfigurationValueShouldChangeAfterChangeOnDb()
        {
            //Arrange
            DbConnection connection = new SqlConnection(_fixture.TestSettings.MssqlConnectionString);
            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Sources.Clear();
            configuration.AddEFCoreConfiguration<PersonDbContext>(
                options => options.UseSqlServer(connection),
                reloadOnChange: true);

            //Act
            IConfigurationRoot configurationRoot = configuration.Build();
            IChangeToken reloadToken = configurationRoot.GetReloadToken();

            using var transaction = Connection.BeginTransaction();
            using var context = CreateContext(transaction);
            SettingEntity setting = context.Find<SettingEntity>("TheValueWillBeChanged", "", "");
            setting.Value = "May the force be with you!";
            context.Update(setting);
            context.SaveChanges();
            transaction.Commit();

            await Task.Delay(1000);

            //Assert
            Assert.True(reloadToken.HasChanged);
            Assert.Equal("May the force be with you!", configurationRoot.GetValue<string>("TheValueWillBeChanged"));
        }

        [Fact]
        public async Task ConfigurationValueShouldNotChangeAfterChangeOnDb()
        {
            //Arrange
            DbConnection connection = new SqlConnection(_fixture.TestSettings.MssqlConnectionString);
            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Sources.Clear();
            configuration.AddEFCoreConfiguration<PersonDbContext>(
                options => options.UseSqlServer(connection),
                reloadOnChange: true);

            //Act
            IConfigurationRoot configurationRoot = configuration.Build();
            IChangeToken reloadToken = configurationRoot.GetReloadToken();

            using var transaction = Connection.BeginTransaction();
            using var context = CreateContext(transaction);
            SettingEntity setting = context.Find<SettingEntity>("TheValueWillBeChanged", "", "");
            setting.Value = "This is the way!";
            context.Update(setting);
            context.SaveChanges();
            transaction.Commit();

            await Task.Delay(1000);

            //Assert
            Assert.False(reloadToken.HasChanged);
            Assert.Equal("This is the way!", configurationRoot.GetValue<string>("TheValueWillBeChanged"));
        }
    }
}
