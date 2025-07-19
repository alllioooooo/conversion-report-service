using FluentMigrator;

namespace ConversionReportService.Infrastructure.Persistence.Migrations;

[Migration(20250715125700, "Initial")]
public class InitSchema : Migration
{
    public override void Up()
    {
        Create.Table("item_interactions")
            .WithColumn("id").AsInt64().PrimaryKey("pk_item_interactions").Identity()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("type").AsCustom("interaction_type").NotNullable()
            .WithColumn("timestamp").AsDateTimeOffset().NotNullable();

        Create.Table("conversion_reports")
            .WithColumn("id").AsInt64().PrimaryKey("pk_conversion_reports").Identity()
            .WithColumn("registration_id").AsInt64().NotNullable()
            .WithColumn("item_id").AsInt64().NotNullable()
            .WithColumn("date_from").AsDateTimeOffset().Nullable()
            .WithColumn("date_to").AsDateTimeOffset().Nullable()
            .WithColumn("status").AsCustom("report_creation_status").NotNullable()
            .WithColumn("ratio").AsDouble().Nullable()
            .WithColumn("payed_amount").AsInt64().Nullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("conversion_reports");
        Delete.Table("item_interactions");
    }
}