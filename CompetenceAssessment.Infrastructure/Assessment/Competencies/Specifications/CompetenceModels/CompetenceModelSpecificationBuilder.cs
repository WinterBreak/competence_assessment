using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Infrastructure.Competencies;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceModelSpecificationBuilder: SpecificationBuilderBase<CompetenceModel>
{
    public CompetenceModelSpecificationBuilder WithQuery(CompetenceModelQuery query)
    {
        return WithIds(query.Ids)
            .WithNames(query.Names)
            .WithCompetencies(query.CompetenceIds);
    }
    
    public CompetenceModelSpecificationBuilder WithIds(IEnumerable<int> ids)
    {
        if (ids is null || !ids.Any())
        {
            return this;
        }
        
        var specification = new CompetenceModelIdSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public CompetenceModelSpecificationBuilder WithNames(IEnumerable<string> names)
    {
        if (names is null || !names.Any())
        {
            return this;
        }
        
        var specification = new CompetenceModelNameSpecification(names);
        AppendSpecification(specification);
        return this;
    }
    
    public CompetenceModelSpecificationBuilder WithCompetencies(IEnumerable<int> competenceIds)
    {
        if (competenceIds is null || !competenceIds.Any())
        {
            return this;
        }
        
        var specification = new CompetenceModelCompetenceSpecification(competenceIds);
        AppendSpecification(specification);
        return this;
    }
}