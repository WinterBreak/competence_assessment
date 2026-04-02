namespace CompetenceAssessment.Domain.Assessment;

public class DeleteTaskCommand(int id)
{
    public int Id { get; set; } = id;
}