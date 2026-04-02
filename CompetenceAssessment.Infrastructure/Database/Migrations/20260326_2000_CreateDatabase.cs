using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202603262000, "CreateDatabase")]
public class _20260326_2000_CreateDatabase : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260326_2000_CreateDatabase.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}