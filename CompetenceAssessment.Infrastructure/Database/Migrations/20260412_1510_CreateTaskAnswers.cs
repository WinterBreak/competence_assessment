using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202604121510, "TaskAnswers")]
public class _20260412_1510_CreateTaskAnswers: Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260412_1500_CreateAnswers.sql");
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260412_1510_AlterTasks.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}