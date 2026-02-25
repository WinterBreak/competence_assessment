namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceQuery
{
    public List<int> Ids { get; } = [];
    
    public List<string> Names { get; } = [];
    
    public List<string> Descriptions { get; } = [];
    
    public CompetenceQuery() {}

    public CompetenceQuery(int id = default, string name = default, string? description = null)
    {
        Ids.Add(id);
        Names.Add(name);
        Descriptions.Add(description);
    }

    public CompetenceQuery(IEnumerable<int> ids = default, IEnumerable<string> names = default
        , IEnumerable<string?> descriptions = null)
    {
        Ids = ids.ToList();
        Names = names.ToList();
        Descriptions = descriptions.ToList();
    }
}