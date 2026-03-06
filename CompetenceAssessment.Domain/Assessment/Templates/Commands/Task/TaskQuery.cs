namespace CompetenceAssessment.Domain.Assessment;

public class TaskQuery
{
    public List<int> Ids { get; } = [];
    
    public TaskType Type { get; }
    
    public TaskQuery() {}
    
    public TaskQuery(List<int> ids = default, TaskType taskType = TaskType.None)
    {
        Ids = ids;
        Type = taskType;
    }

    public TaskQuery(int id = default, TaskType taskType = TaskType.None)
    {
        Ids.Add(id);
        Type = taskType;
    }
}