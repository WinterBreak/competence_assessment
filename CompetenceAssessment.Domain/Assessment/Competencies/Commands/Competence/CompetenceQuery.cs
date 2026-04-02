namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceQuery
{
    public List<int> Ids { get; } = [];
    
    public List<string> Names { get; } = [];
    
    public List<string> Descriptions { get; } = [];
    
    public CompetenceQuery() {}

    public CompetenceQuery(int id = default, string name = default, string? description = null)
    {
        if (id != null && id > 0)
        {
            Ids.Add(id);
        }

        if (name != default)
        {
            Names.Add(name);
        }

        if (description != default)
        {
            Descriptions.Add(description);
        }
    }

    public CompetenceQuery(IEnumerable<int> ids = default, IEnumerable<string> names = default
        , IEnumerable<string> descriptions = default)
    {
        Ids = ids?.ToList() ?? new List<int>();
        Names = names?.ToList() ?? new List<string>();
        Descriptions = descriptions?.ToList() ?? new List<string>();
    }
}