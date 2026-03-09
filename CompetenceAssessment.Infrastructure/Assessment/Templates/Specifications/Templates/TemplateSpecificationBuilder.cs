using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateSpecificationBuilder: SpecificationBuilderBase<Template>
{
    public TemplateSpecificationBuilder WithQuery(TemplateQuery query)
    {
        return WithIds(query.Ids)
            .WithType(query.Type)
            .WithCompetencies(query.CompetenceIds);
    }
    
    public TemplateSpecificationBuilder WithIds(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new TemplateIdSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public TemplateSpecificationBuilder WithType(TemplateType type)
    {
        var specification = new TemplateTypeSpecification(type);
        AppendSpecification(specification);
        return this;
    }

    public TemplateSpecificationBuilder WithCompetencies(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new TemplateCompetenciesSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
}