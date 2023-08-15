# Ziralink.IDS

## Migrations:
```sh
Add-Migration {MIGRATION_NAME} -c ApplicationDbContext -o Migrations/DbContext/ApplicationDbContextMigrations
Add-Migration {MIGRATION_NAME} -c PersistedGrantDbContext -o Migrations/DbContext/PersistedGrantDbContextMigrations
Add-Migration {MIGRATION_NAME} -c ConfigurationDbContext -o Migrations/DbContext/ConfigurationDbContextMigrations
```