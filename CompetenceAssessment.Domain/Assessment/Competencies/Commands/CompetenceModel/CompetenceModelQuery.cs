using System.Collections;

namespace CompetenceAssessment.Domain.Assessment;

public class CompetenceModelQuery
{
    public List<int> Ids { get; } = [];
    
    public List<string> Names { get; } = [];
    
    public List<int> CompetenceIds { get; } = [];

    public CompetenceModelQuery() {}
    
    public CompetenceModelQuery(int id = default, string name = default, int competenceId = default)
    {
        if (id != default)
        {
            Ids.Add(id);
        }

        if (name != default)
        {
            Names.Add(name);
        }

        if (competenceId != default)
        {
            CompetenceIds.Add(competenceId);
        }
    }

    public CompetenceModelQuery(IEnumerable<int> ids = default, IEnumerable<string> names = default
        , IEnumerable<int> competenceIds = default)
    {
        Ids = ids.ToList();
        Names = names.ToList();
        CompetenceIds = competenceIds.ToList();
    }
}