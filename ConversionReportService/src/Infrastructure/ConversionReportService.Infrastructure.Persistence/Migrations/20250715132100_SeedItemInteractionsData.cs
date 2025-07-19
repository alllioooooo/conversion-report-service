using FluentMigrator;

namespace ConversionReportService.Infrastructure.Persistence.Migrations;

[Migration(20250715132100, "SeedItemInteractionsData")]
public class SeedItemInteractionsData : Migration
{
    public override void Up()
    {
        const string sql = @"
INSERT INTO item_interactions (item_id, type, timestamp)
VALUES 
    (52, 'Viewed'::interaction_type, '2025-07-09T10:52:00+00'),
    (52, 'Viewed'::interaction_type, '2025-07-11T12:52:00+00'),
    (52, 'Payed'::interaction_type,  '2025-07-11T12:54:00+00'),

    (239, 'Viewed'::interaction_type, '2025-07-09T01:52:00+00'),
    (239, 'Viewed'::interaction_type, '2025-07-09T11:52:00+00'),
    (239, 'Viewed'::interaction_type, '2025-07-11T01:52:00+00'),

    (1984, 'Viewed'::interaction_type, '2025-07-11T11:52:00+00'),
    (1984, 'Payed'::interaction_type,  '2025-07-11T11:54:00+00'),
    (1984, 'Viewed'::interaction_type, '2025-07-11T18:52:00+00'),
    (1984, 'Payed'::interaction_type,  '2025-07-11T18:54:00+00');
";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        Execute.Sql("DELETE FROM item_interactions;");
    }
}