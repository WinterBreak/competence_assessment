using CompetenceAssessment.Domain.Assessment.DTO;
using CompetenceAssessment.Domain.Ports;
using CompetenceAssessment.Infrastructure.Assessment;
using Microsoft.EntityFrameworkCore;
using BLL = CompetenceAssessment.Domain.Assessment;
using Task = System.Threading.Tasks.Task;

namespace CompetenceAssessment.Infrastructure.Assessments;

internal class AssessmentRepository: BLL.IAssessmentRepository
{
    private readonly AssessmentContext _context;
    private readonly BLL.ITemplateRepository _templateRepository;
    private readonly IUserProvider _userProvider;
    private readonly BLL.IAssessmentResultRepository _resultRepository;

    public AssessmentRepository(AssessmentContext context
        , BLL.ITemplateRepository templateRepository
        , IUserProvider userProvider
        , BLL.IAssessmentResultRepository resultRepository)
    {
        _context = context;
        _templateRepository = templateRepository;
        _userProvider = userProvider;
        _resultRepository = resultRepository;
    }
    
    public async Task<BLL.Assessment?> GetAssessmentAsync(BLL.AssessmentQuery query
                                                        , CancellationToken token = default)
    {
        var assessments = await GetAssessmentsAsync(query, token);
        var assessment = assessments.SingleOrDefault();
        ArgumentNullException.ThrowIfNull(assessment);
        
        var resultQuery = new BLL.AssessmentResultQuery(assessmentId: assessment.Id);
        assessment.Results = await _resultRepository.GetAssessmentsAsync(resultQuery, token);
        
        return assessment;
    }

    public async Task<List<BLL.Assessment>> GetAssessmentsAsync(BLL.AssessmentQuery query
                                                        , CancellationToken token = default)
    {
        var specification = new AssessmentSpecificationBuilder().WithQuery(query).Build();
        var assessments = await _context.Assessments
            .Where(specification)
            .OrderByDescending(a => a.StartDate).ToListAsync(token);
        var participants = await GetParticipantsAsync(assessments, token);
        var templates = await GetTemplatesAsync(assessments, token);
        var assessmentIds = assessments.Select(a => a.Id).ToList();
        var resultQuery = new BLL.AssessmentResultQuery(assessmentIds: assessmentIds);
        var results = await _resultRepository.GetAssessmentsAsync(resultQuery, token);
        var resultDict = results
            .GroupBy(r => r.AssessmentId)
            .ToDictionary(r => r.Key, r => r.ToList());
        
        return assessments.Select(a => {
                var candidate = participants.SingleOrDefault(p => p.Id == a.UserId);
                var inspectorIds = a.Inspectors.Select(i => i.UserId).ToList();
                var inspectors = participants
                    .Where(p => inspectorIds.Contains(p.Id))
                    .ToList();
                var res = resultDict[a.Id];
                var template = templates.SingleOrDefault(t => t.Id == a.TemplateId);
                return new BLL.Assessment(a.Id, a.StartDate, a.EndDate, candidate
                    , (BLL.AssessmentType)a.AssessmentTypeId, template, inspectors, a.IsFinished
                    , res);
            })
            .ToList();
    }

    public async Task AddAssessmentAsync(BLL.Assessment assessment, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        
        var newAssessment = new Assessment.Assessment(assessment.Template.Id, (int)assessment.Type
                                                    , assessment.Candidate.Id, assessment.StartDate
                                                    , assessment.EndDate, assessment.IsFinished);
        var inspectors = assessment.Inspectors.Select(i 
            => new AssessmentInspector(newAssessment, i.Id)).ToList();
        await _context.Assessments.AddAsync(newAssessment, token);
        await _context.AssessmentInspectors.AddRangeAsync(inspectors, token);
    }

    public async Task UpdateAssessmentAsync(BLL.Assessment assessment, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        
        var updatingAssessment = await _context.Assessments
            .SingleOrDefaultAsync(a => a.Id == assessment.Id, token);
        ArgumentNullException.ThrowIfNull(updatingAssessment);
        
        updatingAssessment.IsFinished = assessment.IsFinished; // TODO фронт пока не передает + END DATE
        
        await UpdateInspectors(assessment, updatingAssessment);
        await _resultRepository.AddAssessmentsAsync(assessment.Results, token);
    }

    public async Task RemoveAssessmentAsync(int id, CancellationToken token = default)
    {
        var deletingAssessment = await _context.Assessments
            .SingleOrDefaultAsync(a => a.Id == id, token);
        ArgumentNullException.ThrowIfNull(deletingAssessment);
        _context.Assessments.Remove(deletingAssessment);
    }

    public async Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);

    private async Task<List<AssessmentParticipant>> GetParticipantsAsync(IEnumerable<Assessment.Assessment> assessments
                                                                       , CancellationToken token = default)
    {
        var participantIds = assessments.Select(a => a.UserId).Distinct().ToList();
        var inspectorIds = assessments.SelectMany(a => a.Inspectors)
            .Select(i => i.UserId).Distinct().ToList();
        participantIds = inspectorIds.Union(participantIds).Distinct().ToList();
        return await _userProvider.GetAssessmentParticipantsAsync(participantIds, token);
    }
    
    private async Task<List<BLL.ITemplate>> GetTemplatesAsync(IEnumerable<Assessment.Assessment> assessments
        , CancellationToken token = default)
    {
        var templateIds = assessments.Select(a => a.TemplateId).Distinct().ToList();
        var query = new BLL.TemplateQuery(ids: templateIds);
        return await _templateRepository.GetTemplatesAsync(query, token);
    }

    private async Task UpdateInspectors(BLL.Assessment updated, Assessment.Assessment updating)
    {
        var updatedInspectorIds = updated.Inspectors.Select(i => i.Id).ToList();
        var updatingInspectorIds = updating.Inspectors.Select(i => i.UserId).ToList();
        
        var inspectorIdsToAdd = updatedInspectorIds.Except(updatingInspectorIds).ToList();
        var inspectorsToAdd = updating.Inspectors
            .Where(i => inspectorIdsToAdd.Contains(i.UserId)).ToList();
        await _context.AssessmentInspectors.AddRangeAsync(inspectorsToAdd);
        
        var inspectorIdsToRemove = updatingInspectorIds.Except(inspectorIdsToAdd).ToList();
        var inspectorsToRemove = updating.Inspectors
            .Where(i => inspectorIdsToRemove.Contains(i.UserId)).ToList();
        _context.RemoveRange(inspectorsToRemove);
    }
}