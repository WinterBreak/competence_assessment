using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202603312105, "Scale")]
public class _20260331_2105_InsertScale : Migration
{
    public override void Up()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Database",
            "Sql",
            "20260331_2105_InsertScale.sql"
        );
        Execute.Script(path);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}