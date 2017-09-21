# IdentityServer4.MongoDB

MongoDB persistence layer for IdentityServer4 based on the Official [EntityFramework](https://github.com/IdentityServer/IdentityServer4.EntityFramework) persistence layer.

[![Build status](https://ci.appveyor.com/api/projects/status/c9bqe6pcbppfwrbc/branch/release?svg=true)](https://ci.appveyor.com/project/jwu-au/identityserver4-mongodb/branch/release)
[![identity-server MyGet Build Status](https://www.myget.org/BuildSource/Badge/identity-server?identifier=d7745704-7151-450f-a20f-e4efdafa2e68)](https://www.myget.org/)
[![MyGet Feed](https://img.shields.io/myget/identity-server/v/Selz.IdentityServer4.MongoDB.svg)](https://www.myget.org/feed/identity-server/package/nuget/Selz.IdentityServer4.MongoDB/)
[![NuGet](https://img.shields.io/nuget/v/Selz.IdentityServer4.MongoDB.svg)](https://www.nuget.org/packages/Selz.IdentityServer4.MongoDB/)

## Simple Usage
```c#
// using default connection: mongodb://localhost/identityserver
identityServerBuilder
    .AddConfigurationStore()
    .AddOperationalStore();
```

## Config database connection and collection prefix
```C#
const string connectionString = "mongodb://db.local.com/mydb";
identityServerBuilder
    .AddConfigurationStore(options =>
    {
        options.CollectionNamePrefix = "ids_";
        options.ConnectionString = connectionString;
    })
    .AddOperationalStore(options =>
    {
        options.CollectionNamePrefix = "ids_";
        options.ConnectionString = connectionString;
    });
```
## Token cleanup
```C#
// in Startup.Configure
applicationBuilder
    .UseIdentityServer()
    .UseIdentityServerTokenCleanup(appLifetime);
```
