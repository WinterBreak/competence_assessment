using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateTypeSpecification: SpecificationBase<Template>
{
    private readonly TemplateType _type;

    public TemplateTypeSpecification(TemplateType type)
    {
        _type = type;
    }

    public override Expression<Func<Template, bool>> Criteria
        => c => c.TemplateTypeId == (int)_type || _type == TemplateType.None;
}