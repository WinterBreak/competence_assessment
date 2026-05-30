using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentCalcService: IAssessmentCalcService
{
    private readonly IAssessmentRepository _assessmentRepository;

    public AssessmentCalcService(IAssessmentRepository assessmentRepository)
    {
        _assessmentRepository = assessmentRepository;
    }
    
    public async Task<AssessmentCalculation> CalculateAsync(int assessmentId
        , CancellationToken token = default)
    {
        var query = new AssessmentQuery(id: assessmentId);
        var assessment = await _assessmentRepository.GetAssessmentAsync(query, token);
        ArgumentNullException.ThrowIfNull(assessment);

        if (assessment.Type == AssessmentType._360Degrees_) // TODO отдельный запрос на проверку типа, чтобы тянуть оценку 1 раз
        {
            assessment = await _assessmentRepository.GetFullDegreeAssessment(assessmentId, token);
        }
        
        var strategy = GetCalcStrategy(assessment.Type);
        return strategy.Calculate(assessment);
    }

    private IAssessmentCalcStrategy GetCalcStrategy(AssessmentType type)
    {
        return type switch
        {
            AssessmentType.Testing => new TestCalcStrategy(),
            AssessmentType.Survey => new SurveyCalcStrategy(),
            AssessmentType._360Degrees_ => new _360DegreeCalcStrategy(),
            _ => throw new ArgumentException($"Неизвестный тип оценки: {type}")
        };
    }
}