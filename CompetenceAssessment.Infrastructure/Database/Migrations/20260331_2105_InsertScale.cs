using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202603312105, "Scale")]
public class _20260331_2105_InsertScale : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260331_2105_InsertScale.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}