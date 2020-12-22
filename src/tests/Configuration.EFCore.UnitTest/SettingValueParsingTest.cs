using Configuration.EFCore.TestCommon;
using Configuration.EFCore.UnitTest.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Configuration.EFCore.UnitTest
{
    public class SettingValueParsingTest : TestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public SettingValueParsingTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ReadAndCheckValues()
        {
            //Arrange
            SqliteConnection connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;Cache=Shared");
            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Sources.Clear();
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection));

            //Act
            IConfigurationRoot configurationRoot = configuration.Build();

            //Assert
            Assert.Equal("Answer to the Ultimate Question of Life, the Universe, and Everything", configurationRoot.GetValue<string>("StringValue"));
            Assert.Equal(42, configurationRoot.GetValue<int>("IntegerValue"));
            Assert.True(configurationRoot.GetValue<bool>("BooleanValue"));
            Assert.Equal("Ich komme aus der Tuerkei!", configurationRoot.GetValue<string>("NestedValues:level1value:level2value"));

            Assert.Equal(10, configurationRoot.GetSection("ArrayValue").GetChildren().Count());
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1, configurationRoot.GetValue<int>($"ArrayValue:{i}"));

            Assert.Equal(4, configurationRoot.GetSection("ListValue").GetChildren().Count());
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(i + 1, configurationRoot.GetValue<int>($"ListValue:{i}:Attr1"));
                Assert.Equal($"{i + 1}", configurationRoot.GetValue<string>($"ListValue:{i}:Attr2"));
            }

            SettingClass settingClass = new SettingClass();
            configurationRoot.Bind("ClassValue", settingClass);
            Assert.Equal("Wie alt bist du?", settingClass.StringValue);
            Assert.Equal(25, settingClass.IntegerValue);
            Assert.False(settingClass.BooleanValue);
        }

        [Fact]
        public void ReadAndCheckValuesForEnvironment()
        {
            //Arrange
            SqliteConnection connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;Cache=Shared");
            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Sources.Clear();
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection));
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection), environment: "Development");

            //Act
            IConfigurationRoot configurationRoot = configuration.Build();

            //Assert
            Assert.Equal("Answer to the Ultimate Question of Life, the Universe, and Everything", configurationRoot.GetValue<string>("StringValue"));
            Assert.Equal(41, configurationRoot.GetValue<int>("IntegerValue"));
            Assert.False(configurationRoot.GetValue<bool>("BooleanValue"));
            Assert.Equal("Ich komme aus Tuerkei!", configurationRoot.GetValue<string>("NestedValues:level1value:level2value"));

            Assert.Equal(10, configurationRoot.GetSection("ArrayValue").GetChildren().Count());
            for (int i = 0; i < 10; i++)
                Assert.Equal(10 - i, configurationRoot.GetValue<int>($"ArrayValue:{i}"));

            Assert.Equal(4, configurationRoot.GetSection("ListValue").GetChildren().Count());
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(4 - i, configurationRoot.GetValue<int>($"ListValue:{i}:Attr1"));
                Assert.Equal($"{4 - i}", configurationRoot.GetValue<string>($"ListValue:{i}:Attr2"));
            }

            SettingClass settingClass = new SettingClass();
            configurationRoot.Bind("ClassValue", settingClass);
            Assert.Equal("Wie alt sind Sie?", settingClass.StringValue);
            Assert.Equal(41, settingClass.IntegerValue);
            Assert.True(settingClass.BooleanValue);
        }

        [Fact]
        public void ReadAndCheckValuesForApplication()
        {
            //Arrange
            SqliteConnection connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;Cache=Shared");

            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Sources.Clear();
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection));
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection), application: "Mobile");

            //Act
            IConfigurationRoot configurationRoot = configuration.Build();

            //Assert
            Assert.Equal("Answer to the Ultimate Question of Life, the Universe, and Everything", configurationRoot.GetValue<string>("StringValue"));
            Assert.Equal(42, configurationRoot.GetValue<int>("IntegerValue"));
            Assert.True(configurationRoot.GetValue<bool>("BooleanValue"));
            Assert.Equal("Ich komme aus der Tuerkei!", configurationRoot.GetValue<string>("NestedValues:level1value:level2value"));

            Assert.Equal(10, configurationRoot.GetSection("ArrayValue").GetChildren().Count());
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1, configurationRoot.GetValue<int>($"ArrayValue:{i}"));

            Assert.Equal(4, configurationRoot.GetSection("ListValue").GetChildren().Count());
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(i + 1, configurationRoot.GetValue<int>($"ListValue:{i}:Attr1"));
                Assert.Equal($"{i + 1}", configurationRoot.GetValue<string>($"ListValue:{i}:Attr2"));
            }

            SettingClass settingClass = new SettingClass();
            configurationRoot.Bind("ClassValue", settingClass);
            Assert.Equal("Wie alt bist du?", settingClass.StringValue);
            Assert.Equal(25, settingClass.IntegerValue);
            Assert.False(settingClass.BooleanValue);

            Assert.Equal("3.2.15", configurationRoot.GetValue<string>("Version"));
            Assert.Equal(127, configurationRoot.GetValue<int>("VersionNumber"));
            Assert.True(configurationRoot.GetValue<bool>("ForceToDownload"));
        }

        [Fact]
        public void ReadAndCheckValuesForApplicationAndEnvironment()
        {
            //Arrange
            SqliteConnection connection = new SqliteConnection("Data Source=InMemorySample;Mode=Memory;Cache=Shared");

            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.Sources.Clear();
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection));
            configuration.AddEFCoreConfiguration<PersonDbContext>(options => options.UseSqlite(connection), application: "Mobile", environment: "Development");

            //Act
            IConfigurationRoot configurationRoot = configuration.Build();

            //Assert
            Assert.Equal("Answer to the Ultimate Question of Life, the Universe, and Everything", configurationRoot.GetValue<string>("StringValue"));
            Assert.Equal(42, configurationRoot.GetValue<int>("IntegerValue"));
            Assert.True(configurationRoot.GetValue<bool>("BooleanValue"));
            Assert.Equal("Ich komme aus der Tuerkei!", configurationRoot.GetValue<string>("NestedValues:level1value:level2value"));

            Assert.Equal(10, configurationRoot.GetSection("ArrayValue").GetChildren().Count());
            for (int i = 0; i < 10; i++)
                Assert.Equal(i + 1, configurationRoot.GetValue<int>($"ArrayValue:{i}"));

            Assert.Equal(4, configurationRoot.GetSection("ListValue").GetChildren().Count());
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(i + 1, configurationRoot.GetValue<int>($"ListValue:{i}:Attr1"));
                Assert.Equal($"{i + 1}", configurationRoot.GetValue<string>($"ListValue:{i}:Attr2"));
            }

            SettingClass settingClass = new SettingClass();
            configurationRoot.Bind("ClassValue", settingClass);
            Assert.Equal("Wie alt bist du?", settingClass.StringValue);
            Assert.Equal(25, settingClass.IntegerValue);
            Assert.False(settingClass.BooleanValue);

            Assert.Equal("1.0.0", configurationRoot.GetValue<string>("Version"));
            Assert.Equal(1, configurationRoot.GetValue<int>("VersionNumber"));
            Assert.False(configurationRoot.GetValue<bool>("ForceToDownload"));
        }
    }
}
