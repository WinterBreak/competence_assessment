using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(20260413_2130, "Alter Assessment Result")]
public class _20260413_2130_AlterAssessmentResult: Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260413_2130_AlterAssessmentResult.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}