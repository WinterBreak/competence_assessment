using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(220260522_1030, "Alter Assessment")]
public class _20260522_1030_AlterAssessment : Migration
{
    public override void Up()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Database",
            "Sql",
            "20260522_1030_AlterAssessment.sql"
        );
        Execute.Script(path);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}