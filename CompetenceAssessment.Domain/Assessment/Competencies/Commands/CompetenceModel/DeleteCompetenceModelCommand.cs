namespace CompetenceAssessment.Domain.Assessment;

public class DeleteCompetenceModelCommand
{
    public int Id { get; set; }

    public DeleteCompetenceModelCommand(int id)
    {
        Id = id;
    }
}