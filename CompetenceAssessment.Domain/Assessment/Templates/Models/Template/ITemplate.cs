namespace CompetenceAssessment.Domain.Assessment;

public class ITemplate
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public DateTime CreationDate { get; set; }
    
    public CompetenceModel CompetenceModel { get; set; }
    
    public IEnumerable<ITask> Tasks { get; set; }
}