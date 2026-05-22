using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(220260430_2000, "Alter Users")]
public class _20260430_2000_AlterUsers : Migration
{
    public override void Up()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Database",
            "Sql",
            "20260430_2000_AlterUser.sql"
        );
        Execute.Script(path);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}