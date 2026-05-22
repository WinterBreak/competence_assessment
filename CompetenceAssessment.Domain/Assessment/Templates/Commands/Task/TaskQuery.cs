namespace CompetenceAssessment.Domain.Assessment;

public class TaskQuery
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public List<int> Ids { get; } = [];
    
    public TaskType Type { get; }
    
    public TaskQuery() {}

    public TaskQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
    
    public TaskQuery(List<int> ids = default, int page = default
        , int pageSize = default, TaskType taskType = TaskType.None)
    {
        Ids = ids;
        Type = taskType;
    }

    public TaskQuery(int id = default, int page = default
        , int pageSize = default, TaskType taskType = TaskType.None)
    {
        if (id != null && id > 0)
        {
            Ids.Add(id);
        }
        
        Type = taskType;
    }
}