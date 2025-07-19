using FluentMigrator;

namespace ConversionReportService.Infrastructure.Persistence.Migrations;

[Migration(20250715125900, "AddItemInteractionV1Type")]
public class AddItemInteractionV1Type : Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'item_interactions_v1') THEN
            CREATE TYPE item_interactions_v1 AS (
                  item_id     bigint, 
                  type        interaction_type,
                  timestamp   timestamptz
            );
        END IF;
    END
$$;";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"DROP TYPE IF EXISTS item_interactions_v1;";
        Execute.Sql(sql);
    }
}