namespace CompetenceAssessment.Domain.Assessment;

public class CreateTemplateCommand
{
    public string Name { get; }
    
    public TemplateType Type { get; }
    
    public ScaleType Scale { get; }
    
    public int CompetenceModelId { get;  }
    
    public Dictionary<int, decimal> Weights { get; }
    
    public Dictionary<int, List<int>> CompetenciesToTasks { get; }

    public CreateTemplateCommand(string name, TemplateType type, ScaleType scale, int competenceModelId
                               , Dictionary<int, decimal> weights, Dictionary<int, List<int>> competenciesToTasks)
    {
        Name = name;
        Type = type;
        Scale = scale;
        CompetenceModelId = competenceModelId;
        Weights = weights;
        CompetenciesToTasks = competenciesToTasks;
    }

    public ITemplate Create(List<ITask> tasks)
    {
        var weights = new List<TemplateWeight>();
        foreach (var weight in Weights)
        {
            var task = tasks.Single(t => t.Id == weight.Key);
            var competenceId = CompetenciesToTasks.FirstOrDefault(v 
                => v.Value.Contains(task.Id)).Key;
            weights.Add(new TemplateWeight(task, competenceId, Weights[task.Id]));
        }
        
        return new ITemplate(Name, Type, Scale, DateTime.UtcNow, CompetenceModelId, weights);
    }
}