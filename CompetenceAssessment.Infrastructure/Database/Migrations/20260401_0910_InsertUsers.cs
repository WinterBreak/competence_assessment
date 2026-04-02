using FluentMigrator;

namespace CompetenceAssessment.Infrastructure.Database.Migrations;

[Migration(202604010910, "Users")]
public class _20260401_0910_InsertUsers : Migration
{
    public override void Up()
    {
        Execute.Script("/Users/elizavetalozkina/RiderProjects/CompetenceAssessment.Web/CompetenceAssessment.Infrastructure/Database/Sql/20260401_0910_InsertUsers.sql");
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}