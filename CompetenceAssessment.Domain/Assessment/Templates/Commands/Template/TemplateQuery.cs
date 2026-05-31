namespace CompetenceAssessment.Domain.Assessment;

public class TemplateQuery
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public List<int> Ids { get; set; } = [];
    
    public List<int> CompetenceIds { get; set; } = [];

    public TemplateType Type { get; set; } = TemplateType.None;
    
    public TemplateQuery() {}

    public TemplateQuery(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public TemplateQuery(List<int> ids = default, List<int> competenceIds = default
                       , TemplateType type = TemplateType.None)
    {
        Ids = ids;
        CompetenceIds = competenceIds;
        Type = type;
    }

    public TemplateQuery(int id = default, int competenceId = default
        , TemplateType type = TemplateType.None)
    {
        if (id != default)
        {
            Ids.Add(id);
        }

        if (competenceId != default)
        {
            CompetenceIds.Add(competenceId);
        }
        
        Type = type;
    }
}