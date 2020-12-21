# Configuration.EFCore

[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<br />
<p align="center">
  <a href="https://github.com/dogabariscakmak/Configuration.EFCore">
    <img src="logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Configuration.EFCore</h3>

  <p align="center">
    A simple library that enables Entity Framework Core DbContext as a configuration source.
    <br />
    <a href="https://github.com/dogabariscakmak/Configuration.EFCore/issues">Report Bug</a>
    ·
    <a href="https://github.com/dogabariscakmak/Configuration.EFCore/issues">Request Feature</a>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
* [Getting Started](#getting-started)
    * [Usage](#usage)
* [Roadmap](#roadmap)
* [License](#license)
* [Contact](#contact)


<!-- ABOUT THE PROJECT -->
## About The Project 

![Configuration.EFCore][product-screenshot]

This library enables that you can add any EF Core DbContext as a configuration source with minimal changes.

<!-- GETTING STARTED -->
## Getting Started

To get started with Configuration.EFCore you can clone the repository or add as a reference to your project from NuGet.

#### Package Manager
```Install-Package Configuration.EFCore```

#### .NET CLI
```dotnet add package Configuration.EFCore```

#### ```PackageReference```
```<PackageReference Include="Configuration.EFCore" Version="1.0.0" />```


<!-- USAGE EXAMPLES -->
## Usage

Configuration.EFCore adds EF Core DbContext as a configuration source. The library has its own setting entity to store settings for the application. To use Configuration.EFCore, as a developer you need to do a couple of things.

- First, Setting entity is needed to add to your DbContext. The only thing that is needed that adding ```modelBuilder.BuildSettingEntityModel()``` in ```OnModelCreating()``` method.

```csharp
public class PersonDbContext : DbContext
{
    public DbSet<PersonEntity> Persons { get; set; }
    public PersonDbContext(DbContextOptions<PersonDbContext>options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuildermodelBuilder)
    {
        modelBuilder.Entity<PersonEntity>();
        modelBuilder.ApplyConfiguration(newPersonEntityConfiguration());
        modelBuilder.BuildSettingEntityModel();
        base.OnModelCreating(modelBuilder);
    }
}
```

- After that, ```EFCoreConfigurationSource``` should be added to ```ConfigurationBuilder```.

```csharp
static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.Sources.Clear()
            configuration.AddEFCoreConfiguration<PersonDbContext>(
                options => options.UseSqlServer(connection),
                reloadOnChange: true)
            foreach ((string key, string value) in
                configuration.Build().AsEnumerable().Where(t =t.Value is not null))
            {
                Console.WriteLine($"{key}={value}");
            }
        });
```

- After building the configuration root, settings values can be used in the application. Setting entity consist of key, value, application, and environment properties. Key, environment, and application compose together key for setting table. So, multiple values can be defined with the same key in different applications and/or environments. Value can be any primitive type, a JSON object, or a JSON array.

- ```AddEFCoreConfiguration``` method takes six parameters:
    - ```Action<DbContextOptionsBuilder> optionsAction``` to build related DbContext.
    - ```bool reloadOnChange``` if it is set to true, changes on database automatically reloaded. The default value is false.
    - ```int pollingInterval``` when ```reloadOnChange``` set to true, this value indicates what will be the frequency in milliseconds in order to poll database. The default value is 5000.
    - ```string environment``` to separate environments for configuration values. If you set this to something, only the same environment values can be loaded to the configuration. The default value is an empty string. If you leave this value as default and there is no record with empty environment colon on the database, no configuration value is loaded.
    - ```string application``` to separate applications for configuration values. If you set this to something, only the same application values can be loaded to the configuration. The default value is an empty string. If you leave this value as default and there is no record with empty application colon on the database, no configuration value is loaded.
    - ```Action<ConfigurationEFCoreLoadExceptionContext<TDbContext>> onLoadException``` If an exception occurs when load or poll from the databese, this callback method is invoked. If you handle the exception and decide to continue, ```Ignore``` should be set to true.
```csharp
EFCoreConfigurationSource<PersonDbContext> configurationSource = new EFCoreConfigurationSource<PersonDbContext>(options => options.UseSqlServer(connection),
onLoadException: exceptionContext => { exceptionContext.Ignore = true;});
```

<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/dogabariscakmak/Configuration.EFCore/issues) for a list of proposed features (and known issues).

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<!-- CONTACT -->
## Contact

Doğa Barış Çakmak - dogabaris.cakmak@gmail.com

Project Link: [https://github.com/dogabariscakmak/Configuration.EFCore](https://github.com/dogabariscakmak/Configuration.EFCore)

*Created my free logo at LogoMakr.com*

[issues-shield]: https://img.shields.io/github/issues/dogabariscakmak/Configuration.EFCore.svg?style=flat-square
[issues-url]: https://github.com/dogabariscakmak/Configuration.EFCore/issues
[license-shield]: https://img.shields.io/github/license/dogabariscakmak/Configuration.EFCore.svg?style=flat-square
[license-url]: https://github.com/dogabariscakmak/Configuration.EFCore/blob/master/LICENSE
[product-screenshot]: usage.gif