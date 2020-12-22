using Configuration.EFCore.IntegrationTest.Helpers;
using Configuration.EFCore.TestCommon;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Configuration.EFCore.IntegrationTest
{
    [Collection("DbServer")]
    public class LoadExceptionTest : TestBase
    {
        private readonly DbServerFixture _fixture;

        public LoadExceptionTest(DbServerFixture fixture) : base(fixture.TestSettings.MssqlConnectionString)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void LoadShouldNotThrowExceptionWhenIgnoreSet()
        {
            //Arrange
            bool shoulOnLoadExceptionCalled = false;
            DbConnection connection = new SqlConnection(_fixture.TestSettings.MssqlConnectionString.Replace("P@ssw0rd123", "wrongpassword"));
            EFCoreConfigurationSource<PersonDbContext> configurationSource = new EFCoreConfigurationSource<PersonDbContext>(options => options.UseSqlServer(connection),
                                                                                                                            onLoadException: exceptionContext =>
                                                                                                                            {
                                                                                                                                exceptionContext.Ignore = true;
                                                                                                                                shoulOnLoadExceptionCalled = true;
                                                                                                                            });
            EFCoreConfigurationProvider<PersonDbContext> configurationProvider = new EFCoreConfigurationProvider<PersonDbContext>(configurationSource);

            //Act
            Exception ex = Record.Exception(() => configurationProvider.Load());

            //Assert
            Assert.Null(ex);
            Assert.True(shoulOnLoadExceptionCalled);
        }

        [Fact]
        public void LoadShouldThrowExceptionWhenIgnoreNotSet()
        {
            //Arrange
            bool shoulOnLoadExceptionCalled = false;
            DbConnection connection = new SqlConnection(_fixture.TestSettings.MssqlConnectionString.Replace("P@ssw0rd123", "wrongpassword"));
            EFCoreConfigurationSource<PersonDbContext> configurationSource = new EFCoreConfigurationSource<PersonDbContext>(options => options.UseSqlServer(connection),
                                                                                                                            onLoadException: exceptionContext =>
                                                                                                                            {
                                                                                                                                exceptionContext.Ignore = false;
                                                                                                                                shoulOnLoadExceptionCalled = true;
                                                                                                                            });
            EFCoreConfigurationProvider<PersonDbContext> configurationProvider = new EFCoreConfigurationProvider<PersonDbContext>(configurationSource);

            //Act
            Exception ex = Record.Exception(() => configurationProvider.Load());

            //Assert
            Assert.NotNull(ex);
            Assert.True(shoulOnLoadExceptionCalled);
        }

        [Fact]
        public async Task LoadAndWatchDbShouldNotThrowExceptionWhenIgnoreSet()
        {
            //Arrange
            int onLoadExceptionCalledCount = 0;
            DbConnection connection = new SqlConnection(_fixture.TestSettings.MssqlConnectionString.Replace("P@ssw0rd123", "wrongpassword"));
            EFCoreConfigurationSource<PersonDbContext> configurationSource = new EFCoreConfigurationSource<PersonDbContext>(options => options.UseSqlServer(connection),
                                                                                                                            reloadOnChange: true,
                                                                                                                            pollingInterval: 2000,
                                                                                                                            onLoadException: exceptionContext =>
                                                                                                                            {
                                                                                                                                exceptionContext.Ignore = true;
                                                                                                                                onLoadExceptionCalledCount++;
                                                                                                                            });
            EFCoreConfigurationProvider<PersonDbContext> configurationProvider = new EFCoreConfigurationProvider<PersonDbContext>(configurationSource);

            //Act
            Exception ex = Record.Exception(() => configurationProvider.Load());
            await Task.Delay(3000);

            //Assert
            Assert.Null(ex);
            Assert.Equal(2, onLoadExceptionCalledCount);
        }

        [Fact]
        public async Task LoadAndWatchDbShouldThrowExceptionWhenIgnoreNotSet()
        {
            //Arrange
            int onLoadExceptionCalledCount = 0;
            DbConnection connection = new SqlConnection(_fixture.TestSettings.MssqlConnectionString.Replace("P@ssw0rd123", "wrongpassword"));
            EFCoreConfigurationSource<PersonDbContext> configurationSource = new EFCoreConfigurationSource<PersonDbContext>(options => options.UseSqlServer(connection),
                                                                                                                            reloadOnChange: true,
                                                                                                                            pollingInterval: 2000,
                                                                                                                            onLoadException: exceptionContext =>
                                                                                                                            {
                                                                                                                                exceptionContext.Ignore = false;
                                                                                                                                onLoadExceptionCalledCount++;
                                                                                                                            });
            EFCoreConfigurationProvider<PersonDbContext> configurationProvider = new EFCoreConfigurationProvider<PersonDbContext>(configurationSource);

            //Act
            Exception ex = Record.Exception(() => configurationProvider.Load());
            await Task.Delay(3000);

            //Assert
            Assert.NotNull(ex);
            Assert.Equal(1, onLoadExceptionCalledCount);
        }
    }
}
