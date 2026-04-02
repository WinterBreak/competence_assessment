using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202604010900, "AssessmentType")]
public class _20260401_0900_InsertAssessmentType : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260401_0900_InsertAssessmentType.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}