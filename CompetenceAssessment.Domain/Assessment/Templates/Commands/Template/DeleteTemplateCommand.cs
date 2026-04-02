namespace CompetenceAssessment.Domain.Assessment;

public class DeleteTemplateCommand
{
    public int Id { get; }

    public DeleteTemplateCommand(int id)
    {
        Id = id;
    }
}