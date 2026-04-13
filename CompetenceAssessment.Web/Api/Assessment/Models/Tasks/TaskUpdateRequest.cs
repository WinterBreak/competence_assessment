namespace CompetenceAssessment.Web.Assessment;

public class TaskUpdateRequest
{
    public int Id { get; set; }
    
    public string Text { get; set; }
    
    public int Type { get; set; }
    
    public Dictionary<string, bool> Answers { get; set; }
    
    // TODO запрет редактирования для вопросов, уже состоящих в шаблонах. как вариант предлагать сделать копию с выбранными параметрами
}