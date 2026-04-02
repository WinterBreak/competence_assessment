namespace CompetenceAssessment.Domain.Assessment;

public class DeleteCompetenceCommand
{
    public int Id { get; set; }

    public DeleteCompetenceCommand(int id)
    {
        Id = id;
    }
}