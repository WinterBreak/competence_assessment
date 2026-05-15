using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202603262000, "CreateDatabase")]
public class _20260326_2000_CreateDatabase : Migration
{
    public override void Up()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Database",
            "Sql",
            "20260326_2000_CreateDatabase.sql"
        );
        Execute.Script(path);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}