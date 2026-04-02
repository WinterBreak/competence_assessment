using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202603271100, "Insert task_type")]
public class _20260327_1100_InsertTaskType : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260327_1100_InsertTaskType.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}