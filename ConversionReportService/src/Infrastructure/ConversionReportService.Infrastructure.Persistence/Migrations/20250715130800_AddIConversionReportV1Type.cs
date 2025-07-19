using FluentMigrator;

namespace ConversionReportService.Infrastructure.Persistence.Migrations;

[Migration(20250715130800, "AddConversionReportV1Type")]
public class AddConversionReportV1Type : Migration
{
    public override void Up()
    {
        const string sql = @"
DO $$
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'conversion_reports_v1') THEN
            CREATE TYPE conversion_reports_v1 AS (
                  registration_id bigint,
                  item_id         bigint,
                  date_from       timestamptz,
                  date_to         timestamptz,
                  status          report_creation_status,
                  ratio           double precision,
                  payed_amount    bigint,
                  created_at      timestamptz
            );
        END IF;
    END
$$;";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"DROP TYPE IF EXISTS conversion_reports_v1;";
        Execute.Sql(sql);
    }
}