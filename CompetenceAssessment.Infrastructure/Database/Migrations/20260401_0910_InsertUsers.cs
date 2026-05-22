using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202604010910, "Users")]
public class _20260401_0910_InsertUsers : Migration
{
    public override void Up()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Database",
            "Sql",
            "20260401_0910_InsertUsers.sql"
        );
        Execute.Script(path);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}