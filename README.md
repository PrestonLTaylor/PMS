# PMS

PMS is a product management system API, library and frontend made using C# with gRPC. 

TODO: Individual readmes for each part of PMS

# Building

TODO

# Testing

TODO

# Usage

TODO

## Migrations

To setup the migrations for the database, you need to apply the migrations inside of PMS.Server.

By default, we don't expose postgres outside of docker, however, you will need to expose port 5432 for Update-Database to work.

Modify the postgres container inside of docker-compose.yml to expose port 5432:
```yaml
port:
    - <OUTPUT_PORT>:5432
```

Then you'll need to setup the connection string 'POSTGRESQLCONNSTR_DefaultConnection' to connect to the exposed postgres server:
```
POSTGRESQLCONNSTR_DefaultConnection: "Host=localhost;Port=<OUTPUT_PORT>;Database=<DATABASE_NAME>;Username=postgres;Password=>PASSWORD>"
```

Finally, you can run the Update-Database command to apply the migrations to postgres.

# License

This project is licensed under [MIT](https://github.com/PrestonLTaylor/PMS/blob/master/LICENSE)