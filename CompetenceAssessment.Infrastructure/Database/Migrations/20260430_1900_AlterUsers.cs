using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(220260430_1900, "Alter Users")]
public class _20260430_1900_AlterUsers : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260430_1900_AlterUsers.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}