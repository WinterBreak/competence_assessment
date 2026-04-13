using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Web.Assessment;

public class AssessmentTaskDto
{
    public int Id { get; set; }
    
    public int Type { get; set; }
    
    public string Text { get; set; }
    
    public List<Answer> Answers { get; set; }
    
    public AssessmentTaskDto() { }

    public AssessmentTaskDto(ITask task)
    {
        Id = task.Id;
        Type = (int)task.Type;
        Text = task.Text;
        Answers = task.Answers;
    }
}