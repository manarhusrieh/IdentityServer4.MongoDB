# IdentityServer4.MongoDB

[![Build status](https://ci.appveyor.com/api/projects/status/c9bqe6pcbppfwrbc/branch/release?svg=true)](https://ci.appveyor.com/project/jwu-au/identityserver4-mongodb/branch/release)
[![NuGet](https://img.shields.io/badge/NuGet-1.1.0-blue.svg)](https://www.nuget.org/packages/Selz.IdentityServer4.MongoDB/)

MongoDB persistence layer for IdentityServer4 based on the Official [EntityFramework](https://github.com/IdentityServer/IdentityServer4.EntityFramework) persistence layer.

## Simple Usage
```c#
// using default connection: mongodb://localhost/identityserver
identityServerBuilder.AddConfigurationStoreCache().AddOperationalStore();
```

## Config database connection and collection prefix
```C#
const string connectionString = "mongodb://db.local.com/mydb";
identityServerBuilder.AddConfigurationStore(options =>
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
