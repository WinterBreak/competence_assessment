using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentAnalyticsService: IAssessmentAnalyticsService
{
    private readonly IAssessmentAnalyticsRepository _repository;

    public AssessmentAnalyticsService(IAssessmentAnalyticsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<AssessmentParticipantCompetencies>> GetParticipantsCompetenciesAsync(
        CancellationToken token = default)
    {
        var competencies = await _repository.GetParticipantsCompetenciesAsync(token);
        foreach (var competence in competencies)
        {
            competence.ParticipantCompetences.ForEach(c => c.Level = DetermineLevel(c.Percentage));
        }
        
        return competencies;
    }

    public async Task<List<PositionCompetencies>> GetPositionsCompetenciesAsync(
        CancellationToken token = default)
    {
        var participantsCompetencies = await GetParticipantsCompetenciesAsync(token);
        var groupedByPositions = participantsCompetencies
            .GroupBy(pc => pc.User.PositionId);

        var positionsCompetencies = new List<PositionCompetencies>();
        foreach (var group in groupedByPositions)
        {
            var participantCount = group.AsEnumerable().Count();
            var positionName = group.First().User.Position;
            var competencies = GetCompetenciesByPosition(group.ToList());
            var employees = group.Select(g => g.User);
            var positionCompetencies = GetPositionCompetencies(employees, competencies
                                    , positionName, participantCount, group.Key);
            positionCompetencies.Level = DetermineLevel(positionCompetencies.Percentage);
            positionsCompetencies.Add(positionCompetencies);
        }
        
        return positionsCompetencies.OrderBy(pc => pc.PositionName).ToList();
    }
    
    public async Task<CompetenceDevelopmentData> GetCompetenceDevelopmentAsync(
          int employeeId, CancellationToken token = default)
    {
        return await _repository.GetCompetenceDevelopmentAsync(employeeId, token);
    }

    public async Task<List<DepartmentCompetencies>> GetDepartmentCompetenciesAsync(CancellationToken token = default)
    {
        var participantsCompetencies = await GetParticipantsCompetenciesAsync(token);
        var groupedByDepartments = participantsCompetencies
            .GroupBy(pc => pc.User.DepartmentId);
        
        var departmentsCompetencies = new List<DepartmentCompetencies>();
        foreach (var group in groupedByDepartments)
        {
            var positionName = group.First().User.Department;
            var competencies = GetCompetenciesByPosition(group.ToList());
            var employees = group.Select(g => g.User);
            var departmentCompetencies = GetDepartmentCompetencies(employees, competencies
                                                                 , positionName, group.Key);
            departmentCompetencies.Level = DetermineMaturityLevel(departmentCompetencies.Percentage);
            departmentsCompetencies.Add(departmentCompetencies);
        }
        
        return departmentsCompetencies.OrderBy(pc => pc.DepartmentName).ToList();
    }

    private List<CompetenceResult> GetCompetenciesByPosition(
        IEnumerable<AssessmentParticipantCompetencies> competenciesByPositions)
    {
        var participantCompetencies = competenciesByPositions
            .SelectMany(pc => pc.ParticipantCompetences)
            .DistinctBy(pc => pc.Competence.Id)
            .ToList();
        
        var competencies = participantCompetencies
            .Select(c => new CompetenceResult(c.Competence, c.Score, c.Percentage))
            .ToList();
        
        competencies.ForEach(c => c.Level = DetermineLevel(c.Percentage));
        
        return competencies;
    }

    private PositionCompetencies GetPositionCompetencies(
          IEnumerable<User> employees
        , IEnumerable<CompetenceResult> competencies, string positionName
        , int participantCount, int positionId)
    {
        if (competencies == null || !competencies.Any())
        {
            return null;
        }
        
        var score = competencies.Sum(c => c.Score) / participantCount;
        var percentage = competencies.Sum(c => c.Percentage) / participantCount;
        return new PositionCompetencies(positionId, positionName, score, percentage
                                      , competencies, employees);
    }
    
    private DepartmentCompetencies GetDepartmentCompetencies(
          IEnumerable<User> employees
        , IEnumerable<CompetenceResult> competencies, string departmentName
        , int departmentId)
    {
        if (competencies == null || !competencies.Any())
        {
            return null;
        }
        
        var score = competencies.Sum(c => c.Score / competencies.Count());
        var percentage = competencies.Sum(c => c.Percentage / competencies.Count());
        return new DepartmentCompetencies(departmentId, departmentName, score, percentage
            , employees.ToList(), competencies.ToList());
    }

    private string DetermineLevel(decimal percentage)
    {
        if (percentage >= 90) return "Высокий";
        if (percentage >= 70) return "Выше среднего";
        if (percentage >= 50) return "Средний";
        if (percentage >= 30) return "Ниже среднего";
        return "Низкий";
    }
    
    private string DetermineMaturityLevel(decimal percentage)
    {
        if (percentage >= 100) return "Оптимизирующий";
        if (percentage >= 80) return "Количественно управляемый";
        if (percentage >= 60) return "Определенный";
        if (percentage >= 40) return "Управляемый";
        if (percentage >= 20) return "Начальный";
        return "Низкий";
    }
}