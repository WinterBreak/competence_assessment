using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(2026033312100, "TemplateType")]
public class _20260331_2100_InsertTemplateType : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260331_2100_InsertTemplateType.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}