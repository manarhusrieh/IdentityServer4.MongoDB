# IdentityServer4.MongoDB

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
