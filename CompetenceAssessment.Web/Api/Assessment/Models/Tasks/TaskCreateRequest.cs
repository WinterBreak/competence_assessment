namespace CompetenceAssessment.Web.Assessment;

public class TaskCreateRequest
{
    public string Text { get; set; }
    
    public int Type { get; set; }
    
    public Dictionary<string, bool> Answers { get; set; }
}