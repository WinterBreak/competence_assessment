using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(220260430_2000, "Alter Users")]
public class _20260430_2000_AlterUsers : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260430_2000_AlterUser.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}