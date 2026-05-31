namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceQuery
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public List<int> Ids { get; } = [];
    
    public List<string> Names { get; } = [];
    
    public List<string> Descriptions { get; } = [];
    
    public CompetenceQuery() {}

    public CompetenceQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
    
    public CompetenceQuery(int id = default, string name = default, string? description = null, int page = default
        , int pageSize = default)
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
        , IEnumerable<string> descriptions = default, int page = default
        , int pageSize = default)
    {
        Ids = ids?.ToList() ?? new List<int>();
        Names = names?.ToList() ?? new List<string>();
        Descriptions = descriptions?.ToList() ?? new List<string>();
    }
}