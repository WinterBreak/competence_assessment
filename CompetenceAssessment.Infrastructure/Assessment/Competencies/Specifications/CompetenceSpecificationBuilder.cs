using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceSpecificationBuilder: SpecificationBuilderBase<Competence>
{
    public CompetenceSpecificationBuilder WithId(int id)
    {
        var specification = new CompetenceIdSpecification(id);
        AppendSpecification(specification);
        return this;
    }

    public CompetenceSpecificationBuilder WithIds(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new CompetenceIdSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public CompetenceSpecificationBuilder WithNames(IEnumerable<string> names)
    {
        if (names == null || !names.Any())
        {
            return this;
        }
        
        var specification = new CompetenceNameSpecification(names);
        AppendSpecification(specification);
        return this;
    }
}