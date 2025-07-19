using FluentMigrator;

namespace ConversionReportService.Infrastructure.Persistence.Migrations;

[Migration(20250715125600, "AddEnumTypes")]
public class AddEnumTypes : Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'interaction_type') THEN
        CREATE TYPE interaction_type AS ENUM ('Viewed', 'Payed');
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'report_creation_status') THEN
        CREATE TYPE report_creation_status AS ENUM ('Pending', 'Processing', 'Done', 'Cancelled');
    END IF;
END
$$;";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
DROP TYPE IF EXISTS interaction_type;
DROP TYPE IF EXISTS report_creation_status;
";
        Execute.Sql(sql);
    }
}