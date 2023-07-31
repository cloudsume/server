# Cloudsumé Server

This is a server of [Cloudsumé](https://cloudsume.com), written in C# for .NET 6.

## Building from source

### Prerequisites

- .NET 6 SDK

### Build Cloudsumé Server

If the target machine has .NET runtime installed, run:

```sh
dotnet publish -c Release -o dist src/Cloudsume
```

Otherwise run:

```sh
dotnet publish -c Release -o dist -r RID src/Cloudsume
```

Replace `RID` with the target platform. You can find a list of the available platforms [here](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#using-rids). `dist` directory will contains the output binary once the build is finished.

## Runtime dependencies

Cloudsumé Server depend on the following services:

- OIDC Provider.
- Apache Cassandra.
- Cloudsumé Services

### OIDC Provider

The issuer claim must not be `cloudsume` and the following claims must be present:

- `sub`: A UUID representing the unique identifier of the user.
- `http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier`: Same as the above.
- `name`: A username of the user.
- `email`: An email address of the user.
- `email_verified`: A boolean indicated if the email has been verified.

### Apache Cassandra

You need to enable materialized view in the Cassandra configuration.

### Cloudsumé Services

See its [repository](https://github.com/cloudsume/services) for the instructions.

## Development

### Adding new country

- Edit `src/Ultima.Extensions.Globalization/RegionNames.cs`.
- Add new file to `src/Ultima.Extensions.Globalization/Subdivisions`.
- Edit `src/Ultima.Extensions.Globalization/SubdivisionNames.cs`.

### Adding new template culture

- Update `Application.AllowedTemplateCultures` in `src/Cloudsume/appsettings.*.json`.

### Adding new resume data

- Add a domain model to `src/Cloudsume.Resume/Data`.
- Add a data merger to `src/Cloudsume.Resume/Mergers` and register it to IoC container.
- Add an attribute factory for NetTemplate to `src/Cloudsume.Builder/AttributeFactories` and register it to IoC container.
- Add a database model to `src/Cloudsume.Cassandra/Models`. Don't forget to register its mapping and create a migration script.
- Add a database mapper to `src/Cloudsume.Cassandra/ResumeDataMappers` and register it to IoC container.
- Add a payload manager to `src/Cloudsume.Cassandra/ResumeDataPayloadManagers` if there is one and register it to IoC container.
- Add a sample data model to `src/Cloudsume.Cassandra/Models` with a migration script and register its mapping. Do not forget to seed the sample data in the migration script.
- Add a sample payload manager to `src/Cloudsume.Cassandra/SampleDataPayloadManagers` if there is one and register it to IoC container.
- Add a censor to `src/Cloudsume.Resume/LinkDataCensor.cs`.
- Add a DTO model to `src/Cloudsume/Models`.
- Add a manager for DTO model to `src/Cloudsume/DataManagers` and register it to IoC container.

### Adding new culture for ASP.NET

- Update `RequestLocalizationOptions` in `src/Cloudsume/Startup.cs`.
- Update `ListAsync` in `src/Cloudsume/Controllers/ResumesController.cs`.

## License

GNU AGPLv3
